using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class MonsterCard : Card, IPunObservable
{
    public int CardStatsIndex { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
    private Card attackTarget;
    public bool HasAttacked { get; set; }
    public bool HasBlocked { get; set; }
    [SerializeField] private GameObject swordIcon;
    private void Start()
    {
        try
        {
            Layout = cardObj.gameObject.GetComponent<MonsterCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject);
        }
        
        player.Subscribe(this);
        Type = CardType.Monster;
    }
    private void OnValidate()
    {
        if (cardStats == null) return;
       // DrawValues();
    }
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        base.CardStats = player.StartingDeck[index].gameObject.GetComponent<MonsterCardStats>();
    }
    public void DrawValues()
    {
        Layout = cardObj.GetComponent<MonsterCard_Layout>();
        ((MonsterCard_Layout)Layout).AttackTextUI.text = ((MonsterCardStats)cardStats).Attack.ToString();
        ((MonsterCard_Layout)Layout).NameTextUI.text = ((MonsterCardStats)cardStats).CardName.ToString();
        ((MonsterCard_Layout)Layout).PlayCostTextUI.text = ((MonsterCardStats)cardStats).PlayCost.ToString();
        ((MonsterCard_Layout)Layout).DefenseTextUI.text = ((MonsterCardStats)cardStats).Defense.ToString();
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
    public void Event_DrawLine_MouseDown()
    {
        photonView.RPC(nameof(RPC_UpdateSwordIcon), RpcTarget.All, true);
    }
    public void Event_DrawLine_MouseDrag()
    {
        if (l == null)
            l = gameObject.AddComponent<LineRenderer>();

        Vector3 target = Vector3.zero;
        float closestDistance = 1000;
        Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        for (int i = 0; i < gameManager.Enemy.Field.Count; i++)
        {
            float distance = (start - gameManager.Enemy.Field[i].transform.position).magnitude;
            if (distance < closestDistance)
            {
                target = gameManager.Enemy.Field[i].transform.position;
                attackTarget = gameManager.Enemy.Field[i];
                closestDistance = distance;
            }
        }
        if (closestDistance > 20)
        {
            attackTarget = null;
            if ((start - Board.Instance.EnemyHandParent.transform.position).magnitude < 15)
            {
                target = Board.Instance.EnemyHandParent.transform.position;
            }
            else target = start;
        }
        List<Vector3> pos = new List<Vector3>();
        pos.Add(transform.position);
        pos.Add(target);
        l.startWidth = 1f;
        l.endWidth = 1f;
        l.SetPositions(pos.ToArray());
        l.useWorldSpace = true;
    }
    public void Event_Attack()
    {
        if (attackTarget != null)
        {
            photonView.RPC(nameof(RPC_UpdateSwordIcon), RpcTarget.All, false);

            if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnAttack?.Invoke();
            if (((MonsterCardStats)attackTarget.CardStats).Effect != null) ((MonsterCardStats)attackTarget.CardStats).Effect.OnBlock?.Invoke();
            if (((MonsterCardStats)attackTarget.CardStats).Defense < ((MonsterCardStats)cardStats).Attack)
            {
                ((MonsterCard)attackTarget).Call_SendToGraveyard();
            }
            else if (((MonsterCardStats)attackTarget.CardStats).Defense > ((MonsterCardStats)cardStats).Attack)
            {
                Call_SendToGraveyard();
            }
            HasAttacked = true;
        }
        else if (l.GetPosition(1) == Board.Instance.EnemyHandParent.transform.position)
        {
            if (gameManager.Enemy.Field.Count == 0)
            {
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnAttack?.Invoke();
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnDirectAttackSucceeds?.Invoke();
                player.DrawCard(0);
                photonView.RPC(nameof(RPC_UpdateSwordIcon), RpcTarget.All, false);
            }
            else
            {
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnAttack?.Invoke();
                gameManager.AttackingMonster = this;
                gameManager.Enemy.ShowBlockRequest();
            }
            HasAttacked = true;
        }
        Destroy(l);
        l = null;
    }
    public void Event_Summon()
    {
        if (player.Mana >= base.CardStats.PlayCost)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 direction = Board.Instance.PlayerMonsterFields[i].transform.position - transform.position;
                if (direction.magnitude < 5)
                {
                    ClearEvents();
                    transform.position = Board.Instance.PlayerMonsterFields[i].transform.position;
                    Location = CardLocation.Field;
                    photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
                    photonView.RPC(nameof(RPC_AddToField), RpcTarget.All);
                    player.RedrawHandCards();
                    player.Mana -= base.CardStats.PlayCost;
                    photonView.RPC(nameof(SetRotation), RpcTarget.All, new Quaternion(0,0,0,0));
                    if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnSummon?.Invoke();
                    Assign_BurnEvents(NetworkTarget.Local);
                    return;
                }
            }
        }
        transform.position = mouseDownPos;
    }
    [PunRPC]
    public void SetRotation(Quaternion q)
    {
        transform.rotation = q;
    }
    public void Event_Block()
    {
        if (HasBlocked) { Board.Instance.PlayerInfoText.text = "Already blocked!"; return; }

        gameManager.BlockingMonsterIndex = player.Field.IndexOf(this);
        HasBlocked = true;
    }
    public void Event_Burn()
    {
        if (((Vector2)Board.Instance.BurnField.transform.position - (Vector2)transform.position).magnitude < 10)
        {
            photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
            player.Mana += ((MonsterCardStats)cardStats).PlayCost;
            if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
            PhotonNetwork.Destroy(this.gameObject);
        }
        else
        {
            transform.position = mouseDownPos;
        }
    }
    [PunRPC]
    public void RPC_DrawLine(Vector3 target)
    {
        List<Vector3> pos = new List<Vector3>();
        pos.Add(transform.position);
        pos.Add(new Vector3(target.x, -target.y, target.z));
        l.startWidth = 1f;
        l.endWidth = 1f;
        l.SetPositions(pos.ToArray());
        l.useWorldSpace = true;
    }
    [PunRPC]
    public void RPC_AddToField()
    {
        if (photonView.IsMine) player.Field.Add(this);
        else gameManager.Enemy.Field.Add(this);
        ((MonsterCardStats)CardStats).Attack += player.AttackBoost;
        ((MonsterCard_Layout)Layout).AttackTextUI.text = ((MonsterCardStats)CardStats).Attack.ToString();
        ((MonsterCardStats)CardStats).Defense += player.DefenseBoost;
        ((MonsterCard_Layout)Layout).DefenseTextUI.text = ((MonsterCardStats)CardStats).Defense.ToString();
    }
    public void Assign_BurnEvents(NetworkTarget networkTarget)
    {
        ClearEvents();
        Call_AddEvent(CardEvent.FollowMouse_MouseDown, MouseEvent.Down, networkTarget);
        Call_AddEvent(CardEvent.FollowMouse_MouseDrag, MouseEvent.Drag, networkTarget);
        Call_AddEvent(CardEvent.Burn, MouseEvent.Up, networkTarget);
    }
    public void Assign_AttackPhaseEvents(NetworkTarget networkTarget)
    {
        ClearEvents();
        Call_AddEvent(CardEvent.DrawLine_MouseDown, MouseEvent.Down, networkTarget);
        Call_AddEvent(CardEvent.DrawLine_MouseDrag, MouseEvent.Drag, networkTarget);
        Call_AddEvent(CardEvent.Attack, MouseEvent.Up, networkTarget);
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
            case CardEvent.DrawLine_MouseDrag:
                AssignEvent(Event_DrawLine_MouseDrag, mouseEvent);
                break;
            case CardEvent.DrawLine_MouseDown:
                AssignEvent(Event_DrawLine_MouseDown, mouseEvent);
                break;
            case CardEvent.Summon:
                AssignEvent(Event_Summon, mouseEvent);
                break;
            case CardEvent.Attack:
                AssignEvent(Event_Attack, mouseEvent);
                break;
            case CardEvent.Block:
                AssignEvent(Event_Block, mouseEvent);
                break;
            case CardEvent.Burn:
                AssignEvent(Event_Burn, mouseEvent);
                break;
            case CardEvent.Destroy:
                AssignEvent(Event_Destroy, mouseEvent);
                break;
            case CardEvent.Discard:
                AssignEvent(Event_Discard, mouseEvent);
                break;
            case CardEvent.Recall:
                AssignEvent(Event_Recall, mouseEvent);
                break;
        }
    }
    public void Call_UpdateSwordIcon(bool value)
    {
        photonView.RPC(nameof(RPC_UpdateSwordIcon), RpcTarget.All, true);
    }
    [PunRPC]
    public void RPC_UpdateSwordIcon(bool value)
    {
        swordIcon.SetActive(value);
    }
    private void OnDestroy()
    {
        Local_RemoveFromCurrentLists();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
