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
using System.Collections.Generic;
using NPNF.Collections;

/*
 * Manages popups for new inventory
 */
using UnityEngine.UI;
using System;

public class InventoryController : MonoBehaviour
{
    public Image[] images;
    public Image[] fusions;
    public PopupPrefab popupPrefab;
    private CollectionCore collectionCore;
    List<string> pendingEntitlementIds;

    void Awake()
    {
        GameObject obj = GameObject.Find("CollectionCore");
        collectionCore = obj.GetComponent<CollectionCore>();
        pendingEntitlementIds = new List<string>();
    }

    public void OnNewInventory(string entitlementId)
    {
        this.pendingEntitlementIds.Add(entitlementId);
        OnEntitlementsRefreshed();
    }

    public void OnNewInventory(List<string> entitlementIds)
    {
        this.pendingEntitlementIds.AddRange(entitlementIds);

        OnEntitlementsRefreshed();
    }

    void OnEntitlementsRefreshed()
    {
        foreach (string id in pendingEntitlementIds)
        {
            //notify user of new inventory
            Entitlement ent = collectionCore.GetEntitlementById(id);
            Asset info = collectionCore.GetAssetInfoByName(ent.AssetName);
            string prefabName = (string)info.GetCustom("prefabName", "");
            string descript = (string)info.Description;
            popupPrefab.SetPrefab(prefabName, descript);

            RecipeController ctrler = (RecipeController)AppController.Instance.GetController(Controller.RECIPE);  

            //check recipes for completion
            string outcome = ctrler.OnNewInventory((string)info.Name);
            
            if (!String.IsNullOrEmpty(outcome))
            {
                foreach (Image fuse in fusions)
                {
                    if (fuse.gameObject.name == "icon_fus_" + outcome)
                    {
                        fuse.color = Color.white;
                        break;
                    }
                }
            }

            foreach (Image image in images)
            {
                if (image.gameObject.name == "icon_" + ent.AssetName)
                {
                    image.color = Color.white;

                    foreach (Image fuse in fusions)
                    {
                        if (fuse.gameObject.name == "icon_fus_" + ent.AssetName)
                        {
                            fuse.enabled = false;
                            break;
                        }
                    }

                    break;
                }
            }

        }

        pendingEntitlementIds.Clear();
    }

    public int GetCount(List<string> entitlementIds)
    {
        return collectionCore.GetCount(entitlementIds);
    }
}
