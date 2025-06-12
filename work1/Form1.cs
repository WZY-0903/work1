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



namespace work1
{
    public partial class Form1 : Form
    {
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
            listView2.Clear();
            listView2.View = View.Details;
            listView2.GridLines = true;
            listView2.FullRowSelect = true;
            listView2.Columns.Add("IP Address", 150, HorizontalAlignment.Left);

            foreach (ipBlock block in ipBlockList)
            {
                for (int i = block.start; i <= block.end; i++)
                {
                    string ip = $"{block.ip_block}.{i}";
                    ListViewItem item = new ListViewItem(ip);
                    listView2.Items.Add(item);
                }
            }
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
            foreach(int index in indexToRemove)
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

        }

        private void Progress_Click(object sender, EventArgs e)
        {

        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            progressBar1.Step = 1;
            progressBar1.Value = 0;
            progressBar1.Style = ProgressBarStyle.Continuous;
            progressBar1.ForeColor = Color.Green;
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
            listView2.Items.Clear();
            List<ipBlock> selectedBlocks = new List<ipBlock>();
            foreach(ListViewItem item in listView1.Items)
            {
                if (item.Checked)
                {
                    int index = item.Index;
                    if (index >= 0 && index < ipBlockList.Count)
                    {
                        selectedBlocks.Add(ipBlockList[index]);
                    }
                }
            }

            foreach(ipBlock block in selectedBlocks)
            {
                for (int i = block.start; i <= block.end; i++)
                {
                    string ip = $"{block.ip_block}.{i}";
                    listView2.Items.Add(new ListViewItem(ip));
                }
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

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button19_Click(object sender, EventArgs e)
        {
            ipConfigForm ip_config_form = null;
            ip_config_form = new ipConfigForm();
            ip_config_form.Owner = this;
            ip_config_form.ShowDialog();

        }
    }
}



