using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class HandField : CardField
{
    private void Start()
    {
        if(Button != null)Button.onClick.AddListener(() => { OnFieldButtonClick(); });
    }
    public void AssignCard(CardStats cardStats)
    {
        //if (Card != null) return;
        if(cardStats.GetType().ToString() == nameof(MonsterCardStats))
        {
            Debug.Log("Match");
            Card = Instantiate(GameUIManager.Instance.MonsterCardLayoutPrefab, this.transform);
            layout = card.GetComponent<MonsterCardLayout>();
            layout.MonsterCard = (MonsterCardStats)cardStats;
        }
        else
        {
            Card = Instantiate(GameUIManager.Instance.EffectCardLayoutPrefab, this.transform);
            EffectCardLayout = card.GetComponent<EffectCardLayout>();
            EffectCardLayout.EffectCard = (EffectCardStats)cardStats;
        }
    }
    public void OnFieldButtonClick()
    {
        Board.Instance.CardInfo.AssignCard(Layout.MonsterCard);
        foreach (Button b in Board.Instance.MonsterCardControls)
        {
            b.onClick.RemoveAllListeners();
            b.gameObject.SetActive(false);
        }
        int i = 0;
        if (GameManager.Instance.MainPhaseStates == MainPhaseStates.StandardView
            && GameManager.Instance.LocalDuelist.SummonPower >= Layout.MonsterCard.PlayCost)
        {
            Board.Instance.MonsterCardControls[i].gameObject.SetActive(true);
            Board.Instance.MonsterCardControls[i].gameObject.GetComponentInChildren<TMP_Text>().text = "Summon";
            Board.Instance.MonsterCardControls[i].onClick.AddListener(() => { GameUIManager.Instance.Summon(); });
            GameManager.Instance.LocalDuelist.CardToBeSummoned = this.Layout.MonsterCard;
        }
    }
}

