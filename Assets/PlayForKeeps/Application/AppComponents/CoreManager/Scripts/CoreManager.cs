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
using PF.Base;


[Prefab("CoreManager")]
public class CoreManager : MonoBehaviour
{
	public List<GameObject> corePrefabList = new List<GameObject>();
	public List<GameObject> coreObjectList = new List<GameObject>();

	private bool waitToLoad = false;
	private bool checkUpdate = false;

	public event Action OnAllCoreModuleLoaded;
	
	void Awake ()
	{
	}

	void Update ()
	{
		if (checkUpdate)
		{
			if (CheckCoresReady())
			{
				checkUpdate = false;
				ListenToNetworkStatus ();
				if (OnAllCoreModuleLoaded != null)
				{
					OnAllCoreModuleLoaded();
				}
			}
		}
	}
	

	public void CreateCoreObjects ()
	{
		foreach(GameObject aPrefab in corePrefabList)
		{
			GameObject coreObject = (GameObject) GameObject.Instantiate (aPrefab);
			coreObject.name = aPrefab.name;
			coreObject.transform.parent = this.transform;
			coreObjectList.Add (coreObject);
		}
	}

	public void LoadAllCoreModules ()
	{
		if (AppController.Instance.isOnline)
		{
			if (waitToLoad)
			{
				AppController.Instance.OnNetworkConnected -= LoadAllCoreModules;
				waitToLoad = false;
			}
			CreateCoreObjects();

			if (coreObjectList.Count > 0)
			{
				checkUpdate = true;
				foreach(GameObject aCoreObject in coreObjectList)
				{
					aCoreObject.SendMessage("StartModule");
				}
			}
		}
		else
		{
			waitToLoad = true;
			AppController.Instance.OnNetworkConnected += LoadAllCoreModules;
		}
	}

	private bool CheckCoresReady()
	{
		bool result = true;
		foreach(GameObject aCoreObject in coreObjectList)
		{
			if (aCoreObject.GetComponent<ModuleStatus>().status != ModuleStatusType.READY)
			{
				result = false;
			}
		}

		return result;
	}

	private void ListenToNetworkStatus ()
	{
		AppController.Instance.OnNetworkConnected += ReadyAllCoreModules;
		AppController.Instance.OnNetworkDisconnected += StopAllCoreModules;
	}

	private void StopAllCoreModules ()
	{
		foreach (GameObject aCoreObject in coreObjectList)
		{
			aCoreObject.SendMessage("SetStop");
		}
	}

	private void ReadyAllCoreModules ()
	{
		foreach (GameObject aCoreObject in coreObjectList)
		{
			aCoreObject.SendMessage("SetReady");
		}
	}
}
