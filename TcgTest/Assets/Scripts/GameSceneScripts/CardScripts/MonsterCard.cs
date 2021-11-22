using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
public class MonsterCard : Card
{
    private Card attackTarget;
    public bool HasAttacked { get; set; }
    public bool HasBlocked { get; set; }
    [SerializeField] private GameObject swordIcon;
    [SerializeField] private ParticleSystem summonParticles;
    [SerializeField] private ParticleSystem burnParticles;
    [SerializeField] private ParticleSystem attackParticles;
    [SerializeField] private ParticleSystem blockParticles;
    private void Start()
    {
        try
        {
            Layout = GetComponent<MonsterCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject);
        }

        photonView.RPC(nameof(RPC_AddToDeck), RpcTarget.All);
        Type = CardType.Monster;
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
                photonView.RPC(nameof(RPC_ShowAttackArrow), RpcTarget.Others, target, true); 
                attackTarget = gameManager.Enemy.Field[i];
                closestDistance = distance;
            }
        }
        if (closestDistance > 20)
        {
            attackTarget = null;
            if (Mathf.Abs(Mathf.Abs(start.y) - Mathf.Abs(Board.Instance.EnemyHandParent.transform.position.y)) < 20)
            {
                target = Board.Instance.EnemyHandParent.transform.position;
                photonView.RPC(nameof(RPC_ShowAttackArrow), RpcTarget.Others, target, true); 
            }
            else
            {
                target = start;
                photonView.RPC(nameof(RPC_ShowAttackArrow), RpcTarget.Others, Vector3.zero, false);
            }
        }
        GameUIManager.Instance.Arrow.Draw(transform.position,target);
    }
    [PunRPC]
    public void RPC_ShowAttackArrow(Vector3 vector1, bool value)
    {
        if(value) GameUIManager.Instance.Arrow.Draw(transform.position, new Vector3(vector1.x,-vector1.y,vector1.z));
        else GameUIManager.Instance.Arrow.Hide();
    }
    public void Event_Attack()
    {
        photonView.RPC(nameof(RPC_UpdateSwordIcon), RpcTarget.All, false);
        if (attackTarget != null)
        {
            photonView.RPC(nameof(RPC_PlayAttackParticles), RpcTarget.All);
            if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.Call_OnAttack();
            if (((MonsterCardStats)attackTarget.CardStats).Effect != null) ((MonsterCardStats)attackTarget.CardStats).Effect.Call_OnBlock();
            int value = ((MonsterCardStats)cardStats).Attack - ((MonsterCardStats)attackTarget.CardStats).Defense;
            if (value > 0)
            {
                AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.Other);
                AudioManager.Instance.Call_PlaySound(AudioType.Destroy, NetworkTarget.Local);
                attackTarget.Call_ParticleBomb((-value).ToString(), Color.red, NetworkTarget.All);
                Call_ParticleBomb(value.ToString(), Color.green, NetworkTarget.All);
                ((MonsterCard)attackTarget).Call_SendToGraveyard();
                ((MonsterCardStats)cardStats).Effect.Call_BattleWon();
            }
            else if (value < 0)
            {
                AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.Local);
                AudioManager.Instance.Call_PlaySound(AudioType.Destroy, NetworkTarget.Other);
                ((MonsterCard)attackTarget).Call_PlayBlockParticles();
                ((MonsterCardStats)attackTarget.CardStats).Effect.Call_BlockSuccessfull();
                Call_ParticleBomb(value.ToString(), Color.red, NetworkTarget.All);
                attackTarget.Call_ParticleBomb(Mathf.Abs(value).ToString(), Color.green,NetworkTarget.All);
                Call_SendToGraveyard();
            }
            ClearEvents();
            HasAttacked = true;
        }
        else if (GameUIManager.Instance.Arrow.Pos2.x == Board.Instance.EnemyHandParent.transform.position.x
            || GameUIManager.Instance.Arrow.Pos2.y == Board.Instance.EnemyHandParent.transform.position.y)
        {
            if (gameManager.Enemy.Field.Count == 0)
            {
                AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.All);
                photonView.RPC(nameof(RPC_PlayAttackParticles), RpcTarget.All);
                Player.Call_DrawCards(1);
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.Call_OnAttack();
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.Call_OnDirectAttack();
                HasAttacked = true;
            }
            else
            {
                AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.Other);
                if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.Call_OnAttack();
                gameManager.AttackingMonster = this;
                gameManager.Call_SetTurnState(NetworkTarget.Local, TurnState.Busy);
                HasAttacked = true;
                gameManager.Enemy.ShowBlockRequest();
            }
            ClearEvents();
        }
        GameUIManager.Instance.Arrow.Hide();
        photonView.RPC(nameof(RPC_ShowAttackArrow), RpcTarget.Others, Vector3.zero, false);
    }
    public void Event_Summon()
    {
        GameUIManager.Instance.ParticleManager.Local_Stop(ParticleType.CardOverField);
        if (Player.Mana >= CardStats.PlayCost)
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 direction = Board.Instance.PlayerMonsterFields[i].transform.position - transform.position;
                if (direction.magnitude < 12)
                {
                    foreach (MonsterCard card in Player.Field)
                       if ((card.gameObject.transform.position - Board.Instance.PlayerMonsterFields[i].transform.position).magnitude < 12)
                       {
                           transform.position = mouseDownPos;
                           return;
                       }
                    //AudioManager.Instance.Call_PlaySound(AudioType.Summon, NetworkTarget.Local);
                    ClearEvents();
                    transform.position = Board.Instance.PlayerMonsterFields[i].transform.position;
                    Location = CardLocation.Field;
                    photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
                    photonView.RPC(nameof(RPC_AddToField), RpcTarget.All);
                    Player.RedrawHandCards();
                    Player.Mana -= CardStats.PlayCost;
                    photonView.RPC(nameof(SetRotation), RpcTarget.All, new Quaternion(0, 0, 0, 0));
                    Assign_BurnEvents(NetworkTarget.Local);
                    ((MonsterCardStats)cardStats).Effect?.Call_OnSummon();
                    photonView.RPC(nameof(PlaySummonParticles), RpcTarget.All);
            
                    return;
                }
            }
        }
        transform.position = mouseDownPos;
    }
    public void Call_PlayBlockParticles()
    {
        photonView.RPC(nameof(RPC_PlayBlockParticles), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_PlayBlockParticles()
    {
        blockParticles.Play();
    }
    [PunRPC]
    public void PlaySummonParticles()
    {
        AudioManager.Instance.Call_PlaySound(AudioType.Summon, NetworkTarget.Local);
        summonParticles.Play();
    }
    [PunRPC]
    public void RPC_PlayBurnParticles()
    {
        AudioManager.Instance.Call_PlaySound(AudioType.Burn, NetworkTarget.Local);
        ParticleSystem system = Instantiate(burnParticles, this.transform.position,Quaternion.identity).GetComponent<ParticleSystem>();
        system.Play();
    }
    [PunRPC]
    public void RPC_PlayAttackParticles()
    {
        attackParticles.Play();
    }
    public void Event_Block()
    {
        if (HasBlocked) { Board.Instance.PlayerInfoText.text = "Already blocked!"; return; }
        ((MonsterCardStats)CardStats).Effect?.Call_OnBlock();
        gameManager.BlockingMonsterIndex = Player.Field.IndexOf(this);
        HasBlocked = true;
    }
    public void Event_Burn()
    {
        if (((Vector2)Board.Instance.BurnField.transform.position - (Vector2)transform.position).magnitude < 10)
        {
            photonView.RPC(nameof(RPC_PlayBurnParticles), RpcTarget.All);
            photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
            photonView.RPC(nameof(RPC_AddToGraveyard), RpcTarget.All);
            Player.Mana += ((MonsterCardStats)cardStats).PlayCost;
            if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.Call_OnDestroy();
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
        if(!Player.Field.Contains(this)) Player.Field.Add(this);
        ((MonsterCardStats)CardStats).Attack += Player.AttackBoost;
        ((MonsterCard_Layout)layout).AttackTextUI.text = ((MonsterCardStats)CardStats).Attack.ToString();
        ((MonsterCardStats)CardStats).Defense += Player.DefenseBoost;
        ((MonsterCard_Layout)layout).DefenseTextUI.text = ((MonsterCardStats)CardStats).Defense.ToString();
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
        if(!HasAttacked)Call_AddEvent(CardEvent.Attack, MouseEvent.Up, networkTarget);
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
            case CardEvent.CardOverField:
                AssignEvent(Event_SummonFollowMouseDrag, mouseEvent);
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
}
