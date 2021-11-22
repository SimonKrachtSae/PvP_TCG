using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using System.Threading.Tasks;
using UnityEngine.UI;
using System.Threading;

// Game_Manager: Manages players and assigns cardevents
public class Game_Manager : MonoBehaviourPunCallbacks
{
    public static Game_Manager Instance;
    #region Values
    private int round = 0;
    private int turn = 1;
    private Card blockingMonster;
    private DuelistType currentDuelist;
    private TurnState state;
    private TurnState stateToSet;
    private bool executingEffects;
    public IEnumerator TimerCoroutine { get; set; }
    public float TimerTime { get; set; }
    #endregion
    #region Accessors
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
    /// <summary>
    /// Used by other client to set or to not set a blocking monster on local client. 
    /// </summary>
    /// <remarks>
    /// Calls:
    /// <see cref="RPC_UpdateBlockingMonsterIndex"/>.
    /// <br></br>
    /// Sets on other client:
    /// <see cref="blockingMonster"/>.
    /// </remarks>
    public int BlockingMonsterIndex 
    { 
        get => 0; 
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.Others, value);
    }
    /// <summary>
    /// When local player is instantiated, it automatically assigns himself to this value.
    /// </summary>
    public MyPlayer Player { get; set; }
    /// <summary>
    /// When non-local, opposing 
    /// <see cref="MyPlayer"/>
    /// is instantiated, it automatically assigns himself to this value.
    /// </summary>
    public MyPlayer Enemy { get; set; }
    /// <summary>
    /// This value is set locally and indicates which
    /// <see cref="MyPlayer"/>
    /// is the current playing duelist.
    /// </summary>
    /// <remarks>
    /// Accesses: 
    /// <see cref="currentDuelist"/>.
    /// <br></br>
    /// Set this to player:
    /// <see cref="StartTurn"/>
    /// <br></br>
    /// Set this to enemy: 
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
    /// <summary>
    /// Mainly used for enabling and disabling user controls. 
    /// This is done mainly by managing card events.
    /// </summary>
    /// <remarks>
    /// Accesses:
    /// <see cref="state"/>.
    /// Set by: 
    /// <see cref="Local_SetTurnState(TurnState)"/>
    /// , or by: 
    /// <see cref="Local_SetTurnStateToPrevious"/>
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
    /// <see cref="Local_SetTurnStateToPrevious()"/>
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
    public bool ExecutingEffects { get => executingEffects; set => SetExecutingEffect(value); }
    #endregion
    #region Methods
    void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }

    void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(GameUIManager.Instance.State != GameState.GameOver)
        {
            GameUIManager.Instance.ErrorMenu.SetActive(true);
            transform.GetChild(0).gameObject.SetActive(false);
            Player.gameObject.SetActive(false);
            Enemy.gameObject.SetActive(false);
        }
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
    /// <summary>
    /// Assigns 
    /// <see cref="blockingMonster"/>
    /// and calculates damage.
    /// <br></br>
    /// Parameter:
    /// <paramref name="index"></paramref>
    /// refers to the 
    /// <see cref="MyPlayer.Field"/>
    /// the
    /// <see cref="blockingMonster"/>
    /// is located on.
    /// </summary>
    /// <remarks>
    /// Called by: 
    /// <see cref="BlockingMonsterIndex"/>.
    /// </remarks>
    [PunRPC]
    public void RPC_UpdateBlockingMonsterIndex(int index)
    {
        ((MonsterCard)AttackingMonster).Call_UpdateSwordIcon(false);

        if (index == 6)
        {
            AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.All);
            Player.Call_DrawCards(1);
            if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.Call_OnDirectAttack();
            Call_SetTurnState(NetworkTarget.Local, TurnState.AttackPhase);
            Call_SetTurnState(NetworkTarget.Other, TurnState.Busy);
            return;
        }
        blockingMonster = Enemy.Field[index];
        if (((MonsterCardStats)AttackingMonster.CardStats).Effect != null) ((MonsterCardStats)AttackingMonster.CardStats).Effect.Call_OnAttack();
        int value =((MonsterCardStats)AttackingMonster.CardStats).Attack - ((MonsterCardStats)blockingMonster.CardStats).Defense;
        if (value > 0)
        {
            AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.Local);
            AudioManager.Instance.Call_PlaySound(AudioType.Destroy, NetworkTarget.Other);
            ((MonsterCardStats)AttackingMonster.CardStats).Effect.Call_BattleWon();
            blockingMonster.Call_ParticleBomb((-value).ToString(), Color.red, NetworkTarget.All);
            AttackingMonster.Call_ParticleBomb(value.ToString(), Color.green, NetworkTarget.All);
            blockingMonster.Call_SendToGraveyard();
        }
        else if (value < 0)
        {
            AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.Other);
            AudioManager.Instance.Call_PlaySound(AudioType.Destroy, NetworkTarget.Local);
            ((MonsterCard)blockingMonster).Call_PlayBlockParticles();
            ((MonsterCardStats)blockingMonster.CardStats).Effect.Call_BlockSuccessfull();
            blockingMonster.Call_ParticleBomb((-value).ToString(), Color.red, NetworkTarget.All);
            AttackingMonster.Call_ParticleBomb(value.ToString(), Color.green, NetworkTarget.All);
            AttackingMonster.Call_SendToGraveyard();
        }
        else
        {
            AudioManager.Instance.Call_PlaySound(AudioType.Attack, NetworkTarget.All);
        }
        Call_SetTurnState(NetworkTarget.Local, TurnState.AttackPhase);
        Call_SetTurnState(NetworkTarget.Other, TurnState.Busy);
    }
    /// <summary>
    /// Starts the turn on local player and blocks controls on opposing player.
    /// </summary>
    /// <remarks>
    /// Sets on local player:
    /// <see cref="MyPlayer.Mana"/>.
    /// Calls on local player:
    /// <see cref="MyPlayer.Call_DrawCards"/>.
    /// </remarks>
    public void StartTurn()
    {
        GameUIManager.Instance.Geisha.Play("Players Turn");
        CurrentDuelist = DuelistType.Player;
        Player.Mana = turn + Player.ManaBoost;
        Call_SetTurnState(NetworkTarget.Local, TurnState.StartPhase);
        Call_SetTurnState(NetworkTarget.Other, TurnState.Busy);
        if(!(round == 0 && turn == 1)) Player.Call_DrawCards(1);
        for (int i = 0; i < Player.Field.Count; i++)
        {
            Player.Field[i].HasAttacked = false;
            Player.Field[i].HasBlocked = false;
        }
        if(TimerCoroutine != null)StopCoroutine(TimerCoroutine);
        TimerCoroutine = ManageTimer();
        TimerTime = 240;
        StartCoroutine(TimerCoroutine);
    }
    public IEnumerator ManageTimer()
    {
        Slider slider = GameUIManager.Instance.TimeSlider;
        slider.maxValue = TimerTime;
        slider.value = TimerTime;

        while(slider.value > 0)
        {
            yield return new WaitForFixedUpdate();
            if(state != TurnState.Busy || executingEffects) slider.value -= Time.fixedDeltaTime;
        }

        if (currentDuelist == DuelistType.Player)
        {
            ManageEffectsOnTimOver();
            GameUIManager.Instance.EndTurn();
        }
        else if (currentDuelist == DuelistType.Enemy)
        {
            if (state == TurnState.Blocking) BlockingMonsterIndex = 6;

            ManageEffectsOnTimOver();

            TimerTime = 60;
            StartCoroutine(ManageTimer());
        }
    } 
    private void ManageEffectsOnTimOver()
    {
        if (DestroyCounter > 0)
        {
            if (Player.Field.Count < DestroyCounter)
                DestroyCounter = Player.Field.Count;
            for (int i = 0; i < DestroyCounter; i++)
            {
                Player.Field[i].OnMouseDownEvent?.Invoke();
            }
            DestroyCounter = 0;
        }
        if (DiscardCounter > 0)
        {
            if (Player.Hand.Count < DiscardCounter)
                DiscardCounter = Player.Hand.Count;

            for (int i = 0; i < DiscardCounter; i++)
            {
                Player.Hand[i].OnMouseDownEvent?.Invoke();
            }
            DiscardCounter = 0;
        }
        if (Player.RecallCounter > 0)
        {
            if (Player.RecallArea == MonsterCardLocation.InHand)
            {
                if (Player.Hand.Count < Player.RecallCounter)
                    Player.RecallCounter = Player.Hand.Count;

                for (int i = 0; i < Player.RecallCounter; i++)
                {
                    Player.Hand[i].OnMouseDownEvent?.Invoke();
                }
                Player.RecallCounter = 0;
            }
            else
            {
                if (Player.Field.Count < Player.RecallCounter)
                    Player.RecallCounter = Player.Field.Count;

                for (int i = 0; i < Player.RecallCounter; i++)
                {
                    Player.Field[i].OnMouseDownEvent?.Invoke();
                }
                Player.RecallCounter = 0;
            }
        }
    }
    /// <summary>
    /// This method gets called by the local client, in order to call 
    /// <see cref="RPC_SetTurnState(TurnState)"/>
    /// on any client, or
    /// <see cref="Local_SetTurnState(TurnState)"/>
    /// on local client.
    /// </summary>
    /// <remarks>
    /// Parameter:
    /// <paramref name="networkTarget"></paramref>
    /// => Determines, on which clients to set the 
    /// <see cref="TurnState"/>.
    /// <br></br> 
    /// Paramter:
    /// <paramref name="value"></paramref>
    /// => state to set.
    /// </remarks>
    public void Call_SetTurnState(NetworkTarget networkTarget, TurnState value)
    {
        if (networkTarget == NetworkTarget.Local) Local_SetTurnState(value);
        else if (networkTarget == NetworkTarget.Other) photonView.RPC(nameof(RPC_SetTurnState), RpcTarget.Others, value);
        else if (networkTarget == NetworkTarget.All) photonView.RPC(nameof(RPC_SetTurnState), RpcTarget.All, value);
    }
    /// <summary>
    /// Used by local client for calling
    /// <see cref="Local_SetTurnState(TurnState)"/>
    /// on other or all clients.
    /// </summary>
    /// <remarks>
    /// Paramter:
    /// <paramref name="value"></paramref>
    /// => state to set.
    /// <br></br>
    /// Called by:
    /// <see cref="Call_SetTurnState"/>
    /// </remarks>
    [PunRPC]
    public void RPC_SetTurnState(TurnState value)
    {
        Local_SetTurnState(value);
    }
    /// <summary>
    /// Sets locally: 
    /// <see cref="state"/>.
    /// <br></br>
    /// Assigns card events according to 
    /// <paramref name="value"></paramref>.
    /// </summary>
    /// <remarks>
    /// Mainly called by:
    /// <see cref="RPC_SetTurnState(TurnState)"/>
    /// <br></br>
    /// or by:
    /// <see cref="Call_SetTurnState(NetworkTarget, TurnState)"/>.
    /// </remarks>
    public void Local_SetTurnState(TurnState value)
    {
        if (ExecutingEffects)
        {
            stateToSet = value;
            StartCoroutine(WaitUntilFinishedExecutingEffectBeforeLocal_SetTurnState());
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
                    if(!c.HasAttacked)
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
    /// <summary>
    /// If 
    /// <see cref="ExecutingEffects"/>:
    /// changing the 
    /// <see cref="TurnState"/>
    /// would break the current, executing
    /// <see cref="Effect"/>.
    /// <br></br>
    /// Therefore, this methods waits until the current
    /// <see cref="Effect"/>
    /// finishes executing, before setting the 
    /// <see cref="stateToSet"/>.
    /// </summary>
    /// <remarks>
    /// Called by:
    /// <see cref="Local_SetTurnState(TurnState)"/>.
    /// </remarks>
    private IEnumerator WaitUntilFinishedExecutingEffectBeforeLocal_SetTurnState()
    {
        while (ExecutingEffects) { yield return new WaitForFixedUpdate(); }
        Local_SetTurnState(stateToSet);
    }
    /// <summary>
    /// This method gets called by the local client, in order to call 
    /// <see cref="RPC_SetTurnStateToPrevious()"/>
    /// on any client, or
    /// <see cref="Local_SetTurnStateToPrevious()"/>
    /// on local client.
    /// </summary>
    /// <remarks>
    /// Parameter:
    /// <paramref name="networkTarget"></paramref>
    /// => Determines, on which clients to set the 
    /// <see cref="TurnState"/>.
    /// </remarks>
    public void Call_SetTurnStateToPrevious(NetworkTarget networkTarget)
    {
        if (networkTarget == NetworkTarget.Local) { Local_SetTurnStateToPrevious(); }
        else if (networkTarget == NetworkTarget.Other) photonView.RPC(nameof(RPC_SetTurnStateToPrevious), RpcTarget.Others);
        else if (networkTarget == NetworkTarget.All) photonView.RPC(nameof(RPC_SetTurnStateToPrevious), RpcTarget.All);
    }
    /// <summary>
    /// Used by local client for calling
    /// <see cref="Local_SetTurnStateToPrevious"/>
    /// on other or all clients.
    /// </summary>
    /// <remarks>
    /// Called by:
    /// <see cref="Call_SetTurnStateToPrevious(NetworkTarget)"/>
    /// </remarks>
    [PunRPC]
    public void RPC_SetTurnStateToPrevious()
    {
        Local_SetTurnStateToPrevious();
    }
    /// <summary>
    /// Calls Locally:
    /// <see cref="WaitUntilFinishedExecutingEffectBeforeLocal_SetToPrevState"/>.
    /// </summary>
    /// <remarks>
    /// Mainly called by:
    /// <see cref="RPC_SetTurnState(TurnState)"/>
    /// <br></br>
    /// or by:
    /// <see cref="Call_SetTurnState(NetworkTarget, TurnState)"/>.
    /// </remarks>
    public void Local_SetTurnStateToPrevious()
    {
        StartCoroutine(WaitUntilFinishedExecutingEffectBeforeLocal_SetToPrevState());
    }
    /// <summary>
    /// If 
    /// <see cref="ExecutingEffects"/>:
    /// changing the 
    /// <see cref="TurnState"/>
    /// would break the current, executing
    /// <see cref="Effect"/>.
    /// <br></br>
    /// Therefore, this methods waits until the current
    /// <see cref="Effect"/>
    /// finishes executing, before setting the 
    /// <see cref="stateToSet"/>.
    /// </summary>
    /// <remarks>
    /// Called by:
    /// <see cref="Local_SetTurnStateToPrevious()"/>.
    /// </remarks>
    private IEnumerator WaitUntilFinishedExecutingEffectBeforeLocal_SetToPrevState()
    {
        while (ExecutingEffects) { yield return new WaitForFixedUpdate(); }
        Local_SetTurnState(PrevState);
    }
    /// <summary>
    /// Used for setting 
    /// <see cref="executingEffects"/> 
    /// to parameter
    /// <paramref name="value"></paramref>
    /// on other or all clients.
    /// </summary>
    /// <remarks>
    /// Called by:
    /// <see cref="ExecutingEffects"/>
    /// </remarks>
    public void SetExecutingEffect(bool value)
    {
        executingEffects = value;
        if(value == false)
        {
            if (CurrentDuelist == DuelistType.Enemy)
            {
                if(TimerCoroutine != null) StopCoroutine(TimerCoroutine);
                TimerTime = 60;
                TimerCoroutine = ManageTimer();
                StartCoroutine(TimerCoroutine);
            }
        }
    }
    #endregion
}
