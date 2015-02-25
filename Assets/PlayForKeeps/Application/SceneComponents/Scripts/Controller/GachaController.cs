// Copyright (C) 2014 npnf, inc.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using UnityEngine;
using System.Collections;
using PF.Base;
using NPNF.Gachas;
using NPNF.Collections;
using NPNF.Core;
using System.Collections.Generic;
using NPNF.Core.Users;

public class GachaController : MonoBehaviour
{
    Gacha gacha;
    GachaInput input;
    public bool isSpinning = false;

    void Awake()
    {
    }

    public void SpinGacha(Asset asset)
    {
        isSpinning = true;
        LoadingSpinner.Instance.EnableLoadingSpinner();
        string assetName = "fight " + ((string)asset.GetCustom("Class", "ENG"));

        // npnf feature: Grabbing gacha for this asset
        this.gacha = Gacha.GetCachedByName(assetName);

        if (this.gacha != null)
        {
            OnGachaLoaded();
        } else
        {
            NPNF.Gachas.Gacha.GetByName(assetName, 
                                        (Gacha gacha, NPNFError error) => {
                if (gacha != null)
                {
                    this.gacha = gacha;
                    OnGachaLoaded();
                } else
                {
                    LoadingSpinner.Instance.DisableLoadingSpinner();
                    isSpinning = false;
                    Debug.LogError("Get Gacha Failed: " + error);
                    AppController.Instance.IsNetworkError(error); 
                }
            });
        }
    }

    void OnGachaLoaded()
    {
        // npnf feature: Getting inputs for gacha
        this.input = GachaInput.GetCachedById(gacha.GachaInputIds [0]);
        if (this.input != null)
        {
            OnGachaInputLoaded();
        } else
        {
            NPNF.Gachas.GachaInput.GetById(gacha.GachaInputIds [0], 
                                           (GachaInput input, NPNFError error) => {
                if (input != null)
                {
                    this.input = input;
                    OnGachaInputLoaded();
                } else
                {
                    isSpinning = false;
                    LoadingSpinner.Instance.DisableLoadingSpinner();
                    Debug.LogError("Get Product Failed: " + error);
                    AppController.Instance.IsNetworkError(error); 
                }
            });
        }
    }

    void OnGachaInputLoaded()
    {
        // npnf feature; Playing gacha
        User.CurrentProfile.Gacha.Play(gacha.Id, input.Id, 
                                       (List<string> entitlementIds, NPNFError error) => {
            LoadingSpinner.Instance.DisableLoadingSpinner();
            isSpinning = false;
            if (entitlementIds != null)
            {
                InventoryController inventoryCtrler = (InventoryController)AppController.Instance.GetController(Controller.INVENTORY);  

                // Setup to be guarantee only one entitlement is added in one play
                int count = inventoryCtrler.GetCount(entitlementIds);

                //notify inventory of new entitlements
                inventoryCtrler.OnNewInventory(entitlementIds);

                // If there are same asset exist,it is a dup. Credit user an amount of currency
                if (count > 0)
                {
                    CurrencyController currencyCtrler = (CurrencyController)AppController.Instance.GetController(Controller.CURRENCY);  
                    currencyCtrler.Credit(10);
                }

                //gacha uses 1 energy, update energy
                EnergyController energyCtrler = (EnergyController)AppController.Instance.GetController(Controller.ENERGY);  
                energyCtrler.needsSync = true;
            } else
            {
                Debug.LogWarning("Play Gacha Failed: " + error);
                AppController.Instance.IsNetworkError(error); 
            }
        });
    }
    
}
