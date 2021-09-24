using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectCard : Card,IPunObservable
{
    public int CardStats { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
    private void Start()
    {
        try
        { 
            layout = cardObj.gameObject.GetComponent<EffectCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch 
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject); 
        }


        player.Subscribe(this);
        Type = CardType.Effect;
        DrawValues();
    }
    void Update()
    {
        if (!photonView.IsMine) return;
        if (transform.position != prevPos)
        {
            photonView.RPC(nameof(RPC_UpdatePosition), RpcTarget.Others, transform.position);
        }
        prevPos = transform.position;
    }
    [PunRPC]
    public void RPC_UpdatePosition(Vector3 value)
    {
        transform.position = new Vector3(value.x, value.y * -1, value.z);
    }
    public void DrawValues()
    {
        EffectCardStats stats = (EffectCardStats)cardStats;
        //if (stats.Effect != null) ((EffectCard_Layout)layout).EffectTextUI.text = stats.Effect.ToString();
        ((EffectCard_Layout)layout).NameTextUI.text = stats.CardName.ToString();
        ((EffectCard_Layout)layout).PlayCostTextUI.text = stats.PlayCost.ToString();
        cardStats = stats;
    }
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        cardStats = player.StartingDeck[index];
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
