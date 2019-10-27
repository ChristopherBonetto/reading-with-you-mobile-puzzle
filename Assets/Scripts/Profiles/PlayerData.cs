using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public abstract class SaveGame
{

}


[System.Serializable]
public class PlayerData : SaveGame
{
    public string playerName = "";

    public bool[] easyLevels;
    public bool[] hardLevels;
        
}
