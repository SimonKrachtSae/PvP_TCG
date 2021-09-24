using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Game_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Game_Manager Instance;
    public MyPlayer Player { get; set; }
    public MyPlayer Enemy { get; set; }


    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
