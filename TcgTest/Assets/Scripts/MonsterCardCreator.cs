using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Customs;
using UnityEditor;
[ExecuteInEditMode]
public class MonsterCardCreator : MonoBehaviour
{
    [SerializeField] private GameObject preview;
    // [Slider(0,10,1)] [SerializeField]public int titleFontSize { get => titleFontSize; set { titleFontSize = value; } }
    //Key
    [Range(0,100)]
    [SerializeField] private int nameTextSize;
    [SerializeField] private float effectFontSize;
    [Range(0,100)]
    [SerializeField] private float manaFontSize;
    [Range(0,100)]
    [SerializeField] private float defenseFontSize;
    [Range(0,100)]
    [SerializeField] private int attackFontSize;

    [SerializeField] private bool create;

    [SerializeField] private Sprite picture;
    [SerializeField] private SpriteRenderer frontside;
    private bool assignedPreviewValues;
    private MonsterCard_Layout layout;
    void OnEnable()
    {

    }
    public void HandleNameChange<T>(Value<T> value, Value<T>.ValueEventArgs args)
    {
        Debug.Log("DDDD");
    }

    private void OnValidate()
    {
        if(preview != null && !assignedPreviewValues)
        {
            assignedPreviewValues = true;
            CardLayout layout = preview.GetComponent<CardLayout>();
            //titleFontSize = layout.NameTextUI.fontSize;
        }
    }
}
