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
using System;
using UnityEngine.UI;

/*
 * Manages game time, win/lose conditions
 */
public class GameController : MonoBehaviour {
	public Text timer;
	public GameObject fail;
	public GameObject success;
	public bool hasPitch;
	public float startTime = 50f;
	public bool gameEnded;
    	
	public float currentTime;
	// Use this for initialization
	void Start () {
		hasPitch = false;
		currentTime = startTime;
		gameEnded = false;		

    	int seconds = (int)(currentTime % 60);
		int minutes = (int)(currentTime / 60);
		string time = minutes.ToString("00") + ":" + seconds.ToString("00");
		timer.text = time;

		//uiRoot.SetActive(true);
		GameObject obj = GameObject.Find("MainDialog");
		MainDialog dialog = obj.GetComponent<MainDialog>();
		dialog.ShowDialog("Welcome to your first day at npnf! Woo expects your first game pitch in 3 minutes! Find the other npnfers to help you with your game pitch.", null, null);
	}
	
	void FixedUpdate () {
		if(currentTime > 0f)
		{
			currentTime -= Time.deltaTime;
			int seconds = (int)(currentTime % 60);
			int minutes = (int)(currentTime / 60);
			string time = minutes.ToString("00") + ":" + seconds.ToString("00");
			timer.text = time;
			
			if(currentTime <= 0f)
			{
				currentTime = 0f;
				timer.text = "0:00";
				EndGame(false);
     		}
		}
    }

	void Awake()
	{
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
            CurrencyController currencyCtrler = (CurrencyController)AppController.Instance.GetController(Controller.CURRENCY);  
			currencyCtrler.Convert(5);
		}

        if (Input.GetKeyDown(KeyCode.F))
        {
            RecipeController recipeCtrler = (RecipeController)AppController.Instance.GetController(Controller.RECIPE);  
            recipeCtrler.CheckRecipe();
        }
    }
    
    public void EndGame(bool won)
	{
		gameEnded = true;
		if(won)
		{
			Invoke ("GoToWin", 0.5f);
		}
		else
		{
			Invoke ("GoToLose", 0.5f);
		}
	}
	
	void GoToWin()
	{
   	 	Application.LoadLevel("GameSuccessScene");
  	}
  
  	void GoToLose()
  	{
    	Application.LoadLevel("GameFailScene");
	}
}
