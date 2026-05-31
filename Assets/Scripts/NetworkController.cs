using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkController : NetworkBehaviour
{
    public NetworkPieceCursor networkPieceCursor;
    public Timer timer;

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcPutButton()
    {
        networkPieceCursor.PutButton();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcRotateButton()
    {
        networkPieceCursor.RotateButton();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcFlipButton()
    {
        networkPieceCursor.FlipButton();
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcBoardGiveUp(int i)
    {
        Board.instance.Giveup(i);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcTimerSkip(bool p)
    {
        timer.Skip(p);
    }
}