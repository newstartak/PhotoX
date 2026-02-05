using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;

public class tetePub : MonoBehaviour
{
    public static event Action<string> EventContainer;

    public void Publish(string msg)
    {
        Debug.Log("PUBLISH!");

        EventContainer?.Invoke(msg);
    }
}
