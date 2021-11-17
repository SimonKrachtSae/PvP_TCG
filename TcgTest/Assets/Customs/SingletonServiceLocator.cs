using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace Assets.Customs
{
    public abstract class SingletonServiceLocator<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static bool isShuttingDown = false;
        private Dictionary<Type, T> singletons = new Dictionary<Type, T>();
        public ReadOnlyDictionary<Type,T> Singletons { get => singletons as ReadOnlyDictionary<Type, T>;}
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
        private void OnApplicationQuit()
        {
            isShuttingDown = true;
        }
        public void Subscribe<TType>(T type) where TType : T
        {
            if (singletons.ContainsKey(typeof(TType)))
            {
                Destroy(type.gameObject);
                return;
            }
            singletons.Add(typeof(TType), type);
            Debug.Log("Added: " + typeof(TType).ToString());
        }
        public TType GetSingleton<TType>() where TType : T
        {
            T type;
            if (singletons.TryGetValue(typeof(TType), out type))
            {
                return type as TType;
            }
            else
            {
                type = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity).AddComponent<TType>();
                Subscribe<TType>(type);
                Debug.LogWarning("Couldn't find object of type: " + typeof(TType) + "\n Created new Object of type" + typeof(TType));
                return type as TType;
            }
        }
        public void UnSubscribe<TType>() where TType : T
        {
            if (!singletons.ContainsKey(typeof(TType))) return;

            T type;
            if (singletons.TryGetValue(typeof(TType), out type))
            {
                singletons.Remove(typeof(TType));
                Destroy(type);
            }
        }
    }

    public class MB_SingletonServiceLocator : SingletonServiceLocator<MonoBehaviour>
    {
        private static MB_SingletonServiceLocator instance;
        public static MB_SingletonServiceLocator Instance
        {
            get
            {
                if (isShuttingDown) return null;

                if (instance == null)
                    instance = Instantiate(new GameObject(), Vector3.zero, Quaternion.identity).AddComponent<MB_SingletonServiceLocator>();

                return instance;
            }
        }
    }
}