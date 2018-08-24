using System;
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

namespace Test
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string ffmpeg = AppDomain.CurrentDomain.BaseDirectory + "/ffmpeg.exe";

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = ffmpeg,
                    Arguments = " -i " + "test.mp4",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                process.Start();
                System.Threading.Thread.Sleep(1000);
                string output = process.StandardError.ReadToEnd();
                int i = output.IndexOf("Durat");

                process.WaitForExit();
                Console.WriteLine(output);
            }
        }
    }
}
