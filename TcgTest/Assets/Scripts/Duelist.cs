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
        set
        {
            GameManager.Instance.MainPhaseStates = MainPhaseStates.Summoning;
            cardToBeSummoned = value;
        }
    }
    private int summonPower; 
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
    private void Awake()
    {
        for (int i = 0; i < allCards.MonsterCards.Count; i++) deck.MonsterCards.Add(allCards.MonsterCards[i]);
    }

    void Start()
    {
        foreach (MonsterCardStats stats in deck.MonsterCards) stats.MonsterCardLocation = MonsterCardLocation.InDeck;
        graveyard = new List<MonsterCardStats>();
        handCards = new List<MonsterCardStats>();
        if (photonView.IsMine)
        {
            Debug.Log("tic");
            GameManager.Instance.LocalDuelist = this;
            handCardsParent = Board.Instance.PlayerHandParent;
            handCardFields = new List<HandField>();
            UIs = Board.Instance.PlayerUIs;
            monsterFields = Board.Instance.PlayerMonsterFields;
        }
        else
        {
            GameManager.Instance.Enemy = this;
            handCardFields = new List<HandField>();
            handCardsParent = Board.Instance.EnemyHandParent;
            UIs = Board.Instance.EnemyUIs;
            monsterFields = Board.Instance.EnemyMonsterFields;
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
    [PunRPC]
    public void RPC_UpdateDeckCardCount()
    {
        UIs.CardsInDeckCount.text = deck.MonsterCards.ToString();
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
        monsterFields = fields;
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
        photonView.RPC(nameof(RPC_Summon), RpcTarget.All, monsterFields.IndexOf(field), handCards.IndexOf(CardToBeSummoned));
        CardToBeSummoned = null;
    }
    [PunRPC]
    public void RPC_Summon(int monsterFieldIndex, int handFieldIndex)
    {
        monsterFields[monsterFieldIndex].AssignCard(handCards[handFieldIndex]);
        handCards.RemoveAt(handFieldIndex);
        RedrawHandCards();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
