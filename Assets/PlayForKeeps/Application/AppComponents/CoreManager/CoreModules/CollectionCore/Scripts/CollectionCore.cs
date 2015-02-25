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
using System;
using System.Collections;
using System.Collections.Generic;
using NPNF.Collections;
using PF.Base;
using NPNF.Core;
using NPNF.Core.Users;

[Prefab("CollectionCore")]
[RequireComponent (typeof(ModuleStatus))]
public sealed class CollectionCore : MonoBehaviour, IModule
{
    private ModuleStatus moduleStatus;
    private Dictionary<string, ConversionFormula> conversionsDict = new Dictionary<string, ConversionFormula>();
    
    void Awake()
    {
        moduleStatus = this.GetComponent<ModuleStatus>();
    }
    
    #region IModule
    public void StartModule()
    {
        GetAllAssets();
    }

    public void SetReady()
    {
        moduleStatus.SetStatusReady();
    }

    public void SetStop()
    {
        moduleStatus.SetStatusStop();
    }
    #endregion
    
    public void GetAllAssets()
    {
        if (ModuleHelpers.IsThisModuleActive(this.gameObject))
        {
            // npnf feature: Getting all Assets
            NPNF.Collections.Asset.GetAll((List<Asset> assets, NPNFError error) => {
                if (error != null)
                {
                    Debug.LogWarning("Empty Assets: " + error);
                    AppController.Instance.IsNetworkError(error);
                }

                GetConversionFormulas();
            });
        }
    }

    private void GetConversionFormulas()
    {
        if (ModuleHelpers.IsThisModuleActive(this.gameObject))
        {
            // npnf feature: Getting all Conversion Formulas
            NPNF.Collections.ConversionFormula.GetAll((List<ConversionFormula> formulas, NPNFError error) => {
                if (error == null && formulas != null)
                {
                    foreach (ConversionFormula formula in ConversionFormula.GetAllCachedById().Values)
                    {
                        conversionsDict [formula.Name] = formula;
                    }
                } else
                {
                    AppController.Instance.IsNetworkError(error);
                }
                SetReady();
            });
        }
    }

    public Dictionary<string, ConversionFormula> GetFormulas()
    {
        return conversionsDict;
    }

    public Asset GetAssetInfoByName(string assetName)
    {
        return Asset.GetCachedByName(assetName);
    }

    public Entitlement GetEntitlementById(string id)
    {
        return User.CurrentProfile.Entitlements.GetCached(id);
    }

    public int GetCount(List<string> entitlementIds)
    {
        int count = 0;
        if (entitlementIds.Count == 0)
        {
            return count;
        }

        Dictionary<string, Entitlement> entitlements = User.CurrentProfile.Entitlements.GetAllCached();
        string entitlementName = User.CurrentProfile.Entitlements.GetCached(entitlementIds [0]).AssetName;

        foreach (Entitlement entitlement in entitlements.Values)
        {
            if (entitlement.AssetName.Equals(entitlementName))
            {
                if (entitlement.Id != entitlementIds [0])
                {
                    count++;
                }
            }
        }
        return count;
    }
}
