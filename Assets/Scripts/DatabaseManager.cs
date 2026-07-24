using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{

    public static DatabaseManager Instance { get; private set; }
    private Record currentRecord;

    private List<RecordSummary> rsList;

    private UniTaskCompletionSource databaseOpenTcs;
    private UniTaskCompletionSource savedTcs;
    private UniTaskCompletionSource<Record> loadTcs;
    private UniTaskCompletionSource<List<RecordSummary>> loadListTcs;

    private bool isInitialized = false;
    private bool isSaving = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

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

    public async UniTask InitializeAsync()
    {
        if(isInitialized) return;
#if UNITY_WEBGL && !UNITY_EDITOR
        databaseOpenTcs = new UniTaskCompletionSource();
        OpenDatabase();
        await databaseOpenTcs.Task;
#else
        await UniTask.CompletedTask;
#endif
        isInitialized = true;
    }

    public async UniTask SaveAsync(Record record)
    {
        if(isSaving) return;
#if UNITY_WEBGL && !UNITY_EDITOR
        isSaving = true;
        savedTcs = new UniTaskCompletionSource();
        string json = JsonUtility.ToJson(record);
        SaveRecord(json);
        await savedTcs.Task;
        isSaving = false;
#else
        await UniTask.CompletedTask;
#endif
    }

    public async UniTask<Record> LoadAsync(string id)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        loadTcs = new UniTaskCompletionSource<Record>();

        LoadRecord(id);

        return await loadTcs.Task;
#else
        return null;
#endif
    }

    public async UniTask<List<RecordSummary>> LoadRecordSummaryListAsync()
    {
#if UNITY_WEBGL && !UNITY_EDITOR

        loadListTcs =
            new UniTaskCompletionSource<List<RecordSummary>>();

        LoadRecordList();

        return await loadListTcs.Task;
#else
        return null;
#endif
    }

    // 以下Storage.jslibからのコールバックメソッド
    public void OnDatabaseOpened()
    {
        databaseOpenTcs?.TrySetResult();
    }

    public void OnDatabaseOpenFailed()
    {
        databaseOpenTcs?.TrySetException(
            new System.Exception("Database open failed")
        );
    }

    public void OnRecordSaved()
    {
        savedTcs?.TrySetResult();
    }

    public void OnRecordSaveFailed()
    {
        savedTcs?.TrySetException(
            new System.Exception("Record save failed"));
    }

    public void OnRecordLoaded(string json)
    {
        try
        {
            Record record =
                JsonUtility.FromJson<Record>(json);
            currentRecord = record;
            loadTcs?.TrySetResult(record);
        }
        catch(System.Exception e)
        {
            loadTcs?.TrySetException(e);
        }
    }

    public void OnRecordNotFound()
    {
        loadTcs?.TrySetException(
            new System.Exception("Record not found")
        );
    }

    public void OnRecordLoadFailed()
    {
        loadTcs?.TrySetException(
            new System.Exception("Record load failed")
        );
    }

    public void OnRecordListLoaded(string json)
    {
        RecordSummaryList wrapper =
            JsonUtility.FromJson<RecordSummaryList>(json);

        rsList = wrapper.records;

        loadListTcs?.TrySetResult(rsList);
    }

    // 以下取得用のメソッド
    public void OnRecordListLoadFailed()
    {
        loadListTcs?.TrySetException(
            new System.Exception("Record list load failed")
        );
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