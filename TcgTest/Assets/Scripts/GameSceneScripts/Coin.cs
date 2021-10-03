using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Coin : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Coin Instance;
    private bool tossCoin = false;
    public bool TossCoin 
    { 
        get => tossCoin;
        set { if (!tossCoin && value == true) tossCoin = true; } 
    }
    private bool coinIsSpinning = false;

    private CoinState selectedState;
    public CoinState SelectedState 
    { 
        get => selectedState;
        set
        {
            selectedState = value;

        }
    }
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    private void Update()
    {
        if (TossCoin && !coinIsSpinning)
        {
            coinIsSpinning = true;
            StartCoroutine(CoinToss());
        }
        else if(TossCoin && coinIsSpinning)
        {
            transform.Rotate(Vector3.up * Time.fixedDeltaTime * 300);
        }
    }
    private IEnumerator CoinToss()
    { 
        yield return new WaitForSecondsRealtime(3f);
        tossCoin = false;
        coinIsSpinning = false;
        Stop();
    }
    public void Stop()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            ClientType clientType = ClientType.Host;
            int i = (int)Random.Range(0, 2);
            if(i == 0)
            {
                transform.rotation = new Quaternion(0, 0, 0, 0);
                if (SelectedState != CoinState.Heads) clientType = ClientType.Client;
            }
            else
            {
                transform.rotation = new Quaternion(0, 0.5f, 0, 0);
                if (SelectedState != CoinState.Tails) clientType = ClientType.Client;
            }
            photonView.RPC(nameof(RPC_CoinStopped), RpcTarget.All, clientType);
        }
    }
    [PunRPC]
    public void RPC_CoinStopped(ClientType type)
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient) 
            GameUIManager.Instance.StartButton.gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
