using BookHistory.Entities.RepositoryContracts;
using BookHistory.PostgreRepositories;
using BookHistory.SQLRepositories;

namespace BookHistory.Factory
{
    public class RepositoryGenerator
    {
        private readonly string _connectionString;
        private readonly RepositoryDataSource _repositoryDataSource;

        public RepositoryGenerator(Dictionary<string, string>? connectionStrings)
        {
            _connectionString = string.Empty;

            ArgumentNullException.ThrowIfNull(connectionStrings);

            foreach (var kvp in connectionStrings)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Value))
                {
                    _repositoryDataSource = kvp.Key switch
                    {
                        //If SQLServer connection string exists and is filled, then SQL+Dapper implementation will work
                        "SQLServer" => RepositoryDataSource.Sql,
                        //Otherwise, if PostgreSQL connection string exists and is filled, then PostGre+EF implementation will work
                        "PostgreSQL" => RepositoryDataSource.Postgre,
                        _ => throw new ApplicationException($"Unknown connection string key: {kvp.Key}"),
                    };
                    _connectionString = kvp.Value;
                }
            }

            if (_connectionString == string.Empty)
            {
                throw new ApplicationException("Connection string is not specified");
            }
        }

        public IBookRepository GetBookRepository()
        {
            if (_repositoryDataSource == RepositoryDataSource.Sql)
            {
                return new DapperBookRepository(_connectionString);
            }
            else
            {
                return new EFBookRepository(_connectionString);
            }
        }

        public IBookHistoryRepository GetBookHistoryRepository()
        {
            if (_repositoryDataSource == RepositoryDataSource.Sql)
            {
                return new DapperBookHistoryRepository(_connectionString);
            }
            else
            {
                return new EFBookHistoryRepository(_connectionString);
            }
        }
    }
}
