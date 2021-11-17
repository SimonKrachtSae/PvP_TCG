using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
[CreateAssetMenu(fileName = "DeckData", menuName = "ScriptableObjects/DeckData", order = 1)]
public class DeckData : ScriptableObject
{
    [SerializeField] private List<string> cardNames;
    public List<string> CardNames 
    {
        get
        {
            if(cardNames == null)
            {
                string path = Application.persistentDataPath + "/Deck.fun";

                if (File.Exists(path))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(path, FileMode.Open);
                    cardNames = (List<string>)formatter.Deserialize(stream);
                    stream.Close();
                    return cardNames;
                }
            }
            return cardNames;
        }
        set => cardNames = value; }
}
