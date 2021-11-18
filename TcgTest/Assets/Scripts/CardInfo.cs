using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CardInfo : MonoBehaviour
{
    [SerializeField] private MonsterCard_Layout monsterCardLayout;
    [SerializeField] private EffectCard_Layout effectCardLayout;
	private GameObject card;
	public void AssignCard(string name, float scale)
	{
		if (this.card != null) Destroy(card);
		this.card = (GameObject)Instantiate(Resources.Load(name), transform.position, new Quaternion(0, 0, 0, 0));
		this.card.transform.localScale *= scale;
		card.transform.parent = transform;
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
