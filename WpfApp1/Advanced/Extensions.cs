using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ML.Advanced
{
    public static class Extensions
    {

        public static byte[] GetBytes(this string me) => Encoding.UTF8.GetBytes(me);
    }
}
