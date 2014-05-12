using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PowerSystem.Forms;
using System.IO;
using System.Collections;
namespace Oscillograph
{
    static class Program
    {
        private static ApplicationInstanceMonitor<string> instanceMonitor=new ApplicationInstanceMonitor<string>("Oscillograph");
        private static MainForm MyForm;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string Arg=string.Empty;
            if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null)
            {
                if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null)
                {
                    args = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData;
                }
            }
            foreach (string S in args)
            {
                if (File.Exists(S))
                {
                    Arg = S;
                    break;
                }
            }
            if (instanceMonitor.Assert())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                instanceMonitor.NewInstanceCreated += new EventHandler<NewInstanceCreatedEventArgs<string>>(instanceMonitor_NewInstanceCreated);
                FileQueue = new Queue<string>();
                T = new System.Windows.Forms.Timer();
                T.Interval = 500;
                T.Tick+=new EventHandler(T_Tick);
                T.Start();
                MyForm = new MainForm();
                MyForm.Show();
                if (Arg != string.Empty)
                {
                    FileQueue.Enqueue(Arg);
                }
                Application.Run(MyForm);
                T.Stop();
            }
            else
            {
                instanceMonitor.NotifyNewInstance(Arg);
            }
        }

        static void T_Tick(object sender, EventArgs e)
        {
            if (FileQueue.Count > 0)
            {
                T.Stop();
                FileStream F = File.Open(PowerSystem.Config.AnaFile, FileMode.Open);
                PowerSystem.NetWork.ANA AnaObj = new PowerSystem.NetWork.ANA(F);
                F.Close();
                SetForegroundWindow(MyForm.Handle);
                do
                {
                    MyForm.OpenFile(FileQueue.Dequeue(), AnaObj);
                } while (FileQueue.Count > 0);
                T.Start();
            }
        }
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        static void instanceMonitor_NewInstanceCreated(object sender, NewInstanceCreatedEventArgs<string> e)
        {
            if (e.Message != string.Empty)
            {
                FileQueue.Enqueue(e.Message);
            }
        }
        static private Queue<string> FileQueue;
        static System.Windows.Forms.Timer T;
    }
}