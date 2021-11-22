using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;
using Assets.Customs;
using TMPro;
// Inpsired by: Brackeys Save and Load
// Link: https://www.youtube.com/watch?v=XOjd_qU2Ido
public class Deck: MB_Singleton<Deck>
{
    //public static Deck Instance;
    private DeckData deckData;
    public DeckData DeckData { get => deckData; set => deckData = value; }
    public List<GameObject> Cards { get => cards; set => cards = value; }

    private List<GameObject> cards = new List<GameObject>();
	private int selectedDeckIndex = 0;
	[SerializeField] private TMP_Text deckCountText;
	protected new void Awake()
    {
		base.Awake();
		deckData = (DeckData)Resources.Load("DeckData");
		DontDestroyOnLoad(this.gameObject);
    }
	protected new void OnDestroy()
	{
		base.OnDestroy();
	}
    private void Start()
    {
    }
    public void Save()
	{
		deckData.CardNames = new List<string>();
		foreach (GameObject gameObject in Cards)
		{
			deckData.CardNames.Add(gameObject.name);
		}
		string path = Application.persistentDataPath + "/Deck"+selectedDeckIndex+".fun";
		if (File.Exists(path)) File.Delete(path);
		BinaryFormatter formatter = new BinaryFormatter();
		FileStream stream = new FileStream(path, FileMode.Create);
		formatter.Serialize(stream, deckData.CardNames);
		stream.Close();
	}
	public void LoadUI(int i)
    {
		ClearDeck();
		deckData.CardNames = new List<string>();
		selectedDeckIndex = i;
		string path = Application.persistentDataPath + "/Deck"+i+".fun";
		
		if (File.Exists(path))
        {
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);
			deckData.CardNames = new List<string>();
			deckData.CardNames = (List<string>)formatter.Deserialize(stream);
			stream.Close();
			RectTransform deckScrollField = DeckbuilderUI.Instance.deckScroll.gameObject.transform as RectTransform;
			
			if (MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>() == null || 
				MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>().gameObject.activeInHierarchy == false)
				return;

			foreach (string s in deckData.CardNames)
			{
				GameObject deckCard = Instantiate(MB_SingletonServiceLocator.Instance.GetSingleton<DeckUIManager>().CollectionCard, deckScrollField.GetChild(0));
				deckCard.GetComponent<CollectionCard>().Initiate(s);
			}
        }
	}
	public void Subscribe(GameObject gameObject)
	{
		if (!Cards.Contains(gameObject))
        {
			Cards.Add(gameObject);
			deckCountText.text = Cards.Count + "/20";
        }
	}

	public void Unsubscribe(GameObject gameObject)
	{
		if (Cards.Contains(gameObject))
		{
			Cards.Remove(gameObject);
			deckCountText.text = Cards.Count + "/20";
		}
	}
	public void ClearDeck()
	{
		for(int i = Cards.Count - 1; i >= 0; i--)
        {
			GameObject gameObject = Cards[i];
			Cards.RemoveAt(i);
			Destroy(gameObject);
        }
		Cards = new List<GameObject>();
		deckData.CardNames = new List<string>();
		deckCountText.text = Cards.Count + "/20";
	}
}
