using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public abstract class Card : MonoBehaviourPunCallbacks
{
    [SerializeField] protected GameObject cardObj;
    protected CardLayout layout;
    public CardType Type { get; protected set; }
    public DuelistType DuelistType { get; protected set; }
    protected MyPlayer player;

    public CardLocation Location { get; set; }

    protected CardStats cardStats;
    protected Game_Manager gameManager;
    protected RectTransform targetTransform;
    private Vector3 thisTransform { get => new Vector3(transform.position.x, transform.position.y, transform.position.z); }
    public bool IsMoving = false;
    protected Vector3 prevPos;
    protected Vector3 mouseDownPos;
    protected bool mousePressed = false;
    private LineRenderer l;

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
        StartCoroutine(MoveDeckCardTowardsTarget());
    }
    public IEnumerator MoveDeckCardTowardsTarget()
    {
        Vector3 direction;
        IsMoving = true;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = targetTransform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 15;
            if (direction.magnitude < 0.2f) break;
        }
        IsMoving = false;
        transform.position = targetTransform.position;
        Location = CardLocation.Hand;
        player.Hand.Add(this);
        player.RedrawHandCards();
        yield return new WaitForSeconds(0);
    }
    private void OnMouseDown()
    {
        if (Location == CardLocation.Hand)
        {
            mousePressed = true;
            mouseDownPos = this.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        }
        else if (Location == CardLocation.Field)
        {
            mousePressed = true;
            mouseDownPos = this.transform.position;
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

        }
    }
    private void OnMouseDrag()
    {
        if (!mousePressed) return;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
        Debug.Log(mousePos.x);
        Debug.Log(mousePos.y);

        if (Location == CardLocation.Hand)
            transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        else if (Location == CardLocation.Field)
        {
            if (l == null)
                l = gameObject.AddComponent<LineRenderer>();

            Vector3 target = Vector3.zero;
            float closestDistance = 1000;
            Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
                Debug.Log(gameManager.Enemy.Field.Count);
            for (int i = 0; i < gameManager.Enemy.Field.Count; i++)
            {
                float distance = (start - gameManager.Enemy.Field[i].transform.position).magnitude;
                if (distance < closestDistance)
                {
                    target = gameManager.Enemy.Field[i].transform.position;
                    closestDistance = distance;
                }
            }
            if (closestDistance > 20)
            {
                if ((start - Board.Instance.EnemyHandParent.transform.position).magnitude < 15) target = Board.Instance.EnemyHandParent.transform.position;
                else target = start;
            }
            List<Vector3> pos = new List<Vector3>();
            pos.Add(transform.position);
            pos.Add(target);
            l.startWidth = 1f;
            l.endWidth = 1f;
            l.SetPositions(pos.ToArray());
            l.useWorldSpace = true;
        }

    }
    private void OnMouseUp()
    {
        if (!mousePressed) return;
        if (Location == CardLocation.Hand)
        {
            if (player.Mana >= cardStats.PlayCost)
            {
                for (int i = 0; i < 5; i++)
                {
                    Vector3 direction = Board.Instance.PlayerMonsterFields[i].transform.position - transform.position;
                    if (direction.magnitude < 5)
                    {
                        mousePressed = false;
                        transform.position = Board.Instance.PlayerMonsterFields[i].transform.position;
                        Location = CardLocation.Field;
                        player.Hand.Remove(this);
                        photonView.RPC(nameof(RPC_AddToFields), RpcTarget.All);
                        player.Mana -= cardStats.PlayCost;
                        return;
                    }
                }
            }
            transform.position = mouseDownPos;
        }
        else if (Location == CardLocation.Field)
        {
            Destroy(l);
            l = null;
        }
        mousePressed = false;
    }
    [PunRPC]
    public void RPC_AddToFields()
    {
        if (photonView.IsMine) player.Field.Add(this);
        else gameManager.Enemy.Field.Add(this);
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
