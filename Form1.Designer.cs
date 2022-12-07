
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


//传向服务器的数据
//1：开始匹配
//2：放置完成，准备好了
//3：棋盘信息
//4：攻击信息
//5：失败（退出或者没血了）

//服务器传回的信息
//1：匹配成功，可以开始部署了
//2：游戏终止（有一方退出游戏）
//3：两边都准备好了，可以开始了（传输棋盘）
//4：接收敌方棋盘
//5：本机进攻
//6：本机被攻击
//7：胜利

namespace WinFormsApp1
{
    partial class Form1
    {
        //数据
        static int state = 0; // 该状态是系统状态，0是停止状态。1是开局部署状态，2是进攻状态
        
        static int setstate = 1; // 1是放机头，2是放机身方向
        static int sethead = 0; // 机头位置
        //static int attackpos = 0;//本次攻击位置
        static int planes = 0; // 飞机数量
        static int PrepareAttackPos = -1;//准备攻击的坐标
        int GameType = 0;//0未进行 1人机 2玩家
        ArtificialIntelligence PlayerSuggest;
        ArtificialIntelligence AiSuggest;

        /// <summary>
        /// selfmat有四种状态0, 1, 2, -1；
        /// 0是是初始态，仅作为逻辑处理中出现
        /// 1是机头，为红色
        /// 2是机身，蓝色
        /// -1是被击中的部分，灰色
        /// </summary>
        static int[,] selfmat = new int[10, 10]; // 自己的飞机矩阵
        /// <summary>
        /// 敌方矩阵有三种状态
        /// 1表示打中机头，红色
        /// 2表示击中机身，蓝色
        /// -1表示没有击中，黑色
        /// </summary>
        static int[,] enemymat = new int[10, 10];//己方显示的敌方矩阵（右边的棋盘）
        static int[,] enemysetmat = new int[10, 10];//敌方部署矩阵

        static int selfHP = 0; // 自己的血量
        static int eneHp = 0;//敌人血量
        //0是空白，1是机头，2是机身


