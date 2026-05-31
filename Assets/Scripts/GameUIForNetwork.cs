using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUIForNetwork : MonoBehaviour
{
    private NetworkController networkController;

    public void SetNetworkController(NetworkController controller)
    {
        networkController = controller;

    }

    public void PutButton()
    {
        if (networkController == null) return;

        networkController.PutButton();
    }

    public void RotateButton()
    {
        if (networkController == null) return;

        networkController.RotateButton();
    }

    public void FlipButton()
    {
        if (networkController == null) return;

        networkController.FlipButton();
    }

    public void GiveUpButton(int playerIndex)
    {
        if (networkController == null) return;

        networkController.RpcBoardGiveUp(playerIndex);
    }

    public void TimerSkipButton(bool player)
    {
        if (networkController == null) return;

        networkController.RpcTimerSkip(player);
    }
}