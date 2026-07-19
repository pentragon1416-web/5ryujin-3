using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class KifuListWrapper
{
    public List<KifuEntry> list;
}

[System.Serializable]
public class KifuEntry
{
    public string name;
    public GameRecord record;
}

public class KifuListUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform parent;

    [Header("移動先のMainシーン名")]
    public string mainSceneName = "MainScene";

    void Start()
    {
        CreateList();
    }

    private string FilePath(){
        string path = Path.Combine(Application.persistentDataPath, "kifu.json");
        Debug.Log(path);

        if (!File.Exists(path))
        {
            KifuListWrapper emptyWrapper = new KifuListWrapper
            {
                list = new List<KifuEntry>()
            };

            string emptyJson = JsonUtility.ToJson(emptyWrapper, true);
            File.WriteAllText(path, emptyJson);
            Debug.LogWarning("kifu.json が見つかりません。");
        }

        return path;
    }

    void CreateList()
    {
        if (buttonPrefab == null || parent == null)
        {
            Debug.LogError("KifuListUI の ButtonPrefab または Parent が入っていません。");
            return;
        }

        string path = FilePath();

        string json = File.ReadAllText(path);
        KifuListWrapper wrapper = JsonUtility.FromJson<KifuListWrapper>(json);

        if (wrapper == null || wrapper.list == null || wrapper.list.Count == 0)
        {
            Debug.LogWarning("棋譜データがありません。");
            return;
        }

        foreach (KifuEntry entry in wrapper.list)
        {
            if (entry == null || entry.record == null)
            {
                continue;
            }

            GameObject obj = Instantiate(buttonPrefab, parent);

            TextMeshProUGUI text = obj.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
            {
                text.text = entry.name;
            }

            Button btn = obj.GetComponent<Button>();
            if (btn == null)
            {
                Debug.LogError("buttonPrefab に Button コンポーネントがありません。");
                continue;
            }

            GameRecord selectedRecord = entry.record;

            btn.onClick.AddListener(() =>
            {
                KifuReplayContext.Set(selectedRecord);
                SceneManager.LoadScene(mainSceneName);
            });
        }
    }
}