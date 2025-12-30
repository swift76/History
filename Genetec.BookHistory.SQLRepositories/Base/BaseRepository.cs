using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Genetec.BookHistory.SQLRepositories.Base
{
    public abstract class BaseRepository(string connectionString)
    {
        private readonly string connectionString = connectionString;

        public async Task<IEnumerable<T>> GetListAsync<T>(DynamicParameters parameters
            , string procedureName
            , int timeoutInterval = 180
            , CommandType cmdType = CommandType.StoredProcedure)
        {
            using SqlConnection connection = new(connectionString);
            return await connection.QueryAsync<T>(procedureName, parameters, commandType: cmdType, commandTimeout: timeoutInterval);
        }

        public async Task<T?> GetSingleAsync<T>(DynamicParameters parameters
            , string procedureName
            , int timeoutInterval = 180
            , CommandType cmdType = CommandType.StoredProcedure)
        {
            using SqlConnection connection = new(connectionString);
            return await connection.QueryFirstOrDefaultAsync<T>(procedureName, parameters, commandType: cmdType, commandTimeout: timeoutInterval);
        }

        public async Task ExecuteAsync(DynamicParameters parameters
            , string procedureName
            , int timeoutInterval = 180
            , CommandType cmdType = CommandType.StoredProcedure)
        {
            using SqlConnection connection = new(connectionString);
            await connection.ExecuteAsync(procedureName, parameters, commandType: cmdType, commandTimeout: timeoutInterval);
        }

        public static void AddSystemTableValuedParameter<T>(DynamicParameters parameters
            , string columnName
            , string parameterName
            , IEnumerable<T> rows)
        {
            DataTable dataTable = new();
            dataTable.Columns.Add(new DataColumn(columnName, typeof(T)));
            foreach (T row in rows)
            {
                dataTable.Rows.Add(row);
            }

            parameters.Add(parameterName, dataTable, DbType.Object);
        }
    }
}
