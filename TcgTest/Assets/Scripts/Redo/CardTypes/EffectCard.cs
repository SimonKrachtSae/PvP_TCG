using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectCard : Card, IPunObservable
{
    private void Start()
    {
        try
        { 
            layout = cardObj.GetComponent<EffectCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch 
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject); 
        }


        player.Subscribe(this);
        Type = CardType.Effect;
    }
    public void AssignStats()
    {
        layout.PlayCostTextUI = 

    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
