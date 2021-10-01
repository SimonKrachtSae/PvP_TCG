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
    void Update()
    {
        if (!photonView.IsMine) return;
        if (transform.position != prevPos)
        {
            photonView.RPC(nameof(RPC_UpdatePosition), RpcTarget.Others, transform.position);
        }
        prevPos = transform.position;
    }
    public void DrawThisCard()
    {
        targetTransform = player.HandParent;
        player.Deck.Remove(this);
        StartCoroutine(MoveCardFromDeckToHand());
        //StartCoroutine(RotateToFront());
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
    [PunRPC]
    public void RPC_UpdatePosition(Vector3 value)
    {
        transform.position = new Vector3(value.x, value.y * -1, value.z);
    }
    public IEnumerator RotateToBack()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            transform.rotation = Quaternion.Euler(0, transform.rotation.y + 2, 0);
            if(transform.rotation.y > 175 && transform.rotation.y < 185)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            }
        }
    }
    public IEnumerator RotateToFront()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if ((transform.rotation.y > 350 && (transform.rotation.y < 359) || (transform.rotation.y < 10 && transform.rotation.y >= 0)))
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            else
            {
                transform.Rotate(new Vector3(0,1,0));
            }
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
