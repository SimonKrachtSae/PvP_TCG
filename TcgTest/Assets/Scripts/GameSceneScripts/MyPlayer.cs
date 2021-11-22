using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MyPlayer : MonoBehaviourPunCallbacks
{
    private List<Card> deckList = new List<Card>();
    public List<Card> DeckList
    {
        get => deckList;
        set
        {
            deckList = value;
            UpdateDeckText();
            if (deckList.Count == 1 && gameManager.Turn > 1)
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
    public int RecallCounter { get => recallCounter; set => recallCounter = value; }
    public MonsterCardLocation RecallArea { get => recallArea; set => recallArea = value; }

    private int recallCounter = 0;
    private MonsterCardLocation recallArea;
    [SerializeField] private List<CardName> CardNameList;
    private Deck deck;
    private int drawAmount;
    private List<GameObject> gems = new List<GameObject>();
    void Awake()
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
        DeckList = new List<Card>();
        Hand = new List<Card>();
        Field = new List<MonsterCard>();
        Graveyard = new List<Card>();
	}

	private void Start()
	{
        if (photonView.IsMine) SpawnDeck();
        //UIs.CardsInDeckCount.text = deckList.Count.ToString();
    }
    public void SpawnDeck()
    {
        DeckData deckData = (DeckData)Resources.Load("DeckData");
        foreach (string cardName in deckData.CardNames)
        {
            Card card = PhotonNetwork.Instantiate(cardName, DeckField.transform.position, new Quaternion(0,0.5f,0,0)).GetComponent<Card>();
        }
    }
    public void UpdateDeckText()
    {
        UIs.CardsInDeckCount.text = deckList.Count.ToString();
    }
    private void DrawCard(int index)
    {
        if (deckList.Count == 1 && gameManager.Turn > 1)
        {
            photonView.RPC(nameof(RPC_GameOver), RpcTarget.All);
            return;
        }
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others);
        else DeckList[index].Local_DrawCard();
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        DeckList[index].Local_DrawCard();
    }
    public void Call_DrawCards(int amount)
    {
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_DrawCards), RpcTarget.Others, amount);
        else
        {
            drawAmount = amount;
            StartCoroutine(DrawCardsWithTimeOffset());
        }
    }

    private IEnumerator DrawCardsWithTimeOffset()
    {
        gameManager.ExecutingEffects = true;
        for(int i = 0; i < drawAmount; i++)
        {
            DrawCard(0);
            yield return new WaitForSecondsRealtime(1);
        }
        gameManager.ExecutingEffects = false;
    }
    [PunRPC]
    public void RPC_DrawCards(int amount)
    {
        drawAmount = amount;
        StartCoroutine(DrawCardsWithTimeOffset());
    }
    public void RedrawHandCards()
    {
        float singleWidth = GraveyardObj.transform.lossyScale.x;
        float step = 35;
        float xWidth = Hand.Count * step;
        Vector3 startPos = GraveyardObj.transform.position - new Vector3(singleWidth + xWidth, 0, 0);
        for (int i = 0; i < Hand.Count; i++)
        {
            Vector3 vector = Hand[i].transform.position;
            if (vector.y >= HandParent.transform.position.y -0.5f && vector.y <= HandParent.transform.position.y + 0.5f)
                Hand[i].transform.position = startPos + new Vector3(i * step,0,0);
        }
    }
    public void Subscribe(Card card)
    {
        DeckList.Add(card);
        card.Location = CardLocation.Deck;
       // card.gameObject.transform.localPosition = new Vector3(DeckField.transform.position.x, DeckField.transform.position.y, 0);
    }
    [PunRPC]
    public void RPC_UpdateMana(int value)
    {
        mana = value;
        if (value > 20) 
        {
            mana = 20;
            value = 20;
        }
        int dif = value - gems.Count;
        if (dif == 0) return;
        else if(dif < 0)
        {
            for(int i = 0; i < Mathf.Abs(dif); i++)
            {
                DestroyGem();
            }
        }
        else
        {
            for (int i = 0; i < dif; i++)
            {
                SpawnGem();
            }
        }
    }
    public void SpawnGem()
    {
        GameObject gem = GameUIManager.Instance.Gem;
        GameObject newGem = Instantiate(gem, UIs.ManaPos);
        newGem.transform.position += new Vector3(gems.Count * 13, 0, 0);
        gems.Add(newGem);
    }
    public void DestroyGem()
    {
        GameObject toDestroy = gems[gems.Count - 1];
        gems.Remove(toDestroy);
        Destroy(toDestroy);
    }
    public void ShowBlockRequest()
    {
        Game_Manager.Instance.Call_SetTurnState(NetworkTarget.Local, TurnState.Busy);
        Game_Manager.Instance.Call_SetTurnState(NetworkTarget.Other, TurnState.Blocking);
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
        RecallCounter = amount;
        gameManager.Call_SetTurnState(NetworkTarget.All, TurnState.Busy);
        if(targetLocation == MonsterCardLocation.OnField)
        {
            if (Field.Count < 1)
            {
                RecallCounter = 0;
                Board.Instance.PlayerInfoText.text = "No Card To Send To Deck";
                return;
            }
            gameManager.ExecutingEffects = true;
            foreach (MonsterCard c in Field) 
            { 
                c.ClearEvents(); c.Call_AddEvent(CardEvent.Recall, MouseEvent.Down, NetworkTarget.Local);
                RecallArea = MonsterCardLocation.OnField;
            }
        }
        else if (targetLocation == MonsterCardLocation.InHand)
        {
            if (Hand.Count < 1)
            {
                RecallCounter = 0;
                Board.Instance.PlayerInfoText.text = "No Card To Send To Deck";
                return;
            }
            gameManager.ExecutingEffects = true;
            foreach (Card c in Hand) 
            {
                c.ClearEvents(); c.Call_AddEvent(CardEvent.Recall, MouseEvent.Down, NetworkTarget.Local);
                RecallArea = MonsterCardLocation.InHand;
            }
        }
    }
    public void OnRecall()
    {
        RecallCounter--;
        Board.Instance.PlayerInfoText.text = "Recall: " + RecallCounter;
        if(RecallCounter == 0) Board.Instance.PlayerInfoText.text = "";
        if (RecallArea == MonsterCardLocation.OnField)
        {
            if (RecallCounter == 0 || Field.Count == 0)
            {
                RecallCounter = 0;
                gameManager.ExecutingEffects = false;
                gameManager.Call_SetTurnStateToPrevious(NetworkTarget.All);
            }
        }
        else if(RecallArea == MonsterCardLocation.InHand)
        {
            if (RecallCounter == 0 || Hand.Count == 0)
            {
                RecallCounter = 0;
                gameManager.ExecutingEffects = false;
                gameManager.Call_SetTurnStateToPrevious(NetworkTarget.All);
            }
        }
    }
 
    public void Call_AddDiscardEffects(int amount, NetworkTarget selector)
    {
        if (selector == NetworkTarget.Local) { gameManager.DiscardCounter = amount; StartCoroutine(AddDiscardEffects()); }
        else if (selector == NetworkTarget.Other) { photonView.RPC(nameof(RPC_AddDiscardEffects), RpcTarget.Others, amount); }
    }
    [PunRPC]
    public void RPC_AddDiscardEffects(int amount)
    {
        gameManager.DiscardCounter = amount;
        StartCoroutine(AddDiscardEffects());
    }
    public IEnumerator AddDiscardEffects()
    {
        gameManager.ExecutingEffects = true;
        foreach (Card c in Hand) c.Call_AddEvent(CardEvent.Discard, MouseEvent.Down, NetworkTarget.Local);
        while(gameManager.DiscardCounter != 0)
        {
            yield return new WaitForFixedUpdate();
            Board.Instance.PlayerInfoText.text = "Cards to Discard: " + gameManager.DiscardCounter.ToString();
            if (Hand.Count == 0 || gameManager.DiscardCounter == 0)
            {
                Board.Instance.PlayerInfoText.text = "";
                gameManager.DiscardCounter = 0;
                break; 
            }
        }
        gameManager.ExecutingEffects = false;
    }
    public void Call_AddDestroyEffects(int amount, NetworkTarget selector)
    {
        if(selector == NetworkTarget.Local) {gameManager.DestroyCounter = amount; StartCoroutine(AddDestroyEvents()); }
        else if(selector == NetworkTarget.Other) { photonView.RPC(nameof(RPC_AddDestroyEvents), RpcTarget.Others, amount); }
    }
    [PunRPC]
    public void RPC_AddDestroyEvents(int amount)
    {
        gameManager.DestroyCounter = amount;
        StartCoroutine(AddDestroyEvents());
    }
    public IEnumerator AddDestroyEvents()
    {
        gameManager.Call_SetTurnState(NetworkTarget.All, TurnState.Busy);
        gameManager.ExecutingEffects = true;
        foreach (MonsterCard c in Field) c.Call_AddEvent(CardEvent.Destroy, MouseEvent.Down, NetworkTarget.Local);
        while (gameManager.DestroyCounter != 0)
        {
            if (Field.Count == 0 || gameManager.DestroyCounter == 0)
            { 
                Board.Instance.PlayerInfoText.text = "";
                gameManager.DestroyCounter = 0;
                break;
            }
            Board.Instance.PlayerInfoText.text = "Cards to Destroy: " + gameManager.DestroyCounter.ToString();
            yield return new WaitForFixedUpdate();
        }
        gameManager.ExecutingEffects = false;
        gameManager.Call_SetTurnStateToPrevious(NetworkTarget.All);
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
                    foreach (Card c in DeckList) c.ClearEvents();
                    break;
                case MonsterCardLocation.InGraveyard:
                    foreach (Card c in Graveyard) c.ClearEvents();
                    break;
            }
        }
    }
    private IEnumerator WaitBeforeExecutingNextEffect()
    {
        while (gameManager.ExecutingEffects) { yield return new WaitForFixedUpdate(); }
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
