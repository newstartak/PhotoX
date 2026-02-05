using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class setPixelTest : MonoBehaviour
{
    private void Start()
    {
        
        // 1. 배경 이미지 로드
        Texture2D baseTex = new Texture2D(2, 2);
        baseTex.LoadImage(File.ReadAllBytes("C:\\Users\\xorbis\\Desktop\\srccontent\\frame.png"));

        // 2. 오버레이할 이미지들 로드
        Texture2D overlayTex = new Texture2D(2, 2);

        // 3. 위치를 정해서 덮어쓰기
        int x = 100;
        int y = 700;
        overlayTex.LoadImage(File.ReadAllBytes("C:\\Users\\xorbis\\Desktop\\srccontent\\red400.png"));
        baseTex.SetPixels(x, y, 300, 500, overlayTex.GetPixels(300, 300, 300, 500));

        x = 500;
        y = 700;
        overlayTex.LoadImage(File.ReadAllBytes("C:\\Users\\xorbis\\Desktop\\srccontent\\green400.png"));
        baseTex.SetPixels(x, y, 300, 500, overlayTex.GetPixels(300, 300, 300, 500));

        x = 100;
        y = 100;
        overlayTex.LoadImage(File.ReadAllBytes("C:\\Users\\xorbis\\Desktop\\srccontent\\purple400.png"));
        baseTex.SetPixels(x, y, 300, 500, overlayTex.GetPixels(300, 300, 300, 500));

        x = 500;
        y = 100;
        overlayTex.LoadImage(File.ReadAllBytes("C:\\Users\\xorbis\\Desktop\\srccontent\\red400.png"));
        baseTex.SetPixels(x, y, 300, 500, overlayTex.GetPixels(300, 300, 300, 500));

        // 4. 적용
        baseTex.Apply();

        GetComponent<RawImage>().texture = baseTex;

        // 5. jpg로 저장
        //byte[] jpg = baseTex.EncodeToJPG();

        // 6-1. 로컬에 저장
        //File.WriteAllBytes($"C:\\Users\\xorbis\\Desktop\\resultCombine\\output_{Guid.NewGuid()}.jpg", jpg);

        // 6-2. 서버에 업로드
        //UploadManager.GetBaseUrl();
        //UploadManager.UploadImg(jpg);
        
    }
}