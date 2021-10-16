using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUIManager : MonoBehaviour
{
	[SerializeField] private GameObject deckCardsParent;
	[SerializeField] private List<CardLayout> cards;
	[SerializeField] private GameObject cardPreview;
	[SerializeField] private Transform parent;
	private List<GameObject> spawnedSelectableCards;
	public static DeckUIManager Instance;
	private CardName copyCardName;
    private void Awake()
    {
		if (Instance != null) Destroy(this.gameObject);
		else { Instance = this; }
	}
    private void Start()
	{
		spawnedSelectableCards = new List<GameObject>();
		foreach(CardLayout layout in cards)
		{
			GameObject card = Instantiate(cardPreview, parent);
			card.GetComponent<CardInfo>().AssignCard(layout);
			card.gameObject.name = layout.NameTextUI.text;
			spawnedSelectableCards.Add(card);
		}
	}
    public void SpawnCardOnLoad(CardName cardName)
	{
		foreach (GameObject card in spawnedSelectableCards)
		{
			if(card.name == cardName.ToString())
            {
				card.GetComponent<CardDragHandler>().DuplicateCard();
				return;
            }
		}
	}
}
