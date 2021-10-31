using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCardStats : CardStat
{
    [SerializeField]
    private EffectCardEffect effect;
    public EffectCardEffect Effect { get => effect; set => effect = value; }
    private void OnValidate()
    {
        EffectCard_Layout layout = GetComponent<EffectCard_Layout>();
        if (layout == null)
        {
            Debug.Log("Missing Card Layout!");
            return;
        }
        layout.PlayCostTextUI.text = playCost.ToString();
        layout.EffectTextUI.text = effectText.ToString();
        if (layout.NameTextUI.text != cardName.ToString())
        {
            CardNamesData cardNames = (CardNamesData)Resources.Load("CardNames");

            if (cardNames.CardNames == null) cardNames.CardNames = new List<string>();

            if (cardNames.CardNames.Contains(layout.NameTextUI.text))
                cardNames.CardNames.Remove(layout.NameTextUI.text);
            layout.NameTextUI.text = cardName.ToString();
            gameObject.name = cardName.ToString();
            cardNames.CardNames.Add(cardName.ToString());
        }
    }
}
