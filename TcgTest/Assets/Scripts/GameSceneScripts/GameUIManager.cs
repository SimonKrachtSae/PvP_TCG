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

    [SerializeField] private Arrow arrow;
    [SerializeField] private GameObject CoinFlipCanvas;
    [SerializeField] private GameObject GameOverCanvas;
    [SerializeField] private GameObject PauseMenu;
    [SerializeField] private TMP_Text winText;
    [SerializeField] private Button HeadsButton;
    [SerializeField] private Button TailsButton;
    [SerializeField] private Button startButton;
    [SerializeField] private Button endTurnButton;
    public Button EndTurnButton { get => endTurnButton; set => endTurnButton = value; }
    public Button StartButton { get => startButton; set => startButton = value; }

    [SerializeField] private GameObject boardCanvas;
    [SerializeField] private CardInfo cardInfo;
    [SerializeField] private GameObject attackButton;
    public GameObject AttackButton { get => attackButton; set => attackButton = value; }
    
    public CardInfo CardInfo { get => cardInfo; }
    public TMP_Text NameText;
    public GameState State { get; set; }
    public GameObject BoardCanvas { get => boardCanvas; set => boardCanvas = value; }
    public Arrow Arrow { get => arrow; set => arrow = value; }
    [SerializeField] private TMP_Text roundTime;
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    private void Start()
    {
        SetGameState(GameState.CoinFlip);
        EndTurnButton.onClick.AddListener(() => { EndTurn(); });
        StartCoroutine(NameSelector());
    }
    public IEnumerator NameSelector()
    {
        float timer = 5;
        string name1 = PhotonNetwork.PlayerList[0].NickName;
        string name2 = PhotonNetwork.PlayerList[1].NickName;
        while (timer > 0)
        {
            yield return new WaitForSeconds(0.25f);
            if (NameText.text == name1) NameText.text = name2;
            else NameText.text = name1;
            timer -= 0.25f;
        }
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            StartGame();
            //Debug.Log("cheeeeseee");
            //photonView.RPC(nameof(RPC_SetNameText), RpcTarget.All, NameText.text);
        }
    }
    [PunRPC]
    public void RPC_SetNameText(string text)
    {
        NameText.text = text;
        StartCoroutine(LagTime());
    }
    public IEnumerator LagTime()
    {
        yield return new WaitForSeconds(2);
        if(NameText.text == PhotonNetwork.LocalPlayer.NickName)
        {
            StartGame();
        }
    }
    public void SetGameState(GameState state)
    {
        State = state;
        CoinFlipCanvas.SetActive(false);
        BoardCanvas.SetActive(false);
        GameOverCanvas.SetActive(false);
        PauseMenu.SetActive(false);
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
            case GameState.Paused:
                PauseMenu.SetActive(true);
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
        Game_Manager.Instance.CurrentDuelist = DuelistType.Player;
        Game_Manager.Instance.Player.Mana = Game_Manager.Instance.Turn;
        EndTurnButton.gameObject.SetActive(true);
        Game_Manager.Instance.StartTurn();
    }
    [PunRPC]
    public void RPC_StartGame()
    {
        SetGameState(GameState.Running);
        Game_Manager.Instance.Player.Call_DrawCards(5);
    }
    public void EndTurn()
    {
        int amount = Game_Manager.Instance.Player.Hand.Count;

        Game_Manager.Instance.SetMainPhaseState(GameManagerStates.AttackPhase);
       
        if (amount > 7)
        {
            Game_Manager.Instance.Player.Call_AddDiscardEffects(amount - 7, NetworkTarget.Local);
            return;
        }
        if (Game_Manager.Instance.State == GameManagerStates.Discarding || Game_Manager.Instance.State == GameManagerStates.Destroying)
        {
            Debug.Log(Game_Manager.Instance.State.ToString()); 
            return; 
        }

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
        int counter = 0;
        foreach(MonsterCard c in Game_Manager.Instance.Player.Field)
        {
            if(!c.HasBlocked)
            {
                counter++;
            }
        }
        if(counter == 0)
        {
            Board.Instance.PlayerInfoText.text = "No Monsters left for blocking!";
            return;
        }
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
