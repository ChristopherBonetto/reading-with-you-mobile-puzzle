using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystemNew
{
    /// <summary>
    /// Save the gamemanager in one new PlayerDataNew into a file in a prefixed location in the memory.
    /// </summary>
    public static void Save(GameManager player)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string path = Application.persistentDataPath + "/player.fun";
        
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerDataNew data = new PlayerDataNew(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// Search in the path if exist a rescue. If it finds the file return him, if not take all levels boolean and save them.
    /// </summary>
    public static PlayerDataNew Load()
    {
        string path = Application.persistentDataPath + "/player.fun";

        if (File.Exists(path))
        {
            Debug.Log("exist saved file" + path);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerDataNew data = formatter.Deserialize(stream) as PlayerDataNew;
            stream.Close();

            return data;
        }
        else
        {
            GameManager.Instance.SetWorldBooleans();
            Save(GameManager.Instance);
            return null;
        }
    }
}
