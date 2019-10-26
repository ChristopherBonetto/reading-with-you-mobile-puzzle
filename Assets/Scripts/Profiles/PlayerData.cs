using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{    
    public bool[] LOL;

    public PlayerData(SavingTest player)
    {
        LOL = new bool[10];

        LOL[0] = player.ciao;
        LOL[1] = player.miao;
        LOL[2] = player.wow;
    }
}
