using System.Diagnostics;
using System.IO;
using UnityEngine;

public class FFmpegRecorder : MonoBehaviour
{
    public RenderTexture renderTexture;
    string ffmpegPath = Application.streamingAssetsPath + "/ffmpeg.exe";
    string outputFilePath = "D:/Rec/";  // Change this to your desired output file path
    int frameRate = 30;

    private Process ffmpegProcess;
    private BinaryWriter ffmpegInput;
    Texture2D frame;

    void Start()
    {
        frame = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);

        ffmpegProcess = new Process();
        ffmpegProcess.StartInfo.FileName = ffmpegPath;
        ffmpegProcess.StartInfo.Arguments = $"-y -f rawvideo -vcodec rawvideo -pix_fmt rgba -s {renderTexture.width}x{renderTexture.height} -r {frameRate} -i - -vf \"vflip\" -c:v libx264 -pix_fmt yuv420p -preset ultrafast {outputFilePath+renderTexture.name}.mp4";
        ffmpegProcess.StartInfo.CreateNoWindow = true;
        ffmpegProcess.StartInfo.UseShellExecute = false;
        ffmpegProcess.StartInfo.RedirectStandardInput = true;

        ffmpegProcess.Start();
        ffmpegInput = new BinaryWriter(ffmpegProcess.StandardInput.BaseStream);
    }

    void OnDestroy()
    {
        if (ffmpegProcess != null)
        {
            ffmpegInput.Close();
            ffmpegProcess.WaitForExit();
        }
    }

    void FixedUpdate()
    {
        RenderTexture.active = renderTexture;
        frame.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0, false);
        frame.Apply();
        byte[] pixels = frame.GetRawTextureData();
        ffmpegInput.Write(pixels);
    }
}
