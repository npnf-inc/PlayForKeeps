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
using NPNF.Collections;

public class PopupPrefab : MonoBehaviour
{
    public Text text;
    public GameObject location;
    public float scale;
    private CollectionCore collectionCore;
    GameObject iconObject;
    string assetName;
    bool isShown;

    void Awake()
    {
        text.text = "";
        gameObject.SetActive(false);
        isShown = false;
        GameObject obj = GameObject.Find("CollectionCore");
        collectionCore = obj.GetComponent<CollectionCore>();
    }
    
    public void SetPrefab(string prefabName, string assetName)
    {
        if (iconObject != null)
        {
            Destroy(iconObject);
        }

        gameObject.SetActive(true);
        text.text = "Obtained: " + assetName + "!";
        this.assetName = assetName;

        GameObject prefab = (GameObject)Resources.Load("icons/" + prefabName);
        iconObject = (GameObject)Instantiate(prefab, location.transform.position, Quaternion.identity);
        iconObject.transform.localScale = iconObject.transform.localScale * scale;
        iconObject.transform.parent = transform;

        GameObject go = GameObject.Find("Player");
        go.GetComponent<CharacterMovement>().canMove = false;
        isShown = true;
    }

    void Update()
    {
        if (isShown && Input.GetKeyDown(KeyCode.Space))
        {
            GameObject go = GameObject.Find("Player");
            go.GetComponent<CharacterMovement>().canMove = true;
            if (assetName == "pitch")
            {
                Asset info = collectionCore.GetAssetInfoByName("pitch");
                string[] pitches = ((string)(info.GetCustom("title", ""))).Split(";" [0]);
                string pitch = pitches [Random.Range(0, pitches.Length)];
                GameObject obj = GameObject.Find("MainDialog");
                
                MainDialog dialog = obj.GetComponent<MainDialog>();
                dialog.ShowDialog("Now go show Woo your pitch for " + pitch + "!", null, null);

                GameController gameCtrler = (GameController)AppController.Instance.GetController(Controller.GAME);  
                gameCtrler.hasPitch = true;
            }

            isShown = false;
            gameObject.SetActive(false);
            Destroy(iconObject);
        }
    }
}
