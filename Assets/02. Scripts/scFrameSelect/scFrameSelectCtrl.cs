using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class scFrameSelectCtrl : MonoBehaviour, ITimer
{
    void Start()
    {
        ImageManager.Instance.GetLocalImg(ImageManager.Instance.framePaths[0], "FRAME");

        EventManager.GetEvent<Color>().Publish(Color.white, $"SELECTABLE_0");

        List<int> cropData = JsonManager.GetCropData(0);

        // 이미지 선택 화면에서 고른 4개의 이미지를 프레임 위에 다시 표시
        for (int i = 0; i < 4; i++)
        {
            int index = ImageManager.Instance.selectedImgIndexs[i];

            if(index == -1)
            {
                break;
            }

            ImageManager.Instance.GetCropLocalImg(ImageManager.Instance.imgPaths[index], $"SELECTED_{i}", cropData[0], cropData[1], cropData[2], cropData[3]);
        }
    }

    public void EndTimer()
    {
        SimpleSceneManager.ChangeSceneByIndex(1);
    }
}
