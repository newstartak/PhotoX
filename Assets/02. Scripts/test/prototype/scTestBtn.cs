using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scTestBtn : SimpleBtnAdder
{
    [SerializeField]
    int presetIdx = -1;

    [SerializeField]
    bool isCapture;

    public override async void ClickButton()
    {
        if (isCapture)
        {
            await RobotArmController.SendJson("CAPTURE_START", 0);
        }
        else if (presetIdx != -1)
        {
            await RobotArmController.SendJson("ROBOT_MOVE", presetIdx, true, true, false);
        }
    }
}