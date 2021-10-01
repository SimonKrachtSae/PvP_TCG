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
    }
    public void DrawCard(int index)
    {
        if (deck.Count == 1 && gameManager.Turn > 1)
        {
            photonView.RPC(nameof(RPC_GameOver), RpcTarget.All);
            return;
        }
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_DrawCard), RpcTarget.Others);
        else Deck[index].DrawThisCard();
    }
    [PunRPC]
    public void RPC_DrawCard(int index)
    {
        Deck[index].DrawThisCard();
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
    public void SendToDeckSelected(MonsterCardLocation targetLocation)
    {
        if (photonView.IsMine) SendToDeck(targetLocation);
        else photonView.RPC(nameof(RPC_SendToDeckSelected), RpcTarget.Others, targetLocation);
    }
    [PunRPC]
    public void RPC_SendToDeckSelected(MonsterCardLocation targetLocation)
    {
        SendToDeck(targetLocation);
    }
    public void SendToDeck(MonsterCardLocation targetLocation)
    {
        //if(Field.Count < 1)
        //{
        //    Board.Instance.PlayerInfoText.text = "No Card To Send To Graveyard";
        //    return;
        //}
        gameManager.PrevState = gameManager.State;
        if (targetLocation == MonsterCardLocation.OnField)
        {
            gameManager.State = GameManagerStates.Busy;
            gameManager.SetStateLocally(GameManagerStates.SelectingCardFromFieldToSendToDeck);
            Board.Instance.PlayerInfoText.text = "Select Card From Field To Send To Deck";
        }
        else if (targetLocation == MonsterCardLocation.InHand)
        {
            gameManager.State = GameManagerStates.Busy;
            gameManager.SetStateLocally(GameManagerStates.SelectingCardFromHandToSendToDeck);
            Board.Instance.PlayerInfoText.text = "Select Card From Hand To Send To Deck";
            RedrawHandCards();
        }
    }
    public void Call_Discard(int amount)
    {
        if (photonView.IsMine)
        {
            DiscardCounter = amount;
            StartCoroutine(Discard());
        }
        else photonView.RPC(nameof(RPC_Discard), RpcTarget.Others, amount);
    }
    [PunRPC]
    public void RPC_Discard(int amount)
    {
        DiscardCounter = amount;
        StartCoroutine(Discard());
    }
    public IEnumerator Discard()
    {
        gameManager.PrevState = gameManager.State;
        gameManager.State = GameManagerStates.Busy;
        gameManager.SetStateLocally(GameManagerStates.Discarding);
        while(DiscardCounter != 0)
        {
            yield return new WaitForFixedUpdate();
            Board.Instance.PlayerInfoText.text = "Cards to Discard: " + DiscardCounter.ToString();
            if (Hand.Count == 0) break;
        }
        gameManager.State = gameManager.PrevState;
    }
    public void Call_Destroy(int amount)
    {
        if (photonView.IsMine)
        {
            DestroyCounter = amount;
            StartCoroutine(Destroy());
        }
        else photonView.RPC(nameof(RPC_Destroy), RpcTarget.Others, amount);
    }
    [PunRPC]
    public void RPC_Destroy(int amount)
    {
        DestroyCounter = amount;
        StartCoroutine(Destroy());
    }
    public IEnumerator Destroy()
    {
        gameManager.PrevState = gameManager.State;
        gameManager.State = GameManagerStates.Busy;
        gameManager.SetStateLocally(GameManagerStates.Destroying);
        while (DestroyCounter != 0)
        {
            if (Field.Count == 0) { Board.Instance.PlayerInfoText.text = ""; break; }
            Board.Instance.PlayerInfoText.text = "Cards to Destroy: " + DestroyCounter.ToString();
            yield return new WaitForFixedUpdate();
        }
        gameManager.State = gameManager.PrevState;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
