using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class GameUIManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameUIManager Instance;

    [SerializeField] private GameObject CoinFlipCanvas;
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private TMP_Text winText;
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
        GameOverCanvas.SetActive(false);
        switch(state)
        {
            case GameState.CoinFlip:
                CoinFlipCanvas.SetActive(true);
                break;
            case GameState.Running:
                BoardCanvas.SetActive(true);
                break;
            case GameState.GameOver:
                GameOverCanvas.SetActive(true);
                foreach (GameObject gameObject in Game_Manager.Instance.Player.StartingDeck) PhotonNetwork.Destroy(gameObject);
                if (Game_Manager.Instance.Player.Deck.Count == 0) winText.text = "You Win!!!";
                else winText.text = "You Lose...";
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
        SetGameState(GameState.Running);

        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            Game_Manager.Instance.CurrentDuelist = DuelistType.Player;
            Game_Manager.Instance.Player.Mana = Game_Manager.Instance.Turn;
        }

        Game_Manager.Instance.StartGame();
    }
    public void EndTurn()
    {
        Game_Manager.Instance.CurrentDuelist = DuelistType.Enemy;
        GameUIManager.Instance.EndTurnButton.gameObject.SetActive(false);
        Game_Manager.Instance.Round++;
        photonView.RPC(nameof(RPC_EndTurn), RpcTarget.Others);
    }
    [PunRPC]
    public void RPC_EndTurn()
    {
        GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        Game_Manager.Instance.StartTurn();
    }
    public void Summon()
    {
        Game_Manager.Instance.State = MainPhaseStates.Summoning;
    }
    public void StartAttackPhase()
    {
        Game_Manager.Instance.State = MainPhaseStates.AttackPhase;
    }
    public void Block()
    {
        Game_Manager.Instance.State = MainPhaseStates.Blocking;
        Board.Instance.BlockRequest.SetActive(false);
        Board.Instance.PlayerInfoText.text = "Select Blocking Monster";  
    }
    public void DontBlock()
    {
        Game_Manager.Instance.BlockingMonsterIndex = 6;
        Board.Instance.BlockRequest.SetActive(false);
    }
    public void MainMenuPressed()
    {
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }
    public void QuitPressed()
    {
        Application.Quit();
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
