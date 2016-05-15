using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class AttachmentFile
    {
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string DataMimeType { get; set; }
    }
}
