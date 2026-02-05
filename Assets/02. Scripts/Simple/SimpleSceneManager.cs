using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SimpleSceneManager : SimpleBtnAdder
{
    [SerializeField]
    int prevOrNext;

    /// <summary>
    /// 주어진 이름의 씬으로 이동 가능
    /// </summary>
    /// <param name="scName"></param>
    public static void ChangeScene(string scName)
    {
        UnityMainThreadDispatcher.Instance.Enqueue(() => { SceneManager.LoadScene(scName); });
    }

    /// <summary>
    /// +- i 만큼 빌드 세팅에 나열된 씬으로 이동 가능
    /// </summary>
    /// <param name="i"></param>
    public static void ChangeSceneByIndex(int i)
    {
        UnityMainThreadDispatcher.Instance.Enqueue(() =>
        {
            int changeSceneIndex = SceneManager.GetActiveScene().buildIndex + i;

            if(changeSceneIndex >= SceneManager.sceneCountInBuildSettings || changeSceneIndex < 0)
            {
                ChangeScene("scMain");
            }
            else
            {
                SceneManager.LoadScene(changeSceneIndex);
            }
        });
    }

    public override void ClickButton()
    {
        ChangeSceneByIndex(prevOrNext);
    }
}
