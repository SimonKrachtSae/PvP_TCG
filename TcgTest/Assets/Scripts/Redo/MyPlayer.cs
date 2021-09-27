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
            if (deck.Count == 0 && gameManager.Turn > 1)
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
    public int SummonPowerBoost { get => summonPowerBoost; set => photonView.RPC(nameof(RPC_UpdateSumonPowerBoost), RpcTarget.All, value); }

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
        if (deck.Count == 0 && gameManager.Turn > 1)
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
        //if (Deck.Contains(card)) Destroy(card.gameObject);
        Deck.Add(card);
        card.Location = CardLocation.Deck;
        card.gameObject.transform.position = DeckField.transform.position + new Vector3(0, 0, Deck.IndexOf(card) / 100);
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
    public void RPC_GameOver()
    {
        foreach (GameObject gameObject in startingDeck) PhotonNetwork.Destroy(gameObject);
        GameUIManager.Instance.SetGameState(GameState.GameOver);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
