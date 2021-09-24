using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class Card : MonoBehaviourPunCallbacks
{
    [SerializeField] protected GameObject cardObj;
    protected CardLayout layout;
    public CardType Type { get; protected set; }
    public DuelistType DuelistType { get; protected set; }
    protected MyPlayer player;

    public CardLocation Location { get; set; }

    private CardStats cardStats;
    protected CardStats CardStats { get => cardStats; set => cardStats = value; }

    protected Game_Manager gameManager;
    protected void Awake()
    {
        if (photonView.IsMine)
        {
            DuelistType = DuelistType.Player;
            player = gameManager.Player;
        }
        else
        {
            DuelistType = DuelistType.Enemy;
            player = gameManager.Enemy;
        }
    }
}

public enum CardType
{
    Monster,
    Effect
}
public enum CardLocation
{
    Hand,
    Deck,
    Graveyard,
    Field
}
