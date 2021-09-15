using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
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
    public CardStats CardToBeSummoned { get; set; }

    private Board board;
    private UIDesriptions descriptions;
    public int Turn = 1;
    private TurnPhase currentTurnPhase;
    private int rounds = 0;
    public int Rounds
    {
        get => rounds;
        set
        {
            if (value > 2)
            {
                rounds = 0;
                Turn++;
            }
            else { rounds = value; }
        }
    }
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
                int index = LocalDuelist.Deck.MonsterCards.Count - 1;
                DrawCard(index);
            }
           
            if (isFirst) StartTurn();
            else UpdatePlayerUIs();
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < 4) LocalDuelist.AddHandCard(i);
                Enemy.AddHandCard(i);
            }
        }
    }

    public void DrawCard(int index)
    {
        LocalDuelist.AddHandCard(index);
        if(!PhotonNetwork.OfflineMode) photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others, index);
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        Debug.Log("Toe");
        Enemy.AddHandCard(index);
        Debug.Log("EnemyHandCards:" + Enemy.HandCards.Count);
        Debug.Log("EnemyDeckCards:" + Enemy.Deck.MonsterCards.Count);
    }
    private void StartTurn()
    {
        rounds++;
        DrawCard(localDuelist.Deck.MonsterCards.Count - 1);
        //UpdateUIs
        UpdatePlayerUIs();
        photonView.RPC(nameof(RPC_UpdateGameInfo), RpcTarget.All);
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
    public void UpdatePlayerUIs()
    {
        LocalDuelist.SummonPower = Turn;
        LocalDuelist.UpgradePlayerUIs();
        photonView.RPC(nameof(RPC_UpdatePlayerUIs), RpcTarget.Others);
    }
    [PunRPC]
    public void RPC_UpdatePlayerUIs()
    {
        Enemy.UpgradePlayerUIs();
    }
        //photonView.RPC(nameof(RPC_UpdateGameInfo), RpcTarget.All);
    [PunRPC]
    public void RPC_UpdateGameInfo()
    {
        Board.Instance.TurnCount.text = Turn.ToString();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
