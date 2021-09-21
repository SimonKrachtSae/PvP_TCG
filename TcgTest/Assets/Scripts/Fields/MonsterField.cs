using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterField : CardField
{
    private bool hasAttacked = false;

    public bool HasAttacked { get => hasAttacked; set => hasAttacked = value; }

    [SerializeField] protected DuelistType duelistType;
    public DuelistType DuelistType { get => duelistType; set => duelistType = value; }

    private void Start()
    {
        Button.onClick.AddListener(() => { OnFieldButtonClick(); });
    }
    public void OnFieldButtonClick()
    {

        if (duelistType == DuelistType.Player)
        {
            foreach (Button b in Board.Instance.MonsterCardControls)
            {
                b.onClick.RemoveAllListeners();
                b.gameObject.SetActive(false);
            }
            switch (GameManager.Instance.MainPhaseStates)
            {
                case MainPhaseStates.StandardView:
                    Board.Instance.MonsterCardControls[2].gameObject.SetActive(true);
                    Board.Instance.MonsterCardControls[2].gameObject.GetComponentInChildren<TMP_Text>().text = "Tribute";
                    Board.Instance.MonsterCardControls[2].onClick.AddListener(() => { Tribute(); });
                    break;
                case MainPhaseStates.Summoning:
                    GameManager.Instance.LocalDuelist.Summon(this);
                    GameManager.Instance.LocalDuelist.CardToBeSummoned = null;
                    GameManager.Instance.MainPhaseStates = MainPhaseStates.StandardView;
                    break;
                case MainPhaseStates.AttackPhase:
                    if (Card == null) return;
                    if (!HasAttacked)
                    {
                        Board.Instance.MonsterCardControls[0].gameObject.SetActive(true);
                        Board.Instance.MonsterCardControls[0].gameObject.GetComponentInChildren<TMP_Text>().text = "Attack";
                        Board.Instance.MonsterCardControls[1].gameObject.SetActive(true);
                        Board.Instance.MonsterCardControls[1].gameObject.GetComponentInChildren<TMP_Text>().text = "DirectAttack";
                        Board.Instance.MonsterCardControls[1].onClick.AddListener(() => { DirectAttack(); });

                        GameManager.Instance.LocalDuelist.AttackingCard = this;
                    }
                    break;
                case MainPhaseStates.Blocking:
                    GameManager.Instance.Enemy.BlockingMonsterIndex = GameManager.Instance.LocalDuelist.MonsterFields.IndexOf(this);
                    GameManager.Instance.MainPhaseStates = MainPhaseStates.AttackPhase;
                    break;
            }
        }
        else if (duelistType == DuelistType.Enemy)
        {
            if (Card == null) return;
            if (GameManager.Instance.LocalDuelist.AttackingCard != null && GameManager.Instance.MainPhaseStates == MainPhaseStates.AttackPhase)
            {
                GameManager.Instance.LocalDuelist.AttackingCard.HasAttacked = true;
                MonsterCardStats cardStats = GameManager.Instance.LocalDuelist.AttackingCard.Layout.MonsterCard;
                if(cardStats.Attack > Layout.MonsterCard.Defense) { GameManager.Instance.Enemy.DestroyMonster(this); }
                else if(cardStats.Attack < Layout.MonsterCard.Defense) { GameManager.Instance.LocalDuelist.DestroyMonster(GameManager.Instance.LocalDuelist.AttackingCard); }
                GameManager.Instance.LocalDuelist.AttackingCard = null;
            }
        }
    }
    private void DirectAttack()
    {
        if (hasAttacked) return;
        hasAttacked = true;
        GameManager.Instance.LocalDuelist.AttackingCard = this;
        int i = 0;
        foreach (MonsterField field in GameManager.Instance.Enemy.MonsterFields) if (field.Card != null) i++;
        if(i == 0)
        {
            GameManager.Instance.LocalDuelist.DrawCard(0);
        }
        else
        {
            GameManager.Instance.Enemy.ShowBlockRequest();
        }
    }
    private void Tribute()
    {
        GameManager.Instance.LocalDuelist.SummonPower += Layout.MonsterCard.PlayCost;
        GameManager.Instance.LocalDuelist.DestroyMonster(this);
    }
    public void AssignCard(MonsterCardStats cardStats)
    {
        //if (Card != null) return;
        if (duelistType == DuelistType.Player) GameManager.Instance.LocalDuelist.ActiveMonsterFields.Add(this);
        Card = Instantiate(GameUIManager.Instance.CardLayoutPrefab, this.transform);
        layout = card.GetComponent<CardLayout>();
        layout.MonsterCard = cardStats;
    }
    public void UnAssignCard()
    {
        if (Card == null) return;
        if (duelistType == DuelistType.Player) GameManager.Instance.LocalDuelist.ActiveMonsterFields.Remove(this);
        Destroy(Card.gameObject);
    }
}
