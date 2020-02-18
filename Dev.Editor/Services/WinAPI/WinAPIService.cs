using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Dev.Editor.Services.WinAPI
{
    class WinAPIService
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }

        public const int WM_COPYDATA = 0x004A;

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        public void SendCopyData(IntPtr hwnd, string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message);
            var data = new COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = message,
                cbData = messageBytes.Length + 1
            };

            if (SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        public (bool, string) ProcessCopyData(IntPtr lParam)
        {
            try
            {
                var data = Marshal.PtrToStructure<COPYDATASTRUCT>(lParam);
                var result = string.Copy(data.lpData);
                return (true, result);
            }
            catch
            {
                return (false, null);
            }
        }
    }
}
