using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EffectCardEffect : MonoBehaviour
{
    [SerializeField] private List<Effect> effects;
    public void Call_OnPlay()
    {
        StartCoroutine(Play());
    }
    private IEnumerator Play()
    {
        for (int i = 0; i < effects.Count; i++)
        {
            while (Game_Manager.Instance.ExecutingEffects)
            {
                yield return new WaitForFixedUpdate();
            }
            effects[i].Execute();
        }
    }
}
