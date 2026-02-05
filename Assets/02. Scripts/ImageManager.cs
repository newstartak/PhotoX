using UnityEngine;
using System.IO;
using QRCoder;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class ImageManager : Singleton<ImageManager>
{
    /// <summary>
    /// 촬영한 이미지의 로컬 경로
    /// </summary>
    public List<string> imgPaths = new List<string>();

    /// <summary>
    /// 현재 프레임에 추가한 사진들의 인덱스
    /// </summary>
    public List<int> selectedImgIndexs;

    /// <summary>
    /// 프레임 이미지의 로컬 경로
    /// </summary>
    public List<string> framePaths = new List<string>();

    /// <summary>
    /// 선택한 프레임 이미지의 인덱스
    /// </summary>
    public int selectedFrameIndex;

    // Texture2D 계속 생성 시 메모리 누수되므로 미리 생성 후 재사용
    private Texture2D tex;
    private Texture2D cropTex;
    private Texture2D tempTex;

    protected override void Init()
    {
        selectedImgIndexs = new List<int> { -1, -1, -1, -1 };

#if UNITY_EDITOR

        // 테스트용 로컬 경로
        framePaths = new List<string>
        {
            "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\frame\\frame.png",
            "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\frame\\frameRed.png",
            "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\frame\\frameGreen.png",
            "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\frame\\frameBlue.png"
        };

#else

        // 빌드 파일에 포함된 프레임 경로
        framePaths = new List<string>
        {
            $"{MainManager.Instance.sourcePath}/frame/frame.png",
            $"{MainManager.Instance.sourcePath}/frame/frameRed.png",
            $"{MainManager.Instance.sourcePath}/frame/frameGreen.png",
            $"{MainManager.Instance.sourcePath}/frame/frameBlue.png",
        };

#endif
    }

    /// <summary>
    /// 경로의 이미지에서 불러온 Texture2D를 channel에게 Invoke
    /// </summary>
    /// <param name="path">이미지가 존재하는 로컬의 경로</param>
    /// <param name="channel">가져온 Texture2D를 보내고자 하는 채널</param>
    public void GetLocalImg(string path, string channel)
    {
        if(!File.Exists(path))
        {
            NLogManager.logger.Error($"{path} doesn't exist.");
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);

        tex = new Texture2D(2, 2);
        tex.LoadImage(bytes);

        EventManager.GetEvent<Texture2D>().Publish(tex, channel);
    }

    /// <summary>
    /// 경로의 이미지에서 불러온 Texture2D를 인자에 따라 크롭하여 channel에게 Invoke
    /// </summary>
    /// <param name="path">이미지가 존재하는 로컬의 경로</param>
    /// <param name="channel">가져온 Texture2D를 보내고자 하는 채널</param>
    /// <param name="startX">크롭 시작 x 위치</param>
    /// <param name="startY">크롭 시작 y 위치</param>
    /// <param name="width">크롭할 너비</param>
    /// <param name="height">크롭할 높이</param>
    public void GetCropLocalImg(string path, string channel, int startX, int startY, int width, int height)
    {
        if (!File.Exists(path))
        {
            NLogManager.logger.Error($"{path} doesn't exist.");
            return;
        }

        byte[] bytes = File.ReadAllBytes(path);

        tempTex = new Texture2D(2, 2);
        tempTex.LoadImage(bytes);

        cropTex = new Texture2D(width, height);
        cropTex.SetPixels(0, 0, width, height, tempTex.GetPixels(startX, startY, width, height));
        cropTex.Apply();

        EventManager.GetEvent<Texture2D>().Publish(cropTex, channel);
    }

    /// <param name="storageUrl">QR로 바꾸고자 하는 경로</param>
    /// <returns>QR Texture2D</returns>
    public Texture2D GetQrTexture(string storageUrl)
    {
        byte[] bytes = GetQrBytes(storageUrl);

        if(bytes == null)
        {
            return null;
        }

        tempTex = new Texture2D(2, 2);
        tempTex.LoadImage(bytes);

        return tempTex;
    }

    /// <param name="storageUrl">QR로 바꾸고자 하는 경로</param>
    /// <returns>QR byte[]</returns>
    private byte[] GetQrBytes(string storageUrl)
    {
        try
        {
            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(storageUrl, QRCodeGenerator.ECCLevel.L);
            using var qrCode = new PngByteQRCode(qrData);

            var qrBytes = qrCode.GetGraphic(5);

            return qrBytes;
        }
        catch (Exception e)
        {
            NLogManager.logger.Error(e);

            return null;
        }
    }

    /// <summary>
    /// 업로드할 이미지 생성 후 핫 폴더 프린트로 출력 및 서버에 업로드
    /// </summary>
    /// <param name="objectName">업로드할 이미지의 이름</param>
    public async Task GetCombinedImg(string objectName)
    {
        tex = new Texture2D(2, 2);
        tex.LoadImage(File.ReadAllBytes(framePaths[selectedFrameIndex]));

        // cropData 및 frameData의 인덱스
        // cropData     0: 크롭 시작할 x 위치, 1: 크롭 시작할 y 위치, 2: 크롭할 너비, 3: 크롭할 높이
        // frameData    0 ~ 3: 프레임 위에서의 각 사진의 위치, 4: QR의 위치
        int i = 0;

        List<Vector2> frameData = JsonManager.GetFrameData(0);
        List<int> cropData = JsonManager.GetCropData(0);

        try
        {
            foreach (var selectedImgIndex in selectedImgIndexs)
            {
                // 이미지를 4개 미만으로 고른 경우 빈칸으로 두고 바로 QR 인덱스로 건너뜀
                if (selectedImgIndex == -1)
                {
                    i = 4;
                    break;
                }

                // 넣고자 하는 이미지를 우선 불러옴
                tempTex = new Texture2D(2, 2);
                tempTex.LoadImage(File.ReadAllBytes(imgPaths[selectedImgIndex]));

                // 불러온 이미지를 크롭함
                cropTex = new Texture2D(cropData[2], cropData[3]);
                cropTex.SetPixels(0, 0, cropData[2], cropData[3], tempTex.GetPixels(cropData[0], cropData[1], cropData[2], cropData[3]));

                cropTex.Apply();

                // 크롭한 이미지를 원하는 대로 리사이즈
                cropTex = ResizeTexture(cropTex, cropData[2] / 2, cropData[3] / 2);

                Color[] resizedPixels = cropTex.GetPixels(0, 0, cropTex.width, cropTex.height);

                // 수정 완료된 이미지를 프레임 위에 배치
                tex.SetPixels((int)frameData[i].x, (int)frameData[i].y, cropTex.width, cropTex.height, resizedPixels);

                i++;
            }
        }
        catch (Exception e)
        {
            NLogManager.logger.Error("Error during SetPixels Images");
            NLogManager.logger.Error(e);
        }

        try
        {
            cropTex = new Texture2D(2, 2);

            cropTex = GetQrTexture($"{UploadManager.baseUrl}/htmls/{objectName}_html");

            cropTex = ResizeTexture(cropTex, 80, 80);

            tex.SetPixels((int)frameData[i].x, (int)frameData[i].y, cropTex.width, cropTex.height, cropTex.GetPixels(0, 0, cropTex.width, cropTex.height));
        }
        catch (Exception e)
        {
            NLogManager.logger.Error("Error during SetPixels QR");
            NLogManager.logger.Error(e);
        }

        tex.Apply();

        byte[] pngBytes = tex.EncodeToPNG();

        // 로컬에 저장하여 프린트
#if UNITY_EDITOR

        string localTestDir = "C:\\Users\\xorbis\\Desktop\\resultCombine";

        if (Directory.Exists(localTestDir))
        {
            File.WriteAllBytes($"{localTestDir}/result_{objectName}.png", pngBytes);
        }

#else

        if (Directory.Exists(MainManager.Instance.printPath))
        {
            File.WriteAllBytes($"{MainManager.Instance.printPath}/result_{objectName}.png", pngBytes);
        }

#endif

        // 서버에 업로드
        await UploadManager.UploadImgAsync(pngBytes, objectName);
    }

    /// <param name="srcTex">리사이즈 하고자 하는 Texture2D</param>
    /// <param name="newWidth">바꾸고자 하는 width</param>
    /// <param name="newHeight">바꾸고자 하는 height</param>
    /// <returns>리사이즈된 Texture2D</returns>
    public Texture2D ResizeTexture(Texture2D srcTex, int newWidth, int newHeight)
    {
        /*
        try
        {
            tempTex = new Texture2D(newWidth, newHeight);

            for (int y = 0; y < newHeight; y++)
            {
                for (int x = 0; x < newWidth; x++)
                {
                    float u = (float)x / newWidth;
                    float v = (float)y / newHeight;

                    Color newColor = srcTex.GetPixelBilinear(u, v);
                    tempTex.SetPixel(x, y, newColor);
                }
            }
            tempTex.Apply();
            return tempTex;
        }
        catch (Exception e)
        {
            NLogManager.logger.Error(e);
            return null;
        }
        */

        try
        {
            RenderTexture rendTex = RenderTexture.GetTemporary(newWidth, newHeight);
            RenderTexture.active = rendTex;

            // 현재 렌더 텍스처에 전달받은 텍스처 적용하기
            Graphics.Blit(srcTex, rendTex);

            Texture2D tempTex = new Texture2D(newWidth, newHeight);
            tempTex.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            tempTex.Apply();

            RenderTexture.ReleaseTemporary(rendTex);
            RenderTexture.active = null;

            return tempTex;

        }
        catch (Exception e)
        {
            NLogManager.logger.Error(e);
            return null;
        }
    }
}