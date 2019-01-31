using kCura.SingleFileUpload.Core.Entities;
using Relativity.API;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface ISearchExportManager: IManager
    {
        ExportedMetadata ExportToSearchML(string fileName, byte[] sourceFile, IHelper helper);
        ExportedMetadata ProcessSearchMLString(byte[] searchML, ExportedMetadata result = null);
        void ConfigureOutsideIn();
    }
}
