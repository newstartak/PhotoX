using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teteSub : MonoBehaviour
{
    private void OnEnable()
    {
        tetePub.EventContainer += Memethodod;
    }

    private void OnDisable()
    {
        tetePub.EventContainer -= Memethodod;
    }

    void Memethodod(string msg)
    {
        Debug.Log($"{gameObject.name} get msg: {msg}");
    }
}