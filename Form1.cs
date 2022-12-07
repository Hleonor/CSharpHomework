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
            CheckForIllegalCrossThreadCalls = false;
            InitializeComponent(); // 初始化组件
        }

        private void button1_Click(object sender, EventArgs e) // 人机
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    selfmat[i, j] = 0; // 初始化己方矩阵
                    enemymat[i, j] = 0; // 初始化敌方矩阵
                    enemysetmat[i, j] = 0;
                }
            }

            Invalidate();
            this.PlayerSuggest = new ArtificialIntelligence();
            this.PlayerSuggest.Init();

            this.AiSuggest = new ArtificialIntelligence();
            this.AiSuggest.Init();

            Ai();

        }

        private void button2_Click(object sender, EventArgs e) 
        {
            this.RePlace(); // 重新布局
        }

        private void button4_Click(object sender, EventArgs e) // 连接
        {
            // 向服务器发送连接请求
            //this.SetInformation("正在初始化，请稍等");
            //this.label_State.Text = "正在初始化，请稍等";
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    selfmat[i, j] = 0; // 初始化己方矩阵
                    enemymat[i, j] = 0; // 初始化敌方矩阵
                    enemysetmat[i, j] = 0;
                }
            }

            Invalidate();
            
            this.PlayerSuggest= new ArtificialIntelligence();
            this.PlayerSuggest.Init();
            this.btnConnection_Click(sender, e);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Ready();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Attack();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if(GameType==1)
            {
                EndGame(2);
            }
            else
            {
                EndGame(4);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            CheckSuggest();
        }
    }
}
