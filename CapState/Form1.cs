using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapState
{
    public partial class Form1 : Form
    {

        KeyboardHook hook;
        Icon lower, upper;
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();
            NativeHelper.SetFormToolWindowStyle(this);
            
            lower = new Icon(GetResourceStream("icon-lower.ico"));
            upper = new Icon(GetResourceStream("icon-upper.ico"));

            t = new Thread(new ThreadStart(this.Check));

            hook = new KeyboardHook();
            hook.SetHook();
            hook.OnKeyPressEvent += Hook_OnKeyPressEvent; 
            hook.OnKeyUpEvent += Hook_OnKeyDownEvent;
            this.UpdateState();
        }

        bool running = false;
        private void Check()
        {
            running = true;
            int count = 0;
            int maxcount = 10;
            int sleep = 100;
            while(count < maxcount)
            {
                this.UpdateState();
                Thread.Sleep(sleep);
                count++;
            }
            running = false;
        }

        private void Hook_OnKeyPressEvent(object sender, KeyPressEventArgs e)
        {
        }

        Thread t;

        private void Hook_OnKeyDownEvent(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.CapsLock)
            {
                ThreadPool.QueueUserWorkItem(delegate { this.Check(); });
            }
        }

        protected const string NOTIFYICONTEXT_UPPER = "大写锁定状态：大写";
        protected const string NOTIFYICONTEXT_LOWER = "大写锁定状态：小写";
        protected bool? IsUpperIcon = null;
        private void UpdateState(bool isCapLocked)
        {
            if (IsUpperIcon.HasValue == false || isCapLocked != IsUpperIcon.Value)
            {
                IsUpperIcon = isCapLocked;
                if (isCapLocked)
                {
                    this.notifyIcon1.Icon = upper;
                    this.notifyIcon1.Text = NOTIFYICONTEXT_UPPER;
                }
                else
                {
                    this.notifyIcon1.Icon = lower;
                    this.notifyIcon1.Text = NOTIFYICONTEXT_LOWER;
                }
            }
        }

        private void UpdateState()
        {
            UpdateState(Console.CapsLock);
            return;
        }

        public static Stream GetResourceStream(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("CapState." + filename);
        }

        private void 设置随系统启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(null, "确认设置随系统启动本程序吗？", "CapState", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.SetValue("CapState", path);
                rk2.Close();
                rk.Close();
            }
        }

        private void 取消随系统启动ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(null, "确认取消吗？", "CapState", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string path = Application.ExecutablePath;
                RegistryKey rk = Registry.LocalMachine;
                RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                rk2.DeleteValue("CapState", false);
                rk2.Close();
                rk.Close();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
