using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Musem.Database
{
    internal class DbConn
    {
        public static МузейEntities11 DbConnect = new МузейEntities11();
        public static User User;
        public static Authors Author;
        public static Types Types;
        public static Conditions Conditions;
    }
}
