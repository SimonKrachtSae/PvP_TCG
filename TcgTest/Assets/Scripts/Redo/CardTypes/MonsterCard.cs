using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class MonsterCard : Card, IPunObservable
{
    public int CardStats { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
    private void Start()
    {
        try
        {
            layout = cardObj.gameObject.GetComponent<MonsterCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject);
        }
        
        player.Subscribe(this);
        Type = CardType.Monster;
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
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        cardStats = player.StartingDeck[index];
    }
    public void DrawValues()
    {
        MonsterCardStats stats = (MonsterCardStats)cardStats;

//        if (stats.Effect != null) ((MonsterCard_Layout)layout).EffectTextUI.text = stats.Effect.ToString();
        ((MonsterCard_Layout)layout).AttackTextUI.text = stats.Attack.ToString();
        ((MonsterCard_Layout)layout).NameTextUI.text = stats.CardName.ToString();
        ((MonsterCard_Layout)layout).PlayCostTextUI.text = stats.PlayCost.ToString();
        ((MonsterCard_Layout)layout).DefenseTextUI.text = stats.Defense.ToString();
        cardStats = stats;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
