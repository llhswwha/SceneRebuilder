using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.Modules.Context
{
    public static class AppContext
    {
        public static IDataClient DataClient;

        public static IDevManager DevManager;

        public static IMessageBox MessageBox;

        public static IDevSubSystem DevSystem;
    }
}
