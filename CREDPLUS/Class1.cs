using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CREDPLUS
{
    class Class1
    {
        static string Uname;
        static string Value;
        public static string uname
        {
            get {
                return Uname;
            }

            set {
                Uname = value;
            }

        }

        public static string value
        {
            get
            {
                return Value;
            }

            set
            {
                Value = value;
            }

        }
    }
}
