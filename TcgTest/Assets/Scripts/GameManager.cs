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

    public Duelist CurrentPlayer { get; set; }

    public TurnState TurnState { get; set; }
    public CardStats CardToBeSummoned { get; set; }

    private Board board;
    private UIDesriptions descriptions;
    public int Turn = 1;
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
    }
    public void StartGame()
    {
        descriptions = UIDesriptions.Instance;
        board = Board.Instance;
        TurnState = TurnState.Normal;
        //AssignPlayers
        //CurrentPlayer = localDuelist;
        //Draw Starting Hand
        for (int i = 0; i < 4; i++)
        {
            int index = LocalDuelist.Deck.MonsterCards.Count - 1;
            DrawCard(index);
        }
        LocalDuelist.RedrawHandCards();
        board.PlayerDeckText.text = LocalDuelist.Deck.MonsterCards.Count.ToString();
        //board.EnemyDeckText.text = enemyDuelist.Deck.MonsterCards.Count.ToString();
        //TurnStart(DuelistType.Player);
    }
    void Update()
    {
        
    }
    private void TurnStart(DuelistType type)
    {
        Rounds++;
        if (type == DuelistType.Player)
        {
            CurrentPlayer = LocalDuelist;
        }
        else
        {
            CurrentPlayer = enemyDuelist;
        }
        CurrentPlayer.SummonPower = Turn;
        //DrawTopDeckCard();
    }
    public void DrawCard(int index)
    {
        LocalDuelist.AddHandCard(index);
        photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others, index);
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        Debug.Log("Toe");
        Enemy.AddHandCard(index);
        Enemy.RedrawHandCards();
        Debug.Log("EnemyHandCards:" + Enemy.HandCards.Count);
        Debug.Log("EnemyDeckCards:" + Enemy.Deck.MonsterCards.Count);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
