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
    [SerializeField]private DeckData deckData;
    public DeckData DeckData { get => deckData; set => deckData = value; }
	private List<GameObject> cards = new List<GameObject>();
	void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        else { Instance = this; }
    }
	public void Save()
    {
		deckData.CardNames = new List<string>();
		foreach(GameObject gameObject in cards)
        {
			deckData.CardNames.Add(gameObject.name);
        }
    }
	public void LoadUI()
    {
		foreach (string cardName in deckData.CardNames)
		{
			DeckUIManager.Instance.SpawnCardOnLoad(cardName);
		}
	}
	public void Subscribe(GameObject gameObject)
	{
		if (!cards.Contains(gameObject)) cards.Add(gameObject);
		Debug.Log(cards.Count);
		if(!cards.Contains(gameObject))
			cards.Add(gameObject);
	}

	public void Unsubscribe(GameObject gameObject)
	{
		if(cards.Contains(gameObject)) cards.Remove(gameObject);
	}
}
