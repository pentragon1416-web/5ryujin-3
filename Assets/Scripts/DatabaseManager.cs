using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class DatabaseManager
{

    public static Record currentRecord;

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void OpenDatabase();

    [DllImport("__Internal")]
    private static extern void SaveRecord(string json);

    [DllImport("__Internal")]
    private static extern void LoadRecord(string id);

#endif

    public static void Initialize()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenDatabase();
#endif
    }

    public static void Save(Record record)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        string json = JsonUtility.ToJson(record);
        SaveRecord(json);

#endif
    }

    public static void Load(string id)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        LoadRecord(id);
#endif
    }
}

// 最終的にはここでまとめて保存。
[System.Serializable]
public class Record
{
    public string id;
    public string name;
    public string mdList;
}

// List<MoveData>を直接JSONにシリアライズできないのでここでラッパークラスを作りパース
[System.Serializable]
public class MoveDataList
{
    public List<MoveData> moves;
}