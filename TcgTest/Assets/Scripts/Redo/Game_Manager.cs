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

    private MainPhaseStates state;
    public MainPhaseStates State
    {
        get => state;
        set
        {
            state = value;
            Debug.Log(state.ToString());
            photonView.RPC(nameof(RPC_SetMainPhaseState), RpcTarget.All, value);
        } 
    }
    private Card blockingMonster;
    public int BlockingMonsterIndex 
    { 
        get => 0; 
        set => photonView.RPC(nameof(RPC_UpdateBlockingMonsterIndex), RpcTarget.Others, value);
    }
    public Card AttackingMonster { get; set; }

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        State = MainPhaseStates.StartPhase;
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
    [PunRPC]
    public void RPC_UpdateBlockingMonsterIndex(int index)
    {
        if (index == 6)
        {
            Player.DrawCard(0);
            if(photonView.IsMine) State = MainPhaseStates.AttackPhase;
            return;
        }
        blockingMonster = Enemy.Field[index];
        if (((MonsterCardStats)blockingMonster.CardStats).Defense < ((MonsterCardStats)AttackingMonster.CardStats).Attack)
        {
            ((MonsterCard)blockingMonster).SendToGraveyard();
        }
        else if (((MonsterCardStats)blockingMonster.CardStats).Defense > ((MonsterCardStats)AttackingMonster.CardStats).Attack)
        {
            ((MonsterCard)AttackingMonster).SendToGraveyard();
        }
        State = MainPhaseStates.AttackPhase;
    }
    public void StartTurn()
    {
        CurrentDuelist = DuelistType.Player;
        State = MainPhaseStates.StartPhase;
        Player.Mana = turn + Player.SummonPowerBoost;
        Player.DrawCard(0);
        for(int i = 0; i < Player.Field.Count; i++)
        {
            Player.Field[i].HasAttacked = false;
            Player.Field[i].HasBlocked = false;
        }
    }
    [PunRPC]
    public void RPC_SetMainPhaseState(MainPhaseStates value)
    {
        state = value;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
