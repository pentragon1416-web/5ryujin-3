using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReplayManager : MonoBehaviour
{
    public MoveDataLoader moveDataLoader;
    public List<MoveData> moveDataList;
    public int currentIndex = -1;
    private int listLength = 0;
    private bool isInitialized = false;
    void OnEnable()
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
        currentIndex = -1;
        moveDataList = list;
        listLength = list.Count;
        isInitialized = true;
    }


    public void Next()
    {
        if (currentIndex < listLength-1)
        {
            currentIndex++;
            Debug.Log(currentIndex);
            moveDataLoader.LoadMoveDataFromIndex(moveDataList, currentIndex);
        }

    }

    public void Prev()
    {
        if (currentIndex > -1)
        {
            currentIndex--;
            if(currentIndex == -1)
            {
                moveDataLoader.Reset();
            }
            else
            {
                moveDataLoader.LoadMoveDataFromIndex(moveDataList, currentIndex);
            }
        }
    }

    public void ResetReplay()
    {
        currentIndex = -1;
        moveDataLoader.Reset();
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

    public int GetCurrentIndex(){
        return currentIndex;
    }

    public int GetListLength(){
        return listLength;
    }

    public bool Initialized(){
        return isInitialized;
    }
}
