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
        DontDestroyOnLoad(this.gameObject);
    }
    public void Save()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Deck.fun";
        FileStream stream = new FileStream(path, FileMode.Create);
        DeckData data = new DeckData(cardNames);
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
            DeckData = formatter.Deserialize(stream) as DeckData;
            stream.Close();
        }
        else
        {
            Debug.Log("File not found! \n Path: " + path);
        }
    }
}
