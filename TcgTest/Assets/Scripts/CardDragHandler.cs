using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
	public GameObject deckViewForm;
	public GameObject collectionViewForm;

	public bool draggable = true;
	public bool canShowDetailedCardView = true;
	public bool inDeck = false;

	private RectTransform deckScrollField;
	private RectTransform collectionScrollField;
	private GameObject previousParent;
	private GameObject deckbuilderPanel;
    private void Start()
    {
		deckbuilderPanel = NetworkUIManager.Instance.deckBuilderUI;
		collectionScrollField = DeckbuilderUI.Instance.MonsterCollectionScroll.gameObject.transform as RectTransform;
		deckScrollField = DeckbuilderUI.Instance.deckScroll.gameObject.transform as RectTransform;
	}
	public void StartCard()
	{
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Subscribe(this.gameObject);
	}

	private void OnDestroy()
	{
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Unsubscribe(this.gameObject);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (!draggable) return;
		canShowDetailedCardView = false;
		previousParent = transform.parent.gameObject;
		transform.parent = deckbuilderPanel.transform;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!draggable) return;
		transform.position = Input.mousePosition;
	}

	public void DuplicateCard()
	{
		GameObject deckCard = Instantiate(gameObject);
		deckCard.name = this.name;
		deckCard.transform.SetParent(deckScrollField.GetChild(0));
		deckCard.transform.localScale = Vector3.one;
		deckCard.GetComponent<CardDragHandler>().StartCard();
		deckCard.GetComponent<CardDragHandler>().inDeck = true;
		deckCard.GetComponent<CardDragHandler>().collectionViewForm.SetActive(false);
		deckCard.GetComponent<CardDragHandler>().deckViewForm.SetActive(true);
		deckCard.GetComponent<CardDragHandler>().deckViewForm.GetComponentInChildren<UnityEngine.UI.Text>().text = name;
		transform.SetParent(collectionScrollField.GetChild(0));
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!draggable) return;
		if (!inDeck)
		{
			deckScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().deckScroll.transform as RectTransform;

			if (RectTransformUtility.RectangleContainsScreenPoint(deckScrollField, Input.mousePosition))
			{
				DuplicateCard();
			}
			else
			{
				collectionScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().MonsterCollectionScroll.transform as RectTransform;
				transform.parent = previousParent.transform;
				transform.localPosition = Vector3.zero;
			}
		}
		else if(inDeck)
		{
			collectionScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().MonsterCollectionScroll.transform as RectTransform;

			if(RectTransformUtility.RectangleContainsScreenPoint(collectionScrollField, Input.mousePosition))
			{
				Destroy(gameObject);
			}
			else
			{
				deckScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().deckScroll.transform as RectTransform;
				transform.parent = previousParent.transform;
				transform.localPosition = Vector3.zero;
			}
		}

		canShowDetailedCardView = true;
	}
}
