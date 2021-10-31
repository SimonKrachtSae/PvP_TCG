using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Observer", menuName = "Customs/ObservableValue", order = 1)]
[System.Serializable]
public class ObservableValue: ScriptableObject
{
    [SerializeField] private ValueTypes valueType;
    private void OnValidate()
    {
        SetValueType(valueType);
        //((Observer)Resources.Load(nameof(Observer))).Subscribe<T>(this);
    }
    private void SetValueType(ValueTypes value)
    {
        valueType = value;
        if (valueType == ValueTypes.Int)
        {
        }
    }
    //public ObservableValue(T value)
    //{
    //    this.Value = value;
    //}
    private void OnDestroy()
    {
    }
}
public enum ValueTypes
{
    Object,
    Int,
    Float,
    Bool,
    Char,
    String
}
