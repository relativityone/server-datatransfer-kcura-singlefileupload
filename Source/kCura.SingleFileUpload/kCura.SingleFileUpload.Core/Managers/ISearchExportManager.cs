using kCura.SingleFileUpload.Core.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface ISearchExportManager: IManager
    {
        ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile);
        ExportedMetadata ProcessSearchMLString(byte[] searchML, ExportedMetadata result = null);
    }
}
