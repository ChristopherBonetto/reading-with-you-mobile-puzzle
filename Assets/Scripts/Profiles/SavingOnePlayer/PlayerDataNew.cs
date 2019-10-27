using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDataNew
{
    public string playerName = "";

    public bool[] easyLevels;
    public bool[] hardLevels;



    public PlayerDataNew(GameManager player)
    {
        playerName = player.m_playerName;

        easyLevels = player.easyLevels.ToArray();
        Debug.Log(easyLevels.Length);
        hardLevels = player.hardLevels.ToArray();
        Debug.Log(hardLevels.Length);
    }

    
}