        private System.ComponentModel.IContainer components = null; // 添加一个容器
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose(); // Dispose的作用是释放非托管资源
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        private void InitializeComponent() 
        {
            //this.BackColor = System.Drawing.Color.Blue;
            //this.BackColor = Color.LightBlue;
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label_State = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1639, 45);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(149, 63);
            this.button1.TabIndex = 0;
            this.button1.Text = "人机对战";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            //this.button1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(437, 625);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(147, 63);
            this.button2.TabIndex = 1;
            this.button2.Text = "重置";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            //this.button2.Visible = false;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(268, 625);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(133, 63);
            this.button3.TabIndex = 2;
            this.button3.Text = "准备";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            //this.button3.Visible = false;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(1639, 220);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(149, 63);
            this.button4.TabIndex = 3;
            this.button4.Text = "匹配";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            //this.button4.Visible = false;
            // 
            // label_State
            // 
            this.label_State.AutoSize = true;
            this.label_State.Font = new System.Drawing.Font("微软雅黑", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.label_State.Location = new System.Drawing.Point(348, 9);
            this.label_State.Name = "label_State";
            this.label_State.Size = new System.Drawing.Size(0, 36);
            this.label_State.TabIndex = 4;
            //this.label_State.Visible = false;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(41, 122);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.ShortcutsEnabled = false;
            this.textBox1.Size = new System.Drawing.Size(206, 306);
            this.textBox1.TabIndex = 5;
            //this.textBox1.Visible = false;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(1190, 625);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(145, 63);
            this.button5.TabIndex = 6;
            this.button5.Text = "确认攻击";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            //this.button5.Visible = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(1639, 625);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(149, 60);
            this.button6.TabIndex = 7;
            this.button6.Text = "结束本局";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            //this.button6.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(1557, 181);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(231, 23);
            this.textBox2.TabIndex = 8;
            //this.textBox2.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1575, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(79, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "输入服务器ip";
            //this.label1.Visible = false;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(1045, 625);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(127, 63);
            this.button7.TabIndex = 10;
            this.button7.Text = "查看建议";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            //this.button7.Visible = false;
            // 
            // Form1
            // 

            string picPath = Application.StartupPath + "BackGround.jpg";
            this.BackgroundImage = Image.FromFile(picPath);
            //this.BackColor = System.Drawing.Color.LightBlue;
            this.ClientSize = new System.Drawing.Size(1800, 700);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label_State);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        Rectangle[] selfrects; // 用于存储己方矩阵的矩形，全局变量，用来进行显示在界面上的，成员变量中的mat是用来维护状态的
        Rectangle[] enemyrects; // 用于存储敌方矩阵的矩形，全局变量
        protected override void OnLoad(EventArgs e) // 初始化己方和敌矩阵
        {
            int d = 50, x1 = 340, y1 = 60; // d是矩形的边长，x1和y1是矩形的左上角坐标
            int x2 = 1000, y2 = 60; // x2和y2是敌方矩阵的左上角坐标

            selfrects = new Rectangle[100];
            enemyrects = new Rectangle[100];
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    selfrects[i * 10 + j] = new Rectangle(x1 + i * d, y1 + j * d, d - 2, d - 2); // 己方矩阵的矩形，d - 2的作用是让矩形之间有间隔
                    enemyrects[i * 10 + j] = new Rectangle(x2 + i * d, y2 + j * d, d - 2, d - 2); // 敌方矩阵的矩形
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    selfmat[i, j] = 0; // 初始化己方矩阵
                    enemymat[i, j] = 0; // 初始化敌方矩阵
                }
            }
            this.button5.Enabled = false;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button6.Enabled = false;
        }
        protected override void OnPaint(PaintEventArgs e) // 绘制
        {

            var g = e.Graphics; // 获取画布

            //g.DrawRectangles(Pens.Black, selfrects); // 画己方矩阵的矩形，Pens.Black是画笔
            //g.DrawRectangles(Pens.Black, enemyrects);
            
            for(int i=0;i<10;i++)
            {
                for(int j=0;j<10;j++)
                {
                    if (selfmat[i, j] == 1) // 1表示机头，用红色表示
                    {
                        g.FillRectangle(Brushes.Red, selfrects[i * 10 + j]); 
                    }
                    if (selfmat[i, j] == 2) // 2表示机身，用蓝色表示
                    {
                        g.FillRectangle(Brushes.Blue, selfrects[i * 10 + j]);
                    }
                    if(selfmat[i,j]==-1) // -1表示被打中，用灰色表示
                    {
                        g.FillRectangle(Brushes.Gray, selfrects[i * 10 + j]);//被敌方击中
                    }
                    if (enemymat[i, j] == 1)  // 敌方矩阵，1表示打中机头，红色表示
                    {
                        g.FillRectangle(Brushes.Red, enemyrects[i * 10 + j]);
                    }
                    if (enemymat[i, j] == 2) // 2表示打中机身，蓝色表示
                    {
                        g.FillRectangle(Brushes.Blue, enemyrects[i * 10 + j]);
                    }
                    if (enemymat[i, j] == 3) //选中了，未攻击
                    {
                        g.FillRectangle(Brushes.Gray, enemyrects[i * 10 + j]);
                    }
                    if (enemymat[i, j] == -1) // -1表示没有打中，用黑色表示
                    {
                        g.FillRectangle(Brushes.Black, enemyrects[i * 10 + j]);
                    }
                }
            }
        }
        
        void OnMouseDown(object sender, MouseEventArgs e) // 处理鼠标操作
        {
            if(state==0) // 停止状态，等待对方操作，我方不进行操作，直接返回
            {
                return;
            }
            if(state==1) // 开局部署状态
            {
                if (setstate == 1) // 放置机头
                {
                    int count = 0;
                    int i, j;
                    foreach (var r in selfrects)
                    {
                        if (r.Contains(e.X, e.Y)) // e是鼠标点击的位置，如果点击的位置在矩形内
                        {
                            i = count / 10;
                            j = count % 10;
                            if (selfmat[i, j] != 0)
                            {

                            }
                            if (selfmat[i, j] == 0) // 如果这个矩阵块没有处理过
                            {
                                selfmat[i, j] = 1;
                                sethead = count;
                                setstate = 2;//下次放机身
                                Invalidate(); // 重绘
                                return;
                            }

                        }
                        count++;
                    }
                }
                if (setstate == 2) // 放置机身
                {
                    int count = 0;
                    foreach (var r in selfrects)
                    {
                        if (r.Contains(e.X, e.Y))
                        {
                            int res = SetJudge(count); // 判断是否可以放置机身
                            if (res==1)
                            {
                                setstate = 1; // 如果能放置机身，则重新回到放置机头的状态
                                planes++;
                                SetPlane(sethead, count); // 放置机身，第一个参数是机头的位置，第二个参数是机身的位置
                                if (planes==3)
                                {
                                    selfHP = 3;
                                    state = 0;
                                }
                                Invalidate();
                                return;
                            }
                            if(res==-1)
                            {
                                return;
                            }
                        }
                        count++;
                    }
                }
                
            }
            if(state==2) // 2是进攻状态
            {
                int count = 0;
                int i, j;
                foreach (var r in enemyrects)
                {
                    if (r.Contains(e.X, e.Y))
                    {
                        i = count / 10;
                        j = count % 10;
                        if (enemymat[i, j] != 0)
                        {
                            return;
                        }
                        if (enemymat[i, j] == 0)
                        {
                            //if(enemysetmat[i,j]==0)
                            //{
                            //    enemymat[i, j] = -1;
                            //}
                            //else
                            //{
                            //    enemymat[i, j] = enemysetmat[i, j];
                            //}

                            //state = 0;//攻击之后挂起
                            //SendAttackMessage(count);//把攻击坐标发送过去。
                            if(PrepareAttackPos!=-1)
                            {
                                enemymat[PrepareAttackPos / 10, PrepareAttackPos % 10] = 0;
                            }
                            enemymat[i, j] = 3;
                            PrepareAttackPos = count;

                            this.button5.Enabled = true;

                            Invalidate();
                            //state = 0;
                            return;
                        }

                    }
                    count++;
                }
            }
        }

        private int SetJudge(int direction)//根据机尾的方向和现在的机头方向判断是否合法
        {
            int ih, jh, id, jd;
            if(sethead==direction)
            {
                return -1;
            }
            ih = sethead / 10;
            jh = sethead % 10;
            id = direction / 10;
            jd = direction % 10;
            if(ih!=id&&jh!=jd)//不在一条直线上
            {
                return -1;
            }
            if(ih==id)
            {
                if (ih + 2 > 9 || ih - 2 < 0) 
                {
                    return -1;
                }
                if(jh<jd)
                {
                    if (jh + 3 > 9) 
                    {
                        return -1;
                    }
                    for(int i=1;i<4;i++)
                    {
                        if(selfmat[ih,jh+i]!=0)
                        {
                            return -1;
                        }
                    }
                    for(int i=1;i<3;i++)
                    {
                        if (selfmat[ih + i, jh + 1] != 0 || selfmat[ih - i, jh + 1] != 0) 
                        {
                            return -1;
                        }
                    }
                    if(selfmat[ih + 1, jh + 3] != 0 || selfmat[ih - 1, jh + 3] != 0)
                    {
                        return -1;
                    }
                }
                if (jh > jd)
                {
                    if (jh - 3 < 0)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (selfmat[ih, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (selfmat[ih + i, jh - 1] != 0 || selfmat[ih - i, jh - 1] != 0)
                        {
                            return -1;
                        }
                    }
                    if (selfmat[ih + 1, jh - 3] != 0 || selfmat[ih - 1, jh - 3] != 0)
                    {
                        return -1;
                    }
                }
            }
            if(jh==jd)
            {
                if(jh+2>9||jh-2<0)
                {
                    return -1;
                }
                if(ih<id)
                {
                    if(ih+3>9)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (selfmat[ih+i, jh] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (selfmat[ih + 1, jh + i] != 0 || selfmat[ih + 1, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    if (selfmat[ih + 3, jh + 1] != 0 || selfmat[ih + 3, jh - 1] != 0)
                    {
                        return -1;
                    }
                }
                if(ih>id)
                {
                    if (ih - 3 > 9)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (selfmat[ih - i, jh] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (selfmat[ih - 1, jh + i] != 0 || selfmat[ih - 1, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    if (selfmat[ih - 3, jh + 1] != 0 || selfmat[ih - 3, jh - 1] != 0)
                    {
                        return -1;
                    }
                }
            }
            return 1;
        }

        private void SetPlane(int head,int direction)
        {
            int ih, jh, id, jd;
            ih = head / 10;
            jh = head % 10;
            id = direction / 10;
            jd = direction % 10;


            if (ih == id)
            {

                if (jh < jd)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        selfmat[ih, jh + i] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        selfmat[ih + i, jh + 1] = 2;
                        selfmat[ih - i, jh + 1] = 2;
                    }
                    selfmat[ih + 1, jh + 3] = 2;
                    selfmat[ih - 1, jh + 3] = 2;
                }
                if (jh > jd)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        selfmat[ih, jh - i] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        selfmat[ih + i, jh - 1] = 2;
                        selfmat[ih - i, jh - 1] = 2;
                    }
                    selfmat[ih + 1, jh - 3] = 2;
                    selfmat[ih - 1, jh - 3] = 2;
                }
            }
            if (jh == jd)
            {
                if (ih < id)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        selfmat[ih + i, jh] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        selfmat[ih + 1, jh + i] = 2;
                        selfmat[ih + 1, jh - i] = 2;
                    }
                    selfmat[ih + 3, jh + 1] = 2;
                    selfmat[ih + 3, jh - 1] = 2;
                }
                if (ih > id)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        selfmat[ih - i, jh] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        selfmat[ih - 1, jh + i] = 2;
                        selfmat[ih - 1, jh - i] = 2;
                    }
                    selfmat[ih - 3, jh + 1] = 2;
                    selfmat[ih - 3, jh - 1] = 2;
                }
            }
        }
        
        private void RePlace()//重新放置
        {
            for(int i=0;i<10;i++)
            {
                for(int j=0;j<10;j++)
                {
                    selfmat[i, j] = 0;
                }
            }
            planes = 0;
            state = 1;
            sethead = 0;
            setstate = 1;
            Invalidate();
        }
        
        private int Attacked(int attackPos)//被击中并且给出反馈。0是没打中，1是机头，2是机身
        {
            int i, j;
            i = attackPos / 10;
            j = attackPos % 10;
            if(selfmat[i,j]==1)
            {
                selfHP--;
                if(selfHP==0)//没血了就结束游戏
                {
                    EndGame(4);
                    return -1;
                }
            }
            selfmat[i, j] = -1;
            return 0;
        }
        
        private int SendAttackMessage(int attackPos)//发送攻击坐标，调用ClientSendMsg
        {
            string msgTmp = "4;";
            msgTmp += attackPos.ToString();
            ClientSendMsg(msgTmp);
            return 0;
        }

        private int ListenMessage()//与服务器请求并建立连接，改为按键操作
        {
            return -1;
        }
        
        
        //游戏结束时候调用，可以进行游戏结束的操作，关闭套接字连接
        //1人机获胜 2人机失败 3对战获胜 4对战失败
        void EndGame(int type)
        {
            if(type==4)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (selfmat[i, j] == 1)
                        {
                            selfmat[i, j] = -1;
                        }
                    }
                }
                this.label_State.Text = "失败！您的飞机已经全部被击落";
                ClientSendMsg("5;");
                socketClient.Close();
            }
            if(type==3)
            {
                this.label_State.Text = "胜利！您已击落全部地方飞机";
                socketClient.Close();
            }
            if(type==1)
            {
                this.label_State.Text = "胜利！您已击落全部地方飞机";
            }
            if(type==2)
            {

                for(int i=0;i<10;i++)
                {
                    for(int j=0;j<10;j++)
                    {
                        if(selfmat[i,j]==1)
                        {
                            selfmat[i, j] = -1;
                        }
                    }
                }
                this.label_State.Text = "失败！您的飞机已经全部被击落";
            }
            this.button1.Enabled = true;
            this.button2.Enabled = false;
            this.button3.Enabled = false;
            this.button4.Enabled = true;
            this.button5.Enabled = false;
            this.button6.Enabled = false;
            
            state = 0; // 该状态是系统状态，0是停止状态。1是开局部署状态，2是进攻状态

            setstate = 1; // 1是放机头，2是放机身方向
            sethead = 0; // 机头位置
            //static int attackpos = 0;//本次攻击位置
            planes = 0; // 飞机数量
            PrepareAttackPos = -1;//准备攻击的坐标
            GameType = 0;//0未进行 1人机 2玩家
            selfHP = 0;
            eneHp = 0;
            this.textBox1.Text = "";

            Invalidate();
        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;

        // 创建1个客户端套接字和1个负责监听服务端请求的线程
        Thread threadClient = null;
        Socket socketClient = null;


        private void btnConnection_Click(object sender, EventArgs e)//应设置成自动的
        {
            SetInformation("匹配中");
            this.button6.Enabled = true;
            GameType = 2;
            this.button1.Enabled = false;
            this.button4.Enabled = false; // 将该按钮设置为可以点击可以操作
            // 定义一个套接字监听
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 获取IP地址
            string ipTmp = this.textBox2.Text.ToString();
            

            string ip = "127.0.0.1";

            if (ipTmp.CompareTo("") != 0)
            {
                ip = ipTmp;
            }

            IPAddress address = IPAddress.Parse(ip);

            // 将获取的IP地址和端口号绑定在网络节点上
            IPEndPoint point = new IPEndPoint(address, 8098);

            try
            {
                // 客户端套接字连接到网络节点上，使用connect
                socketClient.Connect(point);
            }
            catch (Exception)
            {
                Debug.Write("连接失败\r\n");
                this.label_State.Text = "连接失败，请检查服务器地址";
                this.button6.Enabled = false;
                GameType = 0;
                this.button1.Enabled = true;
                this.button4.Enabled = true; // 将该按钮设置为可以点击可以操作

                return;
            }

            threadClient = new Thread(recv);
            threadClient.IsBackground = true;
            threadClient.Start();
        }

        // 接受服务端发来的消息
        private void recv()
        {
            // 持续监听服务端发来的消息
            while (true)
            {
                try
                {
                    // 定义一个1M的内存缓冲区，用于临时性存储接收到的消息，此处即为保存的坐标或者棋盘
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    // 将客户端套接字收到的数据存储内存缓冲区，并获取长度
                    int length = socketClient.Receive(arrRecvmsg);

                    string strRecMsg = Encoding.UTF8.GetString(arrRecvmsg);

                    //可以考虑后边的放入函数

                    string Type = MessageType(strRecMsg);

                    if (Type.CompareTo("1") == 0)//匹配成功，可以开始部署了
                    {
                        OnSet();
                        string strTmp = GetMessage(strRecMsg);
                        this.textBox1.Text = "匹配到的玩家为：\n" + strTmp;
                        
                        SetInformation("部署阶段");
                    }
                    if (Type.CompareTo("2") == 0)//游戏终止（有一方退出游戏）
                    {

                    }
                    if (Type.CompareTo("3") == 0)//两边都准备好了，可以开始了（传输棋盘）
                    {
                        SendSelfMat();
                        SetInformation("等待对方进攻");
                    }
                    if (Type.CompareTo("4") == 0)//接收敌方棋盘
                    {
                        ReceiveEmeMat(strRecMsg);//接收
                    }
                    if(Type.CompareTo("41") == 0)
                    {
                        ReceiveEmeMat(strRecMsg);//接收
                        state = 2;
                        SetInformation("选择攻击坐标");
                    }
                    if (Type.CompareTo("5") == 0)
                    {
                        state = 2;
                        SetInformation("选择攻击坐标");
                    }
                    if (Type.CompareTo("6") == 0)
                    {
                        //Attacked()
                        int attackedPos = 0;
                        string strTmp;
                        strTmp=GetMessage(strRecMsg);
                        //attackedPos = int.Parse(strTmp);

                        for(int i=0;i<strTmp.Length;i++)
                        {
                            if (strTmp[i] == '\0')
                            {
                                break;
                            }
                            attackedPos = attackedPos*10 + (int)(strTmp[i]-'0');
                        }


                        //ClientSendMsg(strTmp);

                        if(Attacked(attackedPos)==-1)
                        {
                            return;
                        }
                        Invalidate();
                        state = 2;
                        SetInformation("选择攻击坐标");
                    }
                    if (Type.CompareTo("7") == 0) 
                    {
                        EndGame(3);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("远程服务器已经中断连接" + "\r\n\n");
                    Debug.WriteLine("远程服务器已经中断连接" + "\r\n");
                    break;
                }
            }
        }

        // 发送字符信息到服务端的方方法，sendMsg是要发送的信息，可以是坐标，可以是棋盘
        void ClientSendMsg(string sendMsg)
        {
            // 将输入的内容字符串转换为机器可以识别的字节数组     
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            // 调用客户端套接字发送字节数组

            // 发送坐标
            socketClient.Send(arrClientSendMsg);
        }
        private Button button4;

        //开始匹配的方法
        void OnMatching()
        {
            string Msg = "1;";
            ClientSendMsg(Msg);
        }

        private void OnSet()
        {
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            if (planes == 3) // 飞机数量为3
            {
                return;
            }
            state = 1;
        }

        void Ready()
        {
            if (planes!=3)
            {
                return;
            }
            this.button3.Enabled = false;
            string msg = "2;";//放置完成

            state = 0;
            this.button2.Enabled = false;
            if(GameType==2)
            {
                this.ClientSendMsg(msg);
            }
            

            if(GameType==1)
            {
                state = 2;
                SetInformation("选择攻击坐标");
            }

        }

        string MessageType(string msg)
        {
            string strTmp = new string(msg);
            for(int i=0;i<strTmp.Length;i++)
            {
                if(strTmp[i]==';')
                {
                    strTmp = strTmp.Remove(i);
                    break;
                }
            }
            return strTmp;
        }

        static string GetMessage(string msg)
        {
            int i = 0;
            for(i=0;i<msg.Length;i++)
            {
                if(msg[i]==';')
                {
                    i++;
                    break;
                }
            }
            
            char[] cTmp = new char[1000];
            int count = 0;
            for(;i<msg.Length;i++)
            {
                if (msg[i]=='\0')
                {
                    break;
                }
                cTmp[count] = msg[i];
                count++;
            }
            cTmp[count] = '\0';
            string strTmp = new string(cTmp);
            //strTmp.CopyTo(i, msg.ToCharArray(), 0, msg.Length - i - 1);
            //strTmp=msg.Substring()
            //strTmp.CopyTo(0, cTmp, 0, cTmp.Length);
            return strTmp;
        }

        private void SendSelfMat()
        {
            string Msg = "3;";
            for(int i=0;i<10;i++)
            {
                for(int j=0;j<10;j++)
                {
                    Msg += selfmat[i, j].ToString();
                    Msg += '.';
                }
            }
            ClientSendMsg(Msg);
        }

        private void ReceiveEmeMat(string Msg)
        {
            int count = 2;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    enemysetmat[i, j] = 0;
                }
            }
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    while (true)
                    {
                        if (Msg[count] == '.')
                        {
                            count++;
                            break;
                        }
                        enemysetmat[i, j] += enemysetmat[i, j] * 10 + (int)(Msg[count]-'0');//
                        count++;
                    }

                }
            }
        }

        void Attack()
        {
            this.button5.Enabled = false;
            int i = PrepareAttackPos / 10;
            int j = PrepareAttackPos % 10;

            PlayerSuggest.UpDate(PrepareAttackPos, enemysetmat[i, j]);

            if (enemysetmat[i, j] == 0)
            {
                enemymat[i, j] = -1;
            }
            else
            {
                enemymat[i, j] = enemysetmat[i, j];
                if(GameType==1)
                {
                    if(enemysetmat[i, j]==1)
                    {
                        eneHp--;
                        if(eneHp==0)
                        {
                            EndGame(1);
                            return;
                        }
                    }
                }
            }
            
            state = 0;
            SetInformation("等待对手进攻");
            if(GameType==2)
            {
                SendAttackMessage(PrepareAttackPos);//把攻击坐标发送过去。
            }
            if(GameType==1)
            {
                //if(AIRandomAttack()==-1)
                //{
                //    EndGame(2);
                //    return;
                //}
                if(AiAttack()==-1)
                {
                    EndGame(2);
                    return;
                }
            }

            

            PrepareAttackPos = -1;
            Invalidate();
        }

        void SetInformation(string information)
        {
            this.label_State.Text = information;
        }

        void CheckSuggest()
        {
            if(state==2)
            {
                this.button5.Enabled = true;
                int pos = PlayerSuggest.Suggestion();
                //PrepareAttackPos = pos;
                if(PrepareAttackPos!=-1)
                {
                    enemymat[PrepareAttackPos / 10, PrepareAttackPos % 10] = 0;
                }
                PrepareAttackPos = pos;
                enemymat[PrepareAttackPos / 10, PrepareAttackPos % 10] = 3;
                //int nums = PlayerSuggest.numbers();
                //this.textBox1.Text = nums.ToString();
                Invalidate();
            }
        }

        void Ai()
        {
            this.button2.Enabled = true;
            this.button3.Enabled = true;
            this.button6.Enabled = true;
            this.button1.Enabled = false;
            this.button4.Enabled = false; // 将该按钮设置为可以点击可以操作
            //ai随机生成棋盘
            /*   */
            RandomSetMat();
            SetInformation("部署阶段");
            GameType = 1;
            state = 1;

        }

        void RandomSetMat()//ai生成棋盘
        {
            int count = 0;
            while(true)
            {
                if(count==3)
                {
                    break;
                }
                Random r = new Random();
                int head = r.Next(0, 100);
                int dir = r.Next(0, 100);
                if(AISetJudge(head,dir)==1)
                {
                    AiSetAPlane(head, dir);
                    count++;
                }
            }
            eneHp = 3;
        }
        int AIRandomAttack()//ai随机攻击,如果没血了返回-1
        {
            Random r = new Random();
            while(true)
            {
                
                int x = r.Next(0, 10);
                int y = r.Next(0, 10);
                if(selfmat[x,y]!=-1)
                {
                    if(selfmat[x,y]==1)
                    {
                        selfHP--;
                        if(selfHP==0)
                        {
                            return -1;
                        }
                    }
                    selfmat[x, y] = -1;
                    break;
                }
            }
            
            return 0;
        }
        
        int AiAttack()
        {
            int pos = AiSuggest.Suggestion();
            
            int x = pos / 10;
            int y = pos % 10;
            AiSuggest.UpDate(pos, selfmat[x, y]);
            if (selfmat[x, y] == 1)
            {
                selfmat[x, y] = -1;
                selfHP--;
                if (selfHP == 0)
                {
                    return -1;
                }
            }
            selfmat[x, y] = -1;
            state = 2;
            return 0;
        }
        void AiSetAPlane(int head,int direction)
        {
            int ih, jh, id, jd;
            ih = head / 10;
            jh = head % 10;
            id = direction / 10;
            jd = direction % 10;
            enemysetmat[ih, jh] = 1;

            if (ih == id)
            {

                if (jh < jd)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        enemysetmat[ih, jh + i] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        enemysetmat[ih + i, jh + 1] = 2;
                        enemysetmat[ih - i, jh + 1] = 2;
                    }
                    enemysetmat[ih + 1, jh + 3] = 2;
                    enemysetmat[ih - 1, jh + 3] = 2;
                }
                if (jh > jd)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        enemysetmat[ih, jh - i] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        enemysetmat[ih + i, jh - 1] = 2;
                        enemysetmat[ih - i, jh - 1] = 2;
                    }
                    enemysetmat[ih + 1, jh - 3] = 2;
                    enemysetmat[ih - 1, jh - 3] = 2;
                }
            }
            if (jh == jd)
            {
                if (ih < id)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        enemysetmat[ih + i, jh] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        enemysetmat[ih + 1, jh + i] = 2;
                        enemysetmat[ih + 1, jh - i] = 2;
                    }
                    enemysetmat[ih + 3, jh + 1] = 2;
                    enemysetmat[ih + 3, jh - 1] = 2;
                }
                if (ih > id)
                {
                    for (int i = 1; i < 4; i++)
                    {
                        enemysetmat[ih - i, jh] = 2;
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        enemysetmat[ih - 1, jh + i] = 2;
                        enemysetmat[ih - 1, jh - i] = 2;
                    }
                    enemysetmat[ih - 3, jh + 1] = 2;
                    enemysetmat[ih - 3, jh - 1] = 2;
                }
            }
        }

        private int AISetJudge(int head,int direction)//根据机尾的方向和现在的机头方向判断是否合法
        {
            int ih, jh, id, jd;
            if (head == direction)
            {
                return -1;
            }
            ih = head / 10;
            jh = head % 10;
            id = direction / 10;
            jd = direction % 10;
            if (ih != id && jh != jd)//不在一条直线上
            {
                return -1;
            }
            if (ih == id)
            {
                if (ih + 2 > 9 || ih - 2 < 0)
                {
                    return -1;
                }
                if (jh < jd)
                {
                    if (jh + 3 > 9)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (enemysetmat[ih, jh + i] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (enemysetmat[ih + i, jh + 1] != 0 || enemysetmat[ih - i, jh + 1] != 0)
                        {
                            return -1;
                        }
                    }
                    if (enemysetmat[ih + 1, jh + 3] != 0 || enemysetmat[ih - 1, jh + 3] != 0)
                    {
                        return -1;
                    }
                }
                if (jh > jd)
                {
                    if (jh - 3 < 0)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (enemysetmat[ih, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (enemysetmat[ih + i, jh - 1] != 0 || enemysetmat[ih - i, jh - 1] != 0)
                        {
                            return -1;
                        }
                    }
                    if (enemysetmat[ih + 1, jh - 3] != 0 || enemysetmat[ih - 1, jh - 3] != 0)
                    {
                        return -1;
                    }
                }
            }
            if (jh == jd)
            {
                if (jh + 2 > 9 || jh - 2 < 0)
                {
                    return -1;
                }
                if (ih < id)
                {
                    if (ih + 3 > 9)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (enemysetmat[ih + i, jh] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (enemysetmat[ih + 1, jh + i] != 0 || enemysetmat[ih + 1, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    if (enemysetmat[ih + 3, jh + 1] != 0 || enemysetmat[ih + 3, jh - 1] != 0)
                    {
                        return -1;
                    }
                }
                if (ih > id)
                {
                    if (ih - 3 > 9)
                    {
                        return -1;
                    }
                    for (int i = 1; i < 4; i++)
                    {
                        if (enemysetmat[ih - i, jh] != 0)
                        {
                            return -1;
                        }
                    }
                    for (int i = 1; i < 3; i++)
                    {
                        if (enemysetmat[ih - 1, jh + i] != 0 || enemysetmat[ih - 1, jh - i] != 0)
                        {
                            return -1;
                        }
                    }
                    if (enemysetmat[ih - 3, jh + 1] != 0 || enemysetmat[ih - 3, jh - 1] != 0)
                    {
                        return -1;
                    }
                }
            }
            return 1;
        }

        private Label label_State;
        private TextBox textBox1;
        private Button button5;
        private Button button6;
        private TextBox textBox2;
        private Label label1;
        private Button button7;
    }


    public class ArtificialIntelligence
    {
        enum mapType { unknown, empty, plane, planeHead };
        Tuple<int, int> pos;
        // 声明两个常量
        public const int N = 10;
        public const int P_NUM = 3;

        List<node> s;
        mapType[,] nowMap;
        int tot = 0;

        class node
        {


            // 如何初始化map的内容为mapType的empty
            public mapType[,] map = new mapType[N + 1, N + 1];
            public long hash()
            {

                long M = 10000000000037, p = 107;
                long ret = 1;
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= N; j++)
                    {
                        if (map[i, j] == mapType.planeHead || map[i, j] == mapType.plane)
                        {
                            ret = ret * (i * N + j) % M * p % M;
                        }
                    }
                }
                return ret;
            }
            public node()
            {
                map[0, 0] = mapType.empty;
            }
        }

        void printNode(node x)
        {
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    if (x.map[i, j] == mapType.empty)
                    {
                        Console.Write("_");
                    }
                    else if (x.map[i, j] == mapType.plane)
                    {
                        Console.WriteLine("*");
                    }
                    else if (x.map[i, j] == mapType.planeHead)
                    {
                        Console.WriteLine("&");
                    }
                    Console.WriteLine(" ");
                }
                Console.WriteLine("\n");
            }
        }


        // 声明一个mapType类型的二维变量，并初始化内容为mapType的empty
        mapType[,] a = new mapType[N + 1, N + 1];
        public int[,] upPlane = new int[,] { { +1, -2 }, { +1, -1 }, { +1, 0 }, { +1, +1 }, { +1, +2 }, { +2, 0 }, { +3, -1 }, { +3, 0 }, { +3, +1 } };
        public int[,] downPlane = new int[,] { { -1, -2 }, { -1, -1 }, { -1, 0 }, { -1, +1 }, { -1, +2 }, { -2, 0 }, { -3, -1 }, { -3, 0 }, { -3, +1 } };
        public int[,] leftPlane = new int[,] { { -2, +1 }, { -1, +1 }, { 0, +1 }, { +1, +1 }, { +2, +1 }, { 0, +2 }, { -1, +3 }, { 0, +3 }, { +1, +3 } };
        public int[,] rightPlane = new int[,] { { -2, -1 }, { -1, -1 }, { 0, -1 }, { +1, -1 }, { +2, -1 }, { 0, -2 }, { -1, -3 }, { 0, -3 }, { +1, -3 } };


        void dfs(int nowPNum, ref List<node> VN)
        {
            if (nowPNum > P_NUM)
            {
                node x = new node();
                //x.map = a;
                //a.CopyTo(x.map, 0);

                //for (int i = 1; i <= N; i++)
                //{
                //    for (int j = 1; j <= N; j++)
                //    {
                //        x.map[i, j] = a[i, j];
                //    }
                //}

                Array.Copy(a, x.map, a.Length);

                VN.Add(x);
                return;
            }

            /**
	          * 枚举当前这架飞机的所有可能位置
	          * 总共4种情况，即对应飞机的4种朝向，每种情况枚举机头位置
	         */

            mapType[,] b = new mapType[N + 1, N + 1];
            // 备份数组a到b
            //for (int i = 1; i <= N; i++)
            //{
            //    for (int j = 1; j <= N; j++)
            //    {
            //        b[i, j] = a[i, j];
            //    }
            //}

            Array.Copy(a, b, a.Length);

            for (int dir = 0; dir < 4; dir++)   // 枚举飞机朝向
            {
                for (int i = 1; i <= N; i++)
                {
                    for (int j = 1; j <= N; j++)
                    {
                        // 枚举机头位置
                        //memcpy(a, b, sizeof(b));   // 首先初始化数组a
                        // 将数组b的内容复制到数组a中
                        //for (int m = 1; m <= N; m++)
                        //{
                        //    for (int n = 1; n <= N; n++)
                        //    {
                        //        a[m, n] = b[m, n];
                        //    }
                        //}

                        Array.Copy(b, a, b.Length);

                        bool flag = true;
                        if (a[i, j] != mapType.empty)
                        {
                            continue;
                        }
                        a[i, j] = mapType.planeHead;
                        for (int k = 0; k < 9; k++)
                        {   // 枚举机身位置
                            int ii, jj;
                            if (dir == 0)
                            {
                                ii = i + upPlane[k, 0];
                                jj = j + upPlane[k, 1];
                            }
                            else if (dir == 1)
                            {
                                ii = i + downPlane[k, 0];
                                jj = j + downPlane[k, 1];
                            }
                            else if (dir == 2)
                            {
                                ii = i + leftPlane[k, 0];
                                jj = j + leftPlane[k, 1];
                            }
                            else
                            {
                                ii = i + rightPlane[k, 0];
                                jj = j + rightPlane[k, 1];
                            }
                            if (ii < 1 || ii > N || jj < 1 || jj > N)
                            {
                                flag = false;
                                break;
                            };
                            if (a[ii, jj] != mapType.empty)
                            {
                                flag = false;
                                break;
                            };
                            a[ii, jj] = mapType.plane;
                        }
                        if (flag) dfs(nowPNum + 1, ref VN);
                    }
                }
            }
        }

        List<node> initNodes()
        {
            /**
             * 给出所有可能的摆放方式（已去重）
             */
            a[0, 0] = mapType.empty;
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    a[i, j] = mapType.empty;
                }

            }

            List<node> temp = new List<node>();
            List<node> ret = new List<node>();
            dfs(1, ref temp);
            HashSet<long> s = new HashSet<long>();   // 用于去重
            // 遍历temp，将其中的元素加入到ret中
            foreach (node x in temp)
            {
                long h = x.hash();
                if (!s.Contains(h))
                {

                    ret.Add(x);
                    s.Add(h);
                }
            }
            return ret;
        }

        Tuple<int, int> getNextStep()
        {
            int ii = 0, jj = 0;
            int maxEarn = 0;
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    if (nowMap[i, j] == mapType.unknown)
                    {
                        int p1 = 0, p2 = 0, p3 = 0;
                        foreach (node x in s)
                        {
                            if (x.map[i, j] == mapType.planeHead)
                            {
                                p2++;
                            }
                            else if (x.map[i, j] == mapType.plane)
                            {
                                p1++;
                            }
                            else if (x.map[i, j] == mapType.empty)
                            {
                                p3++;
                            }
                        }
                        int earn = p3 * (p1 + p2) + p2 * (p1 + p3) + p1 * (p2 + p3);
                        if (earn > maxEarn)
                        {
                            ii = i;
                            jj = j;
                            maxEarn = earn;
                        }
                    }
                }
            }
            // 返回元组ii,jj
            return new Tuple<int, int>(ii, jj);
        }

        void elimination(int x, int y, mapType m)
        {
            List<node> temp = new List<node>();
            // 遍历s，将其中的元素加入到temp中
            foreach (node t in s)
            {
                temp.Add(t);
            }
            s.Clear();
            foreach (node t in temp)
            {
                if (t.map[x, y] == m)
                {
                    s.Add(t);
                }
            }
        }

        public void Init()//完成数据的准备
        {
            s = initNodes();
            nowMap = new mapType[N + 1, N + 1];
        }
        public void UpDate(int Pos, int res)//客户端1是机头
        {
            int result=0;
            if(res==1)
            {
                result = 2;
            }
            if(res==2)
            {
                result = 1;
            }

            int _x_, y;
            _x_ = (Pos / 10) + 1;
            y = (Pos % 10) + 1;
            if (s.Count >= 1 && tot < P_NUM)
            {


                if (result == 0)
                {
                    nowMap[_x_, y] = mapType.empty;
                    elimination(_x_, y, mapType.empty);
                }

                else if (result == 1)
                {
                    nowMap[_x_, y] = mapType.plane;
                    elimination(_x_, y, mapType.plane);
                }

                else if (result == 2)
                {
                    nowMap[_x_, y] = mapType.planeHead;
                    elimination(_x_, y, mapType.planeHead);
                    tot++;
                }
                if (tot >= P_NUM)
                {
                    return;
                }
            }

        }

        public int numbers()
        {
            return s.Count;
        }

        public int Suggestion()
        {
            if (s.Count > 1 && tot < P_NUM)
            {
                //Console.WriteLine("总共还有" + s.Count + "种可能，");
                Tuple<int, int> p = getNextStep();
                //Console.WriteLine("推荐下一步选择 (" + p.Item1 + ", " + p.Item2 + ")");
                int pos = (p.Item1 - 1) * 10 + (p.Item2 - 1);
                return pos;
            }

            node x = s.First();
            for (int i = 1; i <= N; i++)
            {
                for (int j = 1; j <= N; j++)
                {
                    if (x.map[i, j] == mapType.planeHead && nowMap[i, j] == mapType.unknown)
                    {
                        //Console.WriteLine("(" + i + ", " + j + ")  ");
                        int pos = (i - 1) * 10 + (j - 1);
                        return pos;
                    }
                }
            }
            return 0;
        }

    }

}

