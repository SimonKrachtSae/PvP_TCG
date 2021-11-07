using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{


    private float waitToLoad = 3f;
   
    void Start()
    {
        StartCoroutine(Wait());
    }

   IEnumerator Wait()
    {
        yield return new WaitForSeconds(waitToLoad);
        SceneManager.LoadScene(1);

    }

}
