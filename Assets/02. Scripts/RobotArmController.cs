using StackExchange.Redis;
using System.Threading.Tasks;
using System;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class RobotArmController
{
    private static string ipAddress = "192.168.0.201";
    private static string port = "6379";
    private static string pw = "!xorbis21569100";
    private static string channel = "PhotoX";
    private static string hashKey = "XorbisPhotoX::STATUS0";

    private static ISubscriber publisher;
    private static IDatabase db;

    /// <summary>
    /// isForce = true일 경우 Redis 메시지 전송 X, 로컬의 테스트 이미지 등 사용
    /// </summary>
    public static bool isForce;

    public static async Task InitAsync()
    {
        isForce = false;

        try
        {
            // Redis에 연결
            var config = new ConfigurationOptions
            {
                EndPoints = { $"{ipAddress}:{port}" },
                Password = pw,
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                AllowAdmin = true
            };

            var redis = await ConnectionMultiplexer.ConnectAsync(config);

            publisher = redis.GetSubscriber();
            db = redis.GetDatabase();
            NLogManager.logger.Info("Redis Connect Success");

#if !UNITY_EDITOR

            await SendJson("ROBOT_MOVE", 0, true, true, false);

#endif

            await publisher.SubscribeAsync(channel, async (channel, message) =>
            {
                try
                {
                    NLogManager.logger.Info($"[Redis Received Message] {message}");

                    JObject jsonObj = JObject.Parse(message);

                    string step = null;
                    int preset = -1;
                    string path = null;

                    if (jsonObj.ContainsKey("step"))
                    {
                        step = jsonObj["step"].ToString();
                    }

                    if (jsonObj.ContainsKey("data"))
                    {
                        JObject dataObj = JObject.Parse(jsonObj["data"].ToString());

                        if (dataObj.ContainsKey("preset"))
                        {
                            int.TryParse(dataObj["preset"].ToString(), out preset);
                        }

                        if (dataObj.ContainsKey("path"))
                        {
                            path = dataObj["path"].ToString();
                        }
                    }

                    switch (step)
                    {
                        case "CAPTURE_END":

                            await Task.Delay(1000);

                            AddCapturedImgPath(path, preset);

                            break;
                    }
                }
                catch (Exception ex)
                {
                    NLogManager.logger.Error($"Error in SubscribeAsync: {ex}");
                }
            });
        }
        catch (Exception ex)
        {
            NLogManager.logger.Error(ex);
        }
    }

    /// <summary>
    /// 특정 요소의 검사 진행 후 진행 가능한 경우 _step, _preset 정보 담은 JSON Redis 메시지 전송
    /// </summary>
    /// <param name="_step">Redis로 보낼 명령어</param>
    /// <param name="_preset">Redis로 보낼 preset 인덱스</param>
    /// <param name="isCheckRobot">로봇 검사 여부</param>
    /// <param name="isCheckCam">카메라 검사 여부</param>
    /// <param name="isCheckInPosition">위치 검사 여부</param>
    /// <returns></returns>
    public static async Task SendJson(string _step, int _preset, bool isCheckRobot = true, bool isCheckCam = true, bool isCheckInPosition = true)
    {
        if(await ReadyCheck(isCheckRobot, isCheckCam, isCheckInPosition) == false)
        {
            return;
        }

        JObject jsonObj = new JObject
        {
            ["step"] = _step,
            ["data"] = new JObject
            {
                ["preset"] = _preset
            }
        };

        string json = jsonObj.ToString();

        NLogManager.logger.Info($"Sent Message: {json}");

        await publisher.PublishAsync(channel, json);
    }

    /// <summary>
    /// 로봇 기능 수행 가능 여부 평가
    /// </summary>
    /// <param name="isCheckRobot">로봇 검사 여부</param>
    /// <param name="isCheckCam">카메라 검사 여부</param>
    /// <param name="isCheckInPosition">위치 검사 여부</param>
    /// <returns></returns>
    public static async Task<bool> ReadyCheck(bool isCheckRobot = true, bool isCheckCam = true, bool isCheckInPosition = true)
    {
        int retry = 5;
        string robotStatus = null, cameraStatus = null, inPositionStatus = null;

        try
        {
            for (; retry > 0; retry--)
            {
                if (isForce)
                {
                    break;
                }

                int.TryParse(DateTime.Now.ToString("yyyyMMddHHmmss"), out int nowTime);

                if (isCheckRobot)
                {
                    robotStatus = await db.HashGetAsync(hashKey, "ROBOT_READY");
                    JObject robotStatusJson = JObject.Parse(robotStatus);

                    if (robotStatusJson.ContainsKey("ready"))
                    {
                        if (string.Equals(robotStatusJson["ready"].ToString(), "0"))
                        {
                            await Task.Delay(1000);
                            continue;
                        }
                    }

                    // keepalive가 2초 이상 차이날 경우 로봇 수행 불가능 처리
                    if (robotStatusJson.ContainsKey("keepalive"))
                    {
                        int.TryParse(robotStatusJson["keepalive"].ToString(), out int robotKeepAlive);

                        if (nowTime - robotKeepAlive >= 2)
                        {
                            NLogManager.logger.Warn($"Robot stopped. last active in {robotKeepAlive}");

                            await Task.Delay(1000);
                            continue;
                        }
                    }
                }

                if (isCheckCam)
                {
                    cameraStatus = await db.HashGetAsync(hashKey, "CAMERA_READY");
                    JObject cameraStatusJson = JObject.Parse(cameraStatus);

                    if (cameraStatusJson.ContainsKey("ready"))
                    {
                        if (string.Equals(cameraStatusJson["ready"].ToString(), "0"))
                        {
                            await Task.Delay(1000);
                            continue;
                        }
                    }

                    // keepalive가 2초 이상 차이날 경우 카메라 수행 불가능 처리
                    if (cameraStatusJson.ContainsKey("keepalive"))
                    {
                        int.TryParse(cameraStatusJson["keepalive"].ToString(), out int cameraKeepAlive);

                        if (nowTime - cameraKeepAlive >= 2)
                        {
                            NLogManager.logger.Warn($"Camera stopped. last active in {cameraKeepAlive}");

                            await Task.Delay(1000);
                            continue;
                        }
                    }
                }

                if (isCheckInPosition)
                {
                    inPositionStatus = await db.HashGetAsync(hashKey, "INPOSITION");

                    if (string.Equals(inPositionStatus, "0"))
                    {
                        await Task.Delay(1000);
                        continue;
                    }
                }

                break;
            }
        }
        catch (Exception ex)
        {
            NLogManager.logger.Error(ex);
            return false;
        }

        if(retry <= 0)
        {
            NLogManager.logger.Error("Robot is not now available.");
            return false;
        }
        else
        {
            return true;
        }
    }

    public static async Task MoveRobot(string _step, int _preset)
    {
        if (!isForce)
        {
            await SendJson(_step, _preset, true, true, false);
        }

        await CapturePhoto("CAPTURE_START", _preset);
    }

    private static async Task CapturePhoto(string _step, int _preset)
    {
        await Task.Delay(1000);

        if (await ReadyCheck() == false)
        {
            return;
        }

        int timer = 3;
        if(isForce)
        {
            timer = 0;
        }

        for (; timer > 0; timer--)
        {
            EventManager.GetEvent<string>().Publish(timer.ToString(), "COUNTDOWN");

            await Task.Delay(1000);
        }

        if (!isForce)
        {
            await SendJson(_step, _preset);
        }

        // isForce 상태일 경우 로봇 활용하지 않기에 로컬에 있는 테스트 이미지 사용
        if (isForce)
        {
#if UNITY_EDITOR

            AddCapturedImgPath("C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\test\\preset.png", 0);

#else

            AddCapturedImgPath($"{MainManager.Instance.sourcePath}/test/preset.png", 0);

#endif
        }

        EventManager.GetEvent<string>().Publish(null, "COUNTDOWN");
    }

    public static void AddCapturedImgPath(string captureImgPath, int preset)
    {
        // 촬영 끝난 후 대상 경로에 이미지 저장되어 있지 않을 경우 해당 프리셋 다시 촬영
        if(!File.Exists(captureImgPath))
        {
            MainManager.Instance.presets.Add(preset);
            return;
        }

        try
        {
            ImageManager.Instance.imgPaths.Add(captureImgPath);

            // 촬영할 프리셋이 남았으면 반복
            if (MainManager.Instance.presets.Count > 0)
            {
                MainManager.Instance.MoveRobot();
            }
            // 끝났을 경우 다음 씬(이미지 선택 씬)으로
            else
            {
                SimpleSceneManager.ChangeSceneByIndex(1);
            }
        }
        catch (Exception ex)
        {
            NLogManager.logger.Error(ex);
        }
    }

    public static void UnsubRedis()
    {
        NLogManager.logger.Info("Redis Disconneted.");
        publisher.UnsubscribeAll();
    }
}