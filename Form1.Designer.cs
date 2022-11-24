
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
        /// <summary>
        /// selfmat有四种状态0, 1, 2, -1；
        /// 0是是初始态，仅作为逻辑处理中出现
        /// 1是机头，为红色
        /// 2是机身，蓝色
        /// -1是被击中的部分，灰色
        /// </summary>
        private int[,] selfmat = new int[10, 10]; // 自己的飞机矩阵
        /// <summary>
        /// 敌方矩阵有三种状态
        /// 1表示打中机头，红色
        /// 2表示击中机身，蓝色
        /// -1表示没有击中，黑色
        /// </summary>
        private int[,] enemymat = new int[10, 10];//己方和敌方矩阵
        private int[,] enemysetmat = new int[10, 10];//敌方部署矩阵

        private int selfHP = 0; // 自己的血量
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(124, 615);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(178, 63);
            this.button1.TabIndex = 0;
            this.button1.Text = "开始放置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(323, 615);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(178, 63);
            this.button2.TabIndex = 1;
            this.button2.Text = "重置";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(523, 615);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(133, 63);
            this.button3.TabIndex = 2;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(684, 615);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(117, 63);
            this.button4.TabIndex = 3;
            this.button4.Text = "Connect";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // Form1
            // 
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1400, 700);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
            this.ResumeLayout(false);

        }


        Rectangle[] selfrects; // 用于存储己方矩阵的矩形，全局变量，用来进行显示在界面上的，成员变量中的mat是用来维护状态的
        Rectangle[] enemyrects; // 用于存储敌方矩阵的矩形，全局变量
        protected override void OnLoad(EventArgs e) // 初始化己方和敌矩阵
        {
            int d = 50, x1 = 40, y1 = 40; // d是矩形的边长，x1和y1是矩形的左上角坐标
            int x2 = 700, y2 = 40; // x2和y2是敌方矩阵的左上角坐标

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
        }
        protected override void OnPaint(PaintEventArgs e) // 绘制
        {
            var g = e.Graphics; // 获取画布

            g.DrawRectangles(Pens.Black, selfrects); // 画己方矩阵的矩形，Pens.Black是画笔
            g.DrawRectangles(Pens.Black, enemyrects);
            
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
                    if(enemymat[i,j]==1) // 敌方矩阵，1表示打中机头，红色表示
                    {
                        g.FillRectangle(Brushes.Red, enemyrects[i * 10 + j]);
                    }
                    if (enemymat[i, j] == 2) // 2表示打中机身，蓝色表示
                    {
                        g.FillRectangle(Brushes.Blue, enemyrects[i * 10 + j]);
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

                        }
                        if (enemymat[i, j] == 0)
                        {
                            if(enemysetmat[i,j]==0)
                            {
                                enemymat[i, j] = -1;
                            }
                            else
                            {
                                enemymat[i, j] = enemysetmat[i, j];
                            }
                            //state = 0;//攻击之后挂起
                            SendAttackMessage(count);//把攻击坐标发送过去。
                            Invalidate();
                            state = 0;
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
        
        private void RePlace()
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
                    EndGame();
                }
            }    
            selfmat[i, j] = -1;
            return selfmat[i, j];
        }
        
        private int SendAttackMessage(int attackPos)//发送攻击坐标
        {
            return 0;
        }

        private int ListenMessage()//与服务器请求并建立连接
        {
            return -1;
        }

        void EndGame()//游戏结束时候调用，可以进行游戏结束的操作
        {

        }

        #endregion

        private Button button1;
        private Button button2;
        private Button button3;

        // 创建1个客户端套接字和1个负责监听服务端请求的线程
        Thread threadClient = null;
        Socket socketClient = null;


        private void btnConnection_Click(object sender, EventArgs e)
        {
            this.button4.Enabled = false; // 将该按钮设置为可以点击可以操作
            // 定义一个套接字监听
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 获取IP地址
            IPAddress address = IPAddress.Parse("127.0.0.1");

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

                return;
            }

            threadClient = new Thread(recv);
            threadClient.IsBackground = true;
            threadClient.Start();
        }

        // 接受服务端发来的消息
        private void recv()
        {
            int x = 0;
            // 持续监听服务端发来的消息
            while (true)
            {
                try
                {
                    // 定义一个1M的内存缓冲区，用于临时性存储接收到的消息  
                    byte[] arrRecvmsg = new byte[1024 * 1024];

                    // 将客户端套接字收到的数据存储内存缓冲区，并获取长度
                    int length = socketClient.Receive(arrRecvmsg);

                    string strRecMsg = Encoding.UTF8.GetString(arrRecvmsg);
                    if (x == 1)
                    {
                        // this.txtDebugInfo.AppendText("服务器:" + GetCurrentTime() + "\r\n" + strRecMsg + "\r\n\n");
                        Debug.WriteLine("服务器:" + GetCurrentTime() + "\r\n" + strRecMsg + "\r\n\n");
                    }
                    else
                    {
                        // this.txtDebugInfo.AppendText(strRevMsg + "\r\n\n");
                        Debug.WriteLine(strRecMsg + "\r\n\n");
                        x = 1;
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

        private DateTime GetCurrentTime()
        {
            DateTime currentTime = new DateTime();
            currentTime = DateTime.Now;
            return currentTime;
        }

        // 发送字符信息到服务端的方法  
        void ClientSendMsg(string sendMsg)
        {
            // 将输入的内容字符串转换为机器可以识别的字节数组     
            byte[] arrClientSendMsg = Encoding.UTF8.GetBytes(sendMsg);
            // 调用客户端套接字发送字节数组

            // 发送坐标
            socketClient.Send(arrClientSendMsg);

            
            // 将发送的信息追加到聊天内容文本框中     
            Debug.WriteLine("hello...." + ": " + GetCurrentTime() + "\r\n" + sendMsg + "\r\n\n");
            //this.txtDebugInfo.AppendText("hello...." + ": " + GetCurrentTime() + "\r\n" + sendMsg + "\r\n\n");
        }

       
        private Button button4;
    }
}

