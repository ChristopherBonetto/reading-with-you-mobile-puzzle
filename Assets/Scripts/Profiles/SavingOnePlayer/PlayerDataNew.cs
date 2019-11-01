using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataNew
{
    public string PlayerName = "";

    public bool[] EasyLevels;
    public bool[] HardLevels;



    public PlayerDataNew(GameManager player)
    {
        PlayerName = player.m_playerName;

        EasyLevels = player.easyLevels.ToArray();

        HardLevels = player.hardLevels.ToArray();
        
    }

    
}
