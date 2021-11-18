using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectionCard : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text text;
    public void Initiate(string text)
    {
        this.text.text = text;
        this.name = text;
    }
    public void OnMouseUp()
    {
        if ((transform.position - DeckbuilderUI.Instance.deckScroll.gameObject.transform.GetChild(0).transform.position).magnitude > 200)
            Destroy(this.gameObject);
    }
    public void Start()
    {
        MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Subscribe(this.gameObject);
    }
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(1)) MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>().CardInfo.AssignCard(text.text,4);
    }

    private void OnDestroy()
    {
        MB_SingletonServiceLocator.Instance?.GetSingleton<Deck>().Unsubscribe(this.gameObject);
    }
}
