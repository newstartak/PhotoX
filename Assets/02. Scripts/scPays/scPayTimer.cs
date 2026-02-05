using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scPayTimer : MonoBehaviour, ITimer
{
    [SerializeField]
    int index;

    public void EndTimer()
    {
        SimpleSceneManager.ChangeSceneByIndex(index);
    }
}
