using UnityEngine;

public class FrameSelect : SimpleBtnAdder
{
    [SerializeField]
    int _index;

    public override void Init()
    {
        ImageManager.Instance.GetLocalImg(ImageManager.Instance.framePaths[_index], $"SELECTABLE_{_index}");
    }

    public override void ClickButton()
    {
        // 현재 선택한 프레임이 이미 고른 프레임이라면
        if (_index == ImageManager.Instance.selectedFrameIndex)
        {
            return;
        }

        // 기존에 선택한 프레임은 색상을 어둡게, 현재 선택할 프레임을 밝게
        EventManager.GetEvent<Color>().Publish(new Color32(150, 150, 150, 255), $"SELECTABLE_{ImageManager.Instance.selectedFrameIndex}");
        EventManager.GetEvent<Color>().Publish(Color.white, $"SELECTABLE_{_index}");

        // index를 현재 프레임으로 변경하고 실제로 보여지는 프레임 이미지도 변경
        ImageManager.Instance.selectedFrameIndex = _index;
        ImageManager.Instance.GetLocalImg(ImageManager.Instance.framePaths[_index], "FRAME");
    }
}