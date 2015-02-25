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

public class CoreStatus : MonoBehaviour
{
    public bool isCoreDataLoaded = false;

    public event Action OnCoreDataReady;

    public void SetCoreDataReady()
    {
        Debug.Log(this.gameObject.name + " -> SetCoreDataReady.");

        isCoreDataLoaded = true;

        if (OnCoreDataReady != null)
        {
            OnCoreDataReady();
        }
    }
}
