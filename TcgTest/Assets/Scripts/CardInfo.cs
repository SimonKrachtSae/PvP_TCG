using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    [SerializeField] private MonsterCard_Layout monsterCardLayout;
    [SerializeField] private EffectCard_Layout effectCardLayout;
    public void AssignCard(Card card)
    {
        monsterCardLayout.gameObject.SetActive(false);
        effectCardLayout.gameObject.SetActive(false);

        if (card.GetType().ToString() == nameof(MonsterCard))
        {
            monsterCardLayout.gameObject.SetActive(true);
            monsterCardLayout.PlayCostTextUI.text = ((MonsterCardStats)card.CardStats).PlayCost.ToString();

			if (((MonsterCard_Layout)card.Layout).EffectTextUI.text != null)
				monsterCardLayout.EffectTextUI.text = ((MonsterCard_Layout)((MonsterCard)card).Layout).EffectTextUI.text;
            monsterCardLayout.NameTextUI.text = ((MonsterCard_Layout)((MonsterCard)card).Layout).NameTextUI.text;

            monsterCardLayout.AttackTextUI.text = ((MonsterCardStats)card.CardStats).Attack.ToString();

            monsterCardLayout.DefenseTextUI.text = ((MonsterCardStats)card.CardStats).Defense.ToString();
        }
        else if (card.GetType().ToString() == nameof(EffectCard))
        {

            effectCardLayout.gameObject.SetActive(true);
            effectCardLayout.PlayCostTextUI.text = ((EffectCardStats)card.CardStats).PlayCost.ToString();

			if (((EffectCard_Layout)card.Layout).EffectTextUI.text != null)
				effectCardLayout.EffectTextUI.text = ((EffectCard_Layout)((EffectCard)card).Layout).EffectTextUI.text;
            effectCardLayout.NameTextUI.text = ((EffectCard_Layout)((EffectCard)card).Layout).NameTextUI.text;
        }
    }

	public void AssignCard(CardLayout cardLayout)
	{
		monsterCardLayout.gameObject.SetActive(false);
		effectCardLayout.gameObject.SetActive(false);

		if (cardLayout.GetType().ToString() == nameof(MonsterCard_Layout))
		{
			monsterCardLayout.gameObject.SetActive(true);
			monsterCardLayout.PlayCostTextUI.text = ((MonsterCard_Layout)cardLayout).PlayCostTextUI.text;

			monsterCardLayout.EffectTextUI.text = ((MonsterCard_Layout)cardLayout).EffectTextUI.text;
			monsterCardLayout.NameTextUI.text = ((MonsterCard_Layout)cardLayout).NameTextUI.text;

			monsterCardLayout.AttackTextUI.text = ((MonsterCard_Layout)cardLayout).AttackTextUI.text;

			monsterCardLayout.DefenseTextUI.text = ((MonsterCard_Layout)cardLayout).DefenseTextUI.text;
		}
		else if (cardLayout.GetType().ToString() == nameof(EffectCard_Layout))
		{

			effectCardLayout.gameObject.SetActive(true);
			effectCardLayout.PlayCostTextUI.text = ((EffectCard_Layout)cardLayout).PlayCostTextUI.text;

			effectCardLayout.EffectTextUI.text = ((EffectCard_Layout)cardLayout).EffectTextUI.text;
			effectCardLayout.NameTextUI.text = ((EffectCard_Layout)cardLayout).NameTextUI.text;
		}
	}
}
