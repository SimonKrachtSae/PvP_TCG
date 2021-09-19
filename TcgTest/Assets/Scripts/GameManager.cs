using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance;
    private Duelist localDuelist;
    public Duelist LocalDuelist { get => localDuelist; set => localDuelist = value; }

    private Duelist enemyDuelist;
    public Duelist Enemy { get => enemyDuelist; set => enemyDuelist = value; }

    public ClientType StartingClient { get; set; }
    public DuelistType CurrentDuelist { get; set; }

    public Duelist CurrentPlayer { get; set; }

    public TurnState TurnState { get; set; }

    private Board board;
    private UIDesriptions descriptions;
    private int turn = 1;
    public int Turn
    {
        get => turn;
        set
        {
            turn = value;
            photonView.RPC(nameof(RPC_UpdateTurnInfo), RpcTarget.All, turn);
        }
    }
    private TurnPhase currentTurnPhase;
    private int rounds = 0;
    public int Rounds
    {
        get => rounds;
        set
        {
            rounds = value;
            if(rounds >= 2)
            {
                Turn++;
                rounds = 0;
            }
            photonView.RPC(nameof(RPC_UpdateRoundInfo), RpcTarget.All, rounds);
        }
    }

    public MainPhaseStates MainPhaseStates
    { 
        get => mainPhaseStates;
        set
        {
            mainPhaseStates = value;
            photonView.RPC(nameof(RPC_SetMainPhaseState), RpcTarget.All, mainPhaseStates);
        }
    }

    private MainPhaseStates mainPhaseStates;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    private void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        if(PhotonNetwork.OfflineMode)PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        currentTurnPhase = TurnPhase.DrawPhase;
    }
    public void StartGame()
    {
        descriptions = UIDesriptions.Instance;
        board = Board.Instance;
        TurnState = TurnState.Normal;
        //AssignPlayers
        //CurrentPlayer = localDuelist;
        //Draw Starting Hand

        int cardCount = 5;
        CurrentDuelist = DuelistType.Enemy;
        if (!PhotonNetwork.OfflineMode)
        {
            bool isFirst = false;
            if (PhotonNetwork.LocalPlayer.IsMasterClient && StartingClient == ClientType.Host)
            {
                isFirst = true;
                cardCount = 4;
                CurrentDuelist = DuelistType.Player;
                GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
            }
            if (!PhotonNetwork.LocalPlayer.IsMasterClient && StartingClient == ClientType.Client)
            {
                GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
                CurrentDuelist = DuelistType.Player;
                isFirst = true;
                cardCount = 4;
            }

            for (int i = 0; i < cardCount; i++)
            {
                LocalDuelist.DrawCard(0);
            }
           
            if (isFirst) StartTurn();
        }
    }
    private void StartTurn()
    {
        Rounds++;
        LocalDuelist.SummonPower = Turn;
        LocalDuelist.DrawCard(LocalDuelist.Deck.MonsterCards.Count - 1);
        MainPhaseStates = MainPhaseStates.StandardView;
    }
    public void EndTurn()
    {
        if(CurrentDuelist == DuelistType.Enemy)
        {
            StartTurn();
            CurrentDuelist = DuelistType.Player;
            GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        }
        else
        {
            GameUIManager.Instance.EndTurnButton.gameObject.SetActive(false);
            CurrentDuelist = DuelistType.Enemy;
        }
    }
    [PunRPC]
    public void RPC_SetMainPhaseState(MainPhaseStates state)
    {
        mainPhaseStates = state;
    }
   
    [PunRPC]
    public void RPC_UpdateRoundInfo(int value)
    {
        rounds = value;
    }
    [PunRPC]
    public void RPC_UpdateTurnInfo(int value)
    {
        turn = value;
        Board.Instance.TurnCount.text = turn.ToString();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
