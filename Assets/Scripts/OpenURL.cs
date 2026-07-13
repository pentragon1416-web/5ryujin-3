using System.Runtime.InteropServices;
using UnityEngine;

public class OpenURL : MonoBehaviour
{
    public string url = "https://5ryujin.com";

    #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenNewTab(string url);
    #endif

    public void Open()
    {
    #if UNITY_WEBGL && !UNITY_EDITOR
            OpenNewTab(url);
    #else
            Application.OpenURL(url);
    #endif
    }
}