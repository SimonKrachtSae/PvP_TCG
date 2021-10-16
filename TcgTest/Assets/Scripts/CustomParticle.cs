using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CustomParticle : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float minLifeSpan;
    [SerializeField] private float maxLifeSpan;
    private float lifeSpan;
    private Vector3 direction;

    public TMP_Text Text { get => text; set => text = value; }

    public void Initiate(string _text, Color _color, Vector3 _direction)
    {
        Text.text = _text;
        Text.color = _color;
        direction = _direction;
        StartCoroutine(SelfDestruct());
    }
    private IEnumerator SelfDestruct()
    {
        lifeSpan = Random.Range(minLifeSpan, maxLifeSpan);
        while (lifeSpan > 0)
        {
            yield return new WaitForFixedUpdate();
            lifeSpan -= Time.fixedDeltaTime;
            transform.position += direction;
        }
        Destroy(this.gameObject);
    }
}
