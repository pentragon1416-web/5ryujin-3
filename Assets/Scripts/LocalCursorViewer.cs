using System.Collections.Generic;
using UnityEngine;
using static NetworkPieceCursor;

public class LocalCursorViewer : MonoBehaviour
{
    public LocalCursorTracker cursorTracker;
    public bool player;

    [SerializeField] private List<PiecePrefabPair> piecesList;

    private Dictionary<PieceType, GameObject> pieces;

    private GameObject currentPiece;

    private PieceType lastPieceType = PieceType.td;

    private void Awake()
    {
        pieces = new Dictionary<PieceType, GameObject>();

        foreach (var p in piecesList)
        {
            if (p.prefab != null)
                pieces[p.type] = p.prefab;
        }
    }

    public void Update()
    {
        // if (player == Board.turn)
        // {
        //     enabled = false;
        //     return;
        // }

        MoveData md = cursorTracker.GetCursorData();
        if (md == null) return;
        Debug.Log("CursorViewer Update: " + md.ToString());
        // =====================================
        // ① 位置・回転・反転は常に更新
        // =====================================
        ApplyTransform(md);

        // =====================================
        // ② PieceTypeだけ差分監視
        // =====================================
        if (md.pieceType != lastPieceType)
        {
            ApplyPiecePrefab(md.pieceType);
            lastPieceType = md.pieceType;
        }
    }

    // =====================================
    // 位置・回転・反転（毎フレーム）
    // =====================================
    private void ApplyTransform(MoveData md)
    {
        transform.position = ConvertToWorldPosition(md.x, md.y + 5);

        // ★ 先にflipを考慮して回転補正
        float rot = md.rotation;

        if (md.flipped)
        {
            rot = -rot;
        }

        transform.rotation = Quaternion.Euler(0, 0, rot);

        // flipは最後に見た目として適用
        transform.localScale = new Vector3(md.flipped ? -1 : 1, 1, 1);
    }

    // =====================================
    // 盤面 → ワールド座標
    // =====================================
    private Vector3 ConvertToWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0);
    }

    // =====================================
    // flip
    // =====================================
    private Vector3 GetScaleFromFlip(bool flip)
    {
        return new Vector3(flip ? -1 : 1, 1, 1);
    }

    // =====================================
    // PieceType変更時のみ差し替え
    // =====================================
    private void ApplyPiecePrefab(PieceType type)
    {
        if (!pieces.ContainsKey(type))
        {
            Debug.LogError($"未登録PieceType: {type}");
            return;
        }

        // 同じなら何もしない
        if (currentPiece != null && lastPieceType == type)
            return;

        if (currentPiece != null)
        {
            Destroy(currentPiece);
        }

        currentPiece = Instantiate(pieces[type], transform);
    }
}