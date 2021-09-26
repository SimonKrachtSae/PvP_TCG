using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class EffectCard : Card,IPunObservable
{
    public int CardStatsIndex { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
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
    private void OnValidate()
    {
        if (cardStats == null) return;
        DrawValues();
    }
    [PunRPC]
    public void RPC_UpdatePosition(Vector3 value)
    {
        transform.position = new Vector3(value.x, value.y * -1, value.z);
    }
    public void DrawValues()
    {
        layout = cardObj.GetComponent<EffectCard_Layout>();
        ((EffectCard_Layout)layout).NameTextUI.text = cardStats.CardName.ToString();
        ((EffectCard_Layout)layout).PlayCostTextUI.text = cardStats.PlayCost.ToString();
    }
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        base.CardStats = player.StartingDeck[index].gameObject.GetComponent<EffectCardStats>();
    }
    private void OnMouseDown()
    {
        if (gameManager.State == MainPhaseStates.StartPhase)
        {
            mouseDownPos = this.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        }
    }
    private void OnMouseDrag()
    {
        if (gameManager.State == MainPhaseStates.StartPhase)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            Debug.Log(mousePos.x);
            Debug.Log(mousePos.y);
            if (Location == CardLocation.Hand) transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z); 
        }
    }
    private void OnMouseUp()
    {
        if (gameManager.State == MainPhaseStates.StartPhase)
        {
            if (player.Mana >= base.CardStats.PlayCost)
            {
                if(((Vector2)Board.Instance.gameObject.transform.position - (Vector2)transform.position).magnitude < 20)
                {
                    transform.position = new Vector3(Board.Instance.transform.position.x, Board.Instance.gameObject.transform.position.y, transform.position.z);
                    player.Mana -= cardStats.PlayCost;
                    StartCoroutine(Play());
                    return;
                }
            }
            transform.position = mouseDownPos;
        }
    }
    private IEnumerator Play()
    {
        if (((EffectCardStats)cardStats).Effect != null) ((EffectCardStats)cardStats).Effect.OnPlay?.Invoke();
        Vector3 direction;
        player.Hand.Remove(this);
        player.RedrawHandCards();
        transform.localScale *= 1.2f;
        yield return new WaitForSeconds(4);
        transform.localScale /= 1.2f;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = Board.Instance.PlayerGraveyard.transform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        transform.position = Board.Instance.PlayerGraveyard.transform.position;
        Location = CardLocation.Graveyard;
        player.Graveyard.Add(this);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
