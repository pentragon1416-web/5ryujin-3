using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public static int limit = 30;
    public static bool stop = false;

    [SerializeField] Image image;
    public static float counter = 0;
    public static int passCounter;

    [SerializeField] PieceCursor pc;
    [SerializeField] TextMeshProUGUI timelimit;

    void Start()
    {
        counter = 0;
        passCounter = 0;

        ApplyLimit();
    }

    void Update()
    {
        if(stop) return;
        // 再現モードなら毎フレーム強制（←重要）
        ApplyLimit();

        if (limit <= 0) return;

        if (image != null)
        {
            image.fillAmount = (limit - counter) / limit;
        }

        counter += Time.deltaTime;

        if (counter > limit)
        {
            counter = 0;
            passCounter += 1;

            if (passCounter == 4)
            {
                Board.instance.Judge();
            }

            if (pc != null)
            {
                pc.Trash();
            }

            Board.instance.Change();
        }
    }

    // ★ ここで一元管理
    void ApplyLimit()
    {
        // 再現時はANL
        if (KifuReplayContext.HasKifu())
        {
            limit = 360000;

            if (timelimit != null)
            {
                timelimit.text = "ANL";
            }

            return;
        }

        // 通常表示
        if (timelimit == null) return;

        ApplyText(counter, limit);

        // switch (limit)
        // {
        //     case 30:
        //         timelimit.text = "30 sec";
        //         break;

        //     case 60:
        //         timelimit.text = "1 min";
        //         break;

        //     case 180:
        //         timelimit.text = "3 min";
        //         break;

        //     default:
        //         timelimit.text = "No limit";
        //         break;
        // }
    }

    void ApplyText(float counter, int limit)
    {
        if (limit <= 0)
        {
            timelimit.text = "No limit";
            return;
        }

        float remain = Mathf.Max(0, limit - counter);

        int minutes = Mathf.FloorToInt(remain / 60);
        int seconds = Mathf.FloorToInt(remain % 60);

        if (minutes > 0)
        {
            timelimit.text = $"{minutes}:{seconds:00}";
        }
        else
        {
            timelimit.text = $"{seconds} sec";
        }
    }

    public static void ResetCounter()
    {
        if (counter != 0)
        {
            counter = 0;
            passCounter = 0;
        }
    }

    public static void StopCounter()
    {
        stop = true;
    }

    public static void StartCounter()
    {
        stop = false;
    }

    public void Skip(bool p)
    {
        if (p == Board.turn)
        {
            counter += limit;
        }
    }
}