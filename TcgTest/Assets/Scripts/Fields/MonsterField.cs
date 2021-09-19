using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterField : CardField
{
    private bool hasAttacked = false;

    public bool HasAttacked { get => hasAttacked; set => hasAttacked = value; }

    private void Start()
    {
        Button.onClick.AddListener(() => { OnFieldButtonClick(); });
    }
    public void OnFieldButtonClick()
    {
        foreach (Button b in Board.Instance.MonsterCardControls)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }
        switch (GameManager.Instance.MainPhaseStates)
        {
            case MainPhaseStates.Summoning:
                GameManager.Instance.LocalDuelist.Summon(this);
                GameManager.Instance.MainPhaseStates = MainPhaseStates.StandardView;
                break;
            case MainPhaseStates.AttackPhase:
                int i = 0;
                if (!HasAttacked)
                {
                    Board.Instance.MonsterCardControls[i].gameObject.SetActive(true);
                    Board.Instance.MonsterCardControls[i].gameObject.GetComponentInChildren<TMP_Text>().text = "Attack";
                    GameManager.Instance.LocalDuelist.AttackingCard = this.Layout.MonsterCard;
                }
                break;
        }
    }
}
