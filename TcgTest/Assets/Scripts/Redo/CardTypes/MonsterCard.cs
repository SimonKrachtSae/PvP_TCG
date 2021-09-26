using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
public class MonsterCard : Card, IPunObservable
{
    public int CardStatsIndex { get => 0; set => photonView.RPC(nameof(RPC_UpdateStats), RpcTarget.All, value); }
    private Card attackTarget;
    public bool HasAttacked { get; set; }
    public bool HasBlocked { get; set; }
    private void Start()
    {
        try
        {
            layout = cardObj.gameObject.GetComponent<MonsterCard_Layout>();
            gameManager = Game_Manager.Instance;
        }
        catch
        {
            Debug.Log("Failed to create Card...");
            Destroy(this.gameObject);
        }
        
        player.Subscribe(this);
        Type = CardType.Monster;
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
    private void OnValidate()
    {
        if (cardStats == null) return;
        DrawValues();
    }
    [PunRPC]
    public void RPC_UpdatePosition(Vector3 value)
    {
        transform.position = new Vector3(value.x, value.y * -1, value.z);
    }
    [PunRPC]
    public void RPC_UpdateStats(int index)
    {
        base.CardStats = player.StartingDeck[index].gameObject.GetComponent<MonsterCardStats>();
    }
    public void DrawValues()
    {
        layout = cardObj.GetComponent<MonsterCard_Layout>();
        ((MonsterCard_Layout)layout).AttackTextUI.text = ((MonsterCardStats)cardStats).Attack.ToString();
        ((MonsterCard_Layout)layout).NameTextUI.text = ((MonsterCardStats)cardStats).CardName.ToString();
        ((MonsterCard_Layout)layout).PlayCostTextUI.text = ((MonsterCardStats)cardStats).PlayCost.ToString();
        ((MonsterCard_Layout)layout).DefenseTextUI.text = ((MonsterCardStats)cardStats).Defense.ToString();
    }

    private void OnMouseDown()
    {
        if (gameManager.State == MainPhaseStates.Blocking && gameManager.CurrentDuelist == DuelistType.Enemy)
        {
            if (HasBlocked) { Board.Instance.PlayerInfoText.text = "Already blocked!"; return; }

            gameManager.BlockingMonsterIndex = player.Field.IndexOf(this);
            HasBlocked = true;
            return;
        }
        else if (gameManager.State == MainPhaseStates.Blocking && gameManager.CurrentDuelist == DuelistType.Enemy) return;
        if(gameManager.State == MainPhaseStates.AttackPhase)
        {
            if (HasAttacked) { Board.Instance.PlayerInfoText.text = "Already attacked!"; return; }

            if(Location == CardLocation.Field)
            {
                mouseDownPos = this.transform.position;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
            }
        }
        else if(gameManager.State == MainPhaseStates.StartPhase)
        {
            if (Location == CardLocation.Hand || Location == CardLocation.Field)
            {
                mouseDownPos = this.transform.position;
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
            }
        }
    }
    private void OnMouseDrag()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));

        if (gameManager.State == MainPhaseStates.AttackPhase)
        {
            if (HasAttacked) { Board.Instance.PlayerInfoText.text = "Already attacked!"; return; }

            if (Location == CardLocation.Field)
            {
                if (l == null)
                    l = gameObject.AddComponent<LineRenderer>();

                Vector3 target = Vector3.zero;
                float closestDistance = 1000;
                Vector3 start = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z));
                for (int i = 0; i < gameManager.Enemy.Field.Count; i++)
                {
                    float distance = (start - gameManager.Enemy.Field[i].transform.position).magnitude;
                    if (distance < closestDistance)
                    {
                        target = gameManager.Enemy.Field[i].transform.position;
                        attackTarget = gameManager.Enemy.Field[i];
                        closestDistance = distance;
                    }
                }
                if (closestDistance > 20)
                {
                    attackTarget = null;
                    if ((start - Board.Instance.EnemyHandParent.transform.position).magnitude < 15)
                    {
                        target = Board.Instance.EnemyHandParent.transform.position;
                    }
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
        else if(gameManager.State == MainPhaseStates.StartPhase)
        {
            if (Location == CardLocation.Hand || Location == CardLocation.Field)
                transform.position = new Vector3(mousePos.x, mousePos.y, transform.position.z);
        }
    }
    private void OnMouseUp()
    {
        if (gameManager.State == MainPhaseStates.AttackPhase)
        {
            if (HasAttacked) { Board.Instance.PlayerInfoText.text = "Already attacked!"; return; }

            if (Location == CardLocation.Field)
            {
                if(attackTarget != null)
                {
                    if (((MonsterCardStats)attackTarget.CardStats).Defense < ((MonsterCardStats)cardStats).Attack)
                    {
                        ((MonsterCard)attackTarget).SendToGraveyard();
                    }
                    else if (((MonsterCardStats)attackTarget.CardStats).Defense > ((MonsterCardStats)cardStats).Attack)
                    {
                        SendToGraveyard();
                    }
                    HasAttacked = true;
                }
                else if(l.GetPosition(1) == Board.Instance.EnemyHandParent.transform.position)
                {
                    if (gameManager.Enemy.Field.Count == 0)
                    {
                        player.DrawCard(0);
                    }
                    else
                    {
                        gameManager.AttackingMonster = this;
                        gameManager.Enemy.ShowBlockRequest();
                    }
                    HasAttacked = true;
                }
                Destroy(l);
                l = null;
            }
        }
        else if (gameManager.State == MainPhaseStates.StartPhase)
        {
            if (Location == CardLocation.Hand)
            {
                if (player.Mana >= base.CardStats.PlayCost)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        Vector3 direction = Board.Instance.PlayerMonsterFields[i].transform.position - transform.position;
                        if (direction.magnitude < 5)
                        {
                            transform.position = Board.Instance.PlayerMonsterFields[i].transform.position;
                            Location = CardLocation.Field;
                            player.Hand.Remove(this);
                            player.RedrawHandCards();
                            photonView.RPC(nameof(RPC_AddToField), RpcTarget.All);
                            player.Mana -= base.CardStats.PlayCost;
                            if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnSummon?.Invoke();
                            return;
                        }
                    }
                }
                transform.position = mouseDownPos;
            }
            else if(Location == CardLocation.Field)
            {
                if (((Vector2)Board.Instance.BurnField.transform.position - (Vector2)transform.position).magnitude < 10)
                {
                    photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
                    player.Mana += ((MonsterCardStats)cardStats).PlayCost;
                    if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
                    PhotonNetwork.Destroy(this.gameObject);
                }
                else
                {
                    transform.position = mouseDownPos;
                }
            }
        }
    }
    public void SendToGraveyard()
    {
        if (!photonView.IsMine) photonView.RPC(nameof(RPC_SendToGraveyard), RpcTarget.Others);
        else StartCoroutine(SendToGraveyardVisuals());
    }
    [PunRPC]
    public void RPC_SendToGraveyard()
    {
        StartCoroutine(SendToGraveyardVisuals());
    }
    public IEnumerator SendToGraveyardVisuals()
    {
        ((MonsterCardStats)cardStats).SetValuesToDefault();
        if (((MonsterCardStats)cardStats).Effect != null) ((MonsterCardStats)cardStats).Effect.OnDestroy?.Invoke();
        Location = CardLocation.Graveyard;
        Vector3 direction;
        photonView.RPC(nameof(RPC_RemoveFromField), RpcTarget.All);
        while (true)
        {
            yield return new WaitForFixedUpdate();
            direction = player.GraveyardObj.transform.position - transform.position;
            transform.position += direction.normalized * Time.fixedDeltaTime * 25;
            if (direction.magnitude < 0.3f) break;
        }
        transform.position = Board.Instance.PlayerGraveyard.transform.position;
        player.Graveyard.Add(this);
    }
    [PunRPC]
    public void RPC_AddToField()
    {
        if (photonView.IsMine) player.Field.Add(this);
        else gameManager.Enemy.Field.Add(this);
    }
    [PunRPC]
    public void RPC_RemoveFromField()
    {
        if (photonView.IsMine) player.Field.Remove(this);
        else gameManager.Enemy.Field.Remove(this);
    }
    private void OnDestroy()
    {
        if (player.Field.Contains(this)) player.Field.Remove(this);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
