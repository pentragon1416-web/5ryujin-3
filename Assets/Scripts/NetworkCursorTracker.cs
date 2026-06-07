using Fusion;
using UnityEngine;

public class NetworkCursor : NetworkBehaviour
{
    [Networked] public float X { get; set; }
    [Networked] public float Y { get; set; }

    [Networked] public PieceType PieceType { get; set; }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateCursor(float x, float y, PieceType pieceType)
    {
        X = x;
        Y = y;
        PieceType = pieceType;
    }
}