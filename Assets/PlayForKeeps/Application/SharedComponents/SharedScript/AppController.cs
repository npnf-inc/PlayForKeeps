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
using System.Globalization;
using NPNF.Energy;
using NPNF.Core;
using NPNF.Core.Users;
using NPNF.Collections;
using PF.Base;

public enum Controller
{
    GAME,
    GACHA,
    ENERGY,
    CURRENCY,
    INVENTORY,
    RECIPE
}

public class AppController : Singleton<AppController>
{   
    public bool isOnline { get; private set; }

    public event Action OnNetworkConnected;
    public event Action OnNetworkDisconnected;

    private GameObject coreManagerObj;
    private CoreManager coreManager;
    private const string GAME_CONTROLLER = "GameController";
    private const string GACHA_CONTROLLER = "GachaController";
    private const string ENERGY_CONTROLLER = "EnergyController";
    private const string CURRENCY_CONTROLLER = "CurrencyController";
    private const string INVENTORY_CONTROLLER = "InventoryController";
    private const string RECIPE_CONTROLLER = "RecipeController";

    public MonoBehaviour GetController(Controller controller)
    {
        GameObject obj;
        switch (controller)
        {
            case Controller.GAME:
                obj = GameObject.Find(GAME_CONTROLLER);
                return obj.GetComponent<GameController>();
            case Controller.GACHA:
                obj = GameObject.Find(GACHA_CONTROLLER);
                return obj.GetComponent<GachaController>();
            case Controller.ENERGY:
                obj = GameObject.Find(ENERGY_CONTROLLER);
                return obj.GetComponent<EnergyController>();
            case Controller.CURRENCY:
                obj = GameObject.Find(CURRENCY_CONTROLLER);
                return obj.GetComponent<CurrencyController>();
            case Controller.INVENTORY:
                obj = GameObject.Find(INVENTORY_CONTROLLER);
                return obj.GetComponent<InventoryController>();
            case Controller.RECIPE:
                obj = GameObject.Find(RECIPE_CONTROLLER);
                return obj.GetComponent<RecipeController>();
            default:
                return null;
        }
    }

    void Awake()
    {
        if (GameObjectRemoveHelper.FindAndRemoveGameObject(gameObject, "AppController"))
        {
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
    }

    public void Init()
    {
        LoadingSpinner.Instance.EnableLoadingSpinner();
        isOnline = true;
        StartLaunchFlow();
    }

    private void StartLaunchFlow()
    {
        // npnf feature: Initializing the Platform
        NPNF.Platform.Init((NPNFError error) => {
            
            if (error == null)
            {
                OnPlatformInitDone();
            } else
            {
                Debug.LogError("Platform Init failed");
                IsNetworkError(error);
            }
            
        });
    }
    
    private void OnPlatformInitDone()
    {
        // npnf feature: Getting App Version data
        NPNF.Core.Configuration.Version.Get((NPNF.Core.Configuration.Version versionData, NPNFError error) => {
            if (error == null)
            {
                coreManagerObj = (GameObject)GameObject.Instantiate(Resources.Load("CoreManager"));
                coreManagerObj.name = "CoreManager";
                coreManagerObj.transform.parent = this.transform;
                coreManager = coreManagerObj.GetComponent<CoreManager>();
                coreManager.OnAllCoreModuleLoaded += OnAllCoreModuleLoaded;
                coreManager.LoadAllCoreModules();
            } else
            {
                Debug.LogWarning("Version Init failed" + error);
                IsNetworkError(error);
            }
        });
    }

    private void OnAllCoreModuleLoaded()
    {
        CreateAnonUser();
    }
    
    private void CreateAnonUser()
    {
        Dictionary<string, object> customData = new Dictionary<string, object>();
        if (ModuleHelpers.IsThisModuleActive(this.gameObject))
        {
            // npnf feature: Creating an Anonymous User (Guest User)
            User.CreateAnonymous(customData, (User user, NPNFError error) => {
                if (error != null)
                {
                    Debug.LogWarning("Create Anonymous User failed, error: " + error.ToString());
                }
                OnUserLoggedIn(error);
            });
        }
    }
    
    private void OnUserLoggedIn(NPNFError error)
    {
        if (error == null)
        {
            // npnf feature: Getting all Custom data for a User Profile
            if (User.CurrentProfile.GetAllCustom() != null)
            {
                foreach (var obj in User.CurrentProfile.GetAllCustom())
                {
                    Debug.Log("keys: " + obj.Key + " " + obj.Value);
                }
            }

            OnUserReady();
        } else
        {
            Debug.LogError("login error: " + error);
            IsNetworkError(error);
        }
    }
    
    private void OnUserReady()
    {
        LoadingSpinner.Instance.DisableLoadingSpinner();

        GameObject obj = GameObject.Find("MainDialog");

        MainDialog dialog = obj.GetComponent<MainDialog>();
        dialog.ShowDialog("Let's start your first day at npnf!", null, () => {
            Application.LoadLevel("MainScene");
        });
    }

    public void IsNetworkError(NPNFError error)
    {
        GameObject obj = GameObject.Find("MainDialog");
        MainDialog dialog = obj.GetComponent<MainDialog>();
        if (error.RequestException == "No such host is known" || error.Messages [0] == "Network not reachable")
        {
            dialog.ShowDialog("Network not reachable. Please check the internet connection and possibly restart the game.", null, null);
        }
    }
}
