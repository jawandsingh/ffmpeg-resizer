using System;
using System.IO;
using System.Diagnostics;

namespace FfmpegResizer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ProcessVideo();
        }

        public static void ProcessVideo()
        {
            var originFilePath = @"D:\ffmpegExperiment\sample.mp4";
            string name = Path.GetFileName(originFilePath);
            byte[] bytes = null;
            using (FileStream fileStream = new FileStream(originFilePath, FileMode.Open, FileAccess.Read))
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    fileStream.CopyTo(ms);
                    bytes = ms.ToArray();
                }

                var localStoragePath = Path.Combine(Path.GetTempPath(), name);
                var directoryPath = Path.GetDirectoryName(localStoragePath);
                Directory.CreateDirectory(directoryPath);
                File.WriteAllBytes(localStoragePath, bytes);
                Console.WriteLine($"File copy successful: {File.Exists(localStoragePath)}");
                var readBack = File.ReadAllBytes(localStoragePath);
                Console.WriteLine($"Read file Back: {readBack.Length}, {localStoragePath}");
                var resizedFolderPath = directoryPath + @"\Resized";
                Directory.CreateDirectory(resizedFolderPath);
                var resizedFiePath = Path.Combine(resizedFolderPath, Path.GetFileName(localStoragePath));

                var psi = new ProcessStartInfo();
                psi.FileName = @"D:\ffmpegExperiment\ffmpeg.exe";
                psi.Arguments = $"-i \"{localStoragePath}\" -vf scale=640:-2 \"{resizedFiePath}\"";
                psi.RedirectStandardOutput = false;
                psi.RedirectStandardError = false;
                psi.UseShellExecute = true;
                Console.WriteLine($"Args: {psi.Arguments}");

                try
                {
                    using (Process exeProcess = Process.Start(psi))
                    {
                        Console.WriteLine($"process started with processId: {exeProcess.Id}");
                        exeProcess.WaitForExit();
                        Console.WriteLine($"Exit Code: {exeProcess.ExitCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace.ToString());
                    Console.WriteLine(ex.Message.ToString());
                    return;
                }
                Console.WriteLine($"process exited");
                Console.WriteLine($"Temp Out Exists: {File.Exists(resizedFiePath)}");
            }

            Console.ReadLine();
        }
    }
}
