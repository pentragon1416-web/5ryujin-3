using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class KifuListUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    [Header("移動先のMainシーン名")]
    public string mainSceneName = "MainScene";

    void Start()
    {
        if(GameRecord.GetMode() == RecordMode.Record){
            // ここに記録するときの処理を書く。
            GameRecord.ResetMoveDataList();
        }
        GameRecord.ChangeModeTo(RecordMode.Normal);
        CreateList();
    }

    void CreateList()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateRecordButton("KIF.010", "2026/07/11");
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
            // ここにGameRecordの記録を入れる。
            SceneManager.LoadScene(mainSceneName);
        });
    }
}