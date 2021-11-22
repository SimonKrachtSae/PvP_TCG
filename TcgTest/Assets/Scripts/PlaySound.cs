using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    int i = 0;
    private void OnEnable()
    {
        if(i>= 1)
            audioSource.Play();
        i++;
    }    
}
