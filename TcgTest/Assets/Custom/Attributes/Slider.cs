using UnityEngine;
using System;
using UnityEditor;
//Source: https://forum.unity.com/threads/range-attribute.451848/
[AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
public sealed class SliderAttribute : PropertyAttribute
{
    public readonly int min;
    public readonly int max;
    public readonly int step;

    public SliderAttribute(int min, int max, int step)
    {
        this.min = min;
        this.max = max;
        this.step = step;
    }
}

[CustomPropertyDrawer(typeof(SliderAttribute))]
internal sealed class Slider : PropertyDrawer
{
    private int value;

    //
    // Methods
    //
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var rangeAttribute = (SliderAttribute)base.attribute;

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            value = EditorGUI.IntSlider(position, label, value, rangeAttribute.min, rangeAttribute.max);

            value = (value / rangeAttribute.step) * rangeAttribute.step;
            property.intValue = value;
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
        }
    }
}

