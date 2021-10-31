using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ParticleManager : MonoBehaviourPun
{
    [SerializeField]private ParticleSystem summon;
    [SerializeField]private ParticleSystem destroy;
    [SerializeField]private ParticleSystem burn;
    [SerializeField]private ParticleSystem drag;
    [SerializeField]private ParticleSystem attack;
    [SerializeField]private ParticleSystem cardOverField;
    [SerializeField] private Vector3 offset;
    public void Call_Play(ParticleType type, Vector3 position, NetworkTarget target)
    {
        if (target == NetworkTarget.Local) Local_Play(type, position);
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_Play), RpcTarget.Others, type, position);
        else if (target == NetworkTarget.All)
        {
            Local_Play(type, position);
            photonView.RPC(nameof(RPC_Play), RpcTarget.Others, type, position);
        }
    }
    [PunRPC]
    public void RPC_Play(ParticleType type, Vector3 position)
    {
        Local_Play(type, new Vector3(position.x, -position.y, position.z));
    }
    public void Local_Play(ParticleType type, Vector3 position)
    {
        switch(type)
        {
            case ParticleType.Drag:
                drag.gameObject.transform.position = position + offset;
                //drag.Play();
                break;
            case ParticleType.Summon:
                summon.gameObject.transform.position = position + offset;
                if(!summon.isPlaying) summon.Play();
                break;
            case ParticleType.Burn:
                burn.gameObject.transform.position = position + offset;
                if (!burn.isPlaying) burn.Play();
                break;
            case ParticleType.CardOverField:
                cardOverField.gameObject.transform.position = position + offset;
                if (!cardOverField.isPlaying) cardOverField.Play();
                break;
        }
    }
    public void Local_Stop(ParticleType type)
    {
        switch (type)
        {
            case ParticleType.CardOverField:
                if (cardOverField.isPlaying) cardOverField.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                break;
        }
    }
}
public enum ParticleType
{
    Summon,
    Attack,
    Drag,
    Destroy,
    Burn,
    CardOverField
}
