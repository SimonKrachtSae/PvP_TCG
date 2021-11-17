using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public abstract class CardLayout : MonoBehaviour
{
    [SerializeField] protected TMP_Text nameTextUI;
    [SerializeField] protected TMP_Text effectTextUI;
    [SerializeField] protected TMP_Text playCostTextUI;

    public TMP_Text NameTextUI { get => nameTextUI; set => nameTextUI = value; }
    public TMP_Text EffectTextUI { get => effectTextUI; set => effectTextUI = value; }
    public TMP_Text PlayCostTextUI { get => playCostTextUI; set => playCostTextUI = value; }
}

