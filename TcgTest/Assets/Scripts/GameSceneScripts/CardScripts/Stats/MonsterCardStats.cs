using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCardStats : CardStat
{
    [SerializeField]
    private int attack;
    public int Attack { get => attack; set => attack = value; }
    private int defaultAttack;
    public int DefaultAttack { get => defaultAttack; set => defaultAttack = value; }

    [SerializeField]
    private int defense;
    public int Defense { get => defense; set => defense = value; }
    private int defaultDefense;
    public int DefaultDefense { get => defaultDefense; set => defaultDefense = value; }

    [SerializeField]
    private MonsterCardEffect effect;
    public MonsterCardEffect Effect { get => effect; set => effect = value; }

    private void Start()
    {
        DefaultAttack = attack;
        DefaultDefense = defense;
    }
    private void OnValidate()
    {
         MonsterCard_Layout layout = GetComponent<MonsterCard_Layout>(); 

        if(layout ==null)
        {
            Debug.Log("Missing Card Layout!");
            return;
        }
        layout.AttackTextUI.text = attack.ToString();
        layout.DefenseTextUI.text = defense.ToString();
        layout.PlayCostTextUI.text = playCost.ToString();
        layout.EffectTextUI.text = effectText.ToString();
        layout.NameTextUI.text = cardName.ToString();
        gameObject.name = cardName.ToString();
    }
    public void SetValuesToDefault()
    {
        attack = DefaultAttack;
        defense = DefaultDefense;
    }
}
