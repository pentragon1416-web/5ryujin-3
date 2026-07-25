using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class Stock : MonoBehaviour
{
    private int count = 5;

    public PieceType pieceType;
    public bool isFirstPlayer;
    public GameObject image;
    public GameObject emptyButton;
    public float imageDistance;

    private readonly List<GameObject> stockIcons = new List<GameObject>();

    void Start()
    {
        if (image == null)
        {
            Debug.LogWarning("Stock image is not set: " + pieceType);
            return;
        }

        for (int i = 0; i < count; i++)
        {
            GameObject im = Instantiate(image, transform);

            im.transform.localScale = new Vector3(15, 15, 1);
            im.transform.localPosition = new Vector3((-2 + i) * imageDistance, 0, 0);

            SortingGroup sg = im.GetComponent<SortingGroup>();
            if (sg != null)
            {
                sg.sortingOrder = 10 - i;
            }

            stockIcons.Add(im);
        }
    }

    public void GenerateButton(Color targetColor) // 1. 引数を追加
    {
        GameObject eb = Instantiate(emptyButton, transform);
        eb.transform.localPosition = new Vector3(0, 0, 2);
        
        // 2. この2行を追加（元の透明度を保ちつつRGBを変更）
        Image img = eb.GetComponent<Image>();
        if (img != null) img.color = new Color(targetColor.r, targetColor.g, targetColor.b, img.color.a);

        Canvas ebCanvas = eb.GetComponent<Canvas>();
        if (ebCanvas == null)
        {
            ebCanvas = eb.AddComponent<Canvas>();
        }

        if (ebCanvas != null)
        {
            // 独自のソート順を使う設定を有効にする
            ebCanvas.overrideSorting = true;
            // インスペクターで設定したかった「5」を代入
            ebCanvas.sortingOrder = 5;
        }
    }

    void OnMouseDown()
    {
        Select(pieceType, isFirstPlayer);
    }

    public void Decrement()
    {
        count--;

        if (stockIcons.Count > 0)
        {
            Destroy(stockIcons[stockIcons.Count - 1]);
            stockIcons.RemoveAt(stockIcons.Count - 1);
        }
    }
    public void Reset()
    {
        // 既存アイコン削除
        foreach (GameObject icon in stockIcons)
        {
            Destroy(icon);
        }

        stockIcons.Clear();

        // 個数初期化
        count = 5;

        // 再生成
        for (int i = 0; i < count; i++)
        {
            GameObject im = Instantiate(image, transform);

            im.transform.localScale = new Vector3(15, 15, 1);
            im.transform.localPosition = new Vector3((-2 + i) * imageDistance, 0, 0);

            SortingGroup sg = im.GetComponent<SortingGroup>();

            if (sg != null)
            {
                sg.sortingOrder = 10 - i;
            }

            stockIcons.Add(im);
        }
    }

    public void Select(PieceType type, bool turn)
    {
        if (turn != Board.turn) return;
        if (count <= 0) return;
        if (PieceCursor.instance != null)
        {
            PieceCursor.instance.Select(type, this);
        }
        if (NetworkPieceCursor.instance != null)
        {
            NetworkPieceCursor.instance.Select(type, this);
        }
    }
}
