using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scOptionTimer : MonoBehaviour, ITimer
{
    public void EndTimer()
    {
        SimpleSceneManager.ChangeSceneByIndex(-1);
    }
}