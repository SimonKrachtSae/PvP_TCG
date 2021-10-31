using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckUIManager : MonoBehaviour
{
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
		CardNamesData cardNames = (CardNamesData)Resources.Load("CardNames");
		foreach(string s in cardNames.CardNames)
		{
			GameObject card = Instantiate(cardPreview, parent);
			card.GetComponent<CardInfo>().AssignCard(((GameObject)Resources.Load(s)).GetComponent<CardLayout>());
			card.gameObject.name = s;
			spawnedSelectableCards.Add(card);
		}
	}
    public void SpawnCardOnLoad(string cardName)
	{
		foreach (GameObject card in spawnedSelectableCards)
		{
			if(card.name == cardName)
            {
				card.GetComponent<CardDragHandler>().DuplicateCard();
				return;
            }
		}
	}
}
