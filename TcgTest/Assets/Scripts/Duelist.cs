using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Duelist : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private Deck deck;
    private List<Field> handCardFields;
    private List<MonsterCardStats> handCards;
    private int summonPower;

    public int SummonPower 
    { 
        get => summonPower;
        set
        {
            summonPower = value;
            UIDesriptions.Instance.PlayerSummonPowerText.text = summonPower.ToString();
        }
    }
    public Deck Deck { get => deck; set => deck = value; }
    public List<MonsterCardStats> HandCards { get => handCards; set => handCards = value; }
    public List<Field> HandCardFields { get => handCardFields; set => handCardFields = value; }
    
    void Start()
    {
        handCards = new List<MonsterCardStats>();
        if(photonView.IsMine)
        {
            Debug.Log("tic");
            GameManager.Instance.LocalDuelist = this;
            handCardFields = new List<Field>();
            handCardFields = Board.Instance.PlayerHandCards;
        }
        else
        {
            //handCards = new List<MonsterCardStats>();
            Debug.Log("tac");
            //for (int i = 0; i < 10; i++) Deck.MonsterCards.Add(GameManager.Instance.cheese);
            GameManager.Instance.Enemy = this;
            handCards = new List<MonsterCardStats>();
            handCardFields = new List<Field>();
            handCardFields = Board.Instance.EnemyHandCards;
        }

    }
    public void AddHandCard(int index)
    {
        //MonsterCardStats stats = Deck.MonsterCards[index];
        handCards.Add(deck.MonsterCards[index]);
        deck.MonsterCards.RemoveAt(index);
    }
    public void RedrawHandCards()
    {
        foreach (Field f in handCardFields) Destroy(f.Card);
        for (int i = 0; i < handCards.Count;i++)
        {
            handCardFields[i].AssignCard(handCards[i]);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
