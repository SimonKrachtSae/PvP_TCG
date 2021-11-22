using Assets.Customs;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;

public class DeckUIManager : MB_Singleton<DeckUIManager>
{
	[SerializeField] private CardInfo cardInfo;
    public CardInfo CardInfo { get => cardInfo; set => cardInfo = value; }
    public GameObject CollectionCard { get => collectionCard; set => collectionCard = value; }

	[SerializeField] private GameObject collectionCard;
    protected new void Awake()
    {
    	base.Awake();
    }
    protected new void OnDestroy()
    {
    	base.OnDestroy();
    }
    void Start()
	{
		CardNamesData cardNames = (CardNamesData)Resources.Load("CardNames");
		for (int i = 0; i < cardNames.CardNames.Count; i++)
		{
			string s = cardNames.CardNames[i];
			GameObject toLoad = (GameObject)Resources.Load(s);
			var card = toLoad.GetComponent<Card>();
			GameObject deckCard;
			if(card.GetType() == typeof(EffectCard))
				deckCard = (GameObject)Instantiate(Resources.Load(s), DeckbuilderUI.Instance.MagicCollectionScroll.transform.GetChild(0));
			else 
				deckCard = (GameObject)Instantiate(Resources.Load(s), DeckbuilderUI.Instance.MonsterCollectionScroll.transform.GetChild(0));
			deckCard.name = s;
			deckCard.transform.localScale = new Vector3(4, 4, 2);
			deckCard.AddComponent(typeof(MyCardDragHandler));
			deckCard.GetComponent<MyCardDragHandler>().Index = i;
		}
		MB_SingletonServiceLocator.Instance.GetSingleton<Deck>().LoadUI(0);
	}
}
