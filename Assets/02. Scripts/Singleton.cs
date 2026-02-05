using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static object _lock = new object();

    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance != null)
            {
                return _instance;
            }

            lock (_lock)
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<T>();

                if (_instance != null)
                {
                    return _instance;
                }

                GameObject go = new GameObject($"{typeof(T).Name} (Singleton)");
                return _instance = go.AddComponent<T>();
            }
        }
    }

    virtual public void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);

            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void Init() { }
}