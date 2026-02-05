using UnityEngine;
using UnityEngine.UI;

public class ClickPayBtn : SimpleBtnAdder
{
    [SerializeField]
    bool isForceSuc;

    public override void ClickButton()
    {
        MainManager.Instance.PayRequest(isForceSuc);
    }
}
