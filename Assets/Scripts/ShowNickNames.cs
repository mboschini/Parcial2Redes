using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ShowNickNames : MonoBehaviour
{
    [SerializeField] Text PlayerListText;

    bool _SkipFirst;

    public void UpdatePlayer(Player[] listOfPlayers)
    {
        PlayerListText.text = "";
        _SkipFirst = false;

        foreach(var player in listOfPlayers)
        {
            if (_SkipFirst)
                PlayerListText.text += player.NickName + "\n";
            else
                _SkipFirst = true;
        }
    }
}
