using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace L2TowerDatabaseGenerator
{
    public partial class Form1 : Form
    {
        public static Form1 me = null;
        public Form1()
        {
            InitializeComponent();
        }

        private Thread tThread = null;

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            me = this;
            tThread = new Thread(StartGeneration);
            tThread.Start();
        }

        private void StartGeneration()
        {
            try
            {
                MysqlManager.StartMysql();
                string[] dir = Directory.GetDirectories(Path.GetDirectoryName(Application.ExecutablePath)+"\\..\\data\\", "*");
                progressBar1.Maximum = (dir.Length-1) * 4;
                progressBar1.Value = 0;
                foreach (string g in dir)
                {
                    FileInfo f = new FileInfo(g);
                    string c = f.Name;
                    if (c.Contains('.')) continue;
                    Log.log("Processing dir: " + c);

                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\" + c + " - L2off.db");
                    File.Delete(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\" + c + " - L2j.db");

                    DirectoryInfo test = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\l2jsql");
                    if (test.Exists)
                    {
                        MysqlManager.LoadDatabase(c);
                        if (!MysqlManager.OpenDatabase())
                            MessageBox.Show("Failed to open mysql database connection");
                        
                        SqliteManager.OpenDatabase(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\" + c + " - L2j.db");
                        SqliteManager.CreateTables();
                        ClientParse cp = new ClientParse();
                        cp.Process(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\");
                        progressBar1.Value++;
                        this.Refresh();
                        L2jParse jp = new L2jParse();
                        jp.Process(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\");
                        progressBar1.Value++;
                        this.Refresh();
                        SqliteManager.CloseDatabase();
                        MysqlManager.CloseDatabase();
                    }
                    else progressBar1.Value += 2;
                    this.Refresh();
                    test = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\l2offscript");
                    if (test.Exists)
                    {
                        SqliteManager.OpenDatabase(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\" + c + " - L2off.db");
                        SqliteManager.CreateTables();
                        ClientParse cp = new ClientParse();
                        cp.Process(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\");
                        progressBar1.Value++;
                        this.Refresh();
                        L2OffParse op = new L2OffParse();
                        op.Process(Path.GetDirectoryName(Application.ExecutablePath) + "\\..\\data\\" + c + "\\");
                        progressBar1.Value++;
                        this.Refresh();

                        SqliteManager.CloseDatabase();
                    }
                    else progressBar1.Value += 2;
                    this.Refresh();
                }
                MysqlManager.StopMysql();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            button1.Enabled = true;
            tThread = null;
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Application.ExitThread();
            }
            catch (Exception e2)
            {
            }
        }
    }

    class Log
    {
        public static void log(string log)
        {
            if (Form1.me != null)
            {
                Form1.me.richTextBox1.Text = DateTime.Now.ToString() + " " + log + "\n" + Form1.me.richTextBox1.Text;
                Form1.me.richTextBox1.Refresh();
            }
        }

        public static void log2(int number, int max)
        {
            int procent = number * 500 / max;
            if (Form1.me.progressBar2.Value != procent)
            {
                if (Form1.me != null)
                {
                    Form1.me.progressBar2.Maximum = 500;
                    Form1.me.progressBar2.Value = procent;
                    Form1.me.progressBar2.Refresh();
                }
            }
        }
    }
}
