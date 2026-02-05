using UnityEngine;
using QRCoder;
using System;
using System.IO;
using TMPro;
using UnityEngine.UI;
using System.Windows.Forms;
using SDColor = System.Drawing.Color;

public class QRMaker : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField storageUrl;
    [SerializeField] private TMP_InputField qrName;
    [SerializeField] private GameObject alertText;
    [SerializeField] private Transform spawnTr;

    [Header("QR 설정")]
    [SerializeField] private int qrSize = 512;
    [SerializeField] private Color32 qrBackgroundColor = new Color32(255, 255, 255, 255);

    [Header("로고 미리보기 UI")]
    [SerializeField] private GameObject logoPreviewPanel;
    [SerializeField] private RawImage logoPreviewImage;
    [SerializeField] private UnityEngine.UI.Button closeLogoButton;

    [Header("컬러 설정 UI")]
    [SerializeField] private GameObject colorPickerPanel;
    [SerializeField] private Image colorPreviewImage;
    [SerializeField] private Slider rSlider;
    [SerializeField] private Slider gSlider;
    [SerializeField] private Slider bSlider;
    [SerializeField] private Slider aSlider;
    [SerializeField] private TMP_InputField rInput;
    [SerializeField] private TMP_InputField gInput;
    [SerializeField] private TMP_InputField bInput;
    [SerializeField] private TMP_InputField aInput;
    [SerializeField] private UnityEngine.UI.Button applyColorButton;
    [SerializeField] private UnityEngine.UI.Button openColorPickerButton;

    private Texture2D selectedLogo;

    private void Start()
    {
        logoPreviewPanel.SetActive(false);
        closeLogoButton.onClick.AddListener(ClearSelectedLogo);

        colorPickerPanel.SetActive(false);
        openColorPickerButton.onClick.AddListener(OpenColorPicker);
        applyColorButton.onClick.AddListener(ApplySelectedColor);

        rSlider.onValueChanged.AddListener(v => OnSliderChanged("r", v));
        gSlider.onValueChanged.AddListener(v => OnSliderChanged("g", v));
        bSlider.onValueChanged.AddListener(v => OnSliderChanged("b", v));
        aSlider.onValueChanged.AddListener(v => OnSliderChanged("a", v));

        rInput.onEndEdit.AddListener(v => OnInputChanged("r", v));
        gInput.onEndEdit.AddListener(v => OnInputChanged("g", v));
        bInput.onEndEdit.AddListener(v => OnInputChanged("b", v));
        aInput.onEndEdit.AddListener(v => OnInputChanged("a", v));
    }

    public void MakeQr()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(storageUrl?.text))
            {
                Debug.LogError("storageUrl 입력이 비어 있습니다.");
                return;
            }

            string encodedUrl = Uri.EscapeUriString(storageUrl.text);
            //string encodedUrl = storageUrl.text;

            Debug.Log(encodedUrl);

            bool isLogoSelected = selectedLogo != null && selectedLogo.width > 2 && selectedLogo.height > 2;

            using var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(encodedUrl,
                isLogoSelected ? QRCodeGenerator.ECCLevel.H : QRCodeGenerator.ECCLevel.L);
            using var qrCode = new PngByteQRCode(qrData);

            var bgColor = SDColor.FromArgb(qrBackgroundColor.a, qrBackgroundColor.r, qrBackgroundColor.g, qrBackgroundColor.b);
            byte[] qrBytes = qrCode.GetGraphic(20, SDColor.Black, bgColor, true);

            Texture2D qrTex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            qrTex.LoadImage(qrBytes);
            qrTex = ResizeTexture(qrTex, qrSize, qrSize);

            if (isLogoSelected)
            {
                Texture2D logo = ResizeTextureKeepAspect(selectedLogo, qrSize / 4, qrSize / 4);
                int logoW = logo.width;
                int logoH = logo.height;
                int startX = (qrTex.width - logoW) / 2;
                int startY = (qrTex.height - logoH) / 2;

                Color[] logoPixels = logo.GetPixels();
                for (int y = 0; y < logoH; y++)
                {
                    for (int x = 0; x < logoW; x++)
                    {
                        Color logoPixel = logoPixels[y * logoW + x];
                        if (logoPixel.a > 0.01f)
                            qrTex.SetPixel(startX + x, startY + y, logoPixel);
                    }
                }

                qrTex.Apply();
            }

            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string fileName = string.IsNullOrWhiteSpace(qrName.text) ? $"QR_{Guid.NewGuid()}.png" : qrName.text;
            if (!fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                fileName += ".png";

            string fullPath = Path.Combine(desktopPath, fileName);
            byte[] finalBytes = qrTex.EncodeToPNG();
            File.WriteAllBytes(fullPath, finalBytes);

            Debug.Log($"QR 저장 완료 ({(isLogoSelected ? "ECC H" : "ECC L")}): {fullPath}");
            Instantiate(alertText, spawnTr);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void OnClickSelectImage()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg";

        if (openFileDialog.ShowDialog() == DialogResult.OK)
        {
            string filePath = openFileDialog.FileName;
            byte[] imageBytes = File.ReadAllBytes(filePath);
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(imageBytes);
            selectedLogo = tex;

            SetPreviewImage(tex);
            Debug.Log("로고 이미지 선택 및 미리보기 표시");
        }
    }

    private void SetPreviewImage(Texture2D tex)
    {
        logoPreviewImage.texture = tex;

        float maxWidth = 200f;
        float maxHeight = 150f;
        float imageWidth = tex.width;
        float imageHeight = tex.height;
        float ratio = imageWidth / imageHeight;

        float finalWidth = maxWidth;
        float finalHeight = maxWidth / ratio;

        if (finalHeight > maxHeight)
        {
            finalHeight = maxHeight;
            finalWidth = maxHeight * ratio;
        }

        logoPreviewImage.rectTransform.sizeDelta = new Vector2(finalWidth, finalHeight);
        logoPreviewPanel.SetActive(true);
    }

    private void ClearSelectedLogo()
    {
        selectedLogo = null;
        logoPreviewImage.texture = null;
        logoPreviewPanel.SetActive(false);
        Debug.Log("로고 이미지 제거됨");
    }

    private Texture2D ResizeTexture(Texture2D source, int width, int height)
    {
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);
        RenderTexture.active = rt;

        Texture2D tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        return tex;
    }

    private Texture2D ResizeTextureKeepAspect(Texture2D source, int maxWidth, int maxHeight)
    {
        float ratio = Mathf.Min((float)maxWidth / source.width, (float)maxHeight / source.height);
        return ResizeTexture(source,
            Mathf.RoundToInt(source.width * ratio),
            Mathf.RoundToInt(source.height * ratio));
    }

    // === 컬러 피커 기능 ===

    private void OpenColorPicker()
    {
        rSlider.value = qrBackgroundColor.r / 255f;
        gSlider.value = qrBackgroundColor.g / 255f;
        bSlider.value = qrBackgroundColor.b / 255f;
        aSlider.value = qrBackgroundColor.a / 255f;
        UpdateColorPreview();
        colorPickerPanel.SetActive(true);
    }

    private void ApplySelectedColor()
    {
        qrBackgroundColor = new Color32(
            (byte)(rSlider.value * 255),
            (byte)(gSlider.value * 255),
            (byte)(bSlider.value * 255),
            (byte)(aSlider.value * 255)
        );
        colorPickerPanel.SetActive(false);
        Debug.Log($"QR 배경색 적용: {qrBackgroundColor}");
    }

    private void UpdateColorPreview()
    {
        colorPreviewImage.color = new Color(rSlider.value, gSlider.value, bSlider.value, aSlider.value);

        rInput.text = Mathf.RoundToInt(rSlider.value * 255).ToString();
        gInput.text = Mathf.RoundToInt(gSlider.value * 255).ToString();
        bInput.text = Mathf.RoundToInt(bSlider.value * 255).ToString();
        aInput.text = Mathf.RoundToInt(aSlider.value * 255).ToString();
    }

    private void OnSliderChanged(string channel, float value)
    {
        int intVal = Mathf.RoundToInt(value * 255);
        switch (channel)
        {
            case "r": rInput.text = intVal.ToString(); break;
            case "g": gInput.text = intVal.ToString(); break;
            case "b": bInput.text = intVal.ToString(); break;
            case "a": aInput.text = intVal.ToString(); break;
        }
        UpdateColorPreview();
    }

    private void OnInputChanged(string channel, string value)
    {
        if (!int.TryParse(value, out int intVal)) return;
        intVal = Mathf.Clamp(intVal, 0, 255);
        float floatVal = intVal / 255f;

        switch (channel)
        {
            case "r": rSlider.value = floatVal; break;
            case "g": gSlider.value = floatVal; break;
            case "b": bSlider.value = floatVal; break;
            case "a": aSlider.value = floatVal; break;
        }
        UpdateColorPreview();
    }
}
