using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Customs
{
    public abstract class MB_Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private bool isShuttingDown = false;
        protected void Awake()
        {
            if (isShuttingDown) return;
            MB_SingletonServiceLocator.Instance.Subscribe<T>(this);
        }
        protected void OnDestroy()
        {
            if (isShuttingDown) return;
            if(MB_SingletonServiceLocator.Instance?.GetSingleton<T>() == this)
                MB_SingletonServiceLocator.Instance?.UnSubscribe<T>();
        }
        private void OnApplicationQuit()
        {
            isShuttingDown = true;
        }
    }
}