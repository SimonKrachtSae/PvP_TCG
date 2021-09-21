using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[CreateAssetMenu(fileName = "SummonPowerBoostChangeEffect", menuName = "ScriptableObjects/Effects/SummonPowerBoostChangeEffect", order = 1)]
public class SummonPowerBoostChangeEffect : Effect
{
    [SerializeField] private DuelistType duelist;
    [SerializeField] private int amount;

    public DuelistType Duelist { get => duelist; set => duelist = value; }
    public int Amount { get => amount; set => amount = value; }
    public override void Execute()
    {
        Debug.Log("Invoke!");
        GameManager.Instance.LocalDuelist.UpdateSummonPowerBoost(Amount);
        //else if (duelist == DuelistType.Enemy) GameManager.Instance.Enemy.SummonPowerBoost += amount;
        Debug.Log(amount.ToString() + "/" + GameManager.Instance.LocalDuelist.SummonPowerBoost.ToString() +" Name: " + PhotonNetwork.LocalPlayer.NickName);
    }
}
