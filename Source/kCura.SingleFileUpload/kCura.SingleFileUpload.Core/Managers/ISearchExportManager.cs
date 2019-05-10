using kCura.SingleFileUpload.Core.Entities;
using System;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface ISearchExportManager: IManager
    {
        ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile);
        ExportedMetadata ProcessSearchMLString(byte[] searchML, ExportedMetadata result = null);
        void ConfigureOutsideIn();
    }
}
