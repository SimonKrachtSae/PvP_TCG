using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DeckData", menuName = "ScriptableObjects/DeckData", order = 1)]
public class DeckData : ScriptableObject
{
    [SerializeField] private List<string> cardNames;
    public List<string> CardNames { get => cardNames; set => cardNames = value; }
}
