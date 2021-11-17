using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckbuilderUI : MonoBehaviour
{
	public static DeckbuilderUI Instance;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject detailedCardViewPanel;

    public GameObject deckScroll;
    public GameObject collectionScroll;

	public GameObject CollectionCard { get => collectionCard; set => collectionCard = value; }

	[SerializeField] private GameObject collectionCard;
	[SerializeField] private GameObject deckBuilderPanel;
	void Awake()
	{
		if (Instance != null) Destroy(this.gameObject);
		else { Instance = this; }
	}
    public void BackToLobbyUI()
	{
		int cardsInDeck = MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Cards.Count;
		if (cardsInDeck < 20)
        {
			MB_SingletonServiceLocator.Instance.GetSingleton<InfoText>().ShowInfoText("Current deck must contain a minimum of 20 cards! Currently: " + cardsInDeck, 1);
			return;
        }
		lobbyUI.SetActive(true);
		deckBuilderPanel.SetActive(false);
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Save();
	}

	public void BackToDeckBuilderUI()
	{
		Transform card = detailedCardViewPanel.transform.GetChild(1);

		if (card.GetComponent<CardDragHandler>().inDeck)
		{
			card.SetParent(deckScroll.transform.GetChild(0));
			card.GetComponent<CardDragHandler>().draggable = true;
			card.GetComponent<CardDragHandler>().collectionViewForm.SetActive(false);
			card.GetComponent<CardDragHandler>().deckViewForm.SetActive(true);
			card.transform.localScale *= .5f;
			card.GetComponent<CardDragHandler>().deckViewForm.GetComponent<Button>().enabled = true;
		}
		else
		{
			card.SetParent(collectionScroll.transform.GetChild(0));
			card.GetComponent<CardDragHandler>().draggable = true;
			card.GetComponent<CardDragHandler>().deckViewForm.SetActive(false);
			card.GetComponent<CardDragHandler>().collectionViewForm.SetActive(true);
			card.transform.localScale *= .5f;
			card.GetComponent<CardDragHandler>().collectionViewForm.GetComponent<Button>().enabled = true;
		}

		detailedCardViewPanel.SetActive(false);
	}

	public void ShowDetailedCardView(GameObject cardViewForm)
	{
		if (!cardViewForm.transform.parent.GetComponent<CardDragHandler>().canShowDetailedCardView) return;

		cardViewForm.transform.parent.GetComponent<CardDragHandler>().draggable = false;
		cardViewForm.transform.parent.GetComponent<CardDragHandler>().deckViewForm.SetActive(false);
		cardViewForm.transform.parent.GetComponent<CardDragHandler>().collectionViewForm.SetActive(true);
		detailedCardViewPanel.SetActive(true);
		cardViewForm.transform.parent.SetParent(detailedCardViewPanel.transform);
		cardViewForm.transform.parent.localPosition = Vector3.zero;
		cardViewForm.transform.parent.localScale *= 2;
		cardViewForm.GetComponent<Button>().enabled = false;
	}
}
