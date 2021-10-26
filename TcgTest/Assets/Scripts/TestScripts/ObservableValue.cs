using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Customs;
namespace Customs
{
    public class ObservableValue<T> : Value<T>
    {
        protected void Awake()
        {
            ((Observer)Resources.Load(nameof(Observer))).Subscribe<T>(this);
        }
        protected void OnDisable()
        {
            ((Observer)Resources.Load(nameof(Observer))).UnSubscribe<T>(this);
        }
    }
    [CreateAssetMenu(fileName = "ObservableInt", menuName = "Customs/Observables/Int", order = 1)]
    public class ObservableInt : ObservableValue<int>
    {
    }
}
