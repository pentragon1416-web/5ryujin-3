using UnityEngine;
public class SessionManager : MonoBehaviour
{
    public static SessionManager Instance;

    public RoomSettings Settings;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // 各シーンの UI は、そのシーンに配置された SessionManager を参照している。
            // 古い永続インスタンスを残して新しい方を破棄すると、復帰後の UI イベントが
            // 破棄済みのコンポーネントを参照してしまうため、設定を引き継いで交代する。
            Settings = Instance.Settings;
            Destroy(Instance.gameObject);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (Settings.roomNumber == null)
        {
            Settings = new RoomSettings
            {
                roomNumber = "0",
                turnTime = 45
            };
        }
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
