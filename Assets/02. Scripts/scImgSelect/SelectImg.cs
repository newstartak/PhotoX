using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectImg : SimpleBtnAdder
{
    [SerializeField]
    int _index;

    public override void Init()
    {
        List<int> cropData = JsonManager.GetCropData(0);

        ImageManager.Instance.GetCropLocalImg(ImageManager.Instance.imgPaths[_index], $"SELECTABLE_{_index}", cropData[0], cropData[1], cropData[2], cropData[3]);
    }

    public override void ClickButton()
    {
        var selectedImgIndexs = ImageManager.Instance.selectedImgIndexs;

        // 이미 고른 이미지를 다시 골랐다면 제외하고 색상도 어둡게
        if (selectedImgIndexs.Contains(_index))
        {
            int targetIndex = selectedImgIndexs.IndexOf(_index);

            selectedImgIndexs[targetIndex] = -1;

            EventManager.GetEvent<Color>().Publish(new Color32(150, 150, 150, 255), $"SELECTABLE_{_index}");
            EventManager.GetEvent<Texture2D>().Publish(null, $"SELECTED_{targetIndex}");
        }
        // 아직 고르지 않은 이미지를 골랐다면 가장 낮은 인덱스에 추가하고 색상도 밝게
        else
        {
            int targetIndex = selectedImgIndexs.IndexOf(-1);

            if (targetIndex == -1)
            {
                return;
            }

            selectedImgIndexs[targetIndex] = _index;

            EventManager.GetEvent<Color>().Publish(Color.white, $"SELECTABLE_{_index}");

            List<int> cropData = JsonManager.GetCropData(0);
            ImageManager.Instance.GetCropLocalImg(ImageManager.Instance.imgPaths[_index], $"SELECTED_{targetIndex}", cropData[0], cropData[1], cropData[2], cropData[3]);
        }
    }
}
