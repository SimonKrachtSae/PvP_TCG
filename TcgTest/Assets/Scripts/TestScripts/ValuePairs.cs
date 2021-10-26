using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Customs
{
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
    [System.Serializable]
    public class ValueContainer<TKey, TValue>
    {
        public TKey Key;
        public TValue Value;
        public ValueContainer() { }
        public ValueContainer(TKey key, TValue value1)
        {
            Key = key;
            Value = value1;
        }
    }

}
