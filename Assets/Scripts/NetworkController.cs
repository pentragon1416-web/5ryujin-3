using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkController : NetworkBehaviour
{
    private NetworkPieceCursor networkPieceCursor;
    private Timer timer;
    private MessageController messageController;
    public void SetNetworkPieceCursor(NetworkPieceCursor cursor)
    {
        networkPieceCursor = cursor;
    }

    public void SetTimer(Timer timer)
    {
        this.timer = timer;
    }

    public void SetMessageController(MessageController controller)
    {
        messageController = controller;
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

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcResetCounter()
    {
        Timer.ResetCounter();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcShowMessage(string msg)
    {
        messageController.ShowMessage(msg);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcShowMessageWithGoTitleButton(string msg)
    {
        messageController.ShowMessageWithGoTitleButton(msg);
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcHideMessage()
    {
        messageController.HideMessage();
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcHideMessageAfterDelay(float delay)
    {
        messageController.HideMessageAfterDelay(delay);
    }
}