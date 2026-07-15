using UnityEngine;
using TMPro;

public class RoomNumber : MonoBehaviour
{
    public TextMeshProUGUI tmp;
    void Start()
    {
        tmp.text = SessionManager.Instance.Settings.roomNumber;
    }
}