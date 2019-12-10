using NSerio.Relativity;
using System;

namespace kCura.SingleFileUpload.Core.Managers
{
    public interface IManager
    {
        void LogError(Exception e);
        int WorkspaceID { get; set; }
        Repository _Repository { get; }

        int GetArtifactTypeByArtifactGuid(string guid);
    }
}
