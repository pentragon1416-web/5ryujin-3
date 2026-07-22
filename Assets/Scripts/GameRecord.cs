using System;
using System.Collections.Generic;

//シングルトンのMonoBehaviourにしてからシーンの間をたらいまわしすることで機能させる。


public static class GameRecord
{
    private static RecordMode mode = RecordMode.Normal;
    private static string id = "";
    private static string name = "";
    private static string date = "";
    private static List<MoveData> moveDataList = new List<MoveData>();

    public static void UpdateDate()
    {
        string updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        date = updateDate;
    }

    public static void SetId(string new_id){
        id = new_id;
    }

    public static void SetName(string new_name){
        name = new_name;
    }

    public static void ChangeModeTo(RecordMode new_mode){
        mode = new_mode;
    }

    public static string GetId()
    {
        return id;
    }

    public static string GetName(){
        return name;
    }

    public static RecordMode GetMode(){
        return mode;
    }

    // moveDataList
    public static void AddMoveData(MoveData md)
    {
        moveDataList.Add(md);
    }

    public static void ResetMoveDataList(){
        moveDataList = new List<MoveData>();
    }

    public static List<MoveData> GetMoveDataList()
    {
        return moveDataList;
    }
}

public enum RecordMode{
    Record,
    Normal,
    Replay,
}