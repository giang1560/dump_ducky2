using System.IO;
using UnityEngine;
//vcmv
public static class SaveManager
{
    /// <summary>
    /// Save data to file or playerPrefs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    public static void SaveData<T>(this T data)
    {
        string json = JsonUtility.ToJson(data);
        //PlayerPrefs.SetString(typeof(T).ToString(), json);
        File.WriteAllText(Application.persistentDataPath + "/" + typeof(T).ToString() + ".json", json);
    }

    /// <summary>
    /// Load data from file or playerPrefs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static void LoadData<T>(ref T t)
    {
        string json = File.ReadAllText(Application.persistentDataPath + "/" + typeof(T).ToString() + ".json");
        //string json = PlayerPrefs.GetString(typeof(T).ToString());
        t = JsonUtility.FromJson<T>(json);
    }

    /// <summary>
    /// Delete data from file or playerPrefs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void DeleteData<T>()
    {
        //PlayerPrefs.DeleteKey(typeof(T).ToString());
        File.Delete(Application.persistentDataPath + "/" + typeof(T).ToString() + ".json");
    }

    /// <summary>
    /// Check if data exists.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool HasData<T>()
    {
        //return PlayerPrefs.HasKey(typeof(T).ToString());
        return File.Exists(Application.persistentDataPath + "/" + typeof(T).ToString() + ".json");
    }

    /// <summary>
    /// Delete file or playerPrefs.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static void DeletaData<T>()
    {
        //PlayerPrefs.DeleteKey(typeof(T).ToString());
        File.Delete(Application.persistentDataPath + "/" + typeof(T).ToString() + ".json");
    }
}
