using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PictureComposite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = true;
        }

        string PictureUnder = string.Empty;

        private void button1_Click(object sender, EventArgs e)
        {
            if (PictureUnder == string.Empty)
            {
                MessageBox.Show("テンプレートを先に選択してください");
            }
            else
            {
                openFileDialog1.Title = "処理したいイメージを選択してください";
                openFileDialog1.Filter = "イメージ|*.jpg;*.gif;*.png;*.bmp|任意（*.*）|*.*";
                openFileDialog1.FileName = "";
                //openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                openFileDialog1.CheckFileExists = true;
                openFileDialog1.Multiselect = true;
                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.label1.ForeColor = System.Drawing.Color.Goldenrod;
                    this.label1.Text = "処理中です";
                    foreach (var file in openFileDialog1.FileNames)
                    {
                        Bitmap srcBmp = new Bitmap(file);
                        Bitmap dstBmp = srcBmp.Clone(new Rectangle(0, 0, 1920, 800), srcBmp.PixelFormat);
                        string[] path = file.Split('.');
                        dstBmp.Save(string.Format(path[0] + "_上" + "." + path[1]));
                        srcBmp.Dispose();
                        dstBmp.Dispose();

                        Image img1 = Image.FromFile(path[0] + "_上" + "." + path[1]);
                        Bitmap map1 = new Bitmap(img1);
                        Image img2 = Image.FromFile(PictureUnder);
                        Bitmap map2 = new Bitmap(img2);
                        var width = Math.Max(img1.Width, img2.Width);
                        var height = img1.Height + img2.Height + 10;
                        // 初始化画布(最终的拼图画布)并设置宽高
                        Bitmap bitMap = new Bitmap(width, height);
                        // 初始化画板
                        Graphics g1 = Graphics.FromImage(bitMap);
                        // 将画布涂为白色(底部颜色可自行设置)
                        g1.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
                        //在x=0，y=0处画上图一
                        g1.DrawImage(map1, 0, 0, img1.Width, img1.Height);
                        //在x=0，y在图一下处画上图二
                        g1.DrawImage(map2, 0, img1.Height, img2.Width, img2.Height);
                        Image img = bitMap;
                        //保存
                        img.Save(path[0] + "_係員" + "." + path[1]);

                        map1.Dispose();
                        map2.Dispose();
                        img1.Dispose();
                        img2.Dispose();
                        if (checkBox1.Checked == true)
                        {
                            File.Delete(path[0] + "_上" + "." + path[1]);
                            File.Delete(file);
                            File.Move(path[0] + "_係員" + "." + path[1], file);
                        }
                    }
                    this.label1.ForeColor = System.Drawing.Color.Green;
                    this.label1.Text = "処理が完了しました";
                    this.timer1.Enabled = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "テンプレートを選択してください";
            openFileDialog1.Filter = "イメージ|*.jpg;*.gif;*.png;*.bmp|任意（*.*）|*.*";
            openFileDialog1.FileName = "";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.Multiselect = false;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (var file in openFileDialog1.FileNames)
                {
                    PictureUnder = Picture2(file);
                }
            }
            this.button2.Text = "テンプレートが\n選択しました";
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Text = "テンプレートが選択しました";
        }

        private void Picture1(string fi)
        {
        }

        private string Picture2(string fi)
        {
            Bitmap srcBmp = new Bitmap(fi);
            Bitmap dstBmp = srcBmp.Clone(new Rectangle(0, 800, 1920, 280), srcBmp.PixelFormat);
            string[] path = fi.Split('.');
            dstBmp.Save(string.Format(path[0] + "_下" + "." + path[1]));
            srcBmp.Dispose();
            if (checkBox1.Checked == true)
            {
                File.Delete(fi);
            }
            dstBmp.Dispose();
            return path[0] + "_下" + "." + path[1];
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.timer1.Enabled = false;
            if (PictureUnder==string.Empty)
            {
                this.label1.ForeColor = System.Drawing.Color.Red;
                this.label1.Text = "処理するイメージを選択する前に\r\nテンプレートを選択してください";
            }
            else
            {
                this.label1.ForeColor = System.Drawing.Color.Gray;
                this.label1.Text = "テンプレートが選択しました";
            }
        }
    }
}
