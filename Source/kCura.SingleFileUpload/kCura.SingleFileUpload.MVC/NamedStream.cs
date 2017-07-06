using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Relativity.SingleFileUpload.MVC
{
    public class NamedStream
    {
        public string Name { get; set; }
        public Stream Stream { get; set; }
        public bool IsValid
        {
            get
            {
                return Stream != null && Stream.Length > 0;
            }
        }

        public Action<string> SaveAs { get; internal set; }
    }

    public static class StreamExtensions
    {
        public static NamedStream ToNamedStream(this HttpPostedFile file)
        {
            return new NamedStream
            {
                Name = file.FileName,
                Stream = file.InputStream,
                SaveAs = file.SaveAs
            };
        }

        public static NamedStream ToNamedStream(this HttpPostedFileBase file)
        {
            return new NamedStream
            {
                Name = file.FileName,
                Stream = file.InputStream,
                SaveAs = file.SaveAs
            };
        }

    }
}