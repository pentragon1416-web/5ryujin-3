using UnityEngine;

public class LocalCursorTracker : MonoBehaviour
{
    public MoveData md { get; set;}

    public void UpdateCursor(MoveData md)
    {
        this.md = md;
    }

    public MoveData GetCursorData()
    {
        return md;
    }
}