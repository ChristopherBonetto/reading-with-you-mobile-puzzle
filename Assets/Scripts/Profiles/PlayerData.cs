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

    public int HighScore { get; set; }

    public AccountButtons buttonRef = null;

    [NonSerialized]
    public string secret = "Nope";
}
