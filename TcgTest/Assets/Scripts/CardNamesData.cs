using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
[CreateAssetMenu(fileName = "CardNames", menuName = "ScriptableObjects/CardNames", order = 1)]
public class CardNamesData : ScriptableObject
{
    [SerializeField] private List<string> cardNames;
    public List<string> CardNames { get => cardNames; set => cardNames = value; }
    [SerializeField] private bool refresh;
    private void OnValidate()
    {
        if (refresh)
        {
            List<string> toRemove = new List<string>();
            foreach(string s in cardNames)
            {
                if (!Resources.Load(s))
                    toRemove.Add(s);
            }
            foreach (string st in toRemove)
                cardNames.Remove(st);
            refresh = false;
        }
    }
}
