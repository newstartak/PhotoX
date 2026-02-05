using UnityEngine;

public class SelectOption : SimpleBtnAdder
{
    [SerializeField]
    int _captureOption;

    public override void ClickButton()
    {
        MainManager.Instance.captureOption = _captureOption;
    }
}
