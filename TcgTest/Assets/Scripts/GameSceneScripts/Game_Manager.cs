using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

// Game_Manager: Manages players and assigns cardevents
public class Game_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]private ParticleManager particleManager;
    public static Game_Manager Instance;
    private int round = 0;
    private int turn = 1;
    public int Round { get => round; set => photonView.RPC(nameof(RPC_UpdateRound), RpcTarget.All, value); }
    public int Turn { get => turn; }
    private Card blockingMonster;
    /// <summary> BlockingMonsterIndex
    /// <see cref="BlockingMonsterIndex"/>
    /// </summary>
    /// <remarks>
    /// Used by other client to set or to not set a blocking monster.
    /// <see cref="RPC_UpdateBlockingMonsterIndex"/>
    /// </remarks>
    public int BlockingMonsterIndex 
    { 
        get => 0; 
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.Others, value);
    }
    public MyPlayer Player { get; set; }
    public MyPlayer Enemy { get; set; }
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
    public Card AttackingMonster { get; set; }
    public int DiscardCounter { get; set; }
    public int DestroyCounter { get; set; }
    private bool executingEffects;
    public bool ExecutingEffects { get => executingEffects; set => photonView.RPC(nameof(SetExecutingEffect),RpcTarget.All, value); }
    public ParticleManager ParticleManager { get => particleManager; set => particleManager = value; }

    private GameManagerStates stateToSet;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    public void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);

    }
    /// <summary> 
    /// <see cref="RPC_UpdateCurrentDuelist"/>
    /// </summary>
    /// <param name="type"> Enum describes duelist type </param>
    /// <remarks>
    /// Draw starting cards in time intervalls
    /// </remarks>
    [PunRPC]
    public void RPC_UpdateCurrentDuelist(DuelistType type)
    {
        if (type == DuelistType.Player) currentDuelist = DuelistType.Enemy;
        else if (type == DuelistType.Enemy) currentDuelist = DuelistType.Player;

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
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
        ((MonsterCard)AttackingMonster).Call_UpdateSwordIcon(false);

        if (index == 6)
        {
            if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.Call_OnDirectAttack();
            Player.Call_DrawCards(1);
            AttackingMonster.ClearEvents();
            return;
        }
        blockingMonster = Enemy.Field[index];
        if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.Call_OnAttack();
        int value =((MonsterCardStats)AttackingMonster.CardStats).Attack - ((MonsterCardStats)blockingMonster.CardStats).Defense;
        if (value > 0)
        {
            blockingMonster.Call_ParticleBomb((-value).ToString(), Color.red, NetworkTarget.All);
            AttackingMonster.Call_ParticleBomb(value.ToString(), Color.green, NetworkTarget.All);
            blockingMonster.Call_SendToGraveyard();
        }
        else if (value < 0)
        {
            blockingMonster.Call_ParticleBomb((-value).ToString(), Color.red, NetworkTarget.All);
            AttackingMonster.Call_ParticleBomb(value.ToString(), Color.green, NetworkTarget.All);
            AttackingMonster.Call_SendToGraveyard();
        }
        Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.AttackPhase);
        Call_SetMainPhaseState(NetworkTarget.Other, GameManagerStates.Busy);
    }
    public void StartTurn()
    {
        CurrentDuelist = DuelistType.Player;
        Player.Mana = turn + Player.ManaBoost;
        Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.StartPhase);
        Call_SetMainPhaseState(NetworkTarget.Other, GameManagerStates.Busy);
        if(!(round == 0 && turn == 1)) Player.Call_DrawCards(1);
        for (int i = 0; i < Player.Field.Count; i++)
        {
            Player.Field[i].HasAttacked = false;
            Player.Field[i].HasBlocked = false;
        }
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
    /// <summary>
    /// Asdf
    /// <see cref="Call_SetMainPhaseState"> This is where this gets called usually</see>
    /// </summary>
    /// <param name="value"></param>
    [PunRPC]
    public void RPC_SetMainPhaseState(GameManagerStates value)
    {
        SetMainPhaseState(value);
    }
    public void SetMainPhaseState(GameManagerStates value)
    {
        if (ExecutingEffects)
        {
            stateToSet = value;
            StartCoroutine(WaitUntilFinishedExecutingEffectBeforeSetMainPhaseState());
            return;
        }
        PrevState = state;
        state = value;
        Board.Instance.PlayerInfoText.text = value.ToString();
        if(state!= GameManagerStates.StartPhase || currentDuelist == DuelistType.Enemy) GameUIManager.Instance.AttackButton.SetActive(false);
        else GameUIManager.Instance.AttackButton.SetActive(true);
        if (state != GameManagerStates.StartPhase && state != GameManagerStates.AttackPhase || currentDuelist == DuelistType.Enemy) GameUIManager.Instance.EndTurnButton.gameObject.SetActive(false);
        else GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        foreach (MonsterCard c in Player.Field) c.ClearEvents();
        foreach (Card c in Player.Hand) c.ClearEvents();
        foreach (MonsterCard c in Enemy.Field) c.ClearEvents();
        foreach (Card c in Enemy.Hand) c.ClearEvents();
        switch (state)
        {
            case GameManagerStates.StartPhase:
                GameUIManager.Instance.AttackButton.SetActive(true);
                if(round == 0 && turn == 1) 
                    GameUIManager.Instance.AttackButton.SetActive(false);
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
                GameUIManager.Instance.AttackButton.SetActive(false);
                foreach (Card c in Player.Hand) c.ClearEvents();
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Assign_AttackPhaseEvents(NetworkTarget.Local);
                }
                break;
            case GameManagerStates.Blocking:
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Call_AddEvent(CardEvent.Block, MouseEvent.Down, NetworkTarget.Local);
                }
                break;
            case GameManagerStates.Busy:
                break;
        }
    }
    private IEnumerator WaitUntilFinishedExecutingEffectBeforeSetMainPhaseState()
    {
        while (ExecutingEffects) { yield return new WaitForFixedUpdate(); }
        SetMainPhaseState(stateToSet);
    }
    private IEnumerator WaitUntilFinishedExecutingEffectBeforeSettingToPrevState()
    {
        while (ExecutingEffects) { yield return new WaitForFixedUpdate(); }
        SetMainPhaseState(PrevState);
    }
    public void Call_SetMainPhaseStateToPrevious(NetworkTarget networkTarget)
    {
        if (networkTarget == NetworkTarget.Local) { SetMainPhaseStateToPrevious(); }
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
        StartCoroutine(WaitUntilFinishedExecutingEffectBeforeSettingToPrevState());
    }
    [PunRPC]
    public void SetExecutingEffect(bool value)
    {
        executingEffects = value;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
