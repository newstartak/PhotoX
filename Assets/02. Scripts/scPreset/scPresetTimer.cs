using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scPresetTimer : MonoBehaviour, ITimer
{
    void Start()
    {
        MainManager.Instance.presets.Clear();
    }

    public void EndTimer()
    {
        // 시간 종료되었을 경우 강제로 AI Choice 하도록 함
        MainManager.Instance.presets.Clear();
        MainManager.Instance.presets.Add(7);

        SimpleSceneManager.ChangeSceneByIndex(1);
    }
}
