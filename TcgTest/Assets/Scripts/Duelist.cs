using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Duelist : MonoBehaviour
{
    private int summonPower;
    public int SummonPower 
    { 
        get => summonPower;
        set
        {
            summonPower = value;
            UIDesriptions.Instance.PlayerSummonPowerText.text = summonPower.ToString();
        }
    }
    public PlayerType type;
}
