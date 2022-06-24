using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterA : MonoBehaviourPun
{
    Player _owner;
    [SerializeField] Rigidbody _rb;
    [SerializeField] Animator _anim;

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
    Vector3 dir;
    float verticalAngle = 0f;
    [SerializeField] Camera cameraView;
    [SerializeField] Transform cameraTransform;
    [SerializeField] AudioListener audioListener;

    [SerializeField] ParticleSystem bulletsPS;
    [SerializeField] ParticleSystem muzzleFlashPS;

    private void Awake()
    {
        matHead.color = Color.red;
        matBody.color = Color.red;
    }

    //se ejecuta en el servidor original y llama por el rpc al cliente local
    public CharacterA SetInitialParams(Player player)
    {
        _owner = player;
        //_rb = GetComponent<Rigidbody>();
        //_anim = GetComponent<Animator>();
        _currentLife = _maxLife;
        matHead.color = Color.yellow;
        matBody.color = Color.yellow;
        dir = new Vector3();
        photonView.RPC("SetLocalParams", _owner, _currentLife);
        return this;
    }

    //se ejecuta en el cliente avatar que ejecuta este personaje
    //se pueden agregar efector de spawn o particulas que solo ve el cliente local
    [PunRPC]
    void SetLocalParams(float life)
    {
        _currentLife = _maxLife = life;
        cameraView.enabled = true;
        audioListener.enabled = true;

        matHead.color = Color.blue;
        matBody.color = Color.blue;
    }

    public void Move(float dirHorizontal, float dirForward)
    {
        //player move
        dir = transform.forward * dirForward + transform.right * dirHorizontal;
        dir.Normalize();
        _rb.MovePosition(_rb.position + dir * _speed * Time.deltaTime);

        //animation
        if (dirHorizontal == 0 && dirForward == 0)
        {
            _anim.SetBool("isMoving", false);
        }
        else
        {
            _anim.SetBool("isMoving", true);
        }
        
    }

    public void CameraMove(Vector3 Rotation, float verticalRot)
    {
        //camera  move + rot
        transform.Rotate(Rotation);

        verticalAngle -= verticalRot * .8f;
        verticalAngle = Mathf.Clamp(verticalAngle, -45f, 45f);

        cameraTransform.localEulerAngles = new Vector3(verticalAngle, 0, 0);
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
        int layerMask = 1 << 8;

        RaycastHit hit;

        _anim.SetBool("isShooting", true);

        if (Physics.Raycast(cameraView.transform.position, cameraView.transform.forward, out hit, Mathf.Infinity, layerMask))
        {
            Debug.DrawRay(cameraView.transform.position, cameraView.transform.forward * hit.distance, Color.yellow, 1f);
            Debug.Log("Did Hit " + hit.transform.gameObject.name);
            hit.transform.gameObject.GetComponent<CharacterA>().TakeDamage(_dmg);
        }
        else
        {
            Debug.DrawRay(cameraView.transform.position, cameraView.transform.forward * 1000, Color.white, 1f);
            Debug.Log("Did not Hit");
        }
    }

    public void StopShooting()
    {
        _anim.SetBool("isShooting", false);
    }

    public void TakeDamage(float dmg)
    {
        _currentLife -= dmg;
        if (_currentLife <= 0)
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

    //public void HandleParticles()
    //{
    //    muzzleFlashPS.Emit(44);
    //    bulletsPS.Emit(2);
    //}
}
