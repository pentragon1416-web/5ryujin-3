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

    private NetworkRunner runner;
    private NetworkRecordManager networkRecordManager;

    private bool isInitialized;

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

            networkRecordManager = obj.GetComponent<NetworkRecordManager>();
        }

        // 全員：取得待ち
        StartCoroutine(WaitForNetworkRecordManager());
    }

    // ----------------------------
    // 重要：全員がここで同じものを取得する
    // ----------------------------
    private IEnumerator WaitForNetworkRecordManager()
    {
        while (networkRecordManager == null)
        {
            networkRecordManager = FindFirstObjectByType<NetworkRecordManager>();
            yield return null;
        }

        if (isInitialized)
            yield break;

        isInitialized = true;

        Debug.Log("NetworkRecordManager 取得完了");
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