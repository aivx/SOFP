using System.Data;
using System.Data.SqlClient;
using static System.Linq.Enumerable;
using System.Windows.Forms;

namespace SOFP
{
    public static class DataUpdate
    {
        public delegate void Update();
        public static event Update GetUpdate = delegate { };

        public static void updates()
        {
            GetUpdate();
        }
    }
}
