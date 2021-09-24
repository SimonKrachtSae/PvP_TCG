using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class Game_Manager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static Game_Manager Instance;
    public MyPlayer Player { get; set; }
    public MyPlayer Enemy { get; set; }
    private int round = 0;
    private int turn = 1;
    public int Round { get => round; set => photonView.RPC(nameof(RPC_UpdateRound), RpcTarget.All, value); }
    private DuelistType currentDuelist;
    public DuelistType CurrentDuelist 
    {
        get => currentDuelist;
        set
        {
            currentDuelist = value;
            photonView.RPC(nameof(RPC_UpdateCurrentDuelist), RpcTarget.Others, value);
        }
    }

    public MainPhaseStates state;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
        state = MainPhaseStates.StandardView;
    }
    public void Start()
    {
        PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
    }
    public void StartGame()
    {
        StartCoroutine(DrawHandCards());
        if(currentDuelist == DuelistType.Player)
        {
            GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        }
    }
    private IEnumerator DrawHandCards()
    {
        for(int i = 0; i < 5; i++)
        {
            Player.DrawCard(0);
            yield return new WaitForSecondsRealtime(1);
        }
    }
    [PunRPC]
    public void RPC_UpdateCurrentDuelist(DuelistType type)
    {
        if (type == DuelistType.Player) currentDuelist = DuelistType.Enemy;
        else if (type == DuelistType.Enemy) currentDuelist = DuelistType.Player;

    }
    [PunRPC]
    public void RPC_UpdateRound(int value)
    {
        round = value;
        if (round == 2)
        {
            turn++;
            round = 0;
            Board.Instance.TurnCount.text = turn.ToString();
        }
    }
    public void StartTurn()
    {
        Player.Mana = turn;
        Player.DrawCard(0);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
