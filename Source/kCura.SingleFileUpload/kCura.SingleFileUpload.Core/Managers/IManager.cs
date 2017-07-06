using NSerio.Relativity;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IManager
    {
        int WorkspaceID { get; set; }
        Repository _Repository { get; }

        int GetArtifactTypeByArtifactGuid(string guid);
    }
}
