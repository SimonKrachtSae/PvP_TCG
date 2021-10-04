using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MyPlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private List<GameObject> startingDeck;
    public List<GameObject> StartingDeck { get => startingDeck; set => startingDeck = value; }
    private List<Card> deck;
    public List<Card> Deck
    {
        get => deck;
        set
        {
            deck = value;
            if (deck.Count == 1 && gameManager.Turn > 1)
            {
                photonView.RPC(nameof(RPC_GameOver), RpcTarget.All);
            }
        }
    }
    public List<Card> Hand { get; set; }
    public List<MonsterCard> Field { get; set; }
    public List<Card> Graveyard { get; set; }
    public GameObject DeckField { get; set; }
    public RectTransform HandParent { get; set; }
    public GameObject GraveyardObj { get; set; }

    private Game_Manager gameManager;
    private DuelistUIs UIs;
    private int mana;
    public int Mana
    {
        get => mana;
        set => photonView.RPC(nameof(RPC_UpdateMana), RpcTarget.All, value);
    }

    private int summonPowerBoost = 0;
    public int ManaBoost { get => summonPowerBoost; set => photonView.RPC(nameof(RPC_UpdateSumonPowerBoost), RpcTarget.All, value); }
    private int attackBoost = 0;
    public int AttackBoost { get => attackBoost; set => photonView.RPC(nameof(RPC_UpdateAttackBoost), RpcTarget.All, value); }
    private int defenseBoost = 0;
    public int DefenseBoost { get => defenseBoost; set => photonView.RPC(nameof(RPC_UpdateDefenseBoost), RpcTarget.All, value); }

    private int discardCounter = 0;
    public int DiscardCounter { get => discardCounter; set => discardCounter = value; }

    private int destroyCounter = 0;
    public int DestroyCounter { get => destroyCounter; set => destroyCounter = value; }

    private int recallCounter = 0;
    private MonsterCardLocation recallArea;
    [SerializeField] private List<CardName> CardNameList;
    public override void OnEnable()
    {
        gameManager = Game_Manager.Instance;

        if (photonView.IsMine)
        {
            gameManager.Player = this;
            UIs = Board.Instance.PlayerUIs;
            DeckField = Board.Instance.PlayerDeckFieldObj;
            HandParent = (RectTransform)Board.Instance.PlayerHandParent.transform;
            GraveyardObj = Board.Instance.PlayerGraveyard.gameObject;
        }
        else
        {
            gameManager.Enemy = this;
            UIs = Board.Instance.EnemyUIs;
            DeckField = Board.Instance.EnemyDeckFieldObj;
            HandParent = (RectTransform)Board.Instance.EnemyHandParent.transform;
            GraveyardObj = Board.Instance.EnemyGraveyard.gameObject;
        }
        Deck = new List<Card>();
        Hand = new List<Card>();
        Field = new List<MonsterCard>();
        Graveyard = new List<Card>();
        //SpawnDeck();
    }
    public void SpawnDeck()
    {
      foreach (CardName cardName in CardNameList)
      {
        GameObject card = PhotonNetwork.Instantiate(cardName.ToString(), DeckField.transform.position,Quaternion.identity);
        
        card.transform.parent = this.transform;
        
        card.GetComponent<Card>().enabled = true;
      }
    }
    public void DrawCard(int index)
    {
        if (deck.Count == 1 && gameManager.Turn > 1)
        {
            photonView.RPC(nameof(RPC_GameOver), RpcTarget.All);
            return;
        }
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others);
        else Deck[index].Local_DrawCard();
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        Deck[index].Local_DrawCard();
    }
    public void RedrawHandCards()
    {
        float step = 10;
        float start = -((Hand.Count / 2) * step);
        for (int i = 0; i < Hand.Count; i++)
        {
            Vector3 vector = Hand[i].transform.position;
            Hand[i].transform.position = new Vector3(HandParent.transform.position.x + start + i * step, vector.y, vector.z);
        }
    }
    public void Subscribe(Card card)
    {
        Deck.Add(card);
        card.Location = CardLocation.Deck;
        card.gameObject.transform.position = DeckField.transform.position;
    }
    [PunRPC]
    public void RPC_UpdateMana(int value)
    {
        mana = value;
        UIs.SummonPower.text = value.ToString();
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
    public void RPC_UpdateSumonPowerBoost(int value)
    {
        summonPowerBoost = value;
    }
    [PunRPC]
    public void RPC_UpdateAttackBoost(int value)
    {
        attackBoost = value;
        for(int i = 0; i < Field.Count; i++)
        {
            int newAttack = ((MonsterCardStats)Field[i].CardStats).DefaultAttack + attackBoost;
            ((MonsterCardStats)Field[i].CardStats).Attack = newAttack;
            ((MonsterCard_Layout)Field[i].Layout).AttackTextUI.text = newAttack.ToString();
        }
    }
    [PunRPC]
    public void RPC_UpdateDefenseBoost(int value)
    {
        defenseBoost = value;
        for (int i = 0; i < Field.Count; i++)
        {
            int newDefense = ((MonsterCardStats)Field[i].CardStats).DefaultDefense + defenseBoost;
            ((MonsterCardStats)Field[i].CardStats).Defense = newDefense;
            ((MonsterCard_Layout)Field[i].Layout).DefenseTextUI.text = newDefense.ToString();
        }
    }
    [PunRPC]
    public void RPC_GameOver()
    {
        GameUIManager.Instance.SetGameState(GameState.GameOver);
    }
    public void Call_AddRecallEvents(MonsterCardLocation targetLocation, NetworkTarget selector, int amount)
    {
        Board.Instance.PlayerInfoText.text = "Recall: " + amount;
        if (selector == NetworkTarget.Local) AddRecallEvents(targetLocation, amount);
        else if (selector == NetworkTarget.Other) photonView.RPC(nameof(RPC_AddRecallEvents), RpcTarget.Others, targetLocation, amount);
    }
    [PunRPC]
    public void RPC_AddRecallEvents(MonsterCardLocation targetLocation, int amount)
    {
        AddRecallEvents(targetLocation, amount);
    }
    public void AddRecallEvents(MonsterCardLocation targetLocation, int amount)
    {
        if (Field.Count < 1)
        {
            Board.Instance.PlayerInfoText.text = "No Card To Send To Graveyard";
            return;
        }
        recallCounter = amount;
        gameManager.Call_SetMainPhaseState(NetworkTarget.All, GameManagerStates.Busy);
        if(targetLocation == MonsterCardLocation.OnField)
            foreach (MonsterCard c in Field) 
            { 
                c.ClearEvents(); c.Call_AddEvent(CardEvent.Recall, MouseEvent.Down, NetworkTarget.Local);
                recallArea = MonsterCardLocation.OnField;
            }
        else if (targetLocation == MonsterCardLocation.InHand)
            foreach (Card c in Hand) 
            {
                c.ClearEvents(); c.Call_AddEvent(CardEvent.Recall, MouseEvent.Down, NetworkTarget.Local);
                recallArea = MonsterCardLocation.InHand;
            }
    }
    public void OnRecall()
    {
        recallCounter--;
        Board.Instance.PlayerInfoText.text = "Recall: " + recallCounter;
        if(recallCounter == 0) Board.Instance.PlayerInfoText.text = "";
        if (recallArea == MonsterCardLocation.OnField)
        {
            if (recallCounter == 0 || Field.Count == 0) gameManager.Call_SetMainPhaseStateToPrevious(NetworkTarget.All);
        }
        else if(recallArea == MonsterCardLocation.InHand)
        {
            if(recallCounter == 0 || Hand.Count == 0) gameManager.Call_SetMainPhaseStateToPrevious(NetworkTarget.All);
        }
    }
 
    public void Call_AddDiscardEffects(int amount, NetworkTarget selector)
    {
        if (selector == NetworkTarget.Local) { DestroyCounter = amount; StartCoroutine(AddDiscardEffects()); }
        else if (selector == NetworkTarget.Other) { photonView.RPC(nameof(RPC_AddDiscardEffects), RpcTarget.Others, amount); }
    }
    [PunRPC]
    public void RPC_AddDiscardEffects(int amount)
    {
        DiscardCounter = amount;
        StartCoroutine(AddDiscardEffects());
    }
    public IEnumerator AddDiscardEffects()
    {
        gameManager.Call_SetMainPhaseState(NetworkTarget.All, GameManagerStates.Busy);
        foreach (Card c in Hand) c.Call_AddEvent(CardEvent.Discard, MouseEvent.Down, NetworkTarget.Local);
        while(DiscardCounter != 0)
        {
            yield return new WaitForFixedUpdate();
            Board.Instance.PlayerInfoText.text = "Cards to Discard: " + DiscardCounter.ToString();
            if (Hand.Count == 0 || DiscardCounter == 0) break;
        }
        gameManager.Call_SetMainPhaseStateToPrevious(NetworkTarget.All);
    }
    public void Call_AddDestroyEffects(int amount, NetworkTarget selector)
    {
        if(selector == NetworkTarget.Local) {DestroyCounter = amount; StartCoroutine(AddDestroyEvents()); }
        else if(selector == NetworkTarget.Other) { photonView.RPC(nameof(RPC_AddDestroyEvents), RpcTarget.Others, amount); }
    }
    [PunRPC]
    public void RPC_AddDestroyEvents(int amount)
    {
        DestroyCounter = amount;
        StartCoroutine(AddDestroyEvents());
    }
    public IEnumerator AddDestroyEvents()
    {
        gameManager.Call_SetMainPhaseState(NetworkTarget.All, GameManagerStates.Busy);
        foreach (MonsterCard c in Field) c.Call_AddEvent(CardEvent.Destroy, MouseEvent.Down, NetworkTarget.Local);
        while (DestroyCounter != 0)
        {
            if (Field.Count == 0 || DestroyCounter == 0) { Board.Instance.PlayerInfoText.text = ""; break; }
            Board.Instance.PlayerInfoText.text = "Cards to Destroy: " + DestroyCounter.ToString();
            yield return new WaitForFixedUpdate();
        }
        gameManager.Call_SetMainPhaseStateToPrevious(NetworkTarget.All);
    }
    public void Call_ClearCardEvents(NetworkTarget networkTarget, List<MonsterCardLocation> locations)
    {
        List<MyPlayer> targetPlayers = new List<MyPlayer>();
        if (networkTarget == NetworkTarget.Local)
        {
            ClearCardEvents(locations);
        }
        else if(networkTarget == NetworkTarget.Other)
        {
            photonView.RPC(nameof(RPC_ClearCardEvents), RpcTarget.Others, locations);
        }
        else if (networkTarget == NetworkTarget.All)
        {
            photonView.RPC(nameof(RPC_ClearCardEvents), RpcTarget.All, locations);
        }
    }
    public void ClearCardEvents(List<MonsterCardLocation> locations)
    {
        foreach (MonsterCardLocation location in locations)
        {
            switch (location)
            {
                case MonsterCardLocation.InHand:
                    foreach (Card c in Hand) c.ClearEvents();
                    break;
                case MonsterCardLocation.OnField:
                    foreach (MonsterCard c in Field) c.ClearEvents();
                    break;
                case MonsterCardLocation.InDeck:
                    foreach (Card c in Deck) c.ClearEvents();
                    break;
                case MonsterCardLocation.InGraveyard:
                    foreach (Card c in Graveyard) c.ClearEvents();
                    break;
            }
        }
    }
    [PunRPC]
    public void RPC_ClearCardEvents(List<MonsterCardLocation> locations)
    {
        ClearCardEvents(locations);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
