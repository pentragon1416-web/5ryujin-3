using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

public class GameLauncher : MonoBehaviour, INetworkRunnerCallbacks
{
    [SerializeField] private NetworkRunner networkRunnerPrefab;
    [SerializeField] private NetworkPrefabRef networkRecordManagerPrefab;
    [SerializeField] private NetworkPrefabRef networkControllerPrefab;
    [SerializeField] private NetworkPrefabRef networkCursorTrackerPrefab;

    [Header("ローカルセッティング用")]
    [SerializeField] private NetworkPieceCursor networkPieceCursor;
    [SerializeField] private GameUIForNetwork gameUIForNetwork;
    [SerializeField] private Timer timer;
    [SerializeField] private MessageController messageController;
    [Header("下側から")]
    [SerializeField] private GameObject LowerTD;
    [SerializeField] private GameObject LowerGU;
    [SerializeField] private GameObject LowerPass;
    [Header("上側から")]
    [SerializeField] private GameObject UpperTD;
    [SerializeField] private GameObject UpperGU;
    [SerializeField] private GameObject UpperPass;

    private NetworkRunner runner;
    private NetworkRecordManager networkRecordManager;
    private NetworkController networkController;
    private NetworkCursorTracker upperNetworkCursorTracker;
    private NetworkCursorTracker lowerNetworkCursorTracker;
    private bool isInitialized;
    private bool shouldStartGame = false;
    private bool turn = true;
    private bool pausingPlayer = false;

