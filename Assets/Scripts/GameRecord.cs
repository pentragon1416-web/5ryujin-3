using System;
using System.Collections.Generic;

//シングルトンのMonoBehaviourにしてからシーンの間をたらいまわしすることで機能させる。
[Serializable]
public class GameRecord
{
    public List<MoveData> moves = new List<MoveData>();
}

// using System;
// using System.Collections.Generic;

// public static class GameRecord
// {
//     private static bool shouldRecord = false;
//     private static string date = "";
//     private static List<MoveData> moveDataList = new List<MoveData>();

//     // shouldRecord
//     public static void SetShouldRecord(bool value)
//     {
//         shouldRecord = value;
//     }

//     public static bool GetShouldRecord()
//     {
//         return shouldRecord;
//     }

//     // date
//     public static void UpdateDate()
//     {
//         string updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//         date = updateDate;
//     }

//     public static string GetDate()
//     {
//         return date;
//     }

//     // moveDataList
//     public static void SetMoveDataList(List<MoveData> value)
//     {
//         moveDataList = value;
//     }

//     public static List<MoveData> GetMoveDataList()
//     {
//         return moveDataList;
//     }
// }