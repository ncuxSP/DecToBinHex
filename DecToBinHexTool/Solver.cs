using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace DecToBinHexTool
{
    class Solver
    {
        [DllImport("dtbh.dll", EntryPoint = "GetBufferSize", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetBufferSize();

        [DllImport("dtbh.dll", EntryPoint = "GetResult", CallingConvention = CallingConvention.Cdecl)]
        private static extern void GetResult(int number, StringBuilder buffer, int bufferSize);

        protected string Compute(int number)
        {
            try
            {
                var sb = new StringBuilder(GetBufferSize());
                GetResult(number, sb, sb.Capacity);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Application.Exit();
            }
            return string.Empty;
        }
    }
}