    private async void Start()
    {
        // NetworkPieceCursorで自分のターンではないときには駒を表示させないようにしているので、この際に2Pに盤を渡す。
        Board.instance.ChangeTo(true);
        runner = Instantiate(networkRunnerPrefab);
        runner.AddCallbacks(this);
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = SessionManager.Instance.Settings.roomNumber,
            PlayerCount = 2,
            IsOpen = true,
            IsVisible = true
        });
        messageController.ShowMessageWithGoTitleButton("Matching...");
    }

    private async void LeaveRoom()
    {

        if (runner != null)
        {
            runner.RemoveCallbacks(this);
            await runner.Shutdown();
            Destroy(runner.gameObject);
            runner = null;
        }
    }

    public void TakeOverStateAuthority()
    {
        networkRecordManager.Object.RequestStateAuthority();
        networkController.Object.RequestStateAuthority();
        upperNetworkCursorTracker.Object.RequestStateAuthority();
        lowerNetworkCursorTracker.Object.RequestStateAuthority();
    }

    // ----------------------------
    // Player Join
    // ----------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // すでにInitializeした方はもう実行しない。
        if(isInitialized) {
            networkController.RpcSetMaster(networkPieceCursor.myTurn);
            return;
        };
        Debug.Log($"Player joined. Active players: {runner.ActivePlayers.Count()}");

        // ホスト（SharedModeMasterClient）だけが生成する
        if (runner.IsSharedModeMasterClient && networkRecordManager == null)
        {
            var obj = runner.Spawn(
                networkRecordManagerPrefab,
                Vector3.zero,
                Quaternion.identity,
                inputAuthority: null,
                onBeforeSpawned: null,
                flags: NetworkSpawnFlags.SharedModeStateAuthMasterClient
            );

            var controllerObj = runner.Spawn(
                networkControllerPrefab,
                Vector3.zero,
                Quaternion.identity,
                inputAuthority: null,
                onBeforeSpawned: null,
                flags: NetworkSpawnFlags.SharedModeStateAuthMasterClient
            );

            var upperCursorTrackerObj = runner.Spawn(
                networkCursorTrackerPrefab,
                Vector3.zero,
                Quaternion.identity,
                inputAuthority: null,
                onBeforeSpawned: null,
                flags: NetworkSpawnFlags.SharedModeStateAuthMasterClient
            );
            var lowerCursorTrackerObj = runner.Spawn(
                networkCursorTrackerPrefab,
                Vector3.zero,
                Quaternion.identity,
                inputAuthority: null,
                onBeforeSpawned: null,
                flags: NetworkSpawnFlags.SharedModeStateAuthMasterClient
            );
            networkController = controllerObj.GetComponent<NetworkController>();
            networkController.RpcSetTimerLimit(SessionManager.Instance.Settings.turnTime);
            upperNetworkCursorTracker = upperCursorTrackerObj.GetComponent<NetworkCursorTracker>();
            upperNetworkCursorTracker.RpcSetCursorTrackerType(CursorTrackerType.Upper);
            upperNetworkCursorTracker.RpcSetForPlayer(true);
            upperNetworkCursorTracker.gameObject.SetActive(false);
            lowerNetworkCursorTracker = lowerCursorTrackerObj.GetComponent<NetworkCursorTracker>();
            lowerNetworkCursorTracker.RpcSetCursorTrackerType(CursorTrackerType.Lower);
            lowerNetworkCursorTracker.RpcSetForPlayer(false);
            lowerNetworkCursorTracker.gameObject.SetActive(false);

            networkRecordManager = obj.GetComponent<NetworkRecordManager>();
            networkPieceCursor.enabled = false;
            networkController.RpcSetMaster(false);
            turn = false;
            pausingPlayer = true;
        }
        // 二人目が来たとき、ゲーム途中への入室でbool値をtrueにしてループ解除
        if (runner.ActivePlayers.Count() == 2)
        {
            shouldStartGame = true;
        }
        StartCoroutine(WaitForNetworkRecordManager());
    }

    // ----------------------------
    // 重要：全員がここで同じものを取得する
    // ----------------------------
    private IEnumerator WaitForNetworkRecordManager()
    {
        // shouldStartGameがtrueになるまで待機
        while (!shouldStartGame)
        {
            Timer.ResetCounter();
            yield return null;
        }
        // ネットワークオブジェクトの取得
        while (networkRecordManager == null)
        {
            networkRecordManager = FindFirstObjectByType<NetworkRecordManager>();
            yield return null;
        }
        while (networkController == null)
        {
            networkController = FindFirstObjectByType<NetworkController>();
            yield return null;
        }
        while (upperNetworkCursorTracker == null || lowerNetworkCursorTracker == null)
        {
            var trackers = FindObjectsByType<NetworkCursorTracker>(
                FindObjectsSortMode.None);

            foreach (var tracker in trackers)
            {
                switch (tracker.TrackerType)
                {
                    case CursorTrackerType.Upper:
                        upperNetworkCursorTracker = tracker;
                        break;

                    case CursorTrackerType.Lower:
                        lowerNetworkCursorTracker = tracker;
                        break;
                }
            }

            yield return null;
        }

        if (isInitialized)
            yield break;
        isInitialized = true;

        // オブジェクトが同期されても、変数が同期されないのでいったん待機。
        yield return new WaitForSeconds(1f);

        Debug.Log("Game initialized with 2 players. Starting game...");

        // ゲーム初期化処理
        networkPieceCursor.SetNetworkRecordManager(networkRecordManager);
        networkPieceCursor.enabled = true;
        Board.instance.SetPieceCursor(networkPieceCursor);
        networkController.SetNetworkPieceCursor(networkPieceCursor);
        networkController.SetTimer(timer);
        networkController.SetMessageController(messageController);
        gameUIForNetwork.SetNetworkController(networkController);
        if (!pausingPlayer)
        {
            turn = !networkController.GetMaster();
        }
        pausingPlayer = false;
        InitializeGame();
        // ここではじめでも途中でもStateAuthorityを持っている側が初期化
        networkController.RpcResetCounter();
        networkController.RpcApplyTimerLimit();
        networkController.RpcBoardChangeTo(networkController.GetMaster());
        networkController.RpcHideMessage();
    }

    private void InitializeGame()
    {
        // 1Pの方がfalseなので!を使っています。
        if (!turn)
        {
            InitializeAsLower();
        }
        else
        {
            InitializeAsUpper();
        }
        messageController.HideMessageAfterDelay(1f);
        networkPieceCursor.StartGame();

        // ここは他のプレイヤー復帰した場合はルームに残っていた方のターンになります。
        Board.instance.ChangeTo(networkController.master);
    }

    private void InitializeAsUpper()
    {
        LowerTD.SetActive(false);
        LowerGU.SetActive(false);
        LowerPass.SetActive(false);
        networkPieceCursor.SetMyTurn(true);
        networkPieceCursor.SetCursorTracker(upperNetworkCursorTracker);
        upperNetworkCursorTracker.gameObject.SetActive(false);
        lowerNetworkCursorTracker.gameObject.SetActive(true);
        messageController.ShowMessage("Upper!");
    }
    private void InitializeAsLower()
    {
        UpperTD.SetActive(false);
        UpperGU.SetActive(false);
        UpperPass.SetActive(false);
        networkPieceCursor.SetMyTurn(false);
        networkPieceCursor.SetCursorTracker(lowerNetworkCursorTracker);
        upperNetworkCursorTracker.gameObject.SetActive(true);
        lowerNetworkCursorTracker.gameObject.SetActive(false);
        messageController.ShowMessage("Lower!");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // 待機画面の用意
        messageController.ShowMessageWithGoTitleButton("left...");
        shouldStartGame = false;

        // // 残っているプレイヤーが自動的にStateAuthorityを持つので自分のターンをnetowrkControllerのmasterに登録する。
        // bool m = networkPieceCursor.myTurn;
        // networkController.RpcSetMaster(m);
        // messageController.ShowMessageWithGoTitleButton($"I am {networkController.master}");

        // 次のプレイヤーが来るまでタイマーをリセットし続けるようにする。
        Timer.StopCounter();
        pausingPlayer = true;
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
    }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }

    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}