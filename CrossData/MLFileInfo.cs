using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossData
{
    [Serializable]
    public class MLFileInfo
    {
        public string FileName { get; set; }
        public byte[] BinaryRealize;
    }
}
