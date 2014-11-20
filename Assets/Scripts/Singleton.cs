using UnityEngine;
using System.Collections;


public class SingletonGeneric<T> : MonoBehaviour where T:Component
{
    private static T instance_singleton;
    public static T Instance {
        get {
            if (instance_singleton == null)
            {
                instance_singleton = FindObjectOfType<T>();
                if (instance_singleton == null)
                {
                    GameObject obj = new GameObject ();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    instance_singleton = obj.AddComponent<T>();
                }
            }
            return instance_singleton;
        }
    }
 
    public virtual void Awake ()
    {
        DontDestroyOnLoad (this.gameObject);
        if (instance_singleton == null)
        {
            instance_singleton = this as T;
        } else {
            Destroy (gameObject);
        }
    }
}