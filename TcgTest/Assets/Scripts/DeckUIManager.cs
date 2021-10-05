using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUIManager : MonoBehaviour
{
	[SerializeField] private GameObject deckbuilderPanel;
	[SerializeField] private List<CardLayout> cards;
	[SerializeField] private GameObject cardPreview;
	[SerializeField] private Transform parent;

	private void Start()
	{
		foreach(CardLayout layout in cards)
		{
			Instantiate(cardPreview, parent);
			cardPreview.GetComponent<CardInfo>().AssignCard(layout);
		}
	}
}
