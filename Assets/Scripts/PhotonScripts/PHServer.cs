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
        CharacterA newCharacter = PhotonNetwork.Instantiate(_characterPrefab.name, Vector3.zero,Quaternion.identity)
                                                            .GetComponent<CharacterA>()
                                                            .SetInitialParams(newPlayer);

        _dictionaryModels.Add(newPlayer, newCharacter);
    }

    #region Request que reciben los servidores avatares

    public void RequestMove(Player player, Vector3 dir)
    {
        photonView.RPC("RPC_Move", _phServer, player, dir);
    }

    public void RequestShoot(Player player)
    {
        photonView.RPC("RPC_Shoot", _phServer, player);
    }

    #endregion

    #region Funciones del server original

    [PunRPC]
    public void RPC_Move(Player playerRequest, Vector3 dir)
    {
        if (_dictionaryModels.ContainsKey(playerRequest))
        {
            _dictionaryModels[playerRequest].Move(dir);
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

    public void PlayerDisconnect(Player player)
    {
        PhotonNetwork.Destroy(_dictionaryModels[player].gameObject);
        _dictionaryModels.Remove(player);
    }
    #endregion
}