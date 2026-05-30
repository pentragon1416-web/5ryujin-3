using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class NetworkRecordManager : NetworkBehaviour
{
    private const int MaxMoves = 512;

    [Networked, Capacity(MaxMoves)]
    public NetworkArray<NetworkMoveData> Moves => default;

    [Networked]
    public int MoveCount { get; set; }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RpcAddMove(NetworkMoveData moveData)
    {
        if (MoveCount >= MaxMoves)
        {
            Debug.LogWarning("Move history is full.");
            return;
        }

        Moves.Set(MoveCount, moveData);
        MoveCount++;
    }

    public MoveData GetMove(int index)
    {
        if (index < 0 || index >= MoveCount)
            return null;

        return Moves.Get(index).ToMoveData();
    }

    public int GetMoveCount()
    {
        return MoveCount;
    }
}

public struct NetworkMoveData : INetworkStruct
{
    public int turn;
    public NetworkBool player;
    public PieceType pieceType;
    public int rotation;
    public NetworkBool flipped;
    public int x;
    public int y;
    public NetworkBool touchdown;

    public static NetworkMoveData FromMoveData(MoveData data)
    {
        return new NetworkMoveData
        {
            turn = data.turn,
            player = data.player,
            pieceType = data.pieceType,
            rotation = data.rotation,
            flipped = data.flipped,
            x = data.x,
            y = data.y,
            touchdown = data.touchdown
        };
    }

    public MoveData ToMoveData()
    {
        return new MoveData
        {
            turn = turn,
            player = player,
            pieceType = pieceType,
            rotation = rotation,
            flipped = flipped,
            x = x,
            y = y,
            touchdown = touchdown
        };
    }
}