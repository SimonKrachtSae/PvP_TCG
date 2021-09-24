using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private List<CardStats> startingDeck;
    public List<Card> Deck { get; set; }
    public GameObject DeckField { get; set; }
    private Game_Manager gameManager;
    private DuelistUIs UIs;
    void Awake()
    {
        Deck = new List<Card>();
    }
    void Start()
    {
        gameManager = Game_Manager.Instance;

        if (photonView.IsMine)
        {
            gameManager.Player = this;
            UIs = Board.Instance.PlayerUIs;
            for(int i = 0; i < startingDeck.Count; i++)
            {
                if (startingDeck[i].GetType().ToString() == nameof(MonsterCardStats))
                    PhotonNetwork.Instantiate("MonsterCard", Vector3.zero, Quaternion.identity);
                else if (startingDeck[i].GetType().ToString() == nameof(EffectCardStats))
                    PhotonNetwork.Instantiate("EffectCard", Vector3.zero, Quaternion.identity);
            }
        }
        else
        {
            gameManager.Enemy = this;
            UIs = Board.Instance.EnemyUIs;
        }
    }
    public void Subscribe(Card card)
    {
        if (Deck.Contains(card)) Destroy(card.gameObject);
        Deck.Add(card);
        card.Location = CardLocation.Deck;
        card.gameObject.transform.position = DeckField.transform.position + new Vector3(0,0,Deck.IndexOf(card)/100);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
