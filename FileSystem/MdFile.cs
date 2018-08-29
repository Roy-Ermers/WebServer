using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CommonMark;
namespace WebServer.Files
{
    class MdFile : File
    {
        string html;

        public MdFile(string path) : base(path)
        {
        }

        public MdFile(FileInfo Fileinfo) : base(Fileinfo)
        {
        }

        public override byte[] ReadBytes()
        {
            if (CachedContent.Length == 0)
                Refresh();
            string text = System.Text.Encoding.UTF8.GetString(CachedContent);

            CommonMarkSettings setting = CommonMarkSettings.Default.Clone();
            setting.AdditionalFeatures = CommonMarkAdditionalFeatures.All;
            html = CommonMarkConverter.Convert(text, setting);
            return System.Text.Encoding.UTF8.GetBytes(html);
        } 
    }
}
