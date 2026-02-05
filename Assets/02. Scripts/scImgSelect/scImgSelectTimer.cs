using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class scImgSelectTimer : MonoBehaviour, ITimer
{

    void Start()
    {
        ImageManager.Instance.GetLocalImg(ImageManager.Instance.framePaths[0], "FRAME");

#if !UNITY_EDITOR
        // 사진 촬영 끝나면 로봇 위치 제자리
        _ = StartAsync();
#endif
    }


    async Task StartAsync()
    {
        await RobotArmController.SendJson("ROBOT_MOVE", 0, true, true, false);
    }

    public void EndTimer()
    {
        var selectedImgIndexs = ImageManager.Instance.selectedImgIndexs;

        // 시간 다 됐을 경우 낮은 인덱스부터 가장 먼저 촬영한 사진들로 채워넣음
        for(int i = 0; i < 8; i++)
        {
            if (!selectedImgIndexs.Contains(i))
            {
                int targetIndex = selectedImgIndexs.IndexOf(-1);

                if (targetIndex <= 0)
                {
                    continue;
                }

                selectedImgIndexs[targetIndex] = i;
            }
        }

        SimpleSceneManager.ChangeSceneByIndex(1);
    }
}
