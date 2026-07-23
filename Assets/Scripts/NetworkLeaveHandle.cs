using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkLeaveHandle : NetworkBehaviour
{
    public GameLauncher gameLauncher;

    public void SetGameLauncher(GameLauncher gl)
    {
        gameLauncher = gl;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSendStateAuthority()
    {
        if (!Object.HasStateAuthority)
        {
            gameLauncher.TakeOverStateAuthority();
        }
    }
}
