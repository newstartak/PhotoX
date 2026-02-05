using UnityEngine;

public class BoolEventSub : MonoBehaviour
{
    [SerializeField]
    private string _channel;

    void Awake()
    {
        EventManager.GetEvent<bool>().Subscribe(SetActiveEvent, _channel);
    }

    void OnDestroy()
    {
        EventManager.GetEvent<bool>().Unsubscribe(SetActiveEvent, _channel);
    }

    private void SetActiveEvent(bool isSetActive)
    {
        gameObject.SetActive(isSetActive);
    }
}