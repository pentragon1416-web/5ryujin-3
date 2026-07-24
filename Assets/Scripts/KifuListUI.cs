using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class KifuListUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    [Header("移動先のMainシーン名")]
    public string mainSceneName = "MainScene";

    public void CreateList(List<RecordSummary> recordSummaries)
    {
        foreach (RecordSummary recordSummary in recordSummaries)
        {
            CreateRecordButton(recordSummary.id, recordSummary.name);
        }
    }

    public void CreateRecordButton(string id, string name){
        GameObject obj = Instantiate(buttonPrefab, parent);
        TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = id + " " + name;
        }
        Button btn = obj.GetComponent<Button>();
        if (btn == null)
        {
            Debug.LogError("buttonPrefab に Button コンポーネントがありません。");
            return;
        }

        btn.onClick.AddListener(() =>
        {
            LoadRecordAsync(id).Forget();
        });
    }

    private async UniTask LoadRecordAsync(string id)
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager が見つからないため、棋譜を読み込めません。");
            return;
        }

        try
        {
            await DatabaseManager.Instance.InitializeAsync();
            Record record = await DatabaseManager.Instance.LoadAsync(id);
            MoveDataList moveDataList = JsonUtility.FromJson<MoveDataList>(record.mdList);

            GameRecord.ResetMoveDataList();

            if (moveDataList?.moves != null)
            {
                GameRecord.SetMoveDataList(moveDataList.moves);
            }

            SceneManager.LoadScene(mainSceneName);
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"棋譜の読み込みに失敗しました: {exception.Message}");
        }
    }
}
