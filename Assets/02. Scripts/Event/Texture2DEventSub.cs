using UnityEngine;
using UnityEngine.UI;

public class Texture2DEventSub : MonoBehaviour
{
    [SerializeField]
    private string _channel;

    private RawImage _rawImg;

    private void Awake()
    {
        _rawImg = GetComponent<RawImage>();
        EventManager.GetEvent<Texture2D>().Subscribe(ChangeTexture, _channel);
    }

    private void OnDestroy()
    {
        EventManager.GetEvent<Texture2D>().Unsubscribe(ChangeTexture, _channel);
    }

    private void ChangeTexture(Texture2D tex)
    {
        _rawImg.texture = tex;
    }
}
