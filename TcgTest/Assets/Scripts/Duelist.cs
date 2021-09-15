using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Duelist : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Deck deck;
    [SerializeField] private List<MonsterCardStats> graveyard;
    private List<Field> handCardFields;
    private GameObject handCardsParent;
    private List<MonsterCardStats> handCards;
    private int summonPower; 
    public int SummonPower { get => summonPower; set => summonPower = value; }
    public Deck Deck { get => deck; set => deck = value; }
    public List<MonsterCardStats> HandCards { get => handCards; set => handCards = value; }
    public List<Field> HandCardFields { get => handCardFields; set => handCardFields = value; }

    private DuelistUIs UIs;
    
    void Start()
    {
        handCards = new List<MonsterCardStats>();
        graveyard = new List<MonsterCardStats>();
        if(!photonView.IsMine || (PhotonNetwork.OfflineMode && GameManager.Instance.LocalDuelist != null))
        {
            GameManager.Instance.Enemy = this;
            handCards = new List<MonsterCardStats>();
            handCardFields = new List<Field>();
            handCardsParent = Board.Instance.EnemyHandParent;
            UIs = Board.Instance.EnemyUIs;
        }
        else if (photonView.IsMine)
        {
            Debug.Log("tic");
            GameManager.Instance.LocalDuelist = this;
            handCardsParent = Board.Instance.PlayerHandParent;
            handCardFields = new List<Field>();
            UIs = Board.Instance.PlayerUIs;
        }

    }
    public void AddHandCard(int index)
    {
        //MonsterCardStats stats = Deck.MonsterCards[index];
        handCards.Add(deck.MonsterCards[index]);
        deck.MonsterCards.RemoveAt(index);
        RedrawHandCards();
    }
    private void RedrawHandCards()
    {
        if (handCardFields == null) handCardFields = new List<Field>();
        foreach (Field f in handCardFields) if(f!= null) Destroy(f.gameObject);
        int cardCount = handCards.Count;
        float panelWidth = ((RectTransform)handCardsParent.transform).rect.width;
        float halfWidth = panelWidth / 2;
        float step = panelWidth / cardCount;
        for (int i = 0; i < cardCount;i++)
        {
            GameObject handField = Instantiate(Board.Instance.HandFieldPrefab, handCardsParent.transform);
            ((RectTransform)handField.transform).localPosition = new Vector2(halfWidth - step * (cardCount-i),0);
            Field field = handField.GetComponent<Field>();
            field.AssignCard(handCards[i]);
            handCardFields.Add(field);
        }
    }
    public void UpgradePlayerUIs()
    {
        UIs.CardsInDeckCount.text = Deck.MonsterCards.Count.ToString();
        UIs.CardsInGraveyardCount.text = graveyard.Count.ToString();
        UIs.SummonPower.text = SummonPower.ToString();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
