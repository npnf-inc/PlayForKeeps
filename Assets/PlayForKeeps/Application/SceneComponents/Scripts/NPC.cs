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
using NPNF.Collections;
using NPNF.Core;
using NPNF.Core.Utils;
using UnityEngine.UI;

/*
 *  Loads an NPC asset
 */
using System.Collections.Generic;
using NPNF.Gachas;

public class NPC : MonoBehaviour
{
    public TextMesh nameLabel;
    public bool entered = false;
    public string assetName;
    private Animator _animator;
    string[] dialogs;
    Asset asset;
    int talkCount;

    // Use this for initialization
    void Start()
    {
        _animator = GetComponent<Animator>();
        // npnf feature: Getting an Asset by Name
        NPNF.Collections.Asset.GetByName(assetName,
                                    (Asset asset, NPNFError error) => {
            if (asset != null)
            {
                this.asset = asset;
                OnAssetLoaded();
            }
        });
        talkCount = 0;
    }

    void OnAssetLoaded()
    {
        nameLabel.text = asset.Description;
        dialogs = ((string)asset.GetCustom("Dialog", "")).Split(";" [0]);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject obj = GameObject.Find("MainDialog");
        MainDialog mainDialog = obj.GetComponent<MainDialog>();

        if (!entered && other.gameObject.name == "Player")
        {
            if (assetName != "Woo" && talkCount > 2)
            {
                mainDialog.ShowDialog("Half day!", null, null);
                return;
            }

            GameController gameCtrler = (GameController)AppController.Instance.GetController(Controller.GAME);  

            //check if user has won
            if (assetName == "Woo" && gameCtrler.hasPitch)
            {
                gameCtrler.EndGame(true);
                return;
            }

            EnergyController energyCtrler = (EnergyController)AppController.Instance.GetController(Controller.ENERGY);  

            string sku = (assetName == "Henry") ? "expensive fight" : "fight";
            GachaInput price = GachaInput.GetCachedBySku(sku);

            if (energyCtrler.currentEnergy >= price.Price.Amount)
            {
                int i = Random.Range(0, dialogs.Length);
                string dialog = dialogs [i];
                _animator.SetFloat("HorizontalInput", other.gameObject.transform.position.x - transform.position.x);
                _animator.SetFloat("VerticalInput", other.gameObject.transform.position.y - transform.position.y);

                mainDialog.ShowDialog(dialog, asset);
                talkCount++;
                StartCoroutine(CancelAnimation());
            } else
            {
                mainDialog.ShowDialog("Go away...", null, null);
            }
        }
    }

    IEnumerator CancelAnimation()
    {
        yield return 0;
        _animator.SetFloat("HorizontalInput", 0);
        _animator.SetFloat("VerticalInput", 0);
    }

    void OnTriggerExit(Collider other)
    {
        if (entered && other.gameObject.name == "Player")
        {
            entered = false;
        }
    }
}
