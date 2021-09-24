using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private List<CardStats> startingDeck;
    public List<CardStats> StartingDeck { get => startingDeck; set => startingDeck = value; }
    public List<Card> Deck { get; set; }
    public List<Card> Hand { get; set; }
    public List<Card> Field { get; set; }
    public GameObject DeckField { get; set; }
    public RectTransform HandParent { get; set; }

    private Game_Manager gameManager;
    private DuelistUIs UIs;
    private int mana;
    public int Mana 
    { 
        get => mana; 
        set => photonView.RPC(nameof(RPC_UpdateMana),RpcTarget.All, value); 
    }

    void Awake()
    {
        Deck = new List<Card>();
        Hand = new List<Card>();
        Field = new List<Card>();
        gameManager = Game_Manager.Instance;

        if (photonView.IsMine)
        {
            gameManager.Player = this;
            UIs = Board.Instance.PlayerUIs;
            DeckField = Board.Instance.PlayerDeckFieldObj;
            HandParent = (RectTransform)Board.Instance.PlayerHandParent.transform;
        }
        else
        {
            gameManager.Enemy = this;
            UIs = Board.Instance.EnemyUIs;
            DeckField = Board.Instance.EnemyDeckFieldObj;
            HandParent = (RectTransform)Board.Instance.EnemyHandParent.transform;
        }
    }
    void Start()
    {
        if (photonView.IsMine)
        {
            for(int i = 0; i < StartingDeck.Count; i++)
            {
                if (StartingDeck[i].GetType().ToString() == nameof(MonsterCardStats))
                {
                    GameObject obj = PhotonNetwork.Instantiate("MonsterCard", Vector3.zero, Quaternion.identity);
                    obj.GetComponent<MonsterCard>().CardStats = i;
                }
                else if (StartingDeck[i].GetType().ToString() == nameof(EffectCardStats))
                {
                    GameObject obj = PhotonNetwork.Instantiate("EffectCard", Vector3.zero, Quaternion.identity);
                    obj.GetComponent<EffectCard>().CardStats = i;
                }
            }
        }
    }
    public void DrawCard(int index)
    {
        Deck[index].DrawThisCard();
    }
    public void RedrawHandCards()
    {
        float step = 5;
        float start = -(Hand.Count / 2);
        for(int i = 0; i < Hand.Count; i++)
        {
            Vector3 vector = Hand[i].transform.position;
            Hand[i].transform.position = new Vector3(HandParent.transform.position.x + start + i * step, vector.y, vector.z);
        }
    }
    public void Subscribe(Card card)
    {
        //if (Deck.Contains(card)) Destroy(card.gameObject);
        Deck.Add(card);
        card.Location = CardLocation.Deck;
        card.gameObject.transform.position = DeckField.transform.position + new Vector3(0,0,Deck.IndexOf(card)/100);
    }
    [PunRPC]
    public void RPC_UpdateMana(int value)
    {
        mana = value;
        UIs.SummonPower.text = value.ToString();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
