using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ControllerA : MonoBehaviourPun
{
    Player _localPlayer;

    float sensitivity = 200f;
    float mousex = 0f;
    float mousey = 0f;
    float _x;
    float _z;

    void Start()
    {
        DontDestroyOnLoad(gameObject);  //va a estar en la escena del main menu y no quiero que se destruya cuando cambio de escena
        _localPlayer = PhotonNetwork.LocalPlayer;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //move
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

        //camera
        mousex = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mousey = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        //mousey = Mathf.Clamp(mousey, -45f, 45f);

        //cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void FixedUpdate()
    {
        if (_x != 0 || _z!=0 || mousex != 0)
        {
            PHServer.serverInstance.RequestMove(_localPlayer, _x, _z, Vector3.up * mousex, -mousey);
        }
    }

    //agregar timeslicing de toma de inputs PackagePerSecond del phserver, no para los inputs getkeydown


    private void OnApplicationQuit()
    {
        PHServer.serverInstance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }
}
