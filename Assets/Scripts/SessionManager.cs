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
    }
}

[Serializable]
public struct RoomSettings
{
    public int roomName;
    public int turnTime;
}