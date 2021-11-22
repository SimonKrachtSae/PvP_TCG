using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static AudioManager Instance;
    [SerializeField] private AudioSource drawSound;
    [SerializeField] private AudioSource summonSound;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource gameMusic;

    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
        this.transform.position = Camera.main.transform.position;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.transform.position = Camera.main.transform.position;
    }
    public void Call_PlaySound(AudioType audioType, NetworkTarget target)
    {
        if (target == NetworkTarget.Local) Local_PlaySound(audioType);
        else if (target == NetworkTarget.Other) photonView.RPC(nameof(RPC_PlaySound), RpcTarget.Others, audioType);
        else if (target == NetworkTarget.All) photonView.RPC(nameof(RPC_PlaySound), RpcTarget.All, audioType);
    }
    [PunRPC]
    public void RPC_PlaySound(AudioType audioType)
    {
        Local_PlaySound(audioType);
    }
    public void Local_PlaySound(AudioType audioType)
    {
        switch (audioType)
        {
            case AudioType.Draw:
                drawSound.volume = 1;
                drawSound.Play();
                break;
            case AudioType.Summon:
                summonSound.Play();
                break;
            case AudioType.MenuMusic:
                menuMusic.Play();
                break;
             case AudioType.GameMusic:
                gameMusic.Play();
                break;
            case AudioType.Click:
                clickSound.Play();
                break;
        }
       // Scene currentScene = SceneManager.GetActiveScene();
       // string sceneName = currentScene.name;
       // if (sceneName == "GameScene")
       // {
          //  gameMusic.Play();
       // }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}
public enum AudioType
{
    Draw,
    Summon,
    Click,
    MenuMusic,
    GameMusic
}
