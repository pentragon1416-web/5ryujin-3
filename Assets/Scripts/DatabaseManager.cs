using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{

    private Record currentRecord;

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private static extern void OpenDatabase();

    [DllImport("__Internal")]
    private static extern void SaveRecord(string json);

    [DllImport("__Internal")]
    private static extern void LoadRecord(string id);

#endif

    public void Initialize()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        OpenDatabase();
#endif
    }

    public void Save(Record record)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        string json = JsonUtility.ToJson(record);
        SaveRecord(json);

#endif
    }

    public void Load(string id)
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        LoadRecord(id);
#endif
    }

    public void OnRecordLoaded(string json)
    {
        currentRecord = JsonUtility.FromJson<Record>(json);
    }

    public string GetCurrentId()
    {
        return currentRecord?.id;
    }

    public string GetCurrentName()
    {
        return currentRecord?.name;
    }

    public List<MoveData> GetCurrentMdList()
    {
        if (currentRecord == null)
            return null;

        MoveDataList moveDataList =
            JsonUtility.FromJson<MoveDataList>(currentRecord.mdList);

        List<MoveData> moves = moveDataList.moves;

        return moves;
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