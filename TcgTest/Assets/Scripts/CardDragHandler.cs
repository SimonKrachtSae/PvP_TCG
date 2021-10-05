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

	public void OnEndDrag(PointerEventData eventData)
	{
		if (!draggable) return;
		if (!inDeck)
		{
			deckScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().deckScroll.transform as RectTransform;
			collectionScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().collectionScroll.transform as RectTransform;

			if (RectTransformUtility.RectangleContainsScreenPoint(deckScrollField, Input.mousePosition))
			{
				GameObject addedCard = Instantiate(gameObject);
				addedCard.transform.SetParent(collectionScrollField.GetChild(0));
				addedCard.transform.localScale = Vector3.one;
				inDeck = true;
				collectionViewForm.SetActive(false);
				deckViewForm.SetActive(true);
				transform.SetParent(deckScrollField.GetChild(0));
			}
			else
			{
				collectionScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().collectionScroll.transform as RectTransform;
				transform.parent = previousParent.transform;
				transform.localPosition = Vector3.zero;
			}
		}
		else if(inDeck)
		{
			collectionScrollField = deckbuilderPanel.GetComponent<DeckbuilderUI>().collectionScroll.transform as RectTransform;

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
