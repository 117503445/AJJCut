using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static TLib.Software.Logger;
namespace AJJCut
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            if (!File.Exists(ffmpeg))
            {
                MessageBox.Show("请去下载ffmpeg.exe,并放置程序文件夹内");
                Close();
            }
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string msg = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();

                CutVideo(msg);
            }
        }
        string ffmpeg = AppDomain.CurrentDomain.BaseDirectory + "/ffmpeg.exe";

        /// <summary>
        /// 秒数
        /// </summary>
        /// <param name="VideoName"></param>
        /// <returns></returns>
        private async Task<double> GetVideoTime(string VideoName)
        {
            double result = 0;

            await Task.Run(() =>
            {

                string output = "";
                using (Process process = new Process())
                {
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = ffmpeg,
                        Arguments = " -i " + VideoName,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    process.Start();
                    output = process.StandardError.ReadToEnd();
                    process.WaitForExit();
                    //Console.WriteLine(output);
                }
                string s = output.Substring(output.IndexOf("Dur") + 10, 11);
                Console.WriteLine(s);
                int hour = int.Parse(s.Substring(0, 2));
                int minute = int.Parse(s.Substring(3, 2));
                int second = int.Parse(s.Substring(6, 2));
                int msecond = int.Parse(s.Substring(9, 2));
                double t = hour * 3600 + minute * 60 + second + msecond * 0.01;
                Write(s);
                Write(t.ToString());
                result = t;
            });
            return result;
        }

        private async void CutVideo(string Video)
        {//./ffmpeg -ss 00:00:6 -t 00:00:12.5 -i test.mp4 -vcodec copy -acodec copy split1.mp4
         //FileInfo newVideo = Video.Clone();
            Tb.Text = "运行中";
            await Task.Run(async () =>
            {
                double time = await GetVideoTime(Video);
                string newVideo = Video.Substring(0, Video.IndexOf('.')) + "-Cure" + Video.Substring(Video.IndexOf('.'), Video.Length - Video.IndexOf('.'));
                Console.WriteLine(newVideo);

                using (Process process = new Process())
                {//{VideoName}+去除爱剪辑片头.mp4
                    process.StartInfo = new ProcessStartInfo()
                    {
                        FileName = ffmpeg,
                        Arguments = $"-ss 00:00:6 -t {time - 12.6} -i {Video} -y {newVideo}",
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true
                    };
                    Write(process.StartInfo.Arguments);
                    process.Start();
                    string s = process.StandardError.ReadToEnd();
                    Write(s);
                    process.WaitForExit();
                }

            });
            Tb.Text = "把视频拖进来,在我会输出无片头片尾的视频";
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
