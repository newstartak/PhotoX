using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class alertCtrl : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a * 0.99f);

        if(text.color.a <= 0.01f)
        {
            Destroy(gameObject);
        }
    }
}
