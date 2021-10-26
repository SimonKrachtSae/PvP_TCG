using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTimeManager : MonoBehaviour
{
    [SerializeField] private float roundTime;
    public void StartRoundCountDown()
    {
        StartCoroutine(RoundCountDown());
    }
    private IEnumerator RoundCountDown()
    {
        float timeLeft = roundTime;
        while(timeLeft > 0)
        {
            yield return new WaitForFixedUpdate();
            timeLeft -= Time.fixedDeltaTime;
        }
        GameUIManager.Instance.EndTurn();
    }
}
