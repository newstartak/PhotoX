using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class MainManager : Singleton<MainManager>
{
    // Build 폴더 안 json, frame 이미지 등이 담긴 폴더 경로
#if UNITY_EDITOR
    public readonly string sourcePath = "C:\\Users\\xorbis\\Desktop\\Build\\srcContents";
#else
    public readonly string sourcePath = $"{Path.GetDirectoryName(UnityEngine.Application.dataPath)}/srcContents";
#endif

    // 사진 촬영 결과물 폴더 경로
    public readonly string capturePath = "C:\\xorbis\\4. result";

    // 핫 폴더 프린트 경로
    public readonly string printPath = "C:\\DNP\\HotFolderPrint\\Prints\\s4x6\\RX1HS";

    /// <summary>
    /// 0: 사진만, 1: 사진 + 10초 영상
    /// </summary>
    public int captureOption;

    public List<int> presets = new List<int>();

    protected override void Init()
    {
        WebSocketClientAsync.Instance.ConnectWebSocket();

        _ = StartAsync();
    }

    async Task StartAsync()
    { 
        await RobotArmController.InitAsync();

        await UploadManager.InitAsync();
    }

    public void MoveRobot()
    {
        int preset = presets[0];
        presets.RemoveAt(0);

        _ = MoveRobotAsync(preset);
    }

    public async Task MoveRobotAsync(int preset)
    {
        await RobotArmController.MoveRobot("ROBOT_MOVE", preset);
    }

    public void PayRequest(bool isSuccessForce = false)
    {
        _ = PayRequestAsync(isSuccessForce);
    }

    async Task PayRequestAsync(bool isSuccessForce = false)
    {
        // 디버깅용!!!! 로봇 작동 안할때 무시하고 결제 시도
        bool isPayForce = true;
        // 디버깅용!!!! 로봇 작동 안할때 무시하고 결제 시도

        bool isRobotReady = await RobotArmController.ReadyCheck(true, true, false);

        if(isSuccessForce)
        {
            SimpleSceneManager.ChangeSceneByIndex(2);
        }
        else if (isRobotReady || isPayForce)
        {
            // 현재 1004원 결제, 즉 테스트 결제가 진행됨.
            WebSocketClientAsync.Instance.SendMessage();
        }
        else
        {
            NLogManager.logger.Error("Robot is not now available");
        }
    }

    /// <summary>
    /// 촬영 구도 선택에서 AI Choice 선택 시 랜덤하게 2가지 프리셋 제외하고 나머지 넷의 프리셋으로 촬영
    /// </summary>
    public void SetPreset()
    {
        presets.Clear();

        presets.Add(0);

        for (int i = 1; i <= 6; i++)
        {
            presets.Add(i);
            presets.Add(i);
        }
    }

    #region Debug

    /// <summary>
    /// isForce가 true일 경우 이미지, 프레임 등 로컬 이미지 활용하며, Redis 메시지 발신하지 않음.
    /// </summary>
    public void SetForceMode(bool isForce)
    {
        RobotArmController.isForce = isForce;

        EventManager.GetEvent<string>().Publish(RobotArmController.isForce.ToString(), "MODE");
    }

    public void UnsubRedis()
    {
        RobotArmController.UnsubRedis();
    }

    #endregion
}