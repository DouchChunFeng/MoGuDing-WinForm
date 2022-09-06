using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.IO;

namespace gxy
{
    public partial class AddProfiles : Form
    {
        public bool isadd = false;
        public string position = "0";
        public AddProfiles(bool isAdd, string[] data)
        {
            InitializeComponent();
            if (isAdd)
            {
                isadd = true;
                this.Text += " (添加模式)";
            }
            else
            {
                isadd = false;
                textBox1.Text = data[0];
                textBox2.Text = EncryptandDecipher.Decipher(data[1]);
                textBox3.Text = data[2];
                textBox4.Text = data[3];
                textBox5.Text = data[4];
                textBox6.Text = data[5];
                textBox7.Text = data[6];
                textBox8.Text = data[7];
                position = data[8];
                this.Text += " (编辑模式)";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //根据地址获取经纬度
            if (textBox3.Text.Length > 0)
            {
                string responseJson = http.Get("http://api.map.baidu.com/geocoder?output=json&address=" + textBox3.Text);
                try
                {
                    JObject jobject = JObject.Parse(responseJson);
                    if (!jobject["status"].ToString().Equals("OK"))
                    {
                        Thread dxc = new Thread(() => MessageBox.Show("经纬度获取不成功!\n请尝试输入更详细的地址!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error));
                        dxc.Start();
                        return;
                    }
                    string confidence = jobject["result"]["confidence"].ToString(); //经纬度可信任度
                    string level = jobject["result"]["level"].ToString(); //位置等级
                    string jd = jobject["result"]["location"]["lng"].ToString(); //经度
                    string wd = jobject["result"]["location"]["lat"].ToString(); //纬度

                    if (Int32.Parse(confidence) < 50)
                    {
                        Thread dxc = new Thread(() => MessageBox.Show(string.Format("获取到的经纬度可信任度为{0} 级别:({1})过低 ,\n推荐您补全地址重新获取.", confidence, level), "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning));
                        dxc.Start();
                    }
                    textBox7.Text = jd;
                    textBox8.Text = wd;
                }
                catch(Exception ex)
                {
                    Thread dxc = new Thread(() => MessageBox.Show("由于请求过于频繁或网络错误,\n获取经纬度失败,请稍后再试.", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    dxc.Start();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (isadd)
            {
                if (textBox1.Text.Length < 1 || textBox2.Text.Length < 1 || textBox3.Text.Length < 1 || textBox4.Text.Length < 1 || textBox5.Text.Length < 1 || textBox6.Text.Length < 1 || textBox7.Text.Length < 1 || textBox8.Text.Length < 1)
                {
                    Thread dxc = new Thread(() => MessageBox.Show("缺项无填", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    dxc.Start();
                    return;
                }
                string[] data = { textBox1.Text, EncryptandDecipher.Encrypt(textBox2.Text), textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text };

                Form1.f1.Invoke(new Action(() =>
                {
                    Form1.f1.AddItems(data);
                }));
                this.Close();
                this.Dispose();
            }
            else
            {
                if (textBox1.Text.Length < 1 || textBox2.Text.Length < 1 || textBox3.Text.Length < 1 || textBox4.Text.Length < 1 || textBox5.Text.Length < 1 || textBox6.Text.Length < 1 || textBox7.Text.Length < 1 || textBox8.Text.Length < 1)
                {
                    Thread dxc = new Thread(() => MessageBox.Show("缺项无填", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    dxc.Start();
                    return;
                }
                string[] data = { textBox1.Text, EncryptandDecipher.Encrypt(textBox2.Text), textBox3.Text, textBox4.Text, textBox5.Text, textBox6.Text, textBox7.Text, textBox8.Text };
                Form1.f1.Invoke(new Action(() =>
                {
                    Form1.f1.ChangeItems(position, data);
                }));
                this.Close();
                this.Dispose();
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text.Length <= 0)
            {
                button2.Enabled = false;
                return;
            }
            if (button2.Enabled == false && textBox3.Text.Length > 0)
            {
                button2.Enabled = true;
                return;
            }
        }
    }
}
