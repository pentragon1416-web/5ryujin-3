using UnityEngine;
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public RoomSettings Settings;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Settings = new RoomSettings
        {
            roomNumber = "0",
            turnTime = 45
        };
    }

    public void SetRoomSettings(RoomSettings roomSettings)
    {
        this.Settings = roomSettings;
    }
    public void SetRoomNumber(string n)
    {
        this.Settings.roomNumber = n;
    }
    public void SetTurnTime(int t)
    {
        this.Settings.turnTime = t;
    }

    public string GetRoomNumber()
    {
        return Settings.roomNumber;
    }
}

public struct RoomSettings
{
    public string roomNumber;
    public int turnTime;
}