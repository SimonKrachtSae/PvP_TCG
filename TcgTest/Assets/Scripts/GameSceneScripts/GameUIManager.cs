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
    [SerializeField] private CardInfo cardInfo;
    [SerializeField] private GameObject attackButton;
    public GameObject AttackButton { get => attackButton; set => attackButton = value; }
    
    public CardInfo CardInfo { get => cardInfo; }

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
                if (Game_Manager.Instance.Player.DeckList.Count == 1) winText.text = "You Win!!!";
                else winText.text = "You Lose...";
                foreach (GameObject gameObject in Game_Manager.Instance.Player.gameObject.GetComponentsInChildren<GameObject>()) PhotonNetwork.Destroy(gameObject);
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

        Game_Manager.Instance.StartGame();
    }
    public void EndTurn()
    {
        int amount = Game_Manager.Instance.Player.Hand.Count;
        if (amount > 7)
        {
            Game_Manager.Instance.Player.Call_AddDiscardEffects(amount - 5, NetworkTarget.Local);
            return;
        }
        if (Game_Manager.Instance.State == GameManagerStates.Discarding || Game_Manager.Instance.State == GameManagerStates.Destroying)
            { Debug.Log(Game_Manager.Instance.State.ToString()); return; }

        AttackButton.SetActive(false);

        Game_Manager.Instance.CurrentDuelist = DuelistType.Enemy;
        GameUIManager.Instance.EndTurnButton.gameObject.SetActive(false);
        Game_Manager.Instance.Round++;
        Game_Manager.Instance.Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.Busy);
        photonView.RPC(nameof(RPC_EndTurn), RpcTarget.Others);
    }
    [PunRPC]
    public void RPC_EndTurn()
    {
        GameUIManager.Instance.EndTurnButton.gameObject.SetActive(true);
        Game_Manager.Instance.StartTurn();
    }
    public void StartAttackPhase()
    {
        Game_Manager.Instance.Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.AttackPhase);
    }
    public void Block()
    {
        Game_Manager.Instance.Call_SetMainPhaseState(NetworkTarget.Local, GameManagerStates.Blocking);
        Game_Manager.Instance.Call_SetMainPhaseState(NetworkTarget.Other, GameManagerStates.Busy);
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
