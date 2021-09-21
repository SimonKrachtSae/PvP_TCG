using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Duelist : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Deck deck;
    [SerializeField] private List<MonsterCardStats> graveyard;
    [SerializeField] private Deck allCards;
    private List<HandField> handCardFields;
    private List<MonsterField> monsterFields;
    private GameObject handCardsParent;
    private List<MonsterCardStats> handCards;
    private List<MonsterCardStats> monstersOnField;
    private MonsterCardStats cardToBeSummoned;
    public MonsterCardStats CardToBeSummoned 
    { 
        get => cardToBeSummoned; 
        set => cardToBeSummoned = value;
    }
    private MonsterField attackingCard;
    public MonsterField AttackingCard
    {
        get => attackingCard;
        set=> attackingCard = value;
    }
    private int summonPower = 0; 
    public int SummonPower
    { 
        get => summonPower;
        set => photonView.RPC(nameof(RPC_UpdateSummonPower), RpcTarget.All, value);
    }
    public Deck Deck
    {
        get => deck;
        set
        {
            deck = value;
            photonView.RPC(nameof(RPC_UpdateDeckCardCount), RpcTarget.All, value);
        }
    }
    private DuelistUIs UIs;
    public List<HandField> HandCardFields { get => handCardFields; set => handCardFields = value; }
    public List<MonsterField> MonsterFields { get => monsterFields; set => monsterFields = value; }
    private int blockingMonsterIndex;
    public int BlockingMonsterIndex
    {
        get => blockingMonsterIndex;
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.All, value); 
    }


    private List<MonsterField> activeMonsterFields;
    public List<MonsterField> ActiveMonsterFields { get => activeMonsterFields; set => activeMonsterFields = value; }
    public int SummonPowerBoost { get => summonPowerBoost; set => summonPowerBoost = value; }

    private int summonPowerBoost;


    private void Awake()
    {
        for (int i = 0; i < allCards.MonsterCards.Count; i++) deck.MonsterCards.Add(allCards.MonsterCards[i]);
    }

    void Start()
    {
        foreach (MonsterCardStats stats in deck.MonsterCards) stats.MonsterCardLocation = MonsterCardLocation.InDeck;
        graveyard = new List<MonsterCardStats>();
        handCards = new List<MonsterCardStats>();
        ActiveMonsterFields = new List<MonsterField>();
        if (photonView.IsMine)
        {
            Debug.Log("tic");
            GameManager.Instance.LocalDuelist = this;
            handCardsParent = Board.Instance.PlayerHandParent;
            handCardFields = new List<HandField>();
            UIs = Board.Instance.PlayerUIs;
            MonsterFields = Board.Instance.PlayerMonsterFields;
            return;
        }
        else
        {
            GameManager.Instance.Enemy = this;
            handCardFields = new List<HandField>();
            handCardsParent = Board.Instance.EnemyHandParent;
            UIs = Board.Instance.EnemyUIs;
            MonsterFields = Board.Instance.EnemyMonsterFields;
            return;
        }
    }
    public void DrawCard(int index)
    {
        photonView.RPC(nameof(RPC_DrawCard), RpcTarget.All, index);
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        handCards.Add(deck.MonsterCards[index]);
        Deck.MonsterCards.RemoveAt(index);
        RedrawHandCards();
    }
    [PunRPC]
    public void RPC_UpdateSummonPower(int value)
    {
        summonPower = value;
        UIs.SummonPower.text = value.ToString();
    }
    public void UpdateSummonPowerBoost(int value)
    {
        photonView.RPC(nameof(RPC_UpdateSummonPowerBoost), RpcTarget.All, value);
    }
    [PunRPC]
    public void RPC_UpdateSummonPowerBoost(int value)
    {
        summonPowerBoost = summonPowerBoost + value;
    }
    [PunRPC]
    public void RPC_UpdateDeckCardCount()
    {
        UIs.CardsInDeckCount.text = deck.MonsterCards.Count.ToString();
    }
    [PunRPC]
    public void RPC_UpdateHandCards(List<MonsterCardStats> value)
    {
        handCards = value;
        RedrawHandCards();
    }
    [PunRPC]
    public void RPC_UpdateMonsterFields(List<MonsterField> fields)
    {
        MonsterFields = fields;
    }
    public void RedrawHandCards()
    {
        Debug.Log("UBJBK");
        if (handCardFields == null) handCardFields = new List<HandField>();
        foreach (HandField f in handCardFields) if(f!= null) Destroy(f.gameObject);
        int cardCount = handCards.Count;
        float panelWidth = ((RectTransform)handCardsParent.transform).rect.width;
        float halfWidth = panelWidth / 2;
        float step = panelWidth / cardCount;
        for (int i = 0; i < cardCount;i++)
        {
            GameObject handField = Instantiate(Board.Instance.HandFieldPrefab, handCardsParent.transform);
            ((RectTransform)handField.transform).localPosition = new Vector2(halfWidth - step * (cardCount-i),0);
            HandField field = handField.GetComponent<HandField>();
            field.AssignCard(handCards[i]);
            handCardFields.Add(field);
        }
    }
    public void Summon(MonsterField field)
    {
        SummonPower -= CardToBeSummoned.PlayCost;
        CardToBeSummoned.Effect.OnSummon?.Invoke();
        photonView.RPC(nameof(RPC_Summon), RpcTarget.All, MonsterFields.IndexOf(field), handCards.IndexOf(CardToBeSummoned));
        CardToBeSummoned = null;
    }
    [PunRPC]
    public void RPC_Summon(int monsterFieldIndex, int handFieldIndex)
    {
        MonsterFields[monsterFieldIndex].AssignCard(handCards[handFieldIndex]);
        handCards.RemoveAt(handFieldIndex);
        RedrawHandCards();
    }
    public void DestroyMonster(MonsterField field)
    {
        field.Layout.MonsterCard.Effect.OnDestroy?.Invoke();
        photonView.RPC(nameof(RPC_DestroyMonster), RpcTarget.All, MonsterFields.IndexOf(field));
    }
    [PunRPC]
    public void RPC_DestroyMonster(int index)
    {
        graveyard.Add(MonsterFields[index].Layout.MonsterCard);
        MonsterFields[index].UnAssignCard();

    }
    public void ShowBlockRequest()
    {
        photonView.RPC(nameof(RPC_ShowBlockRequest), RpcTarget.Others);
    }
    [PunRPC]
    public void RPC_ShowBlockRequest()
    {
        Board.Instance.BlockRequest.SetActive(true);
    }
    [PunRPC]
    public void RPC_UpdateBlockingMonsterIndex(int value)
    {
        blockingMonsterIndex = value;
        if(photonView.IsMine)
        {
            if(value == 6)
            {
                DrawCard(0);
                AttackingCard = null;
                return;
            }
            MonsterCardStats attackingCardStats = AttackingCard.Layout.MonsterCard;
            MonsterCardStats defendingCardStats = GameManager.Instance.Enemy.MonsterFields[value].Layout.MonsterCard;
            if (attackingCardStats.Attack > defendingCardStats.Defense) { GameManager.Instance.Enemy.DestroyMonster(GameManager.Instance.Enemy.MonsterFields[value]); }
            else if (attackingCardStats.Attack < defendingCardStats.Defense) { DestroyMonster(AttackingCard); }
            AttackingCard = null;
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
