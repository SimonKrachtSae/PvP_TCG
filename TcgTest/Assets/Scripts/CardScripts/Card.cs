using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public abstract class Card : MonoBehaviourPunCallbacks
{
    [SerializeField] protected GameObject cardObj;
    protected CardLayout layout;
    public CardLayout Layout { get => layout; set => layout = value; }
    public CardType Type { get; protected set; }
    public DuelistType DuelistType { get; protected set; }
    protected MyPlayer player;

    public CardLocation Location { get; set; }

    [SerializeField]protected CardStats cardStats;
    public CardStats CardStats { get => cardStats; set => cardStats = value; }
    protected Game_Manager gameManager;
    protected RectTransform targetTransform;
    private Vector3 thisTransform { get => new Vector3(transform.position.x, transform.position.y, transform.position.z); }

    public bool IsMoving = false;
    protected Vector3 prevPos;
    protected Vector3 mouseDownPos;
    protected LineRenderer l;

    protected void Awake()
    {
        gameManager = Game_Manager.Instance;
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
        prevPos = transform.position;
    }
    public void DrawThisCard()
    {
        targetTransform = player.HandParent;
        player.Deck.Remove(this);
        StartCoroutine(MoveCardFromDeckToHand());
    }
    public IEnumerator MoveCardFromDeckToHand()
    {
        Vector3 direction;
        IsMoving = true;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = targetTransform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        IsMoving = false;
        transform.position = targetTransform.position;
        Location = CardLocation.Hand;
        player.Hand.Add(this);
        player.RedrawHandCards();
        yield return new WaitForSeconds(0);
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
