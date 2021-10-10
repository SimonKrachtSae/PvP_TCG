using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCard : Effect
{
    [SerializeField] private DuelistType duelistType;
    [SerializeField] private int amount;

    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        if (duelistType == DuelistType.Enemy)
        {
            if (Game_Manager.Instance.Player != Player)StartCoroutine(PlayerDrawCards());
            else StartCoroutine(EnemyDrawCards());
        }
        else
        {
            if (Game_Manager.Instance.Player != Player) StartCoroutine(EnemyDrawCards());
            else StartCoroutine(PlayerDrawCards());
        }
    }
    private IEnumerator PlayerDrawCards()
    {
        for (int i = 0; i < amount; i++)
        {
            Game_Manager.Instance.Player.DrawCard(0);
            yield return new WaitForSecondsRealtime(1);
        }
        yield return new WaitForSecondsRealtime(2);
    }
    private IEnumerator EnemyDrawCards()
    {
        for (int i = 0; i < amount; i++)
        {
            Game_Manager.Instance.Enemy.DrawCard(0);
            yield return new WaitForSecondsRealtime(1);
        }
        yield return new WaitForSecondsRealtime(2);
    }
}
