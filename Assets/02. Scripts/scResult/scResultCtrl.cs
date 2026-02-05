using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class scResultCtrl : MonoBehaviour, ITimer
{
    void Start()
    {
        int frameIdx = ImageManager.Instance.selectedFrameIndex;

        ImageManager.Instance.GetLocalImg(ImageManager.Instance.framePaths[frameIdx], "FRAME");

        List<int> cropData = JsonManager.GetCropData(0);
        
        // 기존에 고른 4개의 이미지 다시 프레임 위에 출력
        for(int i = 0; i < 4; i++)
        {
            int imgIdx = ImageManager.Instance.selectedImgIndexs[i];

            if(imgIdx == -1)
            {
                break;
            }

            ImageManager.Instance.GetCropLocalImg(ImageManager.Instance.imgPaths[imgIdx], $"SELECTED_{i}", cropData[0], cropData[1], cropData[2], cropData[3]);
        }

        string objectName = $"{DateTime.Now:yyyyMMddHHmmss}_{Guid.NewGuid().ToString().Substring(0, 2)}";

        Texture2D tex = ImageManager.Instance.GetQrTexture($"{UploadManager.baseUrl}/htmls/{objectName}_html");

        EventManager.GetEvent<Texture2D>().Publish(tex, "QR");

        _ = StartAsync(objectName);
    }

    async Task StartAsync(string objectName)
    {
        // 이미지, 비디오, html 업로드 병렬 진행
        Task imgTask = ImageManager.Instance.GetCombinedImg(objectName);

        // 로컬 테스트 비디오!!!!!!!
        Task vidTask = UploadManager.UploadVideoAsync("C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\test\\threeSec.mp4", objectName);
        // 로컬 테스트 비디오!!!!!!

        Task htmlTask = UploadManager.UploadHtmlAsync(objectName);

        await Task.WhenAll(imgTask, vidTask, htmlTask);

        /*
        if (Directory.Exists(MainManager.Instance.capturePath))
        {
            string[] files = Directory.GetFiles(MainManager.Instance.capturePath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string deletefile = $"{MainManager.Instance.capturePath}/{fileName}";
                File.Delete(deletefile);
            }
        }
        else
        {
            throw new Exception("no image path");
        }

        if (Directory.Exists(MainManager.Instance.printPath))
        {
            string[] files = Directory.GetFiles(MainManager.Instance.printPath);

            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string deletefile = $"{MainManager.Instance.printPath}/{fileName}";
                File.Delete(deletefile);
            }
        }
        else
        {
            throw new Exception("no printer path");
        }
        */

        // 재시작을 위한 초기화 작업
        ImageManager.Instance.imgPaths.Clear();

        for(int i = 0; i < 4; i++)
        {
            ImageManager.Instance.selectedImgIndexs[i] = -1;
        }

        ImageManager.Instance.selectedFrameIndex = 0;
    }

    public void EndTimer()
    {
        SimpleSceneManager.ChangeScene("scMain");
    }
}
