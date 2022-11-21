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
            Application.SetHighDpiMode(HighDpiMode.SystemAware); // ���ø�DPIģʽ
            Application.EnableVisualStyles(); // �����Ӿ���ʽ
            Application.SetCompatibleTextRenderingDefault(false); // ���ü����ı���ȾĬ��ֵ

            Form1 window = new Form1(); // ��������
            Application.Run(window); // ���д���
        }
    }
}
