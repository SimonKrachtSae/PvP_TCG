using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;
[CreateAssetMenu(fileName = "Observer", menuName = "Customs/Observer",order = 1)]
public  class Observer: ScriptableObject
{
    //public List<ObservableValue<int>> observableInts;
    public UnityAction<string, object> OnObservableValueChanged { get; set; }
    //public void Subscribe<T>(ObservableValue<T> value)
    //{
    //    T type = default(T);
    //    if(typeof(T) == typeof(int))
    //    {
    //        //value = System.Convert.ChangeType(value.Value.GetType(), typeof(T));
    //        //if (observableInts == null) observableInts = new List<ObservableValue<int>>();
    //        //if (!observableInts.Contains(value))
    //        //    observableInts.Add(value);
    //    }
    //}
    public void UnSubscribe(KeyValuePair<Object, object> valuePair)
    {
          //  data.Remove(valuePair);
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