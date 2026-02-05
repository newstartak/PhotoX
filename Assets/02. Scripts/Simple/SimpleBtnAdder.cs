using UnityEngine;
using UnityEngine.UI;

public abstract class SimpleBtnAdder : MonoBehaviour
{
    protected void Start()
    {
        Init();

        if (!gameObject.TryGetComponent(out Button btn))
        {
            btn = gameObject.AddComponent<Button>();
            btn.transition = Button.Transition.None;
        }

        gameObject.GetComponent<Button>().onClick.AddListener(() =>
        {
            ClickButton();
        });
    }

    /// <summary>
    /// 상속받은 자식에서 추가 초기화 작업 필요한 경우 구현
    /// </summary>
    public virtual void Init()
    {
        
    }

    /// <summary>
    /// 버튼 리스너에 추가할 메서드
    /// </summary>
    public abstract void ClickButton();
}
