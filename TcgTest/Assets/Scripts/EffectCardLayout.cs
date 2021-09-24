using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class EffectCardLayout : MonoBehaviour
{
    [SerializeField] private Duelist player;
    [SerializeField] private protected TMP_Text NameTextUI;
    [SerializeField] private protected TMP_Text EffectTextUI;
    [SerializeField] private protected TMP_Text PlayCostTextUI;
    private EffectCardStats effectCard;
    private FieldState state;
    public EffectCardStats EffectCard
    {
        get => effectCard;
        set
        {
            NameTextUI.text = value.CardName;
            //if (value.Effect != null) EffectTextUI.text = value.Effect.EffectDescription;
            EffectTextUI.text = "";
            PlayCostTextUI.text = value.PlayCost.ToString();
            effectCard = value;
        }
    }
}
