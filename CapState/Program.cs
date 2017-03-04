using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CapState
{
    static class Program
    {
        private static System.Threading.Mutex _mutex;
        //[STAThread]
        //static void Main()
        //{
        //    _mutex = new System.Threading.Mutex(false, "Global\\MyApplication");
        //    //Mutex所有权取得
        //    if (_mutex.WaitOne(0, false) == false)
        //    {
        //        MessageBox.Show("应用程序已经启动过了。");
        //        return;
        //    }
        //    Application.Run(new Form1());
        //}

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            _mutex = new System.Threading.Mutex(false, "Global\\CapState");
            if (_mutex.WaitOne(0, false) == false)
            {
                MessageBox.Show(null, "CapState已经在运行了", "CapState", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
