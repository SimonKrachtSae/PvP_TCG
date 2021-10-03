using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBomb : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private int amount;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float minVelocity;
    [SerializeField] private bool explode;
    private void Start()
    {
        Explode();
    }
    private void Update()
    {
        if (explode) { Explode(); explode = false; }

    }
    public void Explode()
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(particle, transform.position, Quaternion.identity);
            CustomParticle particleScript = obj.GetComponent<CustomParticle>();
            Vector3 direction = new Vector3((float)Random.Range(-1, 2), (float)Random.Range(-1, 2), 0);
            if (direction.x == 0 && direction.y == 0) direction = Vector3.up;
            direction.Normalize();
            particleScript.Initiate("10", Color.red, direction * Random.Range(minVelocity,maxVelocity));
        }
    }

}
