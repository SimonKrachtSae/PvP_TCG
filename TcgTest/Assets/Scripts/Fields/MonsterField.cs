using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterField : CardField
{
    private void Start()
    {
        Button.onClick.AddListener(() => { OnFieldButtonClick(); });
    }
    public void OnFieldButtonClick()
    {
        switch(GameManager.Instance.MainPhaseStates)
        {
            case MainPhaseStates.Summoning:
                GameManager.Instance.LocalDuelist.Summon(this);
                GameManager.Instance.MainPhaseStates = MainPhaseStates.StandardView;
                break;
        }
    }
}
