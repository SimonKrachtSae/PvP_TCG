using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterParticleSystem : MonoBehaviour
{
    [SerializeField] private float frequency;
    [SerializeField] private float duration;
    [SerializeField]private bool play;
    [SerializeField] private GameObject particle;
    private void Update()
    {
        if (play) { Play(); play = false; }

    }
    public void Play()
    {
        StartCoroutine(SpawnParticles());
    }
    private void Start()
    {
        StartCoroutine(SpawnParticles());
    }
    private IEnumerator SpawnParticles()
    {
        float timePassed = duration;
        while (timePassed > 0)
        {
            yield return new WaitForFixedUpdate();
            timePassed -= Time.fixedDeltaTime;
            GameObject newParticle = Instantiate(particle, transform.position, Quaternion.identity);
            CustomParticle particleScript = newParticle.GetComponent<CustomParticle>();
            particleScript.Initiate("10", Color.red,new Vector3((float)Random.Range(-0.5f,0.5f), (float)Random.Range(-0.5f, 0.5f),0));
        }
    }
}
