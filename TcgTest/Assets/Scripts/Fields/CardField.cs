using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
public abstract class CardField : MonoBehaviour
{
    [SerializeField] protected Image backgroundImage;

    public Image BackgroundImage { get => backgroundImage; set => backgroundImage = value; }

    protected GameObject card;
    public GameObject Card { get => card; set => card = value; }

    public MonsterCardLayout Layout { get => layout; set => layout = value; }

    protected MonsterCardLayout layout;

    protected EffectCardLayout effectCardLayout;
    public EffectCardLayout EffectCardLayout { get => effectCardLayout; set => effectCardLayout = value; }

    [SerializeField]protected Button button;

    public Button Button { get => button; set => button = value; }
}
