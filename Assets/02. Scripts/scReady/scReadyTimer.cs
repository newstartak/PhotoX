using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyCtrl : MonoBehaviour, ITimer
{
    void Start()
    {
        MainManager.Instance.SetPreset();

        //  디버깅용!!! 나중에 타이머 다시 키면 삭제할 것
        StartCoroutine(CoStart());
        //  디버깅용!!! 나중에 타이머 다시 키면 삭제할 것
    }

    IEnumerator CoStart()
    {
        int timer = 15;
        if(RobotArmController.isForce)
        {
            timer = 3;
        }

        yield return new WaitForSeconds(timer);

        EndTimer();
    }

    public void EndTimer()
    {
        SimpleSceneManager.ChangeSceneByIndex(1);
    }
}