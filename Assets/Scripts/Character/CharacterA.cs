using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterA : MonoBehaviourPun
{
    Player _owner;
    Rigidbody _rb;

    [SerializeField] float _maxLife;
    float _currentLife;
    [SerializeField] float _speed;
    [SerializeField] float _dmg;

    Material _myMaterial;

    [SerializeField] RaycastBullet _bulletPrefab;
    [SerializeField] Transform _bulletSpawnerTranform;

    // Start is called before the first frame update
    void Awake()
    {
        _myMaterial = GetComponent<Renderer>().material;
        _myMaterial.color = Color.red;
    }

    //se ejecuta en el servidor original y llama por el rpc al cliente local
    public CharacterA SetInitialParams(Player player) 
    {
        _owner = player;
        _rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;
        _myMaterial.color = Color.yellow;

        photonView.RPC("SetLocalParams", _owner, _currentLife);

        return this;
    }

    //se ejecuta en el cliente avatar que ejecuta este personaje
    //se pueden agregar efector de spawn o particulas que solo ve el cliente local
    [PunRPC]
    void SetLocalParams(float life) 
    {
        _currentLife = _maxLife = life;

        _myMaterial.color = Color.blue;
    }

    public void Move(Vector3 dir)
    {
        _rb.MovePosition(_rb.position + dir * Time.deltaTime);
    }

    public void Shoot()
    {
        //shoot behaviour
        PhotonNetwork.Instantiate(_bulletPrefab.name, _bulletSpawnerTranform.position, transform.rotation)
                                    .GetComponent<RaycastBullet>()
                                    .SetDmg(_dmg);
    }

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        if(_currentLife <= 0)
        {
            PHServer.serverInstance.PlayerDisconnect(_owner);
            photonView.RPC("RPC_DisconnectOwner", _owner);
        }
        else
        {
            photonView.RPC("RPC_LifeChange", _owner, _currentLife);
        }
    }

    [PunRPC]
    void RPC_DisconnectOwner()
    {
        PhotonNetwork.Disconnect();
    }

    [PunRPC]
    void RPC_LifeChange(float _currentlife)
    {
        _currentLife = _currentlife;
    }
}
