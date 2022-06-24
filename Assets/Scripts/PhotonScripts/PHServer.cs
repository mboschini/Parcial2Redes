using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PHServer : MonoBehaviourPunCallbacks
{
    public static PHServer serverInstance;

    Player _phServer;

    [SerializeField] CharacterA _characterPrefab;
    [SerializeField] Transform _player1pos;
    [SerializeField] Transform _player2pos;

    //diccionario de player que tengo en mi juego,
    //cuando recibo la peticion de cliente(player) en mi diccionario,
    //si es asi tomo al character que lo representa y lo muevo
    Dictionary<Player, CharacterA> _dictionaryModels = new Dictionary<Player, CharacterA>();

    public int PackagePerSecond { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (serverInstance == null)
        {
            if (photonView.IsMine)
            {
                //cuando se conecte un cliente se va a ejecutar esta funcion, por eso usamos el target.allbuffered
                photonView.RPC("SetServer", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer, 1);                
            }
        }
    }

    [PunRPC]
    void SetServer(Player serverClient, int SceneIndex = 1)
    {
        if (serverInstance)
        {
            Destroy(gameObject);
            return;
        }
        serverInstance = this;

        _phServer = serverClient;
        PackagePerSecond = 60;

        PhotonNetwork.LoadLevel(SceneIndex);

        var playerLocal = PhotonNetwork.LocalPlayer;

        if(playerLocal != _phServer)
        {
            photonView.RPC("AddPlayer", _phServer, playerLocal);
        }
    }

    [PunRPC]
    void AddPlayer(Player newPlayer)
    {
        StartCoroutine(waitForLevel(newPlayer));
    }

    IEnumerator waitForLevel(Player newPlayer)
    {
        while (PhotonNetwork.LevelLoadingProgress > 0.9f)
        {
            yield return new WaitForEndOfFrame();
        }

        //se ejecuta en el servidor original, por lo que se puede tener un manager que gestione las posiciones
        //de todos los players y se llamaria desde aca.
        if (_dictionaryModels.Count == 0)
        {
            CharacterA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, _player1pos.position, _player1pos.rotation)
                                                                .GetComponent<CharacterA>()
                                                                .SetInitialParams(newPlayer);
            _dictionaryModels.Add(newPlayer, newCharacter);
        }
        else
        {
            CharacterA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, _player2pos.position, _player2pos.rotation)
                                                                .GetComponent<CharacterA>()
                                                                .SetInitialParams(newPlayer);
            _dictionaryModels.Add(newPlayer, newCharacter);
        }

    }

    #region Request que reciben los servidores avatares

    public void RequestMove(Player player, float dirHorizontal, float dirForward)
    {
        photonView.RPC("RPC_Move", _phServer, player, dirHorizontal, dirForward);
    }

    public void RequestCameraMove(Player player, Vector3 rotation, float verticalRot)
    {
        photonView.RPC("RPC_CameraMove", _phServer, player, rotation, verticalRot);
    }

    public void RequestJump(Player player)
    {
        photonView.RPC("RPC_Jump", _phServer, player);
    }

    public void RequestShoot(Player player)
    {
        photonView.RPC("RPC_Shoot", _phServer, player);
    }
    public void RequestShootGranade(Player player)
    {
        photonView.RPC("RPC_ShootGranade", _phServer, player);
    }

    public void RequestLose(Player player)
    {
        photonView.RPC("RPC_Win", _phServer, player);
    }

    public void RequestDisconnection(Player player)
    {
        photonView.RPC("RPC_Disconnect", _phServer, player);
        PhotonNetwork.SendAllOutgoingCommands();
    }

    public void RequestShowTabScreen(Player player)
    {
        photonView.RPC("RPC_ShowTabScreen", _phServer, player);
    }

    public void RequestCloseTabScreen(Player player)
    {
        photonView.RPC("RPC_CloseTabScreen", _phServer, player);
    }

#endregion

#region Funciones del server original

    [PunRPC]
    public void RPC_Move(Player playerRequest, float dirHorizontal, float dirForward)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].Move(dirHorizontal, dirForward);
        }
    }

    [PunRPC]
    public void RPC_CameraMove(Player playerRequest, Vector3 Rotation, float verticalRot)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].CameraMove(Rotation, verticalRot);
        }
    }

    [PunRPC]
    public void RPC_Jump(Player playerRequest)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].Jump();
        }
    }
    
    [PunRPC]
    public void RPC_Shoot(Player playerRequest)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].Shoot();
        }
    }

    [PunRPC]
    public void RPC_ShootGranade(Player playerRequest)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].ShootGranade();
        }
    }
    

    [PunRPC]
    public void RPC_ShowTabScreen(Player playerRequest)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].ShowTabScreen();
        }
    }

    [PunRPC]
    public void RPC_CloseTabScreen(Player playerRequest)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].CloseTabScreen();
        }
    }

    [PunRPC]
    public void RPC_Win(Player playerRequest)
    {
        foreach(var player in _dictionaryModels)
        {
            if(player.Key != playerRequest)
                player.Value.Win();
            else if(player.Key == playerRequest)
                player.Value.Lose();
        }
    }

    public void RPC_Disconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictionaryModels[player].gameObject);
        _dictionaryModels.Remove(player);
    }
    #endregion
}
