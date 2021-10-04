using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
public class EffectCard : Card,IPunObservable
{
    public int CardStatsIndex { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
    private void Start()
    {
        try
        { 
            Layout = cardObj.gameObject.GetComponent<EffectCard_Layout>();
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
    private void OnValidate()
    {
        if (cardStats == null) return;
        DrawValues();
    }
    public void DrawValues()
    {
        Layout = cardObj.GetComponent<EffectCard_Layout>();
        ((EffectCard_Layout)Layout).NameTextUI.text = cardStats.CardName.ToString();
        ((EffectCard_Layout)Layout).PlayCostTextUI.text = cardStats.PlayCost.ToString();
    }
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        base.CardStats = player.StartingDeck[index].gameObject.GetComponent<EffectCardStats>();
    }
    private void OnMouseDown()
    {
        GameUIManager.Instance.CardInfo.AssignCard(this);
        mouseDownPos = transform.position;
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        OnMouseDownEvent?.Invoke();
    }

    private void OnMouseDrag()
    {
        mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        OnMouseDragEvent?.Invoke();
    }
    private void OnMouseUp()
    {
        OnMouseUpEvent?.Invoke();
    }
    private IEnumerator Play()
    {
        if (((Vector2)Board.Instance.gameObject.transform.position - (Vector2)transform.position).magnitude < 20)
        {
            transform.position = new Vector3(Board.Instance.transform.position.x, Board.Instance.gameObject.transform.position.y, transform.position.z);
            player.Mana -= cardStats.PlayCost;

            if (((EffectCardStats)cardStats).Effect != null) ((EffectCardStats)cardStats).Effect.OnPlay?.Invoke();
            Vector3 direction;
            Local_RemoveFromCurrentLists();
            player.RedrawHandCards();
            transform.localScale *= 1.3f;
            yield return new WaitForSeconds(4);
            transform.localScale /= 1.3f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                direction = Board.Instance.PlayerGraveyard.transform.position - transform.position;
                transform.position += direction.normalized * Time.fixedDeltaTime * 25;
                if (direction.magnitude < 0.3f) break;
            }
        }
        else transform.position = mouseDownPos;
    }
    private IEnumerator SendToGraveyard()
    {
        Vector3 direction = new Vector3();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = player.GraveyardObj.transform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        transform.position = Board.Instance.PlayerGraveyard.transform.position;
        Location = CardLocation.Graveyard;
        player.Graveyard.Add(this);
    }
    public void Assign_PlayEvents(NetworkTarget networkTarget)
    {
        ClearEvents();
        Call_AddEvent(CardEvent.FollowMouse_MouseDown, MouseEvent.Down, networkTarget);
        Call_AddEvent(CardEvent.FollowMouse_MouseDrag, MouseEvent.Drag, networkTarget);
        Call_AddEvent(CardEvent.Play, MouseEvent.Up, networkTarget);
    }
    public override void Call_AddEvent(CardEvent cardEvent, MouseEvent mouseEvent, NetworkTarget target)
    {
        if (target == NetworkTarget.Local) AddEvent(cardEvent, mouseEvent);
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_AddEvent), RpcTarget.Others, cardEvent, mouseEvent);
        else photonView.RPC(nameof(RPC_AddEvent), RpcTarget.All, cardEvent, mouseEvent);
    }
    [PunRPC]
    public void RPC_AddEvent(CardEvent cardEvent, MouseEvent mouseEvent)
    {
        AddEvent(cardEvent, mouseEvent);
    }
    public void AddEvent(CardEvent cardEvent, MouseEvent mouseEvent)
    {
        border.color = Color.yellow;
        switch (cardEvent)
        {
            case CardEvent.FollowMouse_MouseDown:
                AssignEvent(Event_FollowMouseDown, mouseEvent);
                break;
            case CardEvent.FollowMouse_MouseDrag:
                AssignEvent(Event_FollowMouseDrag, mouseEvent);
                break;
            case CardEvent.Discard:
                AssignEvent(Event_Discard, mouseEvent);
                break;
            case CardEvent.Play:
                AssignEvent(Event_Play, mouseEvent);
                break;
            case CardEvent.Recall:
                AssignEvent(Event_Recall, mouseEvent);
                break;
        }
    }
    public void Event_Play()
    {
        if (player.Mana < cardStats.PlayCost) { transform.position = mouseDownPos; return; }
        StartCoroutine(Play());
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
}
