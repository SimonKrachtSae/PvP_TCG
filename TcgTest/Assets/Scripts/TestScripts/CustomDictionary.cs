using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Customs
{
    [System.Serializable]
    public class CustomDictionary<TKey, TValue1>
    {
        public List<KeyValuePair<TKey, TValue1>> Data = new List<KeyValuePair<TKey, TValue1>>();
        public CustomDictionary() { }
        public void Add(KeyValuePair<TKey,TValue1> valuePair)
        {
            Data.Add(valuePair);
        }
       // public void Remove(TKey key)
       // {
       //     foreach (KeyValuePair<TKey, TValue> valuePair in Data)
       //         if (valuePair.Key == key)
       //         {
       //             Data.Remove(valuePair);
       //             return;
       //         }
       // }
    }
    [System.Serializable]
    public class KeyValueTriplet<TKey, TValue1, TValue2>
    {
        public KeyValueTriplet()
        {
        }

        public KeyValueTriplet(TKey key, TValue1 value1, TValue2 value2)
        {
            Key = key;
            Value1 = value1;
            Value2 = value2;
        }

        public TKey Key;
        public TValue1 Value1;
        public TValue2 Value2;
    }
}
    [System.Serializable]
    public class KeyValuePair<TKey, TValue1>
    {
        public KeyValuePair()
        {
        }

        public KeyValuePair(object key, object value1)
        {

            
        }

        public TKey Key;
        public TValue1 Value1;
    }
