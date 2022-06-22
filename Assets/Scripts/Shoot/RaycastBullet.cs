using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RaycastBullet : MonoBehaviourPun
{
    /*
    CharacterA _owner;
    float _dmg;
    public RaycastBullet SetDmg(float dmg)
    {
        _dmg = dmg;
        return this;
    }

    //asi le paso el dmg al player desde la bala si la instacio con un go
    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine) return;

        var character = other.GetComponent<CharacterA>();

        if(character && character!= _owner)
        {
            character.TakeDamage(_dmg);
        }

        PhotonNetwork.Destroy(gameObject);
    }
    */
}
