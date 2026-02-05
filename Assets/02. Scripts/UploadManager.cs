using System.IO;
using System.Text;
using System;
using System.Threading.Tasks;
using Google.Cloud.Storage.V1;
using Google.Apis.Auth.OAuth2;

public class UploadManager
{
    // 접근 URL의 베이스가 되는 부분
    public static string baseUrl = "https://storage.googleapis.com/photox-storage";

    private static string _bucketName = "photox-storage";

    private static StorageClient _storage;

    public static async Task InitAsync()
    {
        try
        {
            // 서비스 계정 키 파일 환경변수 등록
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", $"{MainManager.Instance.sourcePath}/json/xorbisphotox-51a2183cbce9.json");
            GoogleCredential credential = await GoogleCredential.GetApplicationDefaultAsync();

            // StorageClient 생성
            _storage = await StorageClient.CreateAsync(credential);
        }
        catch (Exception e)
        {
            NLogManager.logger.Error(e);
        }
    }

    /// <summary>
    /// 로컬 경로에 있는 파일 비동기 업로드 시 사용.
    /// </summary>
    /// <param name="localPath">로컬 파일 경로</param>
    /// <param name="objectName">저장하고자 하는 저장소 폴더명을 포함한 이름</param>
    /// <param name="contentType">http content-type</param>
    public static async Task UploadFileAsync(string localPath, string objectName, string contentType)
    {
        try
        {
            using var fileStream = File.OpenRead(localPath);

            await _storage.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = contentType,
                CacheControl = "max-age=86400"  // 컨텐츠의 변화는 없으므로 하루 동안 캐시
            }, fileStream);

            NLogManager.logger.Info($"Upload Completed: {baseUrl}/{objectName}");
        }
        catch (Exception ex)
        {
            NLogManager.logger.Error(ex);
        }
    }

    /// <summary>
    /// 바이트로 인코딩되어있는 파일 비동기 업로드 시 사용.
    /// </summary>
    /// <param name="localPath">로컬 파일 경로</param>
    /// <param name="objectName">저장하고자 하는 저장소 폴더명을 포함한 이름</param>
    /// <param name="contentType">http content-type</param>
    public static async Task UploadMemoryAsync(byte[] contentBytes, string objectName, string contentType)
    {
        try
        {
            using var memoryStream = new MemoryStream(contentBytes);

            await _storage.UploadObjectAsync(new Google.Apis.Storage.v1.Data.Object
            {
                Bucket = _bucketName,
                Name = objectName,
                ContentType = contentType,
                CacheControl = "max-age=86400"  // 컨텐츠의 변화는 없으므로 하루 동안 캐시
            }, memoryStream);

            NLogManager.logger.Info($"Upload Completed: {baseUrl}/{objectName}");
        }
        catch (Exception ex)
        {
            NLogManager.logger.Error(ex);
        }
    }

    public static async Task UploadHtmlAsync(string objectName)
    {

#if UNITY_EDITOR

        string htmlPath = "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\web\\src.html";
        string cssPath = "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\web\\style.css";
        string jsPath = "C:\\Users\\xorbis\\Desktop\\Build\\srcContents\\web\\script.js";

#else

        string htmlPath = $"{MainManager.Instance.sourcePath}/web/src.html";
        string cssPath = $"{MainManager.Instance.sourcePath}/web/style.css";
        string jsPath = $"{MainManager.Instance.sourcePath}/web/script.js";

#endif

        Task htmlTask = HtmlTaskAsync();
        async Task HtmlTaskAsync()
        {
            string htmlContent = await File.ReadAllTextAsync(htmlPath);

            // html, 자바스크립트의 이미지 경로 등 동적으로 바꿔주어야 하는 부분 Replace
            htmlContent = htmlContent.Replace("##CSS", $"{baseUrl}/htmls/{objectName}_css");
            htmlContent = htmlContent.Replace("##JS", $"{baseUrl}/htmls/{objectName}_js");

            var contentBytes = Encoding.UTF8.GetBytes(htmlContent);

            await UploadMemoryAsync(contentBytes, $"htmls/{objectName}_html", "text/html");
        }

        Task cssTask = CssTaskAsync();
        async Task CssTaskAsync()
        {
            string cssContent = await File.ReadAllTextAsync(cssPath);

            var contentBytes = Encoding.UTF8.GetBytes(cssContent);

            await UploadMemoryAsync(contentBytes, $"htmls/{objectName}_css", "text/css");
        }

        Task jsTask = JsTaskAsync();
        async Task JsTaskAsync()
        {
            string jsContent = await File.ReadAllTextAsync(jsPath);

            jsContent = jsContent.Replace("##IMAGEPATH", $"{baseUrl}/images/{objectName}_img");
            jsContent = jsContent.Replace("##IMAGENAME", $"image_{objectName}_img");

            jsContent = jsContent.Replace("##VIDEOPATH", $"{baseUrl}/videos/{objectName}_vid");
            jsContent = jsContent.Replace("##VIDEONAME", $"video_{objectName}_vid");

            var contentBytes = Encoding.UTF8.GetBytes(jsContent);

            await UploadMemoryAsync(contentBytes, $"htmls/{objectName}_js", "text/javascript");
        }

        await Task.WhenAll(htmlTask, cssTask, jsTask);
    }

    public static async Task UploadImgAsync(byte[] imgBytes, string objectName)
    {
        await UploadMemoryAsync(imgBytes, $"images/{objectName}_img", "image/png");
    }

    public static async Task UploadVideoAsync(string path, string objectName)
    {
        await UploadFileAsync(path, $"videos/{objectName}_vid", "video/mp4");
    }
}