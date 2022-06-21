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
    [SerializeField] float _jumpForce;
    [SerializeField] float _dmg;

    [SerializeField] Material matHead;
    [SerializeField] Material matBody;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius;
    [SerializeField] LayerMask groundMask;
    bool isGrounded;

    private void Awake()
    {
        matHead.color = Color.red;
        matBody.color = Color.red;
    }

    //se ejecuta en el servidor original y llama por el rpc al cliente local
    public CharacterA SetInitialParams(Player player) 
    {
        _owner = player;
        _rb = GetComponent<Rigidbody>();
        _currentLife = _maxLife;
        matHead.color = Color.yellow;
        matBody.color = Color.yellow;

        photonView.RPC("SetLocalParams", _owner, _currentLife);

        return this;
    }

    //se ejecuta en el cliente avatar que ejecuta este personaje
    //se pueden agregar efector de spawn o particulas que solo ve el cliente local
    [PunRPC]
    void SetLocalParams(float life) 
    {
        _currentLife = _maxLife = life;
        matHead.color = Color.blue;
        matBody.color = Color.blue;
    }

    public void Move(Vector3 dir)
    {
        Debug.Log(dir *_speed * Time.deltaTime);
        _rb.MovePosition(_rb.position + dir * _speed * Time.deltaTime);
    }

    public void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundMask);
        float vel = _rb.velocity.y;
        vel = vel * vel;
        if (isGrounded && vel <= 0.15f)
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.VelocityChange);
    }
    
    public void Shoot()
    {
        //shoot behaviour
        //PhotonNetwork.Instantiate(_bulletPrefab.name, _bulletSpawnerTranform.position, transform.rotation)
        //                            .GetComponent<RaycastBullet>()
        //                            .SetDmg(_dmg);
    }

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        if(_currentLife <= 0)
        {
            PHServer.serverInstance.RPC_Disconnect(_owner);
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
