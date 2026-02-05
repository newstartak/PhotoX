using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerFrameCtrl : MonoBehaviour
{
    [SerializeField]
    private int _time;

    /// <summary>
    /// isDown이 true일 경우 그라데이션이 시간의 흐름에 따라 밑으로 이동, false의 경우 그 반대인 위로 이동.
    /// </summary>
    [SerializeField]
    private bool _isDown = true;

    private RectTransform _rectTr;
    // 이동 목적지 위치
    private Vector2 _fixedReversedRectTr;

    // 초기 그라데이션의 위치와 이동 목적지 위치 사이의 거리
    private float _fixedDistance;

    private ITimer _timerEndAction;

    void Start()
    {
        _rectTr = GetComponent<RectTransform>();

        _fixedReversedRectTr = -_rectTr.anchoredPosition;

        _fixedDistance = Vector2.Distance(_rectTr.anchoredPosition, _fixedReversedRectTr) / _time;

        _timerEndAction = GetComponent<ITimer>();

        _ = DecreaseTime();
    }

    void Update()
    {
        _rectTr.anchoredPosition = Vector2.MoveTowards(_rectTr.anchoredPosition, _fixedReversedRectTr, _fixedDistance * Time.deltaTime);
    }

    async Task DecreaseTime()
    {
        int curSceneIndex = SceneManager.GetActiveScene().buildIndex;

        for (int t = _time; t >= 0; t--)
        {
            EventManager.GetEvent<string>().Publish(t.ToString(), "TIMER");

            await Task.Delay(1000);

            if (t <= 0)
            {
                //_timerEndAction?.EndTimer();
            }

            // 씬이 바뀌었을 경우 비동기 메서드 종료
            if (curSceneIndex != SceneManager.GetActiveScene().buildIndex)
            {
                break;
            }
        }
    }
}
