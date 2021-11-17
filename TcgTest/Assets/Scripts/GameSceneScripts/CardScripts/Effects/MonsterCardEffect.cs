using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class MonsterCardEffect : MonoBehaviour
{
    [SerializeField] private List<Effect> onSummonEffects;
    [SerializeField] private List<Effect> onDestroyEffects;
    [SerializeField] private List<Effect> onAttackEffects;
    [SerializeField] private List<Effect> onBlockEffects;
    [SerializeField] private List<Effect> onBattlleWonEffects;
    [SerializeField] private List<Effect> onBlockSuccessfullEffects;
    [SerializeField] private List<Effect> onDirectAttackSucceedsEffects;
    public void Call_OnSummon()
    {
        StartCoroutine(Summon());
    }
    private IEnumerator Summon()
    {
        for(int i = 0; i < onSummonEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onSummonEffects[i].Execute();
        }
    }
    public void Call_OnDestroy()
    {
        StartCoroutine(Destroy());
    }
    private IEnumerator Destroy()
    {
        for (int i = 0; i < onDestroyEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onDestroyEffects[i].Execute();
        }
    }
    public void Call_BattleWon()
    {
        StartCoroutine(BattleWon());
    }
    private IEnumerator BattleWon()
    {
        for (int i = 0; i < onBattlleWonEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onBattlleWonEffects[i].Execute();
        }
    }
    public void Call_BlockSuccessfull()
    {
        StartCoroutine(BlockSuccessfull());
    }
    private IEnumerator BlockSuccessfull()
    {
        for (int i = 0; i < onBattlleWonEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onBattlleWonEffects[i].Execute();
        }
    }
    public void Call_OnBlock()
    {
        StartCoroutine(Block());
    }
    private IEnumerator Block()
    {
        for (int i = 0; i < onBlockEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onBlockEffects[i].Execute();
        }
    }
    public void Call_OnAttack()
    {
        StartCoroutine(Attack());
    }
    private IEnumerator Attack()
    {
        for (int i = 0; i < onAttackEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onAttackEffects[i].Execute();
        }
    }
    public void Call_OnDirectAttack()
    {
        StartCoroutine(DirectAttackSucceeded());
    }
    private IEnumerator DirectAttackSucceeded()
    {
        for (int i = 0; i < onDirectAttackSucceedsEffects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            onDirectAttackSucceedsEffects[i].Execute();
        }
    }
}

