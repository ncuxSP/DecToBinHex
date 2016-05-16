﻿using System;
using System.Windows.Forms;

namespace DecToBinHexTool
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var f = new MainForm();
            var p = new Presenter(f);
            Application.Run(f);
        }
    }
}
