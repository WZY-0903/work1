using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Threading;
using System.Runtime.Remoting.Messaging;
//using Routrek.SSHC;
using System.Net.Sockets;
using System.IO;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Timers;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using BlackMinerTool;
using System.Resources;
using System.Reflection;
using BitMainMinerTool;

namespace work1
{
    public partial class Form1 : Form
    {
        private Color defaultItemColor;
        private bool next_auto_flag = true;
        private int checked_num = 0;
        static private double ghs_crit = 12300;
        public static int MACHIN_ID = 0;
        static private bool _overTest = false;




        ResourceManager res = new ResourceManager("work1.Form1", Assembly.GetExecutingAssembly());
        List<ipBlock> ipBlockList = new List<ipBlock>();
        List<IPAddress> IPList = new List<IPAddress>();
        private void ShowList1()
        {
            listView1.Clear();
            //设置listView的显示属性
            listView1.GridLines = true;
            listView1.FullRowSelect = true;
            listView1.View = View.Details;
            listView1.Scrollable = true;
            listView1.MultiSelect = false;
            listView1.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            // 针对数据库的字段名称，建立与之适应显示表头
            listView1.Columns.Add("ID", 25, HorizontalAlignment.Center);
            listView1.Columns.Add("IP Block", 100, HorizontalAlignment.Center);
            listView1.Columns.Add("Start", 60, HorizontalAlignment.Center);
            listView1.Columns.Add("End", 60, HorizontalAlignment.Center);

            //添加列表项
            for (int index = 0; index < ipBlockList.Count; index++)
            { 
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();
                item.SubItems[0].Text = (index + 1).ToString();
                item.SubItems.Add(ipBlockList[index].ip_block);
                item.SubItems.Add(ipBlockList[index].start.ToString());
                item.SubItems.Add(ipBlockList[index].end.ToString());
                listView1.Items.Add(item);
            }
            listView1.MultiSelect = true;
            listView1.FullRowSelect = true;
        }
        private void ShowList2()
        {
            //listView2.Clear();
            //listView2.View = View.Details;
            //listView2.GridLines = true;
            //listView2.FullRowSelect = true;
            //listView2.Columns.Add("IP Address", 150, HorizontalAlignment.Left);

            //foreach (ipBlock block in ipBlockList)
            //{
            //    for (int i = block.start; i <= block.end; i++)
            //    {
            //        string ip = $"{block.ip_block}.{i}";
            //        ListViewItem item = new ListViewItem(ip);
            //        listView2.Items.Add(item);
            //    }
            //}
            listView2.Clear();

            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.View = View.Details;
            listView2.Scrollable = true;
            listView2.MultiSelect = false;
            listView2.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            listView2.Columns.Add("ID", 25, HorizontalAlignment.Center);
            listView2.Columns.Add("IP", 100, HorizontalAlignment.Center);
            listView2.Columns.Add("STATUS", 60, HorizontalAlignment.Center);
            listView2.Columns.Add("Pool1", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker1", 50, HorizontalAlignment.Center);

            listView2.Columns.Add("Pool2", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker2", 50, HorizontalAlignment.Center);

            listView2.Columns.Add("Pool3", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker3", 50, HorizontalAlignment.Center);

            // 添加列表项
            for (int index = 0; index < IPList.Count; index++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();
                item.SubItems[0].Text = (index + 1).ToString();
                item.SubItems.Add(IPList[index].ToString());
                listView2.Items.Add(item);
            }
            listView2.MultiSelect = true;
            listView2.FullRowSelect = true;
            button4.Enabled = true;
            enable_buttons();
        }
        private void enable_buttons()
        {
            button11.Enabled = true;
            button10.Enabled = true;
            button9.Enabled = true;
            button12.Enabled = true;
            button17.Enabled = true;
            button16.Enabled = true;
            button15.Enabled = true;
            button14.Enabled = true;
            button13.Enabled = true;
            button24.Enabled = true;
            button21.Enabled = true;
            button22.Enabled = true;
            button8.Enabled = updatePwdChangeButton();
        }
        private bool CheckIPBlock(string text)
        {
            return Regex.IsMatch(text, "^[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}$"); 
        }
        private IPAddress ParseIPFormate(string text)
        {
            IPAddress iP = IPAddress.None;
            iP = IPAddress.Parse(text);
            return iP;
        }
        private bool CheckEndIPFormate(string text)
        {
            int ip = 0;
            try
            {
                ip = int.Parse(text);
                if(ip < 0 || ip >255)
                {
                    MessageBox.Show(res.GetString("InputOutOfRange"));
                    ip = 0;
                    return false;                
                }
            }
            catch(Exception error)
            {
                MessageBox.Show(res.GetString("InputOutOfRangeWithError") + error.Message);
                return false;
            }
            return true;
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string IPBlock;
            int StartIP;
            int EndIP;
            IPBlock = textBox1.Text.Trim();
            if( !CheckIPBlock(textBox1.Text))
            {
                MessageBox.Show(res.GetString("FormatError"));
                return;
            }
            if(!CheckEndIPFormate(startip.Text))
            {
                startip.Clear();
                return;
            }
            if (!CheckEndIPFormate(endip.Text))
            {
                endip.Clear();
                return;
            }
            StartIP = int.Parse(startip.Text);
            EndIP = int.Parse(endip.Text);
            if (StartIP > EndIP)
            {
                MessageBox.Show(res.GetString("StartIpGreaterThanEndIp"));
                startip.Clear();
                endip.Clear();
                return;
            }
            ipBlock nIp = new ipBlock();
            nIp.ip_block = IPBlock;
            nIp.start = StartIP;
            nIp.end = EndIP;

            foreach (ipBlock l in ipBlockList)
            {
                if (nIp.ip_block == l.ip_block)
                {
                    if (nIp.end < l.start || nIp.start > l.end)
                    {

                    }
                    else
                    {
                        MessageBox.Show("OverLap ip block range !");
                        startip.Clear();
                        endip.Clear();
                        return;
                    }
                }
            }

            ipBlockList.Add(nIp);

            ShowList1();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ShowList2_state()
        {
            listView2.Clear();

            //设置listView的显示属性
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.View = View.Details;
            listView2.Scrollable = true;
            listView2.MultiSelect = false;
            listView2.HeaderStyle = ColumnHeaderStyle.Clickable;

            listView2.Columns.Add("ID", 25, HorizontalAlignment.Center);
            listView2.Columns.Add("IP", 100, HorizontalAlignment.Center);

            listView2.Columns.Add("Aging Test", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("MAC", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("SN1", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("SN2", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("SN3", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("Aging Time", 80, HorizontalAlignment.Center);

            listView2.Columns.Add("Miner STATUS", 60, HorizontalAlignment.Center);

            listView2.Columns.Add("Type", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("Version", 80, HorizontalAlignment.Center);

            listView2.Columns.Add("freq", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("Elapsed", 80, HorizontalAlignment.Center);
            listView2.Columns.Add("Hash Rate(5S)", 70, HorizontalAlignment.Center);
            listView2.Columns.Add("Hash Rate(avg)", 70, HorizontalAlignment.Center);

            listView2.Columns.Add("HW", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("HWP", 50, HorizontalAlignment.Center);

            listView2.Columns.Add("Temp", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("Fan", 50, HorizontalAlignment.Center);

            listView2.Columns.Add("Pool1", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("Pool2", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker", 50, HorizontalAlignment.Center);
            listView2.Columns.Add("Pool3", 150, HorizontalAlignment.Center);
            listView2.Columns.Add("Worker", 50, HorizontalAlignment.Center);

            listView2.ListViewItemSorter = new ListViewColumnSorter();
            listView2.ColumnClick += new ColumnClickEventHandler(ListViewHelper.ListView_ColumnClick);

            // 添加列表项
            for (int index = 0; index < IPList.Count; index++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();
                item.SubItems[0].Text = (index + 1).ToString();
                item.SubItems.Add(IPList[index].ToString());
            }
            listView2.MultiSelect = true;
            listView2.FullRowSelect = true;
            if (next_auto_flag)
            {
                button11.Enabled = true;
                button10.Enabled = true;
                button9.Enabled = true;
                button4.Enabled = true;
            }
        }


        private void ShowList3()
        {
            listView3.Clear();

            //设置listView的显示属性
            listView3.GridLines = true;
            listView3.FullRowSelect = true;
            listView3.View = View.Details;
            listView3.Scrollable = true;
            listView3.MultiSelect = false;
            listView3.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            // 针对数据库的字段名称，建立与之适应显示表头
            listView3.Columns.Add("IP", 150, HorizontalAlignment.Center);
            listView3.Columns.Add("STATUS", 100, HorizontalAlignment.Center);


            //添加列表项
            for (int index = 0; index < IPList.Count; index++)
            {
                ListViewItem item = new ListViewItem();
                item.SubItems.Clear();
                item.SubItems[0].Text = IPList[index].ToString();
                listView3.Items.Add(item);
            }
            listView3.MultiSelect = true;
            listView3.FullRowSelect = true;
            button4.Enabled = true;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ipBlockList.RemoveAt(listView1.SelectedItems[0].Index);
            }
            List<int> indexToRemove = new List<int>();
            for (int i = 0; i < listView1.SelectedItems.Count; i++)
            {
                if (listView1.Items[i].Checked)
                {
                    indexToRemove.Add(i);
                }
            }
            indexToRemove.Reverse();
            foreach (int index in indexToRemove)
            {
                if (index >= 0 && index < ipBlockList.Count)
                {
                    ipBlockList.RemoveAt(index);
                }
            }
            ShowList1();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV Files (*.csv)|*.csv|All files(*.*)|*.*";
            openFileDialog.Title = res.GetString("OpenFileDialog_Title");

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filepath = openFileDialog.FileName;
                string ext = Path.GetExtension(filepath).ToLower();
                if (ext != ".csv" && ext != ".txt")
                {
                    MessageBox.Show(res.GetString("Error_FileFormat"));
                    return;
                }
                
                List<string> errorLines = new List<string>();
                try
                {
                    string[] lines = File.ReadAllLines(filepath);
                    if (lines.All(line =>string.IsNullOrWhiteSpace(line)))
                    {
                        MessageBox.Show(res.GetString("Error_FileEmpty"));
                        return;
                    }
                    bool CheckIPBlockValue(string ipBlock)
                    {
                        string[] parts = ipBlock.Split('.');
                        if (parts.Length != 3) return false;
                        foreach(string part in parts)
                        {
                            if (!int.TryParse(part, out int num)) return false;
                            if (num < 0 || num > 255) return false;
                        }
                        return true;
                    }

                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.Trim().Replace("\t", "").Replace(",", "") == "") ;
                        string[] parts = line.Split(',');
                        if (parts.Length != 3)
                        {
                            if (line.Contains('，'))
                            {
                                string msg = string.Format(res.GetString("Error_SplitterWrong"), line);
                                errorLines.Add(msg);
                            }
                            else
                            {
                                string msg = string.Format(res.GetString("Error_ColumnCount"), line);
                                errorLines.Add(msg);
                            }
                            continue;
                        }

                        string ipBlock = parts[0].Trim();
                        string startIpStr = parts[1].Trim();
                        string endIpStr = parts[2].Trim();

                        if (ipBlock.Contains(" ") || startIpStr.Contains(" ") || endIpStr.Contains(" "))
                        {
                            string msg = string.Format(res.GetString("Error_SpaceInData"), ipBlock, startIpStr, endIpStr);
                            errorLines.Add(msg);
                            continue;
                        }
                        if (!CheckIPBlock(ipBlock) || !CheckIPBlockValue(ipBlock))
                        {
                            string msg = string.Format(res.GetString("Error_IPBlockFormat"), ipBlock);
                            errorLines.Add(msg);
                            continue;
                        }
                        if (!int.TryParse(startIpStr,out int startIp) || !int.TryParse(endIpStr, out int endIp))
                        {
                            string msg = string.Format(res.GetString("Error_StartEndIPFormat"), startIpStr, endIpStr);
                            errorLines.Add(msg);
                            continue;
                        }
                        if (startIp < 0 || startIp > 255 || endIp < 0 || endIp > 255)
                        {
                            string msg = string.Format(res.GetString("Error_IPRange"), startIp, endIp);
                            errorLines.Add(msg);
                            continue;
                        }
                        if (startIp > endIp)
                        {
                            string msg = string.Format(res.GetString("Error_StartGTEnd"), startIp, endIp);
                            errorLines.Add(msg);
                            continue;
                        }
                        bool isOverlap = false;
                        foreach(ipBlock existing in ipBlockList)
                        {
                            if (existing.ip_block == ipBlock)
                            {
                                if (!(endIp < existing.start || startIp >existing.end))
                                {
                                    isOverlap = true;
                                    break;
                                }
                            }
                        }
                        if (isOverlap)
                        {
                            string msg = string.Format(res.GetString("Error_IPOverlap"), ipBlock, startIp, endIp);
                            errorLines.Add(msg);
                            continue;
                        }
                        ipBlock newIpBlock = new ipBlock()
                        {
                            ip_block = ipBlock,
                            start = startIp,
                            end = endIp
                        };
                        ipBlockList.Add(newIpBlock);
                        for (int i = newIpBlock.start; i <= newIpBlock.end; i++)
                        {
                            string fullIp = $"{newIpBlock.ip_block}.{i}";
                            if (IPAddress.TryParse(fullIp, out IPAddress ip))
                            {
                                IPList.Add(ip);
                            }
                        }
                    }
                    ShowList1();
                    ShowList2();
                    if (errorLines.Count > 0)
                    {
                        string msg = string.Join(Environment.NewLine, errorLines); 
                        string msg1 = string.Format(res.GetString("Error_ImportLines"), errorLines.Count, Environment.NewLine + msg);
                        string msg2 = res.GetString("Error_ImportTitle");
                        MessageBox.Show(msg1 + msg, msg2, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch(Exception ex)
                {
                    string msg = string.Format(res.GetString("Error_FileRead"), ex.Message);
                    MessageBox.Show(msg);
                }
            }
        }

        private void listView2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            disable_pools_Buttons();
        }

        private void Progress_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {
            toolStriProgressBar1.Minimum = 0;
            toolStriProgressBar1.Maximum = 100;
            toolStriProgressBar1.Step = 1;
            toolStriProgressBar1.Value = 0;
            toolStriProgressBar1.Style = ProgressBarStyle.Continuous;
            toolStriProgressBar1.ForeColor = Color.Green;
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {
            labelProgress.Text = "0%";
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void button7_Click(object sender, EventArgs e)
        {
            Factory factory = null;
            factory = new Factory();
            factory.Owner = this;
            factory.ShowDialog();

        }

        private void contextMenuStrip1_Opening_1(object sender, CancelEventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //listView2.Items.Clear();
            //List<ipBlock> selectedBlocks = new List<ipBlock>();
            //foreach (ListViewItem item in listView1.Items)
            //{
            //    if (item.Checked)
            //    {
            //        int index = item.Index;
            //        if (index >= 0 && index < ipBlockList.Count)
            //        {
            //            selectedBlocks.Add(ipBlockList[index]);
            //        }
            //    }
            //}

            //foreach (ipBlock block in selectedBlocks)
            //{
            //    for (int i = block.start; i <= block.end; i++)
            //    {
            //        string ip = $"{block.ip_block}.{i}";
            //        listView2.Items.Add(new ListViewItem(ip));
            //    }
            //}
            IPList.Clear();
            string IPBlock;
            int StartIP, EndIP;
            for (int index = 0; index < listView1.Items.Count; index++)
            {
                if (listView1.Items[index].Checked)
                {
                    ListViewItem item = listView1.Items[index];
                    defaultItemColor = item.BackColor;

                    if (index >= 0 && index < ipBlockList.Count)
                    {
                        IPBlock = ipBlockList[index].ip_block;
                        StartIP = ipBlockList[index].start;
                        EndIP = ipBlockList[index].end;

                        for (int i = StartIP; i <= EndIP; i++)
                        {
                            IPAddress IP = ParseIPFormate(IPBlock + '.' + i.ToString());
                            IPList.Add(IP);
                        }
                    }
                }
            }
            if (tabControl1.SelectedTab == tabPage1)
            {
                ShowList2();
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                ShowList3();
            }

        }
        public void ExportToExcel(ListView listView)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "Excel文件(*.csv)|*.csv";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                StringBuilder _B = ListViewToCSV(listView, ",", "\r\n");
                File.WriteAllText(saveFileDialog.FileName,_B.ToString(),Encoding.UTF8);
                MessageBox.Show("ok");
            }
        }

        private StringBuilder ListViewToCSV(ListView p_Listview , string p_ColumnChar,string p_RowChar)
        {
            StringBuilder csv = new StringBuilder();
            for (int i = 0; i!=p_Listview.Columns.Count; i++)
            {
                csv.Append(Convert.ToString(p_Listview.Columns[i].Text));
                if (i !=p_Listview.Columns.Count - 1)
                {
                    csv.Append(p_ColumnChar);
                }
                else
                {
                    csv.Append(p_RowChar);
                }
            }
            foreach(ListViewItem _Item in p_Listview.Items)
            {
                for (int i = 0; i !=_Item.SubItems.Count; i++)
                {
                    csv.Append(Convert.ToString(_Item.SubItems[i].Text));
                    if (i !=_Item.SubItems.Count -1)
                    {
                        csv.Append(p_ColumnChar);
                    }
                    else
                    {
                        csv.Append(p_RowChar);
                    }
                }
            }
            return csv;
        }
        private void button9_Click(object sender, EventArgs e)
        {
            ExportToExcel(this.listView2);
        }

        private int howManyModeSelect(bool normal, bool overFreq, bool customize)
        {
            return (normal ? 1 : 0) + (customize ? 1 : 0) + (overFreq ? 1 : 0);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            // 在用户点击“修改矿池”按钮时，先进性输入校验，确认提示，再批量通过SSH修改矿池设置
            //检查矿池输入是否完整
            //检查是否选择了一个且仅一个模式
            //检查是否设置了至少一个矿池
            //弹出确认框
            //开始异步批量执行SSH修改操作
            bool pool1_chk = Pool1.Text.Trim() == string.Empty || Worker1.Text.Trim() == string.Empty || Pwd1.Text.Trim() == string.Empty;
            bool pool2_chk = Pool1.Text.Trim() == string.Empty || Worker2.Text.Trim() == string.Empty || Pwd2.Text.Trim() == string.Empty;
            bool pool3_chk = Pool1.Text.Trim() == string.Empty || Worker3.Text.Trim() == string.Empty || Pwd3.Text.Trim() == string.Empty;

            if (howManyModeSelect(checkBox_normal_mode.Checked, checkBox_overfreq_mode.Checked, checkBox_customize_mode.Checked) == 0)
            {
                MessageBox.Show("please select at least one mode!");
                return;
            }

            if (howManyModeSelect(checkBox_normal_mode.Checked, checkBox_overfreq_mode.Checked, checkBox_customize_mode.Checked) > 1)
            {
                MessageBox.Show("only one mode could be select!");
                return;
            }

            if (pool1_chk && pool2_chk && pool3_chk)
            {
                MessageBox.Show("Set at least one pool1!");
                return;
            }
            if (pool1ck.Checked)
                checked_num++;
            if (pool2ck.Checked)
                checked_num++;
            if (pool3ck.Checked)
                checked_num++;
            DialogResult dr = MessageBox.Show("Are you sure to change pools?", "Change Pools", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (dr == DialogResult.OK)
            {
                disable_pools_Buttons();

                toolStriProgressBar1.Maximum = listView2.Items.Count;
                toolStriProgressBar1.Value = 0;

            }
            else
            {
                return;
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            ipConfigForm ip_config_form = null;
            ip_config_form = new ipConfigForm();
            ip_config_form.Owner = this;
            ip_config_form.ShowDialog();
        }
        static public double Get_ghs_crit(bool _overFreq)
        {
            if (MACHIN_ID == 0)
            {
                return _overFreq ? 16500 : 15700;
            }
            else if (MACHIN_ID == 1)
            {
                return _overFreq ? 18900 : 18000;
            }
            else if (MACHIN_ID == 2)
            {
                return 2135;
            }
            else if (MACHIN_ID == 3)
            {
                return _overFreq ? 7800 : 13580;
            }
            return 17000;
        }
        private void button10_Click(object sender, EventArgs e)
        {
            // 批量检查矿机状态
            //准备阶段：
            //  刷新状态栏
            //  禁用按钮
            //  初始化统计变量和参数（如超频临界值）
            //设置进度条
            //异步对所有矿机执行检查任务
            //每台矿机完成后调回函数更新界面，计数，进度条
            ShowList2_state();
            disable_pools_Buttons();

            if (checkBox_4level.Checked)
            {
                ghs_crit = Get_ghs_crit(true);
                _overTest = true;
            }
            else
            {
                ghs_crit = Get_ghs_crit(false);
                _overTest = false;
            }
            toolStriProgressBar1.Maximum = listView2.Items.Count;
            toolStriProgressBar1.Value = 0;
        }

        private void button17_Click(object sender, EventArgs e)
        {
            // 启动自动扫描矿机状态
            //校验用户输入时间（单位：分钟）
            //注册扫描任务事件
            //启动定时器，设置定时间隔
            //修改按钮状态：启用停止按钮，禁用操作按钮
            //立即执行一次扫描任务
            //后续每隔固定时间自动重复扫描
        }

        private void button20_Click(object sender, EventArgs e)
        {
            // 停止自动扫描矿机状态任务
            //停止计时器
            //恢复按钮状态
            //禁用“停止自动扫描”按钮
        }

        private void button16_Click(object sender, EventArgs e)
        {
            // 批量获取矿机的UTC时间信息，并更新UI状态
            //初始化状态显示和空间状态
            //设置进度条最大值和初始值
            //使用异步方式对每台矿机执行
            //每台完成后由CallBack更新状态和进度
        }

        private void button21_Click(object sender, EventArgs e)
        {
            // 批量重置矿机的IP地址
            //初始化 清空状态，禁用界面按钮
            //设置进度条 最大值=矿机数，初始值=0
            //异步操作 对每台矿机调用，并在CallBack中处理结果
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // 批量修改矿机密码
            //检查是否有IP列表
            //弹出确认修改密码
            //禁用操作按钮
            //初始化进度条
            //遍历矿机列表，异步修改密码
            //每台完成后CallBack处理状态和UI
        }

        private void button15_Click(object sender, EventArgs e)
        {
            // 批量重启矿机
            //初始化设备状态列和禁用按钮
            //初始化进度条
            //使用委托 异步对每台矿机执行重启
            //操作结果通过CallBack函数处理UI和状态
        }

        private void button22_Click(object sender, EventArgs e)
        {
            // 批量执行矿机“恢复出场设置”操作
            //用户确认
            //状态初始化
            //禁用操作
            //进度管理
            //异步恢复
            //回调反馈
        }

        private void button14_Click(object sender, EventArgs e)
        {
            // 清除Refine数据
            //刷新设备状态栏，初始化“清楚中...”状态
            //禁用操作按钮，保障操作过程安全
            //初始化进度条
            //异步对所有矿机执行 操作
            //每个任务完成时自动调用CallBack回调函数更新状态
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // 批量执行矿机时间同步
            //初始化界面和进度条
            //禁用按钮确保操作稳定
            //遍历矿机列表异步执行
            //每执行完成一台矿机，CallBack触发，更新界面和进度
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // 批量打开矿机指示灯
        }

        private void button24_Click(object sender, EventArgs e)
        {
            // 批量关闭矿机指示灯
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
        private void disable_pools_Buttons()
        {
            button11.Enabled = false;
            button10.Enabled = false;
            button9.Enabled = false;
            button8.Enabled = false;
            button12.Enabled = false;
            button17.Enabled = false;
            button16.Enabled = false;
            button15.Enabled = false;
            button14.Enabled = false;
            button13.Enabled = false;
            button20.Enabled = false;
            button21.Enabled = false;
            button22.Enabled = false;
            button24.Enabled = false;
        }

        private bool updatePwdChangeButton()
        {
            if (textBox2.Text.Trim() !=string.Empty && textBox3.Text.Trim() !=string.Empty && textBox4.Text.Trim() != string.Empty)
            {
                if (textBox3.Text.Trim().Equals(textBox4.Text.Trim()))
                    return true;
                else
                    return false;
            }
            else
            {
                return false;
            }    
        }
        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }
}



