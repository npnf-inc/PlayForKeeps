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
using UnityEngine.UI;
using System;
using NPNF.Collections;

public class MainDialog : MonoBehaviour {

	public bool isShowingDialog = false;
	public Text dialog;
	public Text continueLabel;
	public GameObject dialogObj;
	public float scrollSpeed = .05f;
	public float completeDialogDelay = 0.5f;
	private Action callback;
	
	bool dialogDone;
	Asset npcAsset;
	string currentText = "";
	int textIndex;
	float nextTextTime;
	
	void Awake()
	{
		dialogObj.SetActive(false);
		isShowingDialog = false;
	}
	
	public void ShowDialog(string text, Asset npcAsset, Action callback = null)
	{
		if(!isShowingDialog)
		{
			this.callback = callback;
			dialogObj.SetActive(true);
			dialog.text = "";
			continueLabel.enabled = false;
			isShowingDialog = true;
			
			this.npcAsset = npcAsset;
			dialogDone = false;
			currentText = text;
			textIndex = 0;
			nextTextTime = Time.time + scrollSpeed;
		}
	}
	
	void AllowContinue()
	{
		continueLabel.enabled = true;
	}
	
	void CloseDialog()
	{
		dialogObj.SetActive(false);
		isShowingDialog = false;
		if(npcAsset != null)
		{
            GachaController gachaCtrler = (GachaController)AppController.Instance.GetController(Controller.GACHA);  
			gachaCtrler.SpinGacha(npcAsset);
		}
		if (callback != null){
			callback();
		}
	}
	
	void FixedUpdate()
	{
		if(isShowingDialog && dialogDone == false)
		{
			if(Time.time > nextTextTime)
			{
				if(textIndex >= currentText.Length)
				{
					dialogDone = true;
					Invoke ("AllowContinue", completeDialogDelay);
				}
				else
				{
					dialog.text = dialog.text + currentText[textIndex];
					textIndex++;
					nextTextTime = Time.time + scrollSpeed;
        }
      }
    }
    else if(isShowingDialog && continueLabel.enabled == true && Input.GetKeyDown(KeyCode.Space))
    {
      CloseDialog();
    }
  }
  
}
