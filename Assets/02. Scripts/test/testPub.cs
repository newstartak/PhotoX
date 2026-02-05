using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPub : MonoBehaviour
{
    public static event Action OnPub;

    public void Start()
    {
        OnPub?.Invoke();
    }
}
