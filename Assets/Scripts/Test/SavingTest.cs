using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingTest : MonoBehaviour
{
    public AccountButtons[] savedAccount;

    public string Nome;
    public int Valore;
    public string Valore2;

    public void SaveAccount()
    {
        if(savedAccount[0].accountSaved == null)
        {
            savedAccount[0].accountSaved = new PlayerData();
            savedAccount[0].accountSaved.playerName = Nome;
            savedAccount[0].accountSaved.HighScore = Valore;
            savedAccount[0].accountSaved.secret = Valore2;
            savedAccount[0].changeTextButton(Nome);
            savedAccount[0].accountSaved.buttonRef = savedAccount[0];
            SaveSystem.SaveGame(savedAccount[0].accountSaved, Nome);
        }
        else if(savedAccount[1].accountSaved == null)
        {
            savedAccount[1].accountSaved = new PlayerData();
            savedAccount[1].accountSaved.playerName = Nome;
            savedAccount[1].accountSaved.HighScore = Valore;
            savedAccount[1].accountSaved.secret = Valore2;
            savedAccount[1].changeTextButton(Nome);
            savedAccount[1].accountSaved.buttonRef = savedAccount[1];
            SaveSystem.SaveGame(savedAccount[1].accountSaved, Nome);
        }
        else if(savedAccount[2].accountSaved == null)
        {
            savedAccount[2].accountSaved = new PlayerData();
            savedAccount[2].accountSaved.playerName = Nome;
            savedAccount[2].accountSaved.HighScore = Valore;
            savedAccount[2].accountSaved.secret = Valore2;
            savedAccount[2].changeTextButton(Nome);
            savedAccount[2].accountSaved.buttonRef = savedAccount[2];
            SaveSystem.SaveGame(savedAccount[2].accountSaved, Nome);
        }
        else
        {
            Debug.Log("no more slot");
        }
        
        
        //mySaveGame1.playerName = "Ryan";
        //mySaveGame1.HighScore = 1000000;
        //mySaveGame1.secret = Random.Range(0, 1000).ToString();
        //SaveSystem.SaveGame(mySaveGame1, "MySaveGame"); // Saves as MySaveGame.sav
    }

    public void LoadAccount(int accountPosition)
    {
        if(savedAccount[accountPosition].accountSaved != null)
        {
            PlayerData mySaveGame2 = SaveSystem.LoadGame(savedAccount[accountPosition].accountSaved.playerName) as PlayerData;

            if (mySaveGame2 != null)
            {
                Debug.Log(mySaveGame2.playerName); // Will log Ryan
                Debug.Log(mySaveGame2.HighScore);  // Will log 1000000
                Debug.Log(mySaveGame2.secret);
            }
            else
            {
                Debug.Log("u must create account to load");
            }
        }
    }

    public void DeleteAccount(string accountName)
    {
        //Deleting a saved game.
         SaveSystem.DeleteSaveGame(accountName);
    }
}
