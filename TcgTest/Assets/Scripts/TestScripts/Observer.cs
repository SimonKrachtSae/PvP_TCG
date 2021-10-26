using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
namespace Customs
{
    [CreateAssetMenu(fileName = "Observer", menuName = "Customs/Observer",order = 1)]
    [ExecuteInEditMode]
    public class Observer: ScriptableObject
    {
        [SerializeField]private List<KeyValueTriplet<string,ObservableInt,int>> observableInts = new List<KeyValueTriplet<string, ObservableInt, int>>();
        public void Subscribe<T>(Customs.ObservableValue<T> value)
        {
            if(typeof(T) == typeof(int))
            {
                value.valueEvent += HandleValueChange;
                value.valueEvent += HandleNameChange;
                observableInts.Add(new KeyValueTriplet<string, ObservableInt,int>((value as ObservableInt).name, value as ObservableInt, (value as ObservableInt).Value_));
            }
        }
        public void UnSubscribe<T>(Customs.ObservableValue<T> value)
        {
            if (typeof(T) == typeof(int))
            {
                foreach(KeyValueTriplet<string,ObservableInt,int> triplet in observableInts)
                    if (triplet.Value1 = value as ObservableInt)
                    {
                        triplet.Value1.valueEvent -= HandleValueChange;
                        triplet.Value1.valueEvent -= HandleNameChange;
                        observableInts.Remove(triplet);
                        return;
                    }
            }
        }
        public void HandleValueChange<T>(Value<T> value, Value<T>.ValueEventArgs args) 
        {
            Debug.Log(value.name);
            Debug.Log(typeof(T));
        }
        public void HandleNameChange<T>(Value<T> value, Value<T>.ValueEventArgs args)
        {
            if (typeof(T) == typeof(int))
                foreach (KeyValueTriplet<string, ObservableInt, int> triplet in observableInts)
                    if(triplet.Value1 == value)
                    {
                        triplet.Key = name;
                        Debug.Log(triplet.Value1.name);
                        return;
                    }
        }
    }
}
[System.Serializable]
public class CustomVar<TValue>
{
    public TValue Value;
    public CustomVar(TValue value)
    {
        Value = value;
    }
}