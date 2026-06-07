using Fusion;
using UnityEngine;

public class NetworkCursorTracker : NetworkBehaviour
{
    public NetworkCursorViewer cursorViewer;
    [Networked] public NetworkMoveData nmd { get; set; }
    [Networked] public CursorTrackerType TrackerType { get; set; }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateCursor(NetworkMoveData nmd)
    {
        this.nmd = nmd;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetCursorTrackerType(CursorTrackerType type)
    {
        TrackerType = type;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetForPlayer(bool forPlayer)
    {
        cursorViewer.SetForPlayer(forPlayer);
    }
    public MoveData GetCursorData()
    {
        return nmd.ToMoveData();
    }
}


public enum CursorTrackerType
{
    Upper = 0,
    Lower = 1
}
