using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class qrTest : MonoBehaviour
{
    public RawImage sQr;
    public RawImage mQr;
    public RawImage lQr;

    private void Start()
    {
        Texture2D sQR = new Texture2D(2, 2);
        Texture2D mQR = new Texture2D(2, 2);
        Texture2D lQR = new Texture2D(2, 2);

        sQr.texture = ImageManager.Instance.GetQrTexture("image");
        mQr.texture = ImageManager.Instance.GetQrTexture("image_20250101");
        lQr.texture = ImageManager.Instance.GetQrTexture($"image_20250101_{Guid.NewGuid()}");
    }
}
