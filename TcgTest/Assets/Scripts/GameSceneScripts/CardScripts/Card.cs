using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Photon.Pun;
public abstract class Card : MonoBehaviourPunCallbacks
{
    protected CardLayout layout;
    public CardLayout Layout { get => layout; set => layout = value; }
    public CardType Type { get; protected set; }
    public DuelistType DuelistType { get; protected set; }
    protected MyPlayer player;

    public CardLocation Location { get; set; }

    [SerializeField]protected CardStat cardStats;
    public CardStat CardStats { get => cardStats; set => cardStats = value; }
    protected Game_Manager gameManager;
    protected RectTransform targetTransform;
    private Vector3 thisTransform { get => new Vector3(transform.position.x, transform.position.y, transform.position.z); }

    public bool IsMoving = false;
    protected Vector3 prevPos;
    protected Vector3 mouseDownPos;
    protected LineRenderer l;
    public UnityAction OnMouseDownEvent { get; set; }
    public UnityAction OnMouseDragEvent { get; set; }
    public UnityAction OnMouseUpEvent { get; set; }
    public Vector3 Target { get; set; }

    protected Vector3 mousePos;
    [SerializeField] protected Image border;

    [SerializeField] protected ParticleBomb particleBomb;
    public ParticleBomb ParticleBomb { get => particleBomb; set => particleBomb = value; }
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
            transform.rotation = new Quaternion(0.5f, 0, 0, 0);
        }
        prevPos = transform.position;
        transform.position = player.DeckField.transform.position;
        transform.parent = player.transform;
    }
    protected void Update()
    {
        if (!photonView.IsMine) return;
        if (transform.position != prevPos)
        {
            photonView.RPC(nameof(RPC_UpdatePosition), RpcTarget.Others, transform.position);
        }
        prevPos = transform.position;
    }
    public void Call_ParticleBomb(string s, Color color, NetworkTarget target)
    {
        if (target == NetworkTarget.Local) particleBomb.Explode(s, color);
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_ParticleBomb), RpcTarget.Others, s, new byte[3] { (byte)color.r, (byte)color.g, (byte)color.b });
        else if (target == NetworkTarget.All) photonView.RPC(nameof(RPC_ParticleBomb), RpcTarget.All, s, new byte[3] { (byte)color.r, (byte)color.g, (byte)color.b });
    }
    [PunRPC]
    public void RPC_ParticleBomb(string s, byte[] colorVals)
    {
        Color color = new Color(colorVals[0], colorVals[1], colorVals[2]);
        particleBomb.Explode(s, color);
    }
    public void Local_DrawCard()
    {
        photonView.RPC(nameof(RPC_RemoveFromDeck), RpcTarget.All);
        photonView.RPC(nameof(RPC_AddToHand), RpcTarget.All);
        MoveTowardsHand(player.HandParent.transform.position);
        Call_RotateToFront(NetworkTarget.Local);
        if (gameManager.State == GameManagerStates.StartPhase && gameManager.CurrentDuelist != DuelistType.Enemy ) AssignHandEvents(NetworkTarget.Local);
    }
    public void Call_RotateToFront(NetworkTarget target)
    {
        if (target == NetworkTarget.Local) Local_RotateToFront();
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_RotateToFront), RpcTarget.Others);
        else if (target == NetworkTarget.All) photonView.RPC(nameof(RPC_RotateToFront), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_RotateToFront()
    {
        Local_RotateToFront();
    }
    private void Local_RotateToFront()
    {
        StartCoroutine(RotateToFront());
    }
    public void Call_RotateToBack(NetworkTarget target)
    {
        if (target == NetworkTarget.Local) Local_RotateToBack();
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_RotateToBack), RpcTarget.Others);
        else if (target == NetworkTarget.All) photonView.RPC(nameof(RPC_RotateToBack), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_RotateToBack()
    {
        Local_RotateToBack();
    }
    private void Local_RotateToBack()
    {
        StartCoroutine(RotateToBack());
    }
    public void AssignHandEvents(NetworkTarget networkTarget)
    {
        ClearEvents();
        Call_AddEvent(CardEvent.FollowMouse_MouseDown, MouseEvent.Down, networkTarget);
        Call_AddEvent(CardEvent.FollowMouse_MouseDrag, MouseEvent.Drag, networkTarget);
        if(GetType().ToString() == nameof(MonsterCard))Call_AddEvent(CardEvent.Summon, MouseEvent.Up, networkTarget);
        else if(GetType().ToString() == nameof(EffectCard))Call_AddEvent(CardEvent.Play, MouseEvent.Up, networkTarget);
    }
    public IEnumerator MoveCardFromDeckToHand()
    {
        Vector3 direction;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = targetTransform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        transform.position = targetTransform.position;
        Location = CardLocation.Hand;
        player.RedrawHandCards();
    }
    [PunRPC]
    public void SetRotation(Quaternion q)
    {
        transform.rotation = q;
    }
    [PunRPC]
    public void RPC_UpdatePosition(Vector3 value)
    {
        transform.position = new Vector3(value.x, value.y * -1, value.z);
    }
    public IEnumerator RotateToBack()
    {
        float value = 0;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (value > 175)
            {
                transform.rotation = Quaternion.Euler(0, 180, 0);
                break;
            }
            else
            {
                value += 2;
                transform.rotation = Quaternion.Euler(0, value, 0);
            }
        }
    }
    public IEnumerator RotateToFront()
    {
        float value = 180;
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (value < 5)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                break;
            }
            else
            {
                value -= 2;
                transform.rotation = Quaternion.Euler(0, value, 0);
            }
        }
    }
    public void MoveTowardsTarget(Vector3 target)
    {
        Target = target;
        StartCoroutine(TranslateCard());
    }
    protected IEnumerator TranslateCard()
    {
        Vector3 direction;
        int maxIterations = 5000;
        while((Target - transform.position).magnitude > 0.5f)
        {
            yield return new WaitForFixedUpdate();
            direction = Target - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;

            maxIterations--;
            if (maxIterations <= 0) break;
        }
        transform.position = Target;
    }
    public void MoveTowardsHand(Vector3 target)
    {
        Target = target;
        StartCoroutine(TranslateCardTowardsHand());
    }
    protected IEnumerator TranslateCardTowardsHand()
    {
        Vector3 direction;
        int maxIterations = 5000;
        while ((Target - transform.position).magnitude > 0.5f)
        {
            yield return new WaitForFixedUpdate();
            direction = Target - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;

            maxIterations--;
            if (maxIterations <= 0) break;
        }
        transform.position = Target;
        player.RedrawHandCards();
    }
    public void Call_AddToHand()
    {
        photonView.RPC(nameof(RPC_AddToHand), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_AddToHand()
    {
        if (photonView.IsMine)
        {
            if(!player.Hand.Contains(this)) player.Hand.Add(this);
        }
        else
        {
            if (!gameManager.Enemy.Hand.Contains(this)) gameManager.Enemy.Hand.Add(this);
        }

    }
    [PunRPC]
    public void RPC_RemoveFromHand()
    {
        if (photonView.IsMine)
        {
            if (player.Hand.Contains(this)) player.Hand.Remove(this);
            player.RedrawHandCards();
        }
        else
        {
            if (gameManager.Enemy.Hand.Contains(this)) gameManager.Enemy.Hand.Remove(this);
            gameManager.Enemy.RedrawHandCards();
        }
    }
    [PunRPC]
    public void RPC_AddToDeck()
    {
        if (photonView.IsMine) if (!player.DeckList.Contains(this)) player.DeckList.Add(this);
        else if (!gameManager.Enemy.DeckList.Contains(this)) player.DeckList.Add(this);
    }
    [PunRPC]
    public void RPC_RemoveFromDeck()
    {
        if (photonView.IsMine) if (player.DeckList.Contains(this)) player.DeckList.Remove(this);
        else if(gameManager.Enemy.DeckList.Contains(this)) gameManager.Enemy.DeckList.Remove(this);
    }
    [PunRPC]
    public void RPC_AddToGraveyard()
    {
        if (photonView.IsMine) player.Graveyard.Add(this);
        else gameManager.Enemy.Graveyard.Add(this);
    }
    [PunRPC]
    public void RPC_RemoveFromGraveyard()
    {
        if (photonView.IsMine) player.Graveyard.Remove(this);
        else gameManager.Enemy.Graveyard.Remove(this);
    }
    public void Event_FollowMouseDown()
    {
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }
    public void Event_FollowMouseDrag()
    {
        transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
    }
    public void Event_Discard()
    {
        Call_SendToGraveyard();
        gameManager.DiscardCounter--;
        if (player.Hand.Count == 0) gameManager.DiscardCounter = 0;
    }
    public void Event_Destroy()
    {
        Call_SendToGraveyard();
        gameManager.DestroyCounter--;
        if (player.Field.Count == 0) gameManager.DestroyCounter = 0;
    }
    public void Event_Recall()
    {
        Call_SendToDeck();
        Call_RotateToBack(NetworkTarget.All);
        player.OnRecall();
    }
    public void Call_SendToDeck()
    {
        if (this.GetType().ToString() == nameof(MonsterCard).ToString())
            if (((MonsterCardStats)cardStats).Effect != null && player.Field.Contains((MonsterCard)this)) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
        photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
        photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
        photonView.RPC(nameof(RPC_AddToDeck), RpcTarget.All);
        if (this.GetType().ToString() == nameof(MonsterCard).ToString()) photonView.RPC(nameof(RPC_SetValuesToDefault), RpcTarget.All);
        ClearEvents();

        if (!photonView.IsMine) photonView.RPC(nameof(RPC_SendToDeck), RpcTarget.Others);
        else
        {
            Local_SendToDeck();
        }
    }
    [PunRPC]
    public void RPC_SendToDeck()
    {
        Local_SendToDeck();
    }
    public void Local_SendToDeck()
    {
        if (this.GetType().ToString() == nameof(MonsterCard).ToString())
            if (((MonsterCardStats)cardStats).Effect != null && player.Field.Contains((MonsterCard)this)) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
        ClearEvents();
        MoveTowardsTarget(player.DeckField.transform.position);
        RotateToBack();
    }
    public void Call_SendToGraveyard()
    {
        if (this.GetType().ToString() == nameof(MonsterCard).ToString())
            if (((MonsterCardStats)cardStats).Effect != null && player.Field.Contains((MonsterCard)this)) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
        photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
        photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
        photonView.RPC(nameof(RPC_AddToGraveyard), RpcTarget.All);
        if (this.GetType().ToString() == nameof(MonsterCard).ToString()) photonView.RPC(nameof(RPC_SetValuesToDefault), RpcTarget.All);
        ClearEvents();
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_SendToGraveyard), RpcTarget.Others);
        else
        {
            MoveTowardsTarget(player.GraveyardObj.transform.position);
        }
    }
    [PunRPC]
    public void RPC_SendToGraveyard()
    {
        ClearEvents();
        MoveTowardsTarget(player.GraveyardObj.transform.position);
    }
    [PunRPC]
    public void RPC_SetValuesToDefault()
    {
        ((MonsterCardStats)cardStats).SetValuesToDefault();
    }
    protected void Local_RemoveFromCurrentLists()
    {
        if(this.GetType().ToString() == nameof(MonsterCard).ToString())
            if (player.Field.Contains((MonsterCard)this)) photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);

        photonView.RPC(nameof(RPC_RemoveFromHand), RpcTarget.All);
        photonView.RPC(nameof(RPC_RemoveFromDeck), RpcTarget.All);
        photonView.RPC(nameof(RPC_RemoveFromGraveyard), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_RemoveFromField()
    {
        if (this.GetType().ToString() != nameof(MonsterCard).ToString()) return;
        if (photonView.IsMine) player.Field.Remove((MonsterCard)this);
        else gameManager.Enemy.Field.Remove((MonsterCard)this);
        ((MonsterCardStats)CardStats).Attack -= player.AttackBoost;
        ((MonsterCard_Layout)Layout).AttackTextUI.text = ((MonsterCardStats)CardStats).Attack.ToString();
        ((MonsterCardStats)CardStats).Defense -= player.DefenseBoost;
        ((MonsterCard_Layout)Layout).DefenseTextUI.text = ((MonsterCardStats)CardStats).Defense.ToString();
    }
    public virtual void Call_AddEvent(CardEvent cardEvent, MouseEvent mouseEvent, NetworkTarget target) { }
    public void ClearEvents()
    {
        border.color = Color.black;
        OnMouseDownEvent = null;
        OnMouseDragEvent = null;
        OnMouseUpEvent = null;
    }
    protected void AssignEvent(UnityAction action, MouseEvent mouseEvent)
    {
        if (mouseEvent == MouseEvent.Down) OnMouseDownEvent = action;
        else if (mouseEvent == MouseEvent.Drag) OnMouseDragEvent = action;
        else if (mouseEvent == MouseEvent.Up) OnMouseUpEvent = action;
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
