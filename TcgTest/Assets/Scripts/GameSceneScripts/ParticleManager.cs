using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class ParticleManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]private ParticleSystem summon;
    [SerializeField]private ParticleSystem destroy;
    [SerializeField]private ParticleSystem drag;
    [SerializeField]private ParticleSystem attack;
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
                drag.gameObject.transform.position = position;
                drag.Play();
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
public enum ParticleType
{
    Summon,
    Attack,
    Drag,
    Destroy
}
