using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkController : NetworkBehaviour
{
    private NetworkPieceCursor networkPieceCursor;
    private Timer timer;
    public void SetNetworkPieceCursor(NetworkPieceCursor cursor)
    {
        networkPieceCursor = cursor;
    }

    public void SetTimer(Timer timer)
    {
        this.timer = timer;
    }

    public void PutButton()
    {
        networkPieceCursor.PutButton();
    }

    public void RotateButton()
    {
        networkPieceCursor.RotateButton();
    }

    public void FlipButton()
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