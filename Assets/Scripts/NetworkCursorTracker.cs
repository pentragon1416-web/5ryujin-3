using Fusion;
using UnityEngine;

public class NetworkCursorTracker : NetworkBehaviour
{
    [Networked] public NetworkMoveData nmd { get; set;}

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateCursor(NetworkMoveData nmd)
    {
        this.nmd = nmd;
    }

    public MoveData GetCursorData()
    {
        return nmd.ToMoveData();
    }
}