using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUIManager : MonoBehaviour
{
	[SerializeField] private GameObject deckCardsParent;
	[SerializeField] private List<CardLayout> cards;
	[SerializeField] private GameObject cardPreview;
	[SerializeField] private Transform parent;

	private void Start()
	{
		foreach(CardLayout layout in cards)
		{
			Instantiate(cardPreview, parent);
			cardPreview.GetComponent<CardInfo>().AssignCard(layout);
			cardPreview.gameObject.name = layout.NameTextUI.text;
		}
	}

	public void Spawn()
	{
		foreach(CardDragHandler dragHandler in deckCardsParent.GetComponentsInChildren<CardDragHandler>())
		{
			dragHandler.AssignToDeck();
		}
	}
}
