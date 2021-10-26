using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAssignAudio : MonoBehaviour
{
    [SerializeField] private AudioType audioType;
    [SerializeField] private Button button;
    private void Start()
    {
        button.onClick.AddListener(() => AudioManager.Instance.Local_PlaySound(audioType));
    }
}
