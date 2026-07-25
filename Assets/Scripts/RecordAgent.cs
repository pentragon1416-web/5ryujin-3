using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

// RecordManagerはとにかくデータベースのアクセスのみを役割とする。
// RecordManagerは非同期処理を扱うので、Startのみをエントリーポイントとして外部からメソッドを触れさせない。
public class RecordAgent : MonoBehaviour
{
    public KifuListUI kifuListUI;
    void Start()
    {
#if !UNITY_WEBGL
        return;
#endif
        // これ一つだけを実行する。
        AsyncFunctions().Forget();
    }

    //ここに非同期的な処理はすべてまとめる。
    private async UniTask AsyncFunctions()
    {
        await DatabaseManager.Instance.InitializeAsync();
        RecordMode mode = GameRecord.GetMode();
        switch (mode)
        {
            case RecordMode.Record:
                await ForRecord();
            break;
            case RecordMode.Normal:
                await ForNormal();
            break;
            case RecordMode.Replay:
                await ForReplay();
            break;
            default:
                await UniTask.CompletedTask;
            break;
        }
        await BuildKifuUI();
    }

    private async UniTask ForRecord()
    {
        if (DatabaseManager.Instance == null)
        {
            Debug.LogError("DatabaseManager が見つからないため、棋譜を保存できません。");
            return;
        }

        MoveDataList moveDataList = new MoveDataList
        {
            moves = GameRecord.GetMoveDataList()
        };
        List<RecordSummary> rsList = await DatabaseManager.Instance.LoadRecordSummaryListAsync();
        GameRecord.UpdateDate();
        GameRecord.SetId($"KIFU_{rsList.Count}");

        Record record = new Record
        {
            id = GameRecord.GetId(),
            name = GameRecord.GetDate(),
            mdList = JsonUtility.ToJson(moveDataList)
        };

        await DatabaseManager.Instance.SaveAsync(record);
    }

    private async UniTask ForNormal()
    {
        await UniTask.CompletedTask;
    }

    private async UniTask ForReplay()
    {
        await UniTask.CompletedTask;
    }

    private async UniTask BuildKifuUI()
    {
        List<RecordSummary> rsList = await DatabaseManager.Instance.LoadRecordSummaryListAsync();
        kifuListUI.CreateList(rsList);
    }
}
