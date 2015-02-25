using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using NPNF.Core;
using NPNF.Core.Users;
using NPNF.Currency;
using System.Collections.Generic;

public class CurrencyController : MonoBehaviour
{
    public Text currencyLabel;
    public bool needsUpdate;
    private int balance;
    private const string currencyName = "Coin";

    // Use this for initialization
    void Start()
    {
        needsUpdate = false;
        GetBalance();
        balance = 0;
        currencyLabel.text = balance.ToString();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (needsUpdate)
        {
            currencyLabel.text = balance.ToString();
        }
    }

    private void GetBalance()
    {
        // npnf feature: Check balance for current user
        User.CurrentProfile.CurrencyBank.GetBalance(currencyName, (BankReceipt receipt, NPNFError error) => {
            if (error == null)
            {
                balance = receipt.Balance;
                needsUpdate = true;
            } else
            {
                Debug.LogError(error);
            }
        });
    }

    public void Credit(int amount)
    {
        // npnf feature: Credit user an amount of a currency
        User.CurrentProfile.CurrencyBank.Credit(currencyName, amount, (BankReceipt receipt, NPNFError error) => {
            if (error == null)
            {
                balance = receipt.Balance;
                needsUpdate = true;
            } else
            {
                Debug.LogError(error);
            }
        });
    }

    public void Debit(int amount)
    {
        // npnf feature: Debit user an amount of a currency
        User.CurrentProfile.CurrencyBank.Debit(currencyName, amount, (BankReceipt receipt, NPNFError error) => {
            if (error == null)
            {
                balance = receipt.Balance;
                needsUpdate = true;
            } else
            {
                Debug.LogError(error);
            }
        });
    }

    public void Convert(int amount)
    {
        Currency.GetByName(currencyName, (Currency currency, NPNFError error) => {
            // npnf feature: Convert an amount of a currency into an amount of energy
            User.CurrentProfile.CurrencyBank.Convert(currencyName, 1, currency.ExchangeRules [0], (Dictionary<string, BankReceipt> receiptDict, NPNFError convertError) => {
                if (convertError == null)
                {
                    balance = receiptDict [currency.Id].Balance;
                    needsUpdate = true;
                } else
                {
                    // Insufficient fund
                    Debug.LogWarning("Convert Failed: " + error);
                    AppController.Instance.IsNetworkError(error); 
                }
            });
        });
    }
}
