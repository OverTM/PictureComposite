﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace PictureComposite
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = true;
        }

        int FullSizeW = 1024, FullSizeH = 838;
        int CutOutStartLocationX = 840, CutOutStartLocationY = 270;
        int CutOutEndLocationX = 1015, CutOutEndLocationY = 322;
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
                        Image img1 = Image.FromFile(file);
                        Bitmap map1 = new Bitmap(img1);
                        Image img2 = Image.FromFile(PictureUnder);
                        Bitmap map2 = new Bitmap(img2);
                        var width = FullSizeW;
                        var height = FullSizeH;
                        // 初始化画布(最终的拼图画布)并设置宽高
                        Bitmap bitMap = new Bitmap(width, height);
                        // 初始化画板
                        Graphics g1 = Graphics.FromImage(bitMap);
                        // 将画布涂为白色(底部颜色可自行设置)
                        g1.FillRectangle(Brushes.White, new Rectangle(0, 0, width, height));
                        //在x=0，y=0处画上图一
                        g1.DrawImage(map1, 0, 0, img1.Width, img1.Height);
                        //在x=0，y在图一下处画上图二
                        g1.DrawImage(map2, CutOutStartLocationX, CutOutStartLocationY, img2.Width, img2.Height);
                        Image img = bitMap;
                        //保存
                        string SavePath1 = Path.GetDirectoryName(file) + "\\" + Path.GetFileNameWithoutExtension(file) + "_处理完成" + Path.GetExtension(file);
                        img.Save(SavePath1);

                        map1.Dispose();
                        map2.Dispose();
                        img1.Dispose();
                        img2.Dispose();
                        if (checkBox1.Checked == true)
                        {
                            File.Delete(file);
                            File.Move(SavePath1, file);
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
            string SavePath = Path.GetDirectoryName(fi) + "\\" + "模板" + "_裁剪" + Path.GetExtension(fi);
            Bitmap srcBmp = new Bitmap(fi);
            Bitmap dstBmp = srcBmp.Clone(new Rectangle(CutOutStartLocationX, CutOutStartLocationY,
                CutOutEndLocationX - CutOutStartLocationX, CutOutEndLocationY - CutOutStartLocationY), srcBmp.PixelFormat);
            dstBmp.Save(SavePath);
            srcBmp.Dispose();
            //if (checkBox1.Checked == true)
            //{
            //    File.Delete(fi);
            //}
            File.Move(fi, fi.Replace(Path.GetFileNameWithoutExtension(fi), "模板"));
            dstBmp.Dispose();
            return SavePath;
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

        private void button3_Click(object sender, EventArgs e)
        {
            string Settings = Interaction.InputBox(string.Format("请在一行内按以下格式输入（中间用空格或逗号隔开）\n\n  图片宽,\n  图片高,\n  模板图片裁剪起始位置X轴," +
                "\n  模板图片裁剪起始位置Y轴,\n  模板图片裁剪结束位置X轴,\n  模板图片裁剪结束位置Y轴"), "图片尺寸与位置设置",
                string.Format("{0}　{1}　{2}　{3}　{4}　{5}",
                FullSizeW.ToString(), FullSizeH.ToString(), CutOutStartLocationX.ToString(),
                CutOutStartLocationY.ToString(), CutOutEndLocationX.ToString(), CutOutEndLocationY.ToString()), -1, -1);
            try
            {
                string[] str = Settings.Split(' ', ',', '，','　');
                str = str.Where(s => !string.IsNullOrEmpty(s)).ToArray();//去掉空字符串

                FullSizeW = int.Parse(str[0]);
                FullSizeH = int.Parse(str[1]);
                CutOutStartLocationX = int.Parse(str[2]);
                CutOutStartLocationY = int.Parse(str[3]);
                CutOutEndLocationX = int.Parse(str[4]);
                CutOutEndLocationY = int.Parse(str[5]);
            }
            catch
            {
                MessageBox.Show("输入无效！");
            }
        }
    }
}
