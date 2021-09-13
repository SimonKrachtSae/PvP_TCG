using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private Duelist player;
    [SerializeField] private Deck PlayerDeck;
    [SerializeField] private Duelist enemy;
    [SerializeField] private Deck EnemyDeck;

    public Duelist CurrentPlayer { get; set; }
    private Deck currentDeck;
    private List<Field> currentHandCards;

    public TurnState TurnState { get; set; }
    public Field cardToBeSummoned { get; set; }

    private Board board;
    private UIDesriptions descriptions;
    public int Turn = 1;
    private int rounds = 0;
    public int Rounds 
    {
        get => rounds;
        set 
        { 
            if(value > 2)
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
    void Start()
    {
        descriptions = UIDesriptions.Instance;
        board = Board.Instance;
        TurnState = TurnState.Normal;
        //AssignFirstPlayer
        CurrentPlayer = player;
        currentDeck = PlayerDeck;
        currentHandCards = board.PlayerHandCards;
        //Draw Starting Hand
        for (int i = 0; i < 4; i++)
        {
            DrawTopDeckCard();
        }

        board.PlayerDeckText.text = PlayerDeck.MonsterCards.Count.ToString();
        TurnStart(PlayerType.Player);
    }
    void Update()
    {
        
    }
    private void TurnStart(PlayerType type)
    {
        Rounds++;
        if (type == PlayerType.Player)
        {
            CurrentPlayer = player;
            currentDeck = PlayerDeck;
            currentHandCards = board.PlayerHandCards;
        }
        else
        {
            CurrentPlayer = enemy;
            currentDeck = EnemyDeck;
            currentHandCards = board.EnemyHandCards;
        }
        CurrentPlayer.SummonPower = Turn;
        DrawTopDeckCard();
    }
    private void DrawTopDeckCard()
    {
        Field field = null;
        for(int i = 0; i < 5; i++)
        {
            if (currentHandCards[i].monsterCard == null) field = currentHandCards[i];
        }
        if (field == null) return;
        int index = currentDeck.MonsterCards.Count - 1;
        field.MonsterCard = currentDeck.MonsterCards[index];
        currentDeck.MonsterCards.RemoveAt(index);
    }
}
