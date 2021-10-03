using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CustomParticle : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private float lifeTime;
    private Vector3 direction;

    public void Initiate(string _text, Color _color, float _lifeTime, Vector3 _direction)
    {
        text.text = _text;
        text.color = _color;
        lifeTime = _lifeTime;
        direction = _direction;
        StartCoroutine(SelfDestruct());
    }
    private IEnumerator SelfDestruct()
    {
        float timePassed = lifeTime;
        while (timePassed > 0)
        {
            yield return new WaitForFixedUpdate();
            timePassed -= Time.fixedDeltaTime;
            transform.position += direction;
        }
        Destroy(this.gameObject);
    }
}
