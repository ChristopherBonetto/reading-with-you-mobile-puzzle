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
        Debug.Log(EasyLevels.Length);
        HardLevels = player.hardLevels.ToArray();
        Debug.Log(HardLevels.Length);
    }

    
}
