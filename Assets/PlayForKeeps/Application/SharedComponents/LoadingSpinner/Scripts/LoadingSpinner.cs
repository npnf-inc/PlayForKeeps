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

[Prefab("LoadingSpinner")]
public class LoadingSpinner : Singleton<LoadingSpinner>
{
    public GameObject animationGameObject;
    private bool isReady = false;

    void Awake()
    {
        isReady = true;
        if (animationGameObject == null)
        {
            isReady = false;
        }
    }

    public void EnableLoadingSpinner()
    {
        if (isReady)
        {
            animationGameObject.SetActive(true);
        }
    }

    public void DisableLoadingSpinner()
    {
        if (isReady)
        {
            animationGameObject.SetActive(false);
        }
    }
}
