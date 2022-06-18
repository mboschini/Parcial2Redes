using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerA : MonoBehaviourPun
{
    Player _localPlayer;

    float _x;

    void Start()
    {
        DontDestroyOnLoad(gameObject);  //va a estar en la escena del main menu y no quiero que se destruya cuando cambio de escena
        _localPlayer = PhotonNetwork.LocalPlayer;
    }

    void Update()
    {
        _x = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //disparar
        }
    }
    private void FixedUpdate()
    {
        if (_x != 0)
        {
            var dir = Vector3.right * _x;

            PHServer.serverInstance.RequestMove(_localPlayer, dir);
        }
    }

    //agregar timeslicing de toma de inputs PackagePerSecond del phserver, no para los inputs getkeydown
}
