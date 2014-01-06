using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogConverter
{
    class Util
    {
        public static string FixQuotes(string s)
        {
            return s.Replace('‘', '\'').Replace('’', '\'');
        }
    }
}
