using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{

    private Record currentRecord;

    private List<RecordSummary> rsList;

#if UNITY_WEBGL && !UNITY_EDITOR

    [DllImport("__Internal")]
    private extern void OpenDatabase();

    [DllImport("__Internal")]
    private extern void SaveRecord(string json);

    [DllImport("__Internal")]
    private extern void LoadRecord(string id);

    [DllImport("__Internal")]
    private extern void LoadRecordList();

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

    public void LoadRecordSummaryList()
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        LoadRecordList();
#endif
    }

    public void OnRecordLoaded(string json)
    {
        currentRecord = JsonUtility.FromJson<Record>(json);
    }

    public void OnRecordListLoaded(string json)
    {
        RecordSummaryList wrapper =
            JsonUtility.FromJson<RecordSummaryList>(json);

        rsList = wrapper.records;
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

        return moveDataList.moves;
    }

    public List<RecordSummary> GetRecordList()
    {
        return rsList;
    }
}

// 一覧を取得するためのラッパークラス
[System.Serializable]
public class RecordSummary
{
    public string id;
    public string name;
}

[System.Serializable]
public class RecordSummaryList
{
    public List<RecordSummary> records;
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