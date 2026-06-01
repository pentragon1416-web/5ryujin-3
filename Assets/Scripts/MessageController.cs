using UnityEngine;

public class MessageController : MonoBehaviour
{
    [Header("Message Objects")]
    public GameObject messagePanelObj;
    public GameObject messageTextObj;
    public GameObject goTitleButtonObj;
    [Header("Message Text")]
    public TMPro.TextMeshProUGUI messageText;

    public void ShowMessage(string msg)
    {
        messageText.text = msg;
        messagePanelObj.SetActive(true);
        messageTextObj.SetActive(true);
        goTitleButtonObj.SetActive(false);
    }

    public void ShowMessageWithGoTitleButton(string msg)
    {
        messageText.text = msg;
        messagePanelObj.SetActive(true);
        messageTextObj.SetActive(true);
        goTitleButtonObj.SetActive(true);
    }

    public void HideMessage()
    {
        messagePanelObj.SetActive(false);
        messageTextObj.SetActive(false);
        goTitleButtonObj.SetActive(false);
    }
}