using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerA : MonoBehaviourPun
{
    Player _localPlayer;

    float _x;
    float _z;

    void Start()
    {
        DontDestroyOnLoad(gameObject);  //va a estar en la escena del main menu y no quiero que se destruya cuando cambio de escena
        _localPlayer = PhotonNetwork.LocalPlayer;
    }

    void Update()
    {
        _z = Input.GetAxis("Vertical");
        _x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PHServer.serverInstance.RequestJump(_localPlayer);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            PHServer.serverInstance.RequestShoot(_localPlayer);
        }
    }

    private void FixedUpdate()
    {
        if (_x != 0 || _z!=0)
        {
            PHServer.serverInstance.RequestMove(_localPlayer, _x, _z);
        }
    }

    //agregar timeslicing de toma de inputs PackagePerSecond del phserver, no para los inputs getkeydown


    private void OnApplicationQuit()
    {
        PHServer.serverInstance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }
}
