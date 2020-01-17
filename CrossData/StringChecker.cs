using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossData
{
    public class StringChecker
    {
        public static string GetVersionFromText(string text)
        {
            var fstVersionSymbolPos = text.IndexOf('{');
            var scdVersionSymbolPos = text.IndexOf('}');
            var Oldversion = text.Substring(fstVersionSymbolPos + 1, scdVersionSymbolPos - fstVersionSymbolPos - 1);
            return Oldversion;
        }
    }
}
