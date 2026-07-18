using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour
{
    public MoveDataLoader moveDataLoader;
    public List<MoveData> moveDataList;
    public int currentIndex = 0;
    private int listLength = 0;
    void Start()
    {
        List<MoveData> mdList2 = new List<MoveData>
    {
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.P,
            rotation = 180,
            flipped = false,
            x = 32,
            y = 1,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.N,
            rotation = 0,
            flipped = false,
            x = 33,
            y = 4,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.Y,
            rotation = 0,
            flipped = false,
            x = 33,
            y = 9,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.I,
            rotation = 0,
            flipped = false,
            x = 33,
            y = 13,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.I,
            rotation = 0,
            flipped = false,
            x = 33,
            y = 18,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = true,
            pieceType = PieceType.P,
            rotation = 0,
            flipped = false,
            x = 15,
            y = 28,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.I,
            rotation = 0,
            flipped = false,
            x = 33,
            y = 23,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = true,
            pieceType = PieceType.X,
            rotation = 0,
            flipped = false,
            x = 15,
            y = 25,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = false,
            pieceType = PieceType.W,
            rotation = 270,
            flipped = false,
            x = 34,
            y = 27,
            touchdown = false
        },
        new MoveData
        {
            turn = 0,
            player = true,
            pieceType = PieceType.X,
            rotation = 0,
            flipped = false,
            x = 15,
            y = 22,
            touchdown = false
        }
    };
        SetMoveDataList(mdList2);
    }
    public void SetMoveDataList(List<MoveData> list)
    {
        moveDataList = list;
        listLength = list.Count;
        moveDataLoader.LoadMoveDataFromIndex(moveDataList, 0);
    }


    public void Next()
    {
        if (currentIndex >= 0 && currentIndex < listLength)
        {
            currentIndex++;
            moveDataLoader.LoadMoveDataFromIndex(moveDataList, currentIndex);
        }
    }

    public void Prev()
    {
        if (currentIndex >= 0 && currentIndex < listLength)
        {
            currentIndex--;
            moveDataLoader.LoadMoveDataFromIndex(moveDataList, currentIndex);
        }
    }

    public void ResetReplay()
    {
        currentIndex = 0;
        moveDataLoader.LoadMoveDataFromIndex(moveDataList, 0);
    }

    public void GoHome()
    {
        SceneManager.LoadScene("HomeScene");
    }
    public void ReplayFromIndex(int index)
    {
        currentIndex = index;
        moveDataLoader.LoadMoveDataFromIndex(moveDataList, index);
    }
}
