using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

/// <summary>
/// 메인 스레드에서 행해져야 하는 메서드를 Enqueue하여 Update 시점에서 수행
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher _instance;

    public static UnityMainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                NLogManager.logger.Warn("Dispatcher is not initialized. Add Dispatcher in the scene first.");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(action);
        }
    }

    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue()?.Invoke();
            }
        }
    }
}