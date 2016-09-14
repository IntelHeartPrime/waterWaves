using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Drawing.Imaging;
using Gif.Components;


namespace drawtest
{
    public partial class Form1 : Form
    {
        /*************************
        建立ArrayList数据结构
        **************************/
        ArrayList ArrayForGIF = new ArrayList();
        private Bitmap box1;

        public Form1()
        {
            InitializeComponent();
            panel1.AutoScroll = true;
            panel2.AutoScroll = true;
        }
        protected override void OnPaint(PaintEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open1 = new OpenFileDialog();
            open1.Filter = "JPG File(*.jpg)|*.jpg";
            if (open1.ShowDialog() == DialogResult.OK)
            {
                Bitmap image = new Bitmap(open1.FileName);
                pictureBox1.Image = image;
                pictureBox1.Width = image.Width + 10;
                pictureBox1.Height = image.Height + 10;
                pictureBox2.Width = pictureBox1.Width;
                pictureBox2.Height = pictureBox1.Height;
                box1 = image;

            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetDataObject(pictureBox1.Image);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IDataObject idata = Clipboard.GetDataObject();
            if (idata.GetDataPresent(DataFormats.Bitmap))
            {
                pictureBox2.Image = (Bitmap)idata.GetData(DataFormats.Bitmap);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap box2 = new Bitmap(pictureBox1.Image);
            int r, g, b, i, j, size, xes, yes, k1, k2;
            Color c;
            size = 4;
            xes = pictureBox1.Image.Width;
            yes = pictureBox1.Image.Height;
            for (i = 0; i <= xes - 1; i = i + size)
            {
                for (j = 0; j <= yes - 1; j = j + size)
                {
                    c = box1.GetPixel(i, j);
                    r = c.R;
                    g = c.G;
                    b = c.B;
                    Color cc = Color.FromArgb(r, g, b);
                    for (k1 = 0; k1 <= size - 1; k1++)
                    {
                        for (k2 = 0; k2 <= size - 1; k2++)
                        {
                            if ((i + k1 < pictureBox1.Image.Width) && (j + k2 < pictureBox1.Image.Height))
                            {
                                box2.SetPixel(i + k1, j + k2, cc);
                            }
                        }
                    }

                }
            }
            pictureBox2.Refresh();
            pictureBox2.Image = box2;



        }

        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap box2 = GetAlphMap(box1);
            pictureBox2.Refresh();
            pictureBox2.Image = box2;
        }

        //测试放大镜算法
        private void button6_Click(object sender, EventArgs e)
        {
            Bitmap box2 = new Bitmap(pictureBox1.Image);         //Box2 为2号图片

            int xes = pictureBox1.Image.Width;
            int yes = pictureBox1.Image.Height;

            int[] VarsX = new int[xes];
            int[] VarsY = new int[yes];

            int[] EndLoctionX = CaculateLocation(VarsX, xes * 2, xes / 2);
            int[] EndLoctionY = CaculateLocation(VarsY, yes * 2, yes / 2);


            for (int i = 0; i <= xes - 1; i++)
            {
                for (int j = 0; j <= yes - 1; j++)
                {
                    Color color = box1.GetPixel(EndLoctionX[i], EndLoctionY[j]);
                    box2.SetPixel(i, j, color);
                }
            }
            //刷新图片
            pictureBox2.Refresh();
            pictureBox2.Image = box2;

        }
        //计算
        private int[] CaculateLocation(int[] Vars, int L2, int L1)
        {
            /********************************************
            Vars只是从1--width/height的一个数组，传入索引，也传出索引
            **********************************************/
            int[] EndLocation = new int[Vars.Length];
            //赋值，方便下一步计算
            for (int h = 0; h <= Vars.Length - 1; h++)
            {
                EndLocation[h] = 0;
            }
            for (int i = 0; i <= Vars.Length - 1; i++)
            {
                int Location = (-i * L2) / (L1 - L2);         //计算完成
                //判定传入
                if (Location <= Vars.Length - 1)
                {
                    EndLocation[Location] = i;
                }
            }
            //解析EndLocation，填充
            for (int j = 0; j <= Vars.Length - 1; j++)
            {
                if (EndLocation[j] == 0)
                {
                    if (j != 0)
                    {
                        EndLocation[j] = EndLocation[j - 1];
                    }

                }
            }
            return EndLocation;
        }
        //实现局部放大算法
        /*************************************************
        1，根据偏移量以及长宽（必须是图片长与宽的整数倍）来提取相应区域的像素数组
        2，使用caculateLocataion函数进行计算，返回目标数组
        3，使用目标数组进行定点替换。
        **************************************************/
        private Bitmap GetPartPixel(Bitmap bt, Point offset, Size rc)
        {
            Bitmap result = new Bitmap(rc.Width, rc.Height);  //从参数获得图像
            //获取传入图片参数
            int xes = rc.Width;
            int yes = rc.Height;
            int xesbt = bt.Width;
            int yesbt = bt.Height;

            for (int i = 0; i <= xes-1; i++)
            {
                for (int j = 0; j <= yes-1; j++)
                {
                    if((i + offset.X <= xesbt - 1) && (j + offset.Y <= yesbt - 1) && (i + offset.X >= 0) && (j + offset.Y >= 0))
                    {
                        Color color = bt.GetPixel(i+offset.X, j+offset.Y);
                        result.SetPixel(i , j , color);
                    }

                }
            }
            //貌似不需要填充。
            return result;
        }
        //替换
        private Bitmap ReplacePixel(Bitmap bt, Bitmap rp, Point offset)
        {
            Bitmap rpBig = new Bitmap(rp);

            int xes = rp.Width;
            int yes = rp.Height;

            int[] VarsX = new int[xes];
            int[] VarsY = new int[yes];

            int[] EndLoctionX = CaculateLocation(VarsX, xes * 2, xes / 2);
            int[] EndLoctionY = CaculateLocation(VarsY, yes * 2, yes / 2);

            for (int i = 0; i <= xes - 1; i++)
            {
                for (int j = 0; j <= yes - 1; j++)
                {
                    Color color = rp.GetPixel(EndLoctionX[i], EndLoctionY[j]);
                    rpBig.SetPixel(i, j, color);
                }
            }
            //进行替换，并且返回图像

            //获取传入图片参数
            int xesbt = bt.Width;
            int yesbt = bt.Height;


            for (int i = 0; i <= xes-1; i++)
            {
                for (int j = 0; j <= yes-1; j++)
                {
 
                    if ((i + offset.X <= xesbt - 1)&& (j +offset.Y <= yesbt - 1)&&(i+offset.X>=0)&&(j+offset.Y>=0))
                    {
                        Color color = rpBig.GetPixel(i, j);
                        bt.SetPixel(i + offset.X, j + offset.Y, color);
                    }
                }

            }
            return bt;
        }

        //区域放大算法的测试
        private void button7_Click(object sender, EventArgs e)
        {
            Bitmap result = new Bitmap(box1);
            Bitmap bt = GetPartPixel(box1, new Point(-100, -100), new Size(300, 300));
            result = ReplacePixel(result, bt, new Point(-100, -100));
            pictureBox2.Refresh();
            pictureBox2.Image = result;
        }
       //多区域放大覆盖算法。
       /**************************
       1，创建一个ArrayList，根据环形分别将各个方大过的局部图像存储于其中
       2，遍历替换
       3，更改总体偏移位置，生成多张图片，存储于ArrayList2中
       4，建立动画播放函数进行播放
       5，生成相关Gif文件。
       ****************/
       private Bitmap GetCircle(Bitmap bt,int Radius,Size RectangleSize)
        {
            Bitmap new1 = new Bitmap(bt);
            ArrayList ArrayForPoint = new ArrayList();
            ArrayList ArrayForCircle = new ArrayList();
            double coefficient = double.Parse(textBox2.Text);
            int Size = (int)(Radius * 2 * 3.1415/ RectangleSize.Width * coefficient);   //计算出要绘制的放大区域的数量
            Point point = new Point();
            for (double angle = 0.01; angle <= 2 * 3.1415; angle +=(2*3.1415/Size))
            {     
                point.X = (int)(Radius * Math.Cos(angle) + (pictureBox1.Image.Width / 2));
                point.Y = (int)(Radius * Math.Sin(angle) + (pictureBox1.Image.Height / 2));
                ArrayForPoint.Add(point);
                ArrayForCircle.Add(GetAChileBitmap(point, RectangleSize,true));
            }
            //遍历替换
            for(int i = 0; i <= ArrayForCircle.Count - 1; i++)
            {
                bt= ReplacePixel(new1,(Bitmap) ArrayForCircle[i],(Point)ArrayForPoint[i] );
            }
            return new1;
            
        }
        //传入偏移与大小，返回一个被放大的图片区域
        private Bitmap GetAChileBitmap(Point offset , Size size,bool Alph)
        {
            Bitmap bt = GetPartPixel(box1,offset, size);
            if (Alph)
            {
                bt = GetAlphMap(bt);
            }
            return bt; 
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Bitmap bt = new Bitmap(box1);
            Bitmap new1=GetCircle(bt, bt.Width/2-200, new Size(20, 20));
            pictureBox2.Refresh();
            pictureBox2.Image = new1;
        }
        //黑白变换函数
        private Bitmap GetAlphMap(Bitmap bt)
        {
            Bitmap box2 = new Bitmap(bt);
            int r, g, b, i, j, xes, yes, alph;
            Color c;
            Color cc;
            xes = bt.Width;
            yes = bt.Height;
            for (i = 0; i <= xes - 1; i++)
            {
                for (j = 0; j <= yes - 1; j++)
                {
                    c = bt.GetPixel(i, j);
                    r = c.R;
                    g = c.G;
                    b = c.B;
                    alph = (int)((r + g + b) / 3);
                    cc = Color.FromArgb(alph, alph, alph);
                    box2.SetPixel(i, j, cc);
                }
            }
            return box2;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        //偏移radius，生成多张图片
        private void GetLotsOfImage(int number)
        {
            int Max;
            int steps = number / 5;
            if (box1.Width >= box1.Height)
            {
                Max = box1.Width;
            }
            else
            {
                Max = box1.Height;
            }
            Bitmap bt = new Bitmap(box1);
            int RadiusUnit = (int)((Max/2) / number);
            for(int i = 0; i <= number - 1; i++)
            {
                ArrayForGIF.Add(GetCircle(bt, (bt.Width / 2 - (number - i) * RadiusUnit), new Size(20, 20)));
            }

            textBox1.Text+="正常";
            textBox1.Text += ArrayForGIF.Count.ToString();
            
            System.IO.Directory.CreateDirectory("D:\\环形水波纹效果");

            SaveFiles(number, steps, bt, RadiusUnit);

        }
        //分步输出与保存
        private void SaveFiles(int number ,int steps,Bitmap bt,int RadiusUnit)
        {
            number = number + 1;
            int Unit = number / steps;
            int Index;
            string FileName1 = "D:\\环形水波纹效果";
            string ImageFormat = ".PNG";
            for (int i = 0; i <= steps - 1; i++)
            {
                for (Index = i * Unit;( Index <= ((i + 1) * Unit))&&(Index<=number); Index++)
                {
                    ArrayForGIF.Add(GetCircle(bt, (bt.Width / 2 - (number - Index) * RadiusUnit), new Size(20, 20)));
                    //保存文件
                    for (int j = 0; j <= ArrayForGIF.Count - 1; j++)
                    {
                        ((Bitmap)ArrayForGIF[j]).Save(FileName1 + "\\" + Index.ToString() + ImageFormat);
                    }
                    //清空Array
                    ArrayForGIF.Clear();
                }

            }

        }
  

        private void button9_Click(object sender, EventArgs e)
        {
            int number = int.Parse(textBox3.Text);
            GetLotsOfImage(number);
            //第二线程播放动画
            
        }
        private void GetFlowImage(int number,int count,Size RectangleSize) 
        {

            string FileName1 = "D:\\环形水波纹效果";
            string ImageFormat = ".PNG";
            //count为波纹密集度
            int Max;
            if (box1.Width >= box1.Height)
            {
                Max = box1.Width;
            }
            else
            {
                Max = box1.Height;
            }
            int num1=count;   //新的波纹要产生的标识器
           
            int RadiusUnit = (int)((Max / 2) / number);
            //波纹生成器 Array
            ArrayList Generator = new ArrayList();
            for(int index = 0;index <= number - 1; index++)
            {
                //产生一个新的波纹，波纹自身index为int
                if (num1 == count)
                {
                    Generator.Add(1);
                    num1 = 0;
                }
                //生产过程
                //一个新的ArrayList，保存生成的图片片段
                Bitmap bt=ReturnNewBit(Generator, RadiusUnit, RectangleSize);
                //保存为一张图
                bt.Save(FileName1 + "\\" + index.ToString() + ImageFormat);
                //Generator对象自增
                for (int h = 0; h <= Generator.Count - 1; h++)
                {
                    Generator[h] = (int)Generator[h] + 1;
                }
                num1++;
                
            }
            textBox1.Text += "非常正常";
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }
        //不进行替换，返回一个数组的getCircle
        private ArrayList[] GetCircleArray(Bitmap bt, int Radius, Size RectangleSize)
        {
            ArrayList ArrayForPoint = new ArrayList();
            ArrayList ArrayForCircle = new ArrayList();
            double coefficient = double.Parse(textBox2.Text);
            int Size = (int)(Radius * 2 * 3.1415 / RectangleSize.Width * coefficient);   //计算出要绘制的放大区域的数量
            Point point = new Point();
            for (double angle = 0.01; angle <= 2 * 3.1415; angle += (2 * 3.1415 / Size))
            {
                point.X = (int)(Radius * Math.Cos(angle) + (pictureBox1.Image.Width / 2));
                point.Y = (int)(Radius * Math.Sin(angle) + (pictureBox1.Image.Height / 2));
                ArrayForPoint.Add(point);
                ArrayForCircle.Add(GetAChileBitmap(point, RectangleSize, true));
            }
            
            ArrayList[] AR = { ArrayForCircle, ArrayForPoint };
            return AR;

        }

        private void button10_Click(object sender, EventArgs e)
        {
            //获取信息
            int number = int.Parse(textBox4.Text);
            int count = int.Parse(textBox5.Text);
            Size size = new Size();
            size.Width = int.Parse(textBox6.Text);
            size.Height = int.Parse(textBox7.Text);

            System.IO.Directory.CreateDirectory("D:\\环形水波纹效果");

            GetFlowImage(number, count, size);

        }
        private Bitmap ReturnNewBit(ArrayList Generator,int RadiusUnit ,Size RectangleSize)
        {
            Bitmap bt = new Bitmap(box1);
            ArrayList Maps = new ArrayList();               //存放maps
            ArrayList Points = new ArrayList();               //存放Points
            for (int i = 0; i <= Generator.Count - 1; i++)
            {
                ArrayList[] a = GetCircleArray(bt, RadiusUnit * ((int)Generator[i]), RectangleSize);
                Maps = a[0];
                Points = a[1];
                //遍历替换
                for (int j = 0; j <= Maps.Count - 1; j++)
                {
                    bt = ReplacePixel(bt, (Bitmap)Maps[j], (Point)Points[j]);
                }

                //清空Array
                Maps.Clear();
                Points.Clear();
            }
            //保存为一张图
            return bt;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            string FileName1 = "D:\\环形水波纹效果";
            string ImageFormat = ".PNG";
            int number = int.Parse(textBox4.Text);
            String[] imageFilePaths = new string[number];
            for (int i = 0; i <= number - 1; i++)
            {
                imageFilePaths[i] = FileName1 + "\\" + i.ToString() + ImageFormat;
            }

            String outputFilePath = "d:\\test.gif";
            AnimatedGifEncoder e1 = new AnimatedGifEncoder();
            e1.Start(outputFilePath);
            e1.SetDelay(50);    // 延迟间隔
            e1.SetRepeat(0);  //-1:不循环,0:总是循环 播放  
            for (int j = 0; j <= number - 1; j++)
            {
                e1.AddFrame(Image.FromFile(imageFilePaths[j]));
            }
            e1.Finish();
        }
 
    }
     
    
}
