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
		foreach(GameObject gameObject in cards)
		{
			CardName cardName;
			System.Enum.TryParse(gameObject.name, out cardName);
			names.Add(cardName);
		}
        string path = Application.persistentDataPath + "/Deck.fun";
        if (File.Exists(path)) File.Delete(path);
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
            try 
			{
				BinaryFormatter formatter = new BinaryFormatter();
				FileStream stream = new FileStream(path, FileMode.Open);
				deckData = formatter.Deserialize(stream) as DeckData;
				stream.Close();
			}
            catch (IOException)
            {
				Debug.Log("Caught Exeption");
				StartCoroutine(WaitBeforeRetryLoad());
            }
        }
        else
        {
            Debug.Log("File not found! \n Path: " + path);
        }
    }
	private IEnumerator WaitBeforeRetryLoad()
    {
		yield return new WaitForSecondsRealtime(1.5f);
		LoadData();
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
		//cardName = GetCardName(gameObject.name);
		//System.Enum.TryParse(gameObject.name, out cardName);
		//names.Add(cardName);
		if(!cards.Contains(gameObject))
			cards.Add(gameObject);
	}

	public void Unsubscribe(GameObject gameObject)
	{
		if(cards.Contains(gameObject)) cards.Remove(gameObject);
	}
}
