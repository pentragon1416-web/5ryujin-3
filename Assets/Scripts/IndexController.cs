using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndexController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private TextMeshProUGUI lengthText;
    [SerializeField] private ReplayManager replayManager;

    public int currentIndexForInput = 0;
    public bool updatable = false;

    void Start(){
        StartCoroutine(WaitReplayManagerInitialized());
    }

    void Update(){
        if(!updatable) return;
        indexText.text = $"{replayManager.GetCurrentIndex() + 1}";
    }

    private IEnumerator WaitReplayManagerInitialized()
    {
        while(!replayManager.Initialized()){
            yield return new WaitForSeconds(0.1f);
        }
        Initialize();
        Debug.Log("IndexController初期化完了。");
    }

    public void Initialize(){
        lengthText.text = replayManager.GetListLength().ToString();
        updatable = true;
    }

    public void SetIndexText(string indexText){
        if (int.TryParse(indexText, out int index))
        {
            if(index >= 0 && index <= replayManager.GetListLength()){
                currentIndexForInput = index;
            }
        }
    }

    public void Select(){
        Debug.Log($"再生されるのは{currentIndexForInput}");
        replayManager.ReplayFromIndex(currentIndexForInput - 1);
    }
}
