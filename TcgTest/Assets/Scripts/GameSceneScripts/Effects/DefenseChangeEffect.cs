using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseChangeEffect : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;
    [SerializeField] private AttackTarget target;
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player)
            { 
                foreach (MonsterCard card in Game_Manager.Instance.Player.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Defense += amount;
                        ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<DefenseChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Defense += amount;
                            ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (MonsterCard card in Game_Manager.Instance.Enemy.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Defense += amount;
                        ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<DefenseChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Defense += amount;
                            ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            if (Game_Manager.Instance.Player != Player)
            {
                foreach (MonsterCard card in Game_Manager.Instance.Enemy.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Defense += amount;
                        ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<DefenseChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Defense += amount;
                            ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                            return;
                        }
                    }
                }
            }
            else
            {
                foreach (MonsterCard card in Game_Manager.Instance.Player.Field)
                {
                    if (target == AttackTarget.All)
                    {
                        ((MonsterCardStats)card.CardStats).Defense += amount;
                        ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                    }
                    else
                    {
                        if (card.gameObject.GetComponent<DefenseChangeEffect>() == this)
                        {
                            ((MonsterCardStats)card.CardStats).Defense += amount;
                            ((MonsterCard_Layout)card.Layout).DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
                            return;
                        }
                    }
                }
            }
        }
    }
}
