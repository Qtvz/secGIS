using System;
using System.Diagnostics;

namespace Calling
{
    public class Call
    {
        public void Callexe(string exename)
        {
            string debugPath = System.Environment.CurrentDirectory;
            string pyexePath = debugPath + @"\" + exename;
            Process p = new Process();
            p.StartInfo.FileName = pyexePath;//需要执行的文件路径 
            p.StartInfo.UseShellExecute = false; //必需
            p.StartInfo.CreateNoWindow = true;
            p.Start();
            p.WaitForExit();//关键，等待外部程序退出后才能往下执行
            p.Close();
        }
    }
}
