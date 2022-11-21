using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware); // 设置高DPI模式
            Application.EnableVisualStyles(); // 启用视觉样式
            Application.SetCompatibleTextRenderingDefault(false); // 设置兼容文本渲染默认值

            Form1 window = new Form1(); // 创建窗口
            Application.Run(window); // 运行窗口
        }
    }
}
