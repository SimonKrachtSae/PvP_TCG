using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class GameUIManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameUIManager Instance;

    [SerializeField] private GameObject CoinFlipCanvas;
    [SerializeField] private Button HeadsButton;
    [SerializeField] private Button TailsButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endTurnButton;
    public Button EndTurnButton { get => endTurnButton; set => endTurnButton = value; }
    public Button StartButton { get => startButton; set => startButton = value; }

    [SerializeField] private GameObject BoardCanvas;

    [SerializeField] private GameObject monsterCardLayoutPrefab;
    public GameObject MonsterCardLayoutPrefab { get => monsterCardLayoutPrefab; set => monsterCardLayoutPrefab = value; }

    [SerializeField] private GameObject effectCardLayoutPrefab;
    public GameObject EffectCardLayoutPrefab { get => effectCardLayoutPrefab; set => effectCardLayoutPrefab = value; }
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    private void Start()
    {
        SetGameState(GameState.CoinFlip);
        HeadsButton.onClick.AddListener(() => { OnHeadsClicked(); });
        TailsButton.onClick.AddListener(() => { OnTailsClicked(); });
        StartButton.onClick.AddListener(() => { StartGame(); });
        EndTurnButton.onClick.AddListener(() => { EndTurn(); });
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            HeadsButton.gameObject.SetActive(true);
            TailsButton.gameObject.SetActive(true);
        }
    }
    public void SetGameState(GameState state)
    {
        CoinFlipCanvas.SetActive(false);
        BoardCanvas.SetActive(false);
        switch(state)
        {
            case GameState.CoinFlip:
                CoinFlipCanvas.SetActive(true);
                break;
            case GameState.Running:
                BoardCanvas.SetActive(true);
                break;
        }
    }
    public void OnHeadsClicked()
    {
        Coin.Instance.SelectedState = CoinState.Heads;
        Coin.Instance.TossCoin = true;
        HeadsButton.gameObject.SetActive(false);
        TailsButton.gameObject.SetActive(false);
    }
    public void OnTailsClicked()
    {
        Coin.Instance.TossCoin = true;
        Coin.Instance.SelectedState = CoinState.Tails;
        HeadsButton.gameObject.SetActive(false);
        TailsButton.gameObject.SetActive(false);
    }
    public void StartGame()
    {
        photonView.RPC(nameof(RPC_StartGame), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_StartGame()
    {
        GameUIManager.Instance.SetGameState(GameState.Running);
        GameManager.Instance.StartGame();
    }
    public void EndTurn()
    {
        photonView.RPC(nameof(RPC_EndTurn), RpcTarget.All);
    }
    [PunRPC]
    public void RPC_EndTurn()
    {
        GameManager.Instance.EndTurn();
    }
    public void Summon()
    {
        GameManager.Instance.MainPhaseStates = MainPhaseStates.Summoning;
    }
    public void StartAttackPhase()
    {
        GameManager.Instance.MainPhaseStates = MainPhaseStates.AttackPhase;
    }
    public void Block()
    {
        GameManager.Instance.MainPhaseStates = MainPhaseStates.Blocking;
        Board.Instance.BlockRequest.SetActive(false);
        Board.Instance.PlayerInfoText.text = "Select Blocking Monster";  
    }
    public void DontBlock()
    {
        GameManager.Instance.Enemy.BlockingMonsterIndex = 6;
        Board.Instance.BlockRequest.SetActive(false);
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
