using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SavingTest : MonoBehaviour
{
    
    //public AccountButtons[] savedAccount;

    public string Nome;
    
    

    private void Update()
    {
        
    }

    public void SaveAccount()
    {
        //if(savedAccount[0].accountSaved == null)
        //{
        //    savedAccount[0].accountSaved = new PlayerData();
        //    savedAccount[0].accountSaved.playerName = Nome;
        //    //savedAccount[0].accountSaved.easyLevels = easyLevels;
        //    //savedAccount[0].accountSaved.hardLevels = hardLevels;
        //    savedAccount[0].changeTextButton(Nome);
        //    SaveSystem.SaveGame(savedAccount[0].accountSaved, Nome);
            
        //}
        //else if(savedAccount[1].accountSaved == null)
        //{
        //    savedAccount[1].accountSaved = new PlayerData();
        //    savedAccount[1].accountSaved.playerName = Nome;
        //    //savedAccount[1].accountSaved.easyLevels = easyLevels;
        //    //savedAccount[1].accountSaved.hardLevels = hardLevels;
        //    savedAccount[1].changeTextButton(Nome);
        //    SaveSystem.SaveGame(savedAccount[1].accountSaved, Nome);
            
        //}
        //else if(savedAccount[2].accountSaved == null)
        //{
        //    savedAccount[2].accountSaved = new PlayerData();
        //    savedAccount[2].accountSaved.playerName = Nome;
        //    //savedAccount[2].accountSaved.easyLevels = easyLevels;
        //    //savedAccount[2].accountSaved.hardLevels = hardLevels;
        //    savedAccount[2].changeTextButton(Nome);
        //    SaveSystem.SaveGame(savedAccount[2].accountSaved, Nome);
            
        //}
        //else
        //{
        //    Debug.Log("no more slot");
        //}
        
    }

    public void LoadAccount(int accountPosition)
    {
        //PlayerData loadGame = SaveSystem.LoadGame(savedAccount[accountPosition].accountSaved.playerName) as PlayerData;

        //if(loadGame != null)
        //{
        //    Nome = loadGame.playerName;
        //    //easyLevels = loadGame.easyLevels;
        //    //hardLevels = loadGame.hardLevels;
        //}
        //else
        //{
        //    Debug.Log("u must create account to load");
        //}
        
    }

    public void DeleteAccount()
    {
        
         SaveSystem.DeleteSaveGame("MySaveGame");
    }
}
