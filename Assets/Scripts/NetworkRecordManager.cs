using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class NetworkRecordManager : NetworkBehaviour
{
    public NetworkPieceCursor networkPieceCursor;
    public MoveDataLoader moveDataLoader;
    private const int MaxMoves = 512;

    [Networked, Capacity(MaxMoves)]
    public NetworkArray<NetworkMoveData> Moves => default;

    [Networked]
    public int MoveCount { get; set; }
    [Networked]
    public bool Turn { get; set; }
    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            MoveCount = 0;
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcAddMove(NetworkMoveData moveData)
    {
        if (MoveCount >= MaxMoves)
        {
            Debug.LogWarning("Move history is full.");
            return;
        }

        Moves.Set(MoveCount, moveData);
        moveDataLoader.LoadMoveData(moveData.ToMoveData());
        MoveCount++;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcChangeTurn()
    {
        Turn = !Turn;
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


    public bool GetTurn()
    {
        return Turn;
    }

    public bool CanAdd(MoveData md)
    {
        return moveDataLoader.mm.CanAdd(md);
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