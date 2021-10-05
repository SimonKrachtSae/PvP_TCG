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
    [SerializeField] private List<CardName> cardNames;
    private DeckData deckData;
    public DeckData DeckData { get => deckData; set => deckData = value; }
    private void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Deck.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        DeckData data = new DeckData(cardNames);
		data.CardNames = cardNames;
        formatter.Serialize(stream, data);
        stream.Close();
    }
    public void Load()
    {
        string path = Application.persistentDataPath + "/Deck.fun";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            deckData = formatter.Deserialize(stream) as DeckData;
            stream.Close();
			//foreach(CardName cardName in deckData.CardNames)
			//{
			//	Instantiate(cardPreview, parent);
			//	cardPreview.GetComponent<CardInfo>().AssignCard(layout);
			//	cardPreview.gameObject.name = layout.NameTextUI.text;
			//}
        }
        else
        {
            Debug.Log("File not found! \n Path: " + path);
			deckData = new DeckData(cardNames);
			deckData.CardNames = new List<CardName>();
        }
    }

	public void Subscribe(string s)
	{
		if (deckData == null)
		{
			deckData = new DeckData(cardNames);
			deckData.CardNames = new List<CardName>();
		}

		CardName cardName = GetCardName(s);
		deckData.CardNames.Add(cardName);
		Deck.Instance.Save();
		Debug.Log(cardName);
		Debug.Log(deckData.CardNames.Count);
	}

	public void Unsubscribe(string s)
	{
		CardName cardName = GetCardName(s);
		deckData.CardNames.Remove(cardName);
		Deck.Instance.Save();
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
