using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

// Game_Manager: Manages players and assigns cardevents
public class Game_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Game_Manager Instance;
    private int round = 0;
    private int turn = 1;
    /// <summary>
    /// </summary>
    /// <remarks>
    /// When a player ends his round, this value increases by 1.
    /// When this value equals 2:
    /// <see cref = "Turn"/>
    /// is increased by 1 and this value gets set back to 0.
    /// </remarks>
    public int Round { get => round; set => photonView.RPC(nameof(RPC_UpdateRound), RpcTarget.All, value); }
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Used by other scripts for referencing:
    /// <see cref="turn"/>
    /// </remarks>
    public int Turn { get => turn; }
    private Card blockingMonster;
    /// <summary>
    /// Used by other client to set or to not set a blocking monster on local client. 
    /// </summary>
    /// <remarks>
    /// Calls:
    /// <see cref="RPC_UpdateBlockingMonsterIndex"/>.
    /// Sets on other client:
    /// <see cref="blockingMonster"/>.
    /// </remarks>
    public int BlockingMonsterIndex 
    { 
        get => 0; 
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.Others, value);
    }
    /// <summary>
    /// </summary>
    /// <remarks>
    /// When local player is instantiated, it automatically assigns himself to this value.
    /// </remarks>
    public MyPlayer Player { get; set; }
    /// <summary>
    /// </summary>
    /// <remarks>
    /// When non-local, opposing player is instantiated, it automatically assigns himself to this value.
    /// </remarks>
    public MyPlayer Enemy { get; set; }
    private DuelistType currentDuelist;
    /// <summary>
    /// </summary>
    /// <remarks>
    /// Accesses: 
    /// <see cref="currentDuelist"/>.
    /// This value is set locally and indicates which player is the current playing duelist.
    /// Set to player:
    /// <see cref="StartTurn"/>
    /// Set to enemy: 
    /// <see cref="GameUIManager.EndTurn"/>
    /// </remarks>
    public DuelistType CurrentDuelist 
    {
        get => currentDuelist;
        set
        {
            currentDuelist = value;
            photonView.RPC(nameof(RPC_UpdateCurrentDuelist), RpcTarget.Others, value);
        }
    }

    private TurnState state;
    /// <summary>
    /// Mainly used for enabling and disabling user controls. 
    /// This is done mainly by managing card events.
    /// </summary>
    /// <remarks>
    /// Accesses:
    /// <see cref="state"/>.
    /// Set by: 
    /// <see cref="SetMainPhaseState(TurnState)"/>
    /// , or by: 
    /// <see cref="SetMainPhaseStateToPrevious"/>
    /// </remarks>
    public TurnState State  { get => state; }
    /// <summary>
    /// Used for setting TurnState to the previous state.
    /// For example, from SummoningState to StartPhaseState.
    /// This is done mainly by managing card events.
    /// </summary>
    /// <remarks>
    /// Accesses:
    /// <see cref="PrevState"/>.
    /// Set by: 
    /// <see cref="SetMainPhaseStateToPrevious()"/>
    /// before changing the current state.
    /// </remarks>
    public TurnState PrevState { get; set; }
    /// <summary>
    /// Used for storing a reference to the attacking
    /// <see cref="MonsterCard"/>,
    /// while the opponent is selecting the
    /// <see cref="BlockingMonster"/>.
    /// </summary>
    /// <remarks>
    /// Set at: 
    /// <see cref="MonsterCard.Event_Attack"/>.
    /// Reference stored for:
    /// <see cref="RPC_UpdateBlockingMonsterIndex(int)"/>
    /// </remarks>
    public Card AttackingMonster { get; set; }
    /// <summary>
    /// Used for managing effect of type:
    /// <see cref="DiscardEffect"/>. 
    /// While this value is greater than 0, or there are no more cards left in:
    /// <see cref="MyPlayer.Hand"/>,
    /// controls other than discarding are disabled.
    /// </summary>
    /// <remarks>
    /// Set by:
    /// <see cref="MyPlayer"/>.
    /// Mainly used for:
    /// <see cref="MyPlayer.AddDiscardEffects"/>
    /// </remarks>
    public int DiscardCounter { get; set; }
    /// <summary>
    /// Used for managing effect of type:
    /// <see cref="DestroyEffect"/>. 
    /// While this value is greater than 0, or there are no more cards left in:
    /// <see cref="MyPlayer.Field"/>,
    /// controls other than destroying are disabled.
    /// </summary>
    /// <remarks>
    /// Set by:
    /// <see cref="MyPlayer"/>.
    /// Mainly used for:
    /// <see cref="MyPlayer.AddDestroyEvents"/>
    /// </remarks>
    public int DestroyCounter { get; set; }
    private bool executingEffects;
    /// <summary>
    /// Used for preventing multiple
    /// <see cref="Effect"/>s
    /// from being executed at once.
    /// Also this prevents the
    /// <see cref="State"/>
    /// from changing on either player.
    /// </summary>
    /// <remarks>
    /// Calls:
    /// <see cref="RPC_SetExecutingEffect(bool)"/>.
    /// Sets on all Clients:
    /// <see cref="executingEffects"/>.
    /// </remarks>
    public bool ExecutingEffects { get => executingEffects; set => photonView.RPC(nameof(RPC_SetExecutingEffect),RpcTarget.All, value); }

    private TurnState stateToSet;
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
    /// Use this, for setting the 
    /// <see cref="currentDuelist"/>
    /// on other clients.
    /// </summary>
    /// <remarks>
    /// Mainly called by:
    /// <see cref="CurrentDuelist"/>
    /// </remarks>
    [PunRPC]
    public void RPC_UpdateCurrentDuelist(DuelistType type)
    {
        if (type == DuelistType.Player) currentDuelist = DuelistType.Enemy;
        else if (type == DuelistType.Enemy) currentDuelist = DuelistType.Player;

    }
    /// <summary>
    /// Use this, for updating
    /// <see cref="round"/>
    /// on other clients.
    /// </summary>
    /// <remarks>
    /// Used/called by 
    /// <see cref="Round"/>.
    /// </remarks>
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
        Call_SetMainPhaseState(NetworkTarget.Local, TurnState.AttackPhase);
        Call_SetMainPhaseState(NetworkTarget.Other, TurnState.Busy);
    }
    public void StartTurn()
    {
        CurrentDuelist = DuelistType.Player;
        Player.Mana = turn + Player.ManaBoost;
        Call_SetMainPhaseState(NetworkTarget.Local, TurnState.StartPhase);
        Call_SetMainPhaseState(NetworkTarget.Other, TurnState.Busy);
        if(!(round == 0 && turn == 1)) Player.Call_DrawCards(1);
        for (int i = 0; i < Player.Field.Count; i++)
        {
            Player.Field[i].HasAttacked = false;
            Player.Field[i].HasBlocked = false;
        }
    }
    public void SetStateLocally(TurnState value)
    {
        state = value;
    }
    public void Call_SetMainPhaseState(NetworkTarget networkTarget, TurnState value)
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
    public void RPC_SetMainPhaseState(TurnState value)
    {
        SetMainPhaseState(value);
    }
    public void SetMainPhaseState(TurnState value)
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
        if(state!= TurnState.StartPhase || currentDuelist == DuelistType.Enemy) GameUIManager.Instance.AttackButton.SetActive(false);
        else GameUIManager.Instance.AttackButton.SetActive(true);
        if (state != TurnState.StartPhase && state != TurnState.AttackPhase || currentDuelist == DuelistType.Enemy) GameUIManager.Instance.EndTurnButton.gameObject.SetActive(false);
        else GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        foreach (MonsterCard c in Player.Field) c.ClearEvents();
        foreach (Card c in Player.Hand) c.ClearEvents();
        foreach (MonsterCard c in Enemy.Field) c.ClearEvents();
        foreach (Card c in Enemy.Hand) c.ClearEvents();
        switch (state)
        {
            case TurnState.StartPhase:
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
            case TurnState.AttackPhase:
                GameUIManager.Instance.AttackButton.SetActive(false);
                foreach (Card c in Player.Hand) c.ClearEvents();
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Assign_AttackPhaseEvents(NetworkTarget.Local);
                }
                break;
            case TurnState.Blocking:
                foreach (MonsterCard c in Player.Field)
                {
                    c.ClearEvents();
                    c.Call_AddEvent(CardEvent.Block, MouseEvent.Down, NetworkTarget.Local);
                }
                break;
            case TurnState.Busy:
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
    public void RPC_SetExecutingEffect(bool value)
    {
        executingEffects = value;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
