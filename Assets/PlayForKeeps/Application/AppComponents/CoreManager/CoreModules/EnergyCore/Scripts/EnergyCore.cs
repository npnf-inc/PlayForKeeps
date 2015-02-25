﻿// Copyright (C) 2014 npnf, inc.
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
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NPNF.Energy;
using PF.Base;
using NPNF.Core;

[Prefab("EnergyCore")]
[RequireComponent (typeof(ModuleStatus))]
public class EnergyCore : MonoBehaviour, IModule
{
    private ModuleStatus moduleStatus;

    void Awake()
    {
        moduleStatus = this.GetComponent<ModuleStatus>();
    }

    #region IModule
    public void StartModule()
    {
        if (ModuleHelpers.IsThisModuleActive(this.gameObject))
        {
            // npnf feature: Getting all Energy Bars
            Energy.GetAll((Dictionary<string, Energy> energyDict, NPNFError error) => {
                if (error == null)
                {
                    SetReady();
                } else
                {
                    AppController.Instance.IsNetworkError(error);
                }
            });
        }
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
}

