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

        private void UpdateState()
        {
            if (Console.CapsLock)
            {
                this.notifyIcon1.Icon = upper;
            }
            else
            {
                this.notifyIcon1.Icon = lower;

            }
        }

        public static Stream GetResourceStream(string filename)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("CapState." + filename);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
