using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace gxy
{
    public partial class Form1 : Form
    {
        public static Form1 f1;
        public string year = "", month = "", day = "", hour = "", minute = "", second = ""; //日期
        public string files_data_local = Directory.GetCurrentDirectory() + @"\UserData.txt"; //数据文件路径
        public string logs_data_local = Directory.GetCurrentDirectory() + @"\UserData_Logs.txt"; //日志文件路径
        public static JArray files_data = new JArray(); //数据
        public bool isStart = false; //是否已经启动打卡
        public string sj_minute_start = "00"; //随机到的上班分钟
        public string sj_minute_stop = "00"; //随机到的下班分钟

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            f1 = this;
        }
        private void Form1_Shown(object sender, EventArgs e)
        {
            //打开时读取数据
            if (File.Exists(files_data_local))
            {
                try
                {
                    JArray jarray = JArray.Parse(File.ReadAllText(files_data_local));
                    files_data = jarray;
                    listView1.BeginUpdate();
                    foreach(JToken jtoken in jarray)
                    {
                        ListViewItem ls = new ListViewItem();
                        ls.Text = jtoken["账号"].ToString();
                        ls.SubItems.Add(jtoken["密码"].ToString());
                        ls.SubItems.Add(jtoken["详细地址"].ToString());
                        ls.SubItems.Add(jtoken["国家"].ToString());
                        ls.SubItems.Add(jtoken["省份"].ToString());
                        ls.SubItems.Add(jtoken["地市"].ToString());
                        ls.SubItems.Add(jtoken["经度"].ToString());
                        ls.SubItems.Add(jtoken["纬度"].ToString());
                        listView1.Items.Add(ls);
                    }
                    listView1.EndUpdate();
                    Log("读取用户信息成功!");
                }
                catch(Exception ex)
                {
                    listView1.EndUpdate();
                    Log("读取用户配置文件时出现错误: \n" + ex.Message);
                    Thread dxc = new Thread(() => MessageBox.Show("读取用户配置文件时出现错误: \n" + ex.Message, "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error));
                    dxc.Start();
                }
            }
            LoginCache.Get().Init(); 
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isStart)
            {
                if (MessageBox.Show("正在运行中,确定要关闭程序?", "Are you OK?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (listView1.Items.Count < 1) return;
            isStart = true;
            groupBox1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = true;
            Log("用户启用自动打卡");
            随机上下班时间();
            开或关ToolStripMenuItem.Text = "停止(&S)";
        }
        private void button2_Click(object sender, EventArgs e)
        {
            isStart = false;
            groupBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = false;
            label2.Text = "--:--";
            label9.Text = "--:--";
            Log("用户关闭自动打卡");
            开或关ToolStripMenuItem.Text = "启动(&K)";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime date = DateTime.Now;
            year = date.Year.ToString();
            month = date.Month.ToString();
            day = date.Day.ToString();
            hour = date.Hour.ToString();
            minute = date.Minute.ToString();
            second = date.Second.ToString();

            //如果长度小于2位 加零；
            if (month.Length < 2)
            {
                month = "0" + month;
            }
            if (day.Length < 2)
            {
                day = "0" + day;
            }
            if (hour.Length < 2)
            {
                hour = "0" + hour;
            }
            if (minute.Length < 2)
            {
                minute = "0" + minute;
            }
            if (second.Length < 2)
            {
                second = "0" + second;
            }

            label7.Text = year + "-" + month + "-" + day + " " + hour + ":" + minute + ":" + second;

            
            if (isStart)
            {
                string hms = hour + ":" + minute + ":" + second;
                if (hms.Equals("00:00:05"))
                {
                    随机上下班时间();
                }
                if (hms.Equals(label2.Text + ":05") && checkBox1.Checked)
                {
                    执行多个打卡任务("START");
                }
                if (hms.Equals(label9.Text + ":10") && checkBox2.Checked)
                {
                    执行多个打卡任务("END");
                }
            }
        }
        public void 执行多个打卡任务(string state)
        {
            foreach (ListViewItem ls in listView1.Items)
            {
                Thread dxc = new Thread(() =>
                {
                    lock (this)
                    {
                        Log(daka.执行单个打卡任务(ls.Text, EncryptandDecipher.Decipher(ls.SubItems[1].Text), state, ls.SubItems[2].Text, ls.SubItems[3].Text, ls.SubItems[4].Text, ls.SubItems[5].Text, ls.SubItems[6].Text, ls.SubItems[7].Text));
                    }
                });
                dxc.Start();
            }
        }

        public void 随机上下班时间()
        {
            Random rm = new Random();
            if (textBox3.Text.Length == 1)
            {
                textBox3.Text = "0" + textBox3.Text;
            }
            else if (textBox3.Text.Length < 1)
            {
                button2_Click(null, null);
                Log("上班时间小时设置失败：未知格式");
                MessageBox.Show("上班时间小时设置失败: 未知格式\n例: 05或5均可", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (textBox4.Text.Length == 1)
            {
                textBox4.Text = "0" + textBox4.Text;
            }
            else if (textBox4.Text.Length < 1)
            {
                button2_Click(null, null);
                Log("下班时间小时设置失败：未知格式");
                MessageBox.Show("下班时间小时设置失败: 未知格式\n例: 05或5均可", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (textBox5.Text.Contains('-'))
            {
                string[] split1 = textBox5.Text.Split('-');
                string minute1 = rm.Next(Int32.Parse(split1[0]), Int32.Parse(split1[1])).ToString();
                if (minute1.Length < 2)
                {
                    minute1 = "0" + minute1;
                }
                label2.Text = string.Format("{0}:{1}", textBox3.Text, minute1);
            }
            else
            {
                if (textBox5.Text.Length > 2)
                {
                    button2_Click(null, null);
                    Log("上班时间分钟设置失败：未知格式");
                    MessageBox.Show("上班固定时间设置失败: 未知格式\n例: 05或5均可", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (textBox5.Text.Length == 2)
                {
                    label2.Text = textBox3.Text + ":" + textBox5.Text.Substring(0, 2);
                }
                else if (textBox5.Text.Length == 1)
                {
                    label2.Text = textBox3.Text + ":0" + textBox5.Text;
                    textBox5.Text = "0" + textBox5.Text;
                }
                else
                {
                    label2.Text = textBox3.Text + ":00";
                    textBox5.Text = "00";
                }
            }

            if (textBox6.Text.Contains('-'))
            {
                string[] split2 = textBox6.Text.Split('-');
                string minute2 = rm.Next(Int32.Parse(split2[0]), Int32.Parse(split2[1])).ToString();
                if (minute2.Length < 2)
                {
                    minute2 = "0" + minute2;
                }
                label9.Text = string.Format("{0}:{1}", textBox4.Text, minute2);
            }
            else
            {
                if (textBox6.Text.Length > 2)
                {
                    button2_Click(null, null);
                    Log("下班时间分钟设置失败：未知格式");
                    MessageBox.Show("下班固定时间设置失败: 未知格式\n例: 05或5均可", "Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else if (textBox6.Text.Length == 2)
                {
                    label9.Text = textBox4.Text + ":" + textBox6.Text.Substring(0, 2);
                }
                else if (textBox6.Text.Length == 1)
                {
                    label9.Text = textBox4.Text + ":0" + textBox6.Text;
                    textBox6.Text = "0" + textBox6.Text;
                }
                else
                {
                    label9.Text = textBox4.Text + ":00";
                    textBox6.Text = "0" + textBox6.Text;
                }
            }
            Log(string.Format("刷新上下班签到时间为: {0},{1}", label2.Text, label9.Text));
        }

        public void Log(string str)
        {
            if (str == "")
            {
                return;
            }
            if (year.Equals(""))
            {
                month = DateTime.Now.Month.ToString();
                day = DateTime.Now.Day.ToString();
                hour = DateTime.Now.Hour.ToString();
                minute = DateTime.Now.Minute.ToString();
                second = DateTime.Now.Second.ToString();
                if (month.Length < 2)
                {
                    month = "0" + month;
                }
                if (day.Length < 2)
                {
                    day = "0" + day;
                }
                if (hour.Length < 2)
                {
                    hour = "0" + hour;
                }
                if (minute.Length < 2)
                {
                    minute = "0" + minute;
                }
                if (second.Length < 2)
                {
                    second = "0" + second;
                }
                string mylogss = "\r\n[" + DateTime.Now.Year.ToString() + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second + "] " + str;
                textBox1.AppendText(mylogss);
                File.AppendAllText(logs_data_local, mylogss);
                return;
            }
            string mylogs = "\r\n[" + year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second + "] " + str;
            textBox1.AppendText(mylogs);
            File.AppendAllText(logs_data_local, mylogs);
        }
        private void listView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (listView1.SelectedItems.Count > 0)
                {
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenAddItemsWindow();
        }
        public void OpenAddItemsWindow()
        {
            AddProfiles form = new AddProfiles(true, null);
            form.Show();
        }
        public void AddItems(string[] data)
        {
            //先加到列表
            listView1.BeginUpdate();
            ListViewItem ls = new ListViewItem();
            ls.Text = data[0];
            ls.SubItems.Add(data[1]);
            ls.SubItems.Add(data[2]);
            ls.SubItems.Add(data[3]);
            ls.SubItems.Add(data[4]);
            ls.SubItems.Add(data[5]);
            ls.SubItems.Add(data[6]);
            ls.SubItems.Add(data[7]);
            listView1.Items.Add(ls);
            listView1.EndUpdate();
            //修改文件
            JObject jobject = new JObject { { "账号", data[0] }, { "密码", data[1] }, { "详细地址", data[2] }, { "国家", data[3] }, { "省份", data[4] }, { "地市", data[5] }, { "经度", data[6] }, { "纬度", data[7] } };
            files_data.Add(jobject);
            File.WriteAllText(files_data_local, files_data.ToString()); //写入文本.
            Log("添加新用户: " + data[0] + "成功!");
        }

        private void 编辑此项EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenEditItemsWindow(listView1.SelectedIndices[0].ToString());
        }
        public void OpenEditItemsWindow(string position)
        {
            ListViewItem ls = listView1.Items[Int32.Parse(position)];
            string[] data = { ls.Text, ls.SubItems[1].Text, ls.SubItems[2].Text, ls.SubItems[3].Text, ls.SubItems[4].Text, ls.SubItems[5].Text, ls.SubItems[6].Text, ls.SubItems[7].Text, position };
            AddProfiles form = new AddProfiles(false, data);
            form.Show();
        }
        public void ChangeItems(string strposition, string[] data)
        {
            int position = Int32.Parse(strposition);
            //先修改列表
            listView1.BeginUpdate();
            ListViewItem ls = listView1.Items[position];
            ls.Text = data[0];
            ls.SubItems[1].Text = data[1];
            ls.SubItems[2].Text = data[2];
            ls.SubItems[3].Text = data[3];
            ls.SubItems[4].Text = data[4];
            ls.SubItems[5].Text = data[5];
            ls.SubItems[6].Text = data[6];
            ls.SubItems[7].Text = data[7];
            listView1.EndUpdate();

            //修改文件
            JObject jobject = (JObject)files_data[position];
            jobject["账号"] = data[0];
            jobject["密码"] = data[1];
            jobject["详细地址"] = data[2];
            jobject["国家"] = data[3];
            jobject["省份"] = data[4];
            jobject["地市"] = data[5];
            jobject["经度"] = data[6];
            jobject["纬度"] = data[7];
            File.WriteAllText(files_data_local, files_data.ToString()); //写入文本.
            Log(string.Format("修改序列{0}-用户[{1}]信息成功!", strposition, data[0]));
        }
        private void 删除该项ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                Thread dxc = new Thread(() =>
                {
                    if (MessageBox.Show("您确定删除此条目?", "Are you Sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        int position = listView1.SelectedIndices[0];
                        string account = listView1.Items[position].Text;
                        if (LoginCache.Get().isExist(account))
                        {
                            LoginCache.Get().Delete(LoginCache.Get().getIndex(account));
                        }
                        files_data.RemoveAt(position); //删除变量里的该行。
                        File.WriteAllText(files_data_local, files_data.ToString()); //写入文本.
                        listView1.Items[position].Remove(); //删除列表中的行。
                    }
                });
                dxc.Start();
            }
        }

        private void 添加项AToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenAddItemsWindow();
        }
        private void 开或关ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (开或关ToolStripMenuItem.Text.Equals("启动(&K)"))
            {
                if (button1.Enabled)
                {
                    button1_Click(null, null);
                }
                else
                {
                    开或关ToolStripMenuItem.Text = "停止(&S)";
                }
            }
            else
            {
                if (button2.Enabled)
                {
                    button2_Click(null, null);
                }
                else
                {
                    开或关ToolStripMenuItem.Text = "启动(&K)";
                }
            }
        }
        private void 退出QToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;
                this.Visible = false;
                this.Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            this.Visible = true;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }

        

        

    }
}
