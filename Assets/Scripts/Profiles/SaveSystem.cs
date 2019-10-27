using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class SaveSystem
{
    public static bool SaveGame(SaveGame saveGame, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Create))
        {
            try
            {
                formatter.Serialize(stream, saveGame);
            }
            catch (Exception)
            {
                return false;
            }
        }

        return true;
    }

    public static SaveGame LoadGame(string name)
    {
        if (!DoesSaveGameExist(name))
        {
            return null;
        }

        BinaryFormatter formatter = new BinaryFormatter();

        using (FileStream stream = new FileStream(GetSavePath(name), FileMode.Open))
        {
            try
            {
                return formatter.Deserialize(stream) as SaveGame;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public static bool DeleteSaveGame(string name)
    {
        try
        {
            File.Delete(GetSavePath(name));
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    public static bool DoesSaveGameExist(string name)
    {
        return File.Exists(GetSavePath(name));
    }

    private static string GetSavePath(string name)
    {
        return Path.Combine(Application.persistentDataPath, name + ".sav");
    }
}

    //public static void Save(SavingTest player)
    //{
    //    BinaryFormatter formatter = new BinaryFormatter();

    //    string path = Application.persistentDataPath + "/player.fun";
    //    FileStream stream = new FileStream(path, FileMode.Create);

    //    PlayerData data = new PlayerData(player);

    //    formatter.Serialize(stream, data);
    //    stream.Close();
    //}

    //public static PlayerData Load()
    //{
    //    string path = Application.persistentDataPath + "/player.fun";

    //    if (File.Exists(path))
    //    {
    //        BinaryFormatter formatter = new BinaryFormatter();
    //        FileStream stream = new FileStream(path, FileMode.Open);

    //        PlayerData data = formatter.Deserialize(stream) as PlayerData;
    //        stream.Close();

    //        return data;
    //    }
    //    else
    //    {
    //        Debug.Log("save file not found");
    //        return null;
    //    }
    //}

