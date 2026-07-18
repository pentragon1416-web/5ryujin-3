using System;
using System.Collections.Generic;

//シングルトンのMonoBehaviourにしてからシーンの間をたらいまわしすることで機能させる。
[Serializable]
public class GameRecord
{
    public List<MoveData> moves = new List<MoveData>();
}