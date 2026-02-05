using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StringEventSub : MonoBehaviour
{
    [SerializeField]
    private string _channel;

    private TextMeshProUGUI _text;

    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();

        EventManager.GetEvent<string>().Subscribe(SetActiveEvent, _channel);
    }

    void OnDestroy()
    {
        EventManager.GetEvent<string>().Unsubscribe(SetActiveEvent, _channel);
    }

    private void SetActiveEvent(string msg)
    {
        UnityMainThreadDispatcher.Instance.Enqueue(() => { _text.text = msg; });
    }
}