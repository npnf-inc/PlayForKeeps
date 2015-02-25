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
using UnityEngine.UI;

/*
 * Manages EnergyBar UI
 */
using NPNF.Energy;
using NPNF.Core;
using NPNF.Core.Users;

public class EnergyController : MonoBehaviour
{
    public GameObject energyBar;
    public Text energyLabel;
    public bool needsSync;
    public bool needsUpdate;
    public int currentEnergy;
    int maxEnergy = 0;
    float startScale;

    // Use this for initialization
    void Start()
    {
        startScale = energyBar.transform.localScale.x;
        SyncEnergy();
        needsUpdate = false;
        needsSync = false;
        energyLabel.text = "";
    }

    public void SyncEnergy()
    {
        if (ModuleHelpers.IsThisModuleActive(this.gameObject))
        {
            // npnf feature: Syncing an Energy bar to the game - This will first get energy if not exist in local cache, then sync the energy
            const string energyName = "Energy";
            User.CurrentProfile.EnergyBank.Sync(energyName, (EnergyStatus status, NPNFError error) => {
                if (error == null)
                {
                    Energy energy = Energy.GetCachedByName(energyName);
                    maxEnergy = energy.Bounds [0].Upper;

                    int currentEnergy = User.CurrentProfile.EnergyBank.GetCached(energyName).CurrentEnergy;
                    OnUserEnergyRefresh(currentEnergy);

                    // npnf feature: An handler for value change on energy and recharge time
                    User.CurrentProfile.EnergyBank.AddTimerHandler(energyName, HandleOnEnergyTimerUpdate);
                    User.CurrentProfile.EnergyBank.AddValueUpdateHandler(energyName, HandleOnCurrentEnergyUpdate);
                    User.CurrentProfile.EnergyBank.AddMaxUnitReachedHandler(energyName, HandleOnMaxEnergyReached);
                } else
                {
                    Debug.LogError(error);
                    AppController.Instance.IsNetworkError(error); 
                }
            });
        }
    }

    void OnUserEnergyRefresh(int energy)
    {
        currentEnergy = energy;
        needsUpdate = true;
    }

    void UpdateBar()
    {
        Vector3 scale = energyBar.transform.localScale;
        scale.x = ((float)currentEnergy) / maxEnergy * startScale;
        energyBar.transform.localScale = scale;
        energyLabel.text = currentEnergy.ToString() + "/" + maxEnergy.ToString();
    }

    void Update()
    {
        if (needsSync)
        {
            needsSync = false;
            SyncEnergy();
        }

        if (needsUpdate)
        {
            needsUpdate = false;
            UpdateBar();
        }
    }

    #region Energy Update CallBack Helper
    private void HandleOnEnergyTimerUpdate(object sender, TimerArgs e)
    {
        // handle e.TimeLeft
    }

    private void HandleOnCurrentEnergyUpdate(object sender, EnergyArgs e)
    {
        int currentEnergy = e.CurrentEnergy;
        OnUserEnergyRefresh(currentEnergy);
    }

    private void HandleOnMaxEnergyReached(object sender, EnergyArgs e)
    {
        // max reached.
    }
    #endregion
}
