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
            roomNumber = 0,
            turnTime = 45
        };
    }

    public void SetRoomSettings(RoomSettings roomSettings)
    {
        this.Settings = roomSettings;
    }
}

public struct RoomSettings
{
    public int roomNumber;
    public int turnTime;
}