using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBomb : MonoBehaviour
{
    [SerializeField] private GameObject particle;
    [SerializeField] private int amount;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float minVelocity;
    public void Explode(string s, Color color)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject obj = Instantiate(particle, transform.position, Quaternion.identity);
            CustomParticle particleScript = obj.GetComponent<CustomParticle>();
            Vector3 direction = new Vector3((float)Random.Range(-1, 2), (float)Random.Range(-1, 2), 0);
            if (direction.x == 0 && direction.y == 0) direction = Vector3.up;
            direction.Normalize();
            particleScript.Initiate(s, color, direction * Random.Range(minVelocity,maxVelocity));
        }
    }

}
