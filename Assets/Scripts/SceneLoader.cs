using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void LoadHomeScene()
    {
        SceneManager.LoadScene("HomeScene");
    }

    public void LoadNetworkScene()
    {
        SceneManager.LoadScene("NetworkScene");
    }

    public void LoadLocalScene()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void LoadScene(SceneType sceneType)
    {
        switch (sceneType)
        {
            case SceneType.Home:
                SceneManager.LoadScene("HomeScene");
                break;

            case SceneType.Net:
                SceneManager.LoadScene("NetworkScene");
                break;

            case SceneType.Local:
                SceneManager.LoadScene("MainScene");
                break;
        }
    }
}

public enum SceneType
{
    Home,
    Net,
    Local
}
