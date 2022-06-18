using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PHServer serverPrefab;           //servidor
    public ControllerA ControllerPrefab;    //control del player
    
    public void BTN_Connect()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 3; //max player = 1 (server) + cantidad de players 

        PhotonNetwork.JoinOrCreateRoom("ServerFullAuth", roomOptions, TypedLobby.Default);
    }

    public override void OnCreatedRoom() //solo se llama al creador del primer cliente
    {
        PhotonNetwork.Instantiate(serverPrefab.name, Vector3.zero, Quaternion.identity);
    }

    public override void OnJoinedRoom()
    {
        if (!PhotonNetwork.IsMasterClient) //si somos el server no queremos crearnos un controller
        {
            Instantiate(ControllerPrefab);  //toma los inputs del player
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Connection Failed: " + cause.ToString());
    }
}
