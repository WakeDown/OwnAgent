using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    public class FileHelper
    {
        public static byte[] StreamToByteArray(Stream stream)
        {
            byte[] data;
            using (Stream inputStream = stream)
            {
                MemoryStream memoryStream = inputStream as MemoryStream;
                if (memoryStream == null)
                {
                    memoryStream = new MemoryStream();
                    inputStream.CopyTo(memoryStream);
                }
                data = memoryStream.ToArray();
            }
            return data;
        }
    }
}
