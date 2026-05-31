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

    [Header("ローカルセッティング用")]
    [SerializeField] private NetworkPieceCursor networkPieceCursor;
    [SerializeField] private GameUIForNetwork gameUIForNetwork;
    [SerializeField] private Timer timer;
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
    private bool isInitialized;
    private bool shouldStartGame = false;
    private bool isHost = false;

    private async void Start()
    {
        runner = Instantiate(networkRunnerPrefab);
        runner.AddCallbacks(this);
        runner.ProvideInput = true;

        await runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = "OnlineMatch",
            PlayerCount = 2,
            IsOpen = true,
            IsVisible = true
        });
    }

    private async void OnDestroy()
    {
        if (runner != null)
        {
            runner.RemoveCallbacks(this);
            await runner.Shutdown();
            Destroy(runner.gameObject);
            runner = null;
        }
    }

    // ----------------------------
    // Player Join
    // ----------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        Debug.Log($"Player joined. Active players: {runner.ActivePlayers.Count()}");

        // ホスト（SharedModeMasterClient）だけが生成する
        if (runner.IsSharedModeMasterClient && networkRecordManager == null)
        {
            isHost = true;
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

            networkController = controllerObj.GetComponent<NetworkController>();

            networkRecordManager = obj.GetComponent<NetworkRecordManager>();
            networkPieceCursor.enabled = false;
        }
        // 二人目が来たときにbool値をtrueにしてループ解除
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

        if (isInitialized)
            yield break;

        isInitialized = true;

        Debug.Log("Game initialized with 2 players. Starting game...");

        // ゲーム初期化処理
        networkPieceCursor.SetNetworkRecordManager(networkRecordManager);
        Board.instance.SetPieceCursor(networkPieceCursor);
        networkController.SetNetworkPieceCursor(networkPieceCursor);
        networkController.SetTimer(timer);
        networkController.RpcResetCounter();
        gameUIForNetwork.SetNetworkController(networkController);
        networkPieceCursor.enabled = true;
        InitializeGame();
    }

    private void InitializeGame()
    {
        if (isHost)
        {
            UpperTD.SetActive(false);
            UpperGU.SetActive(false);
            UpperPass.SetActive(false);
            networkPieceCursor.SetMyTurn(false);
        }
        else
        {
            LowerTD.SetActive(false);
            LowerGU.SetActive(false);
            LowerPass.SetActive(false);
            networkPieceCursor.SetMyTurn(true);
        }
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }

    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
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