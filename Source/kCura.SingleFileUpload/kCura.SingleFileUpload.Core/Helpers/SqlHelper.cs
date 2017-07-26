using Relativity.API;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

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
    /// <summary>
    /// Transaction scope for write actions in DDL/DML
    /// </summary>
    /// <typeparam name="T">result of the wirte action</typeparam>
    /// <param name="dbContext">DBContext</param>
    /// <param name="tranLogic">Write Action</param>
    /// <returns></returns>
    public static T RunTransaction<T>(this IDBContext dbContext, Func<IDBContext, T> tranLogic)
    {
        T elem;
        try
        {
            dbContext.BeginTransaction();
            elem = tranLogic(dbContext);
            dbContext.CommitTransaction();
        }
        catch (System.Exception)
        {
            dbContext.RollbackTransaction();
            throw;
        }
        return elem;
    }
    /// <summary>
    /// Used to provide DBNull, when value is null
    /// </summary>
    /// <typeparam name="T">input Type</typeparam>
    /// <param name="value">value to check</param>
    /// <returns></returns>
    public static object CoalesceParameter<T>(T value)
    {
        return (object)value ?? DBNull.Value;
    }
    /// <summary>
    /// Clean invalid chars when it's required
    /// </summary>
    /// <param name="dirtyString">string to be cleared</param>
    /// <returns></returns>
    public static string RemoveNonASCIIChars(string dirtyString)
    {
        return Regex.Replace(dirtyString, @"[^\u0000-\u007F]", string.Empty);
    }
}