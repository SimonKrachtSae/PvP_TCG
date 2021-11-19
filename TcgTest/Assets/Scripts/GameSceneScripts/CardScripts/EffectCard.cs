using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
public class EffectCard : Card
{
    [SerializeField] private ParticleSystem playParticles;
    private void Start()
    {
        try
        { 
            Layout = GetComponent<EffectCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch 
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject); 
        }


        photonView.RPC(nameof(RPC_AddToDeck), RpcTarget.All);
        Type = CardType.Effect;
    }
    private void OnMouseDown()
    {
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
        if (Player == null) return;
        if (Player.Hand.Contains(this) && transform.position.y != Player.HandParent.transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, Player.HandParent.transform.position.y, transform.position.z);
            Player.RedrawHandCards();
        }
    }
    private IEnumerator Play()
    {
        if (((Vector2)Board.Instance.gameObject.transform.position - (Vector2)transform.position).magnitude < 25)
        {
            transform.position = new Vector3(Board.Instance.transform.position.x, Board.Instance.gameObject.transform.position.y, transform.position.z);
            Player.Mana -= cardStats.PlayCost;

            if (((EffectCardStats)cardStats).Effect != null) ((EffectCardStats)cardStats).Effect.Call_OnPlay();
            Vector3 direction;
            photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
            Player.RedrawHandCards();
            photonView.RPC(nameof(SetRotation), RpcTarget.All, new Quaternion(0, 0, 0, 0));
            transform.localScale *= 1.5f;
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            photonView.RPC(nameof(RPC_PlayParticles), RpcTarget.All);
            yield return new WaitForSeconds(4);
            transform.localScale /= 1.5f;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                direction = Board.Instance.PlayerGraveyard.transform.position - transform.position;
                transform.position += direction.normalized * Time.fixedDeltaTime * 25;
                if (direction.magnitude < 0.3f) break;
            }
            ClearEvents();
        }
        else transform.position = mouseDownPos;
    }
    [PunRPC]
    public void RPC_PlayParticles()
    {
        playParticles.Play();
        GameUIManager.Instance.CardInfo.AssignCard(cardStats.CardName, 2);
    }
    private IEnumerator SendToGraveyard()
    {
        Vector3 direction = new Vector3();
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = Player.GraveyardObj.transform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        transform.position = Board.Instance.PlayerGraveyard.transform.position;
        Location = CardLocation.Graveyard;
        Player.Graveyard.Add(this);
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
        backgroundAnimator.SetBool("Play", true);
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
        if (Player.Mana < cardStats.PlayCost) { transform.position = mouseDownPos; return; }
        StartCoroutine(Play());
    }
}
