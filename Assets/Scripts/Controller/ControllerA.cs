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
    float granadeCD = 2f;
    float granadeCDtimer = 0;
    bool canShootGrande = true;
    float timer;

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

        if (Input.GetKeyDown(KeyCode.Mouse1) && canShootGrande)
        {
            canShootGrande = false;
            PHServer.serverInstance.RequestShootGranade(_localPlayer);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            PHServer.serverInstance.RequestShowTabScreen(_localPlayer);
        }

        if (Input.GetKeyUp(KeyCode.Tab))
        {
            PHServer.serverInstance.RequestCloseTabScreen(_localPlayer);
        }

        //camera
        mousex = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        mousey = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        //cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }

        if (!canShootGrande)
        {
            granadeCDtimer += Time.deltaTime;
            if (granadeCDtimer >= granadeCD)
            {
                canShootGrande = true;
                granadeCDtimer = 0f;
            }
        }
    }

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;

        if (_x != 0 || _z != 0)
        {
            PHServer.serverInstance.RequestMove(_localPlayer, _x, _z);
        }
        else if (_x == 0 && _z == 0 && timer > .1f)
        {
            PHServer.serverInstance.RequestMove(_localPlayer, _x, _z);
        }

        if (mousex != 0 || mousey != 0)
        {
            PHServer.serverInstance.RequestCameraMove(_localPlayer, Vector3.up * mousex, mousey);
        }
    }


    private void OnApplicationQuit()
    {
        PHServer.serverInstance.RequestDisconnection(_localPlayer);
        PhotonNetwork.Disconnect();
    }
}
