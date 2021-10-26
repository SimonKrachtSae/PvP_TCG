using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Customs
{
    public delegate void ValueEventHandler<T, U>(T sender, U eventArgs);
    [System.Serializable]
    public abstract class Value<T> : ScriptableObject
    {
        public class ValueEventArgs : EventArgs { }
        public event ValueEventHandler<Value<T>, ValueEventArgs> valueEvent;

        [SerializeField] protected T value;
        public T Value_
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged(new ValueEventArgs());
            }
        }
        protected void OnValidate()
        {
           OnValueChanged(new ValueEventArgs());
        }
        protected void OnValueChanged(ValueEventArgs a)
        {
             valueEvent?.Invoke(this, a);
            valueEvent = null;
        }
        public void ClearValueChangedEvent()
        {
            valueEvent = null;
        }
    }
    [CreateAssetMenu(fileName = "new Int", menuName = "Customs/Values/Int", order = 1)]
    public class Int : Value<int>
    {
    }
    [CreateAssetMenu(fileName = "Float", menuName = "Customs/Values/Float", order = 2)]
    public class Float : Value<float>
    {
    }
    public class String : Value<string>
    {
    }
}
