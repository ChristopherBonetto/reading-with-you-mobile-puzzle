using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystemNew
{
    public static void Save(GameManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerDataNew data = new PlayerDataNew(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerDataNew Load()
    {
        string path = Application.persistentDataPath + "/player.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerDataNew data = formatter.Deserialize(stream) as PlayerDataNew;
            stream.Close();

            return data;
        }
        else
        {
            Debug.Log("save file not found");
            return null;
        }
    }
}
