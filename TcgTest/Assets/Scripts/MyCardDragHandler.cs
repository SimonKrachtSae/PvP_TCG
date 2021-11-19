using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyCardDragHandler : MonoBehaviour
{
    Transform prevTransform;
    public int Index { get; set; }
    private void OnMouseDown()
    {
        Index = transform.GetSiblingIndex();
        prevTransform = gameObject.transform.parent;
        transform.parent = MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>().gameObject.transform;
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x,pos.y,0);
    }
    private void OnMouseDrag()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, 0);
    }
    private void OnMouseUp()
    {
        RectTransform deckScrollField = DeckbuilderUI.Instance.deckScroll.gameObject.transform as RectTransform;
        if((deckScrollField.gameObject.transform.position - transform.position).magnitude < 200)
        {
            CardStat stats = GetComponent<CardStat>();
            int amount = 0;
            for(int i = 0; i < MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Cards.Count; i++)
            {
                if(MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Cards[i].name == stats.CardName)
                    amount++;
            }
            if(amount >= stats.MaxCount)
            {
                MB_SingletonServiceLocator.Instance.GetSingleton<InfoText>().ShowInfoText("Deck can't contatin more than " + stats.MaxCount + " copies of: " + stats.CardName,1);
                transform.parent = prevTransform;
                transform.SetSiblingIndex(Index);
                transform.localPosition = Vector3.zero;
                return;
            }
            GameObject deckCard = Instantiate(MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>().CollectionCard, deckScrollField.GetChild(0));
            deckCard.GetComponent<CollectionCard>().Initiate(this.gameObject.name);
        }

        transform.parent = prevTransform;
        transform.SetSiblingIndex(Index);
        transform.localPosition = Vector3.zero;
    }

}
