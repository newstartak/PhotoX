using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualScreenEnabler : MonoBehaviour
{
    void Start()
    {
#if !UNITY_EDITOR
        // 카메라 위 모니터에 활용할 Display 2 활성화
        Display.displays[1].Activate();
#endif
    }
}
