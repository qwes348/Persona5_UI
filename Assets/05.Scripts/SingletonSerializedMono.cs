using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oni
{
    public class SingletonSerializedMono<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance = null;
        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<T>();
                    if(instance == null)
                    {
                        instance = new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
                    }                    
                }

                return instance;
            }
        }
    }
}
