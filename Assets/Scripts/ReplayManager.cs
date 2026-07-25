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
        SetMoveDataList(GameRecord.GetMoveDataList());
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
