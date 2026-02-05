using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

public class JsonManager
{
    private static string _filePath;

    private static List<int> cropDatas = new List<int>();
    private static List<Vector2>frameDatas = new List<Vector2>();

    /// <param name="fileName">파일명 (확장자 없이)</param>
    /// <returns>파일에서 읽어들여온 string</returns>
    private static string GetFileData(string fileName)
    {
#if UNITY_EDITOR

        _filePath = Path.Combine("C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\json", $"{fileName}.json");

#else

        _filePath = $"{MainManager.Instance.sourcePath}/json/{fileName}.json";

#endif


        if (!File.Exists(_filePath))
        {
            NLogManager.logger.Error($"{fileName}.json Not Found");
            return null;
        }

        return File.ReadAllText(_filePath);
    }

    /// <summary>
    /// preset.json에서 이미지 크롭 데이터 Get
    /// </summary>
    /// <param name="index">preset.json에서 얻고자 하는 데이터의 인덱스</param>
    /// <returns>크롭 시작 위치 및 크롭 양 List</returns>
    public static List<int> GetCropData(int index)
    {
        try
        {
            cropDatas.Clear();

            string fileData = GetFileData("preset");
            if (fileData == null)
            {
                return null;
            }

            JObject jsonObj = JObject.Parse(fileData);

            if (jsonObj.ContainsKey("preset"))
            {
                JArray presets = (JArray)jsonObj["preset"];

                JObject rect = (JObject)presets[index];

                cropDatas.Add(rect["startX"].Value<int>());
                cropDatas.Add(rect["startY"].Value<int>());
                cropDatas.Add(rect["width"].Value<int>());
                cropDatas.Add(rect["height"].Value<int>());
            }

            return cropDatas;
        }
        catch(Exception e)
        {
            NLogManager.logger.Error(e);
            return null;
        }
    }

    /// <summary>
    /// frame.json에서 프레임 데이터 Get
    /// </summary>
    /// <param name="index">frame.json에서 얻고자 하는 데이터의 인덱스</param>
    /// <returns>프레임 위에서의 각 사진 및 QR의 위치 데이터 List</returns>
    public static List<Vector2> GetFrameData(int index)
    {
        try
        {
            frameDatas.Clear();

            string fileData = GetFileData("frame");
            if (fileData == null)
            {
                return null;
            }

            JObject jsonObj = JObject.Parse(fileData);

            if (jsonObj.ContainsKey("preset"))
            {
                JArray presets = (JArray)jsonObj["preset"];

                JObject frame = (JObject)presets[index];

                JArray positions = (JArray)frame["positions"];

                foreach (JObject position in positions)
                {
                    frameDatas.Add(new Vector2(position["x"].Value<int>(), position["y"].Value<int>()));
                }
            }

            return frameDatas;
        }
        catch (Exception e)
        {
            NLogManager.logger.Error(e);
            return null;
        }
    }
}
