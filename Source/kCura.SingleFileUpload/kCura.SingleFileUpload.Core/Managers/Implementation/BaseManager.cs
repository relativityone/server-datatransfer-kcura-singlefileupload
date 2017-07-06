using NSerio.Relativity;
using System.Data.SqlClient;

namespace kCura.SingleFileUpload.Core.Managers.Implementation
{
    public abstract class BaseManager : IManager
    {
        public Repository _Repository
        {
            get
            {
                return (Repository)Repository.Instance;
            }
        }

        public int WorkspaceID
        {
            get
            {
                return _Repository.WorkspaceID;
            }
            set
            {
                _Repository.WorkspaceID = value;
            }
        }

        public int GetArtifactTypeByArtifactGuid(string guid)
        {
            return (int)_Repository.CaseDBContext.ExecuteSqlStatementAsScalar(SQL.Queries.GetArtifactTypeByArtifactGuid, new SqlParameter[] {
                new SqlParameter("@Guid", guid)
            });
        }
    }
}
