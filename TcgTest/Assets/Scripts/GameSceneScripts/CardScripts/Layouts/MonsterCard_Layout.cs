using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonsterCard_Layout : CardLayout
{
    [SerializeField] private TMP_Text attackTextUI;
    [SerializeField] private TMP_Text defenseTextUI;

    public TMP_Text AttackTextUI { get => attackTextUI; set => attackTextUI = value; }
    public TMP_Text DefenseTextUI { get => defenseTextUI; set => defenseTextUI = value; }
}
