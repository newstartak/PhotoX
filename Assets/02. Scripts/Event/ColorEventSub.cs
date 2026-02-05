using UnityEngine;
using UnityEngine.UI;

public class ColorEventSub : MonoBehaviour
{
    [SerializeField]
    private string _channel;

    private RawImage _rawImg;

    void Awake()
    {
        _rawImg = GetComponent<RawImage>();
        _rawImg.color = new Color32(150, 150, 150, 255);
        EventManager.GetEvent<Color>().Subscribe(ChangeColor, _channel);
    }

    void OnDestroy()
    {
        EventManager.GetEvent<Color>().Unsubscribe(ChangeColor, _channel);
    }

    private void ChangeColor(Color color)
    {
        _rawImg.color = color;
    }
}