using System;
using System.Collections.Generic;
using System.Text;

namespace SOFP
{
    public static class StatusUpdate
    {
        public delegate void Status(string mess);
        public static event Status GetUpdate = delegate { };

        public static void setStatus(string mess)
        {
            GetUpdate(mess);
        }
    }
}
