using System.Data.SqlClient;

public static class SqlHelper
{
    /// <summary>
    /// Get Sql Parameter
    /// </summary>
    /// <param name="name">parameter name</param>
    /// <param name="value">parameter value</param>
    /// <returns></returns>
    public static SqlParameter CreateSqlParameter(string name, object value)
    {
        return new SqlParameter(name, value);
    }
}