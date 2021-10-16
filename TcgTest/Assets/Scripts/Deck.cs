using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

// Inpsired by: Brackeys Save and Load
// Link: https://www.youtube.com/watch?v=XOjd_qU2Ido
public class Deck: MonoBehaviour
{
    public static Deck Instance;
    [SerializeField] private List<GameObject> cards;
    private DeckData deckData;
    public DeckData DeckData { get => deckData; set => deckData = value; }
	List<CardName> names;

	void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
		cards = new List<GameObject>();
		names = new List<CardName>();
    }
    public void Save()
    {
        string path = Application.persistentDataPath + "/Deck.fun";
        if (File.Exists(path)) return;
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);
		DeckData data = new DeckData(names);
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public void LoadData()
    {
        string path = Application.persistentDataPath + "/Deck.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            deckData = formatter.Deserialize(stream) as DeckData;
            stream.Close();
        }
        else
        {
            Debug.Log("File not found! \n Path: " + path);
        }
    }
	public void LoadUI()
    {
		string path = Application.persistentDataPath + "/Deck.fun";

		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);
			deckData = formatter.Deserialize(stream) as DeckData;
			stream.Close();
			foreach (CardName cardName in deckData.CardNames)
			{
				DeckUIManager.Instance.SpawnCardOnLoad(cardName);
			}
		}
	}
	public void Subscribe(GameObject gameObject)
	{
		if (!cards.Contains(gameObject)) cards.Add(gameObject);
		Debug.Log(cards.Count);
		CardName cardName;
		//cardName = GetCardName(gameObject.name);
		System.Enum.TryParse(gameObject.name, out cardName);
		names.Add(cardName);
	}

	public void Unsubscribe(GameObject gameObject)
	{
		if(cards.Contains(gameObject)) cards.Remove(gameObject);
	}

	private CardName GetCardName(string s)
	{
		CardName cardName;
		switch (s)
		{
			case "Cheese":
				cardName = CardName.Cheese;
				break;

			case "Tomato":
				cardName = CardName.Tomato;
				break;

			case "Recall":
				cardName = CardName.Recall;
				break;

			case "Destroy":
				cardName = CardName.Destroy;
				break;

			case "Discard":
				cardName = CardName.Discard;
				break;

			default:
				cardName = CardName.Cheese;
				break;
		}

		return cardName;
	}
}
