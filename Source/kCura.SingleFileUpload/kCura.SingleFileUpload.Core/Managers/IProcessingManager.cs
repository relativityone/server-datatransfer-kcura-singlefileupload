using kCura.SingleFileUpload.Core.Entities;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IProcessingManager: IManager
    {
        ProcessingDocument GetErrorInfo(int errorID);

        void ReplaceFile(byte[] file, ProcessingDocument document);

    }
}
