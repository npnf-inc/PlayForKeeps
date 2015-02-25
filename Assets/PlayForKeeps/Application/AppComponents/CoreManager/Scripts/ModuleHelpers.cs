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

public class ModuleHelpers
{
    public static bool IsThisModuleActive(GameObject gameObject)
    {
        bool result = true;

        // if it doesn't have ModuleStatus component, it'll be treated as active.
        if (gameObject.GetComponent<ModuleStatus>() != null)
        {
            if (gameObject.GetComponent<ModuleStatus>().status == ModuleStatusType.STOP)
            {
                result = false;
            }
        }
        
        return result;
    }
}
