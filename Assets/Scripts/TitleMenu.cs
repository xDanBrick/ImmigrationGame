using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

public class TitleMenu : MonoBehaviour {
    public string gameScene;

    public void HostGame()
    {
        //run host game
        SceneManager.LoadScene(gameScene);
        NetworkManager.singleton.StartHost();
    }

    public void JoinGame()
    {
        SceneManager.LoadScene(gameScene);
        NetworkManager.singleton.StartServer();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
