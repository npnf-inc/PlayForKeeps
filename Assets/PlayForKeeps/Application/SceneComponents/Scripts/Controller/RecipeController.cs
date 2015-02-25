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
using System.Collections.Generic;
using NPNF.Collections;
using NPNF.Core;
using NPNF.Core.Users;

public class Recipe
{
    public string formulaName;
    public Dictionary<string, int> ingredients;
    public string outcome;
    public bool isDone = false;

    public bool isReady
    {
        get
        {
            int total = 0;
            foreach (int count in ingredients.Values)
            {
                total += count;
            }
            return (total == 3);
        }
    }

    public Recipe(Dictionary<string, int> ingredients, string outcome, string formulaName)
    {
        this.ingredients = ingredients;
        this.outcome = outcome;
        this.formulaName = formulaName;
    }
}

/*
 * add and remove entitlements for fusion manually. Replace with fusion formulas in future
 */
public class RecipeController : MonoBehaviour
{
    List<Recipe> recipes;
    private CollectionCore collectionCore;

    void Awake()
    {
        recipes = new List<Recipe>();
        Dictionary<string, int> dict1 = new Dictionary<string, int>();
        dict1 ["morale"] = 0;
        dict1 ["funding"] = 0;
        dict1 ["strength"] = 0;
        Recipe recipe1 = new Recipe(dict1, "pitch", "ConvertToPitch");
        recipes.Add(recipe1);

        Dictionary<string, int> dict2 = new Dictionary<string, int>();
        dict2 ["cup_ramen"] = 0;
        dict2 ["catered_lunch"] = 0;
        dict2 ["philz_coffee"] = 0;
        Recipe recipe2 = new Recipe(dict2, "strength", "ConvertToStrength");
        recipes.Add(recipe2);

        Dictionary<string, int> dict3 = new Dictionary<string, int>();
        dict3 ["woos_wallet"] = 0;
        dict3 ["chris_credit_card"] = 0;
        dict3 ["expense_account"] = 0;
        Recipe recipe3 = new Recipe(dict3, "funding", "ConvertToFunding");
        recipes.Add(recipe3);

        Dictionary<string, int> dict4 = new Dictionary<string, int>();
        dict4 ["music"] = 0;
        dict4 ["cat_videos"] = 0;
        dict4 ["happy_hour"] = 0;
        Recipe recipe4 = new Recipe(dict4, "morale", "ConvertToMorale");
        recipes.Add(recipe4);
    }

    public string OnNewInventory(string assetName)
    {
        foreach (Recipe recipe in recipes)
        {
            if (!recipe.isDone)
            {
                if (recipe.outcome.Equals(assetName))
                {
                    recipe.isDone = true;
                    return null;
                } else if (recipe.ingredients.ContainsKey(assetName))
                {
                    if (recipe.ingredients [assetName] == 0)
                    {
                        recipe.ingredients [assetName] = 1;
                        if (recipe.isReady)
                        {
                            return recipe.outcome;
                        }
                    }
                }
            }
        }
        return null;
    }

    public void CheckRecipe()
    {
        Recipe recipe = FindReadyRecipe();
        if (recipe != null)
        {
            GameObject obj = GameObject.Find("CollectionCore");
            collectionCore = obj.GetComponent<CollectionCore>();

            Dictionary<string, ConversionFormula> formulas = collectionCore.GetFormulas();
            ConversionFormula formulaToUse = formulas [recipe.formulaName];

            User.CurrentProfile.Entitlements.ApplyConversion(formulaToUse.Id, (ConversionResult result, NPNFError error) => {
                if (error == null)
                {
                    InventoryController ctrler = (InventoryController)AppController.Instance.GetController(Controller.INVENTORY);  

                    foreach (Entitlement entitlement in result.Added)
                    {
                        ctrler.OnNewInventory(entitlement.Id);
                    }
                    recipe.isDone = true;
                } else if (error != null)
                {
                    Debug.LogError("Conversion failed" + error);
                    AppController.Instance.IsNetworkError(error); 
                }
            });
        }
    }

    private Recipe FindReadyRecipe()
    {
        foreach (Recipe recipe in recipes)
        {
            if (recipe.isReady && !recipe.isDone)
            {
                return recipe;
            }
        }
        return null;
    }
}
