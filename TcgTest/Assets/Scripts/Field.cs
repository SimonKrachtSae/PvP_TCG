using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public class Field : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;

    public Image BackgroundImage { get => backgroundImage; set => backgroundImage = value; }

    private GameObject card;
    public GameObject Card { get => card; set => card = value; }
    public void AssignCard(MonsterCardStats cardStats)
    {
        if (Card != null) return;
        GameObject card = Instantiate(GameUIManager.Instance.CardLayoutPrefab, this.transform);
        CardLayout cardLayout = card.GetComponent<CardLayout>();
        cardLayout.MonsterCard = cardStats;
    }
}
