using System.Collections.Generic;
using UnityEngine;
using Fusion;
using static NetworkPieceCursor;

public class NetworkCursorViewer : MonoBehaviour
{
    public NetworkCursorTracker cursorTracker;
    public bool forPlayer;
    public Color32 color1p;
    public Color32 color2p;

    [SerializeField] private List<PiecePrefabPair> piecesList;

    private Dictionary<PieceType, GameObject> pieces;

    private GameObject currentPiece;

    private PieceType lastPieceType = PieceType.td;
    private bool initialized = false;

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
        // はじめのフレームは見送る。
        if (!initialized)
        {
            initialized = true;
            return;
        }
        if (forPlayer != Board.turn)
        {
            ClearPiece();
            return;
        }
        MoveData md = cursorTracker.GetCursorData();
        if (md == null) return;
        Debug.Log(
            $"[MoveData]\n" +
            $"pieceType: {md.pieceType}\n" +
            $"x: {md.x}, y: {md.y}\n" +
            $"rotation: {md.rotation}\n" +
            $"flipped: {md.flipped}\n" +
            $"player: {md.player}\n" +
            $"touchdown: {md.touchdown}\n"
        );
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
    public void SetForPlayer(bool forPlayer)
    {
        this.forPlayer = forPlayer;
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

    private void ApplyPlayerColor()
    {
        for (int i = 0; i < currentPiece.transform.childCount; i++)
        {
            Transform child = currentPiece.transform.GetChild(i);

            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();

            if (sr != null)
            {
                sr.sortingOrder = 10;
                sr.color = forPlayer ? color2p : color1p;
            }
        }
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

        ApplyPlayerColor();
    }

    public void ClearPiece()
    {
        if (currentPiece != null)
        {
            Destroy(currentPiece);
            currentPiece = null;
        }
    }
}