using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get { return _instance; } }
    static GameManager _instance;

    public GameObject loseScreen;
    public GameObject winScreen;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(this);
    }

    public void ExitGame()
    {
        PhotonNetwork.Disconnect();
        PhotonNetwork.LoadLevel(0);
    }
}
