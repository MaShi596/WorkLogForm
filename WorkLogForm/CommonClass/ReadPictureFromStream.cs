using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace WorkLogForm.CommonClass
{
    public class ReadPictureFromStream
    {
        ///
        /// 将文件转为内存流
        ///
        /// 
        /// 
        static public MemoryStream ReadFile(string path)
        {
            if (!File.Exists(path))
                return null;

            using (FileStream file = new FileStream(path, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                byte[] b = new byte[file.Length];
                file.Read(b, 0, b.Length);

                MemoryStream stream = new MemoryStream(b);
                file.Close();
                return stream;
            }
        }

        ///
        /// 将内存流转为图片
        ///
        /// 
        /// 
        static public Image GetFile(string path)
        {
            MemoryStream stream = ReadFile(path);
            return stream == null ? null : Image.FromStream(stream);
        }

    }
}
