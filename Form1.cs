using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent(); // 初始化组件
        }

        private void button1_Click(object sender, EventArgs e) // 定义按钮点击事件
        {
            if(planes==3) // 飞机数量为3
            {
                return;
            }
            state = 1;
            ListenMessage();
        }

        private void button2_Click(object sender, EventArgs e) 
        {
            this.RePlace(); // 重新布局
        }

        private void button4_Click(object sender, EventArgs e) // 连接
        {
            // 向服务器发送连接请求
            
        }
    }
}
