using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
public class Game_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Game_Manager Instance;
    public MyPlayer Player { get; set; }
    public MyPlayer Enemy { get; set; }
    private int round = 0;
    private int turn = 1;
    public int Turn { get => turn; }
    public int Round { get => round; set => photonView.RPC(nameof(RPC_UpdateRound), RpcTarget.All, value); }
    private DuelistType currentDuelist;
    public DuelistType CurrentDuelist 
    {
        get => currentDuelist;
        set
        {
            currentDuelist = value;
            photonView.RPC(nameof(RPC_UpdateCurrentDuelist), RpcTarget.Others, value);
        }
    }

    private GameManagerStates state;
    public GameManagerStates State  { get => state; }
    public GameManagerStates PrevState { get; set; }
    private Card blockingMonster;
    public int BlockingMonsterIndex 
    { 
        get => 0; 
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.Others, value);
    }
    public Card AttackingMonster { get; set; }
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    public void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
    public void StartGame()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            CurrentDuelist = DuelistType.Player;
            Player.Mana = Turn;
            GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
            StartTurn();
        }
        StartCoroutine(DrawHandCards());
    }
    private IEnumerator DrawHandCards()
    {
        for(int i = 0; i < 5; i++)
        {
            Player.DrawCard(0);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    [PunRPC]
    public void RPC_UpdateCurrentDuelist(DuelistType type)
    {
        if (type == DuelistType.Player) currentDuelist = DuelistType.Enemy;
        else if (type == DuelistType.Enemy) currentDuelist = DuelistType.Player;

    }
    [PunRPC]
    public void RPC_UpdateRound(int value)
    {
        round = value;
        if (round == 2)
        {
            turn++;
            round = 0;
            Board.Instance.TurnCount.text = turn.ToString();
        }
    }
    [PunRPC]
    public void RPC_UpdateBlockingMonsterIndex(int index)
    {
        if (index == 6)
        {
            if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.OnDirectAttackSucceeds?.Invoke();
            Player.DrawCard(0);
            return;
        }
        blockingMonster = Enemy.Field[index];
        if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.OnAttack?.Invoke();
        if (((MonsterCardStats)blockingMonster.CardStats).Effect != null) ((MonsterCardStats)blockingMonster.CardStats).Effect.OnBlock?.Invoke();
        if (((MonsterCardStats)blockingMonster.CardStats).Defense < ((MonsterCardStats)AttackingMonster.CardStats).Attack)
        {
            blockingMonster.Call_SendToGraveyard();
        }
        else if (((MonsterCardStats)blockingMonster.CardStats).Defense > ((MonsterCardStats)AttackingMonster.CardStats).Attack)
        {
            AttackingMonster.Call_SendToGraveyard();
        }
        Call_SetMainPhaseState(NetworkTarget.Other, GameManagerStates.Busy);
        Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.AttackPhase);
    }
    public void StartTurn()
    {
        Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.StartPhase);
        Call_SetMainPhaseState(NetworkTarget.Other, GameManagerStates.Busy);
        CurrentDuelist = DuelistType.Player;
        Player.Mana = turn + Player.ManaBoost;
        Player.DrawCard(0);
        for(int i = 0; i < Player.Field.Count; i++)
        {
            Player.Field[i].HasAttacked = false;
            Player.Field[i].HasBlocked = false;
        }
        Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.StartPhase);
    }
    public void SetStateLocally(GameManagerStates value)
    {
        state = value;
    }
    public void Call_SetMainPhaseState(NetworkTarget networkTarget, GameManagerStates value)
    {
        if (networkTarget == NetworkTarget.Local) SetMainPhaseState(value);
        else if (networkTarget == NetworkTarget.Other) photonView.RPC(nameof(RPC_SetMainPhaseState), RpcTarget.Others, value);
        else if (networkTarget == NetworkTarget.All) photonView.RPC(nameof(RPC_SetMainPhaseState), RpcTarget.All, value);

    }

    [PunRPC]
    public void RPC_SetMainPhaseState(GameManagerStates value)
    {
        SetMainPhaseState(value);
    }
    public void SetMainPhaseState(GameManagerStates value)
    {
        PrevState = state;
        state = value;
        switch(state)
        {
            case GameManagerStates.StartPhase:
                foreach (Card c in Player.Hand)
                {
                    c.ClearEvents();
                    if (c.GetType().ToString() == nameof(MonsterCard)) ((MonsterCard)c).AssignHandEvents(NetworkTarget.Local);
                    else if (c.GetType().ToString() == nameof(EffectCard)) ((EffectCard)c).AssignHandEvents(NetworkTarget.Local);
                }
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Assign_BurnEvents(NetworkTarget.Local);
                }
                break;
            case GameManagerStates.AttackPhase:
                foreach (Card c in Player.Hand) c.ClearEvents();
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Assign_AttackPhaseEvents(NetworkTarget.Local);
                }
                break;
            case GameManagerStates.Blocking:
                {
                    foreach (MonsterCard c in Player.Field)
                    {
                        c.ClearEvents();
                        c.Call_AddEvent(CardEvent.Block, MouseEvent.Down, NetworkTarget.Local);
                    }
                }
                break;
            case GameManagerStates.Busy:
                {
                    foreach (MonsterCard c in Player.Field) c.ClearEvents();
                    foreach (Card c in Player.Hand)  c.ClearEvents();
                }
                break;
        }
    }
    public void Call_SetMainPhaseStateToPrevious(NetworkTarget networkTarget)
    {
        if (networkTarget == NetworkTarget.Local) SetMainPhaseStateToPrevious();
        else if (networkTarget == NetworkTarget.Other) photonView.RPC(nameof(RPC_SetMainPhaseStateToPrevious), RpcTarget.Others);
        else if (networkTarget == NetworkTarget.All) photonView.RPC(nameof(RPC_SetMainPhaseStateToPrevious), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_SetMainPhaseStateToPrevious()
    {
        SetMainPhaseStateToPrevious();
    }
    public void SetMainPhaseStateToPrevious()
    {
        SetMainPhaseState(PrevState);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
