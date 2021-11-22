using Assets.Customs;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckbuilderUI : MonoBehaviour
{
	public static DeckbuilderUI Instance;
    [SerializeField] private GameObject lobbyUI;
    [SerializeField] private GameObject detailedCardViewPanel;

    public GameObject deckScroll;
    public GameObject MonsterCollectionScroll;
    public GameObject MagicCollectionScroll;

	public GameObject CollectionCard { get => collectionCard; set => collectionCard = value; }
    public Transform TrashBin { get => trashBin; set => trashBin = value; }

    [SerializeField] private GameObject collectionCard;
	[SerializeField] private GameObject deckBuilderPanel;
	[SerializeField] private TMP_InputField searchText;
	private List<GameObject> outsourced = new List<GameObject>();
	[SerializeField] private GameObject continueButton;
	[SerializeField] private Transform trashBin;
	void Awake()
	{
		if (Instance != null) Destroy(this.gameObject);
		else { Instance = this; }
	}
    public void BackToLobbyUI()
	{
		continueButton.SetActive(true);
		lobbyUI.SetActive(true);
		deckBuilderPanel.SetActive(false);
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Save();
	}
	public void Continue()
    {
		int cardsInDeck = MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Cards.Count;
		if (cardsInDeck < 20)
		{
			MB_SingletonServiceLocator.Instance.GetSingleton<InfoText>().ShowInfoText("Current deck must contain a minimum of 20 cards! Currently: " + cardsInDeck, 1);
			return;
		}
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().Save();
		PhotonNetwork.JoinLobby();
	}
	public void GoToDeckbuilder()
	{
		NetworkUIManager.Instance.SetConnectionStatus(ConnectionStatus.DeckBuilder);
		continueButton.SetActive(false);
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
			card.SetParent(MonsterCollectionScroll.transform.GetChild(0));
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
	public void ShowMonsterPanel()
	{
		MonsterCollectionScroll.SetActive(true);
		MagicCollectionScroll.SetActive(false);
		SearchByName();
	}
	public void ShowMagicPanel()
	{
		MonsterCollectionScroll.SetActive(false);
		MagicCollectionScroll.SetActive(true);
		SearchByName();
	}
	public void SearchByName()
    {
		Debug.Log("sdfg");
		Transform collectionParent;

		if (MonsterCollectionScroll.activeSelf)
			collectionParent = MonsterCollectionScroll.transform.GetChild(0);
        else 
			collectionParent = MagicCollectionScroll.transform.GetChild(0);

		if (string.IsNullOrEmpty(searchText.text))
        {
			for (int i = 0; i < collectionParent.childCount; i++)
			{
					collectionParent.GetChild(i).gameObject.SetActive(true);
			}
			return;
        }

		for (int i = 0; i < collectionParent.childCount; i++)
        {
			string cardName = collectionParent.GetChild(i).gameObject.name.ToUpper();
			Debug.Log(cardName);
			if (cardName.StartsWith(searchText.text.ToUpper()))
			{
				collectionParent.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
				collectionParent.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
