using System;
using System.Collections.Generic;
using System.Text;

namespace SOFP
{
    public static class CloseAllWindow
    {
        public delegate void Close();
        public static event Close GetClose = delegate { };

        public static void close()
        {
            GetClose();
        }
    }
}
