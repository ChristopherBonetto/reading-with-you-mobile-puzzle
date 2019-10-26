using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavingTest : MonoBehaviour
{
    public bool ciao = false;
    public bool miao = false;
    public bool wow = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            SaveTest();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LoadTest();
        }
    }


    public void SaveTest()
    {
        SaveSystem.Save(this);
        Debug.Log(ciao + " " + miao + " " + wow);
    }

    public void LoadTest()
    {
        PlayerData data = SaveSystem.Load();

        ciao = data.LOL[0];
        miao = data.LOL[1];
        wow = data.LOL[2];

        Debug.Log(ciao + " " + miao + " " + wow);

    }
}
