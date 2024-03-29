﻿using Dapper;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Xml.Linq;

namespace DorisApp.WebAPI.DataAccess.Database
{

    public class SqlDataAccess : IDisposable, ISqlDataAccess
    {
        private IDbConnection? _dbconnection;
        public IDbTransaction? _dbTransaction;
        private bool _isClosedTransaction;
        private readonly string _connectionString;
        private readonly ILogger<SqlDataAccess> _logger;

        public SqlDataAccess(IConfiguration config, ILogger<SqlDataAccess> logger)
        {
            _connectionString = config.GetConnectionString("DorisAppDbConnection");
            _logger = logger;
        }

        public string GetConnectionString()
        {
            return _connectionString;
        }

        public async Task<List<T>> LoadDataAsync<T, U>(string storeProcedure, U parameters)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            List<T> rows = (await connection.QueryAsync<T>(storeProcedure, parameters,
                commandType: CommandType.StoredProcedure)).ToList();
            return rows;
        }

        public async Task<int> CountAsync(string tableName)
        {
            int result = 0;

            try
            {
                string sql = $"SELECT COUNT(*) FROM {tableName} WHERE MarkAsDeleted != 1";
                using IDbConnection connection = new SqlConnection(_connectionString);
                result = (await connection.QueryAsync<int>(sql)).FirstOrDefault();
            }
            catch
            {
                string sql = $"SELECT COUNT(*) FROM {tableName}";
                using IDbConnection connection = new SqlConnection(_connectionString);
                result = (await connection.QueryAsync<int>(sql)).FirstOrDefault();
            }

            return result;
        }

        public async Task<int> SaveDataAsync<T>(string storeProcedure, T parameters)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(storeProcedure, parameters,
                  commandType: CommandType.StoredProcedure);
        }

        public async Task<int> UpdateDataAsync<T>(string storeProcedure, T parameters)
        {
            using IDbConnection connection = new SqlConnection(_connectionString);
            return await connection.ExecuteAsync(storeProcedure, parameters,
                 commandType: CommandType.StoredProcedure);
        }

        public void StartTransaction()
        {
            _dbconnection = new SqlConnection(_connectionString);
            _dbconnection.Open();
            _isClosedTransaction = false;
            _dbTransaction = _dbconnection.BeginTransaction();
        }

        public void SaveDataInTransaction<T>(string storeProcedure, T parameters)
        {
            _dbconnection.Execute(storeProcedure, parameters,
                    commandType: CommandType.StoredProcedure, transaction: _dbTransaction);
        }

        public List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters)
        {
            List<T> rows = _dbconnection.Query<T>(storeProcedure, parameters,
                commandType: CommandType.StoredProcedure, transaction: _dbTransaction).ToList();
            return rows;
        }

        public void CommitTransaction()
        {
            _dbTransaction?.Commit();
            _dbconnection?.Close();
            _isClosedTransaction = true;
        }

        public void RollBackTransaction()
        {
            _dbTransaction?.Rollback();
            _dbconnection?.Close();
            _isClosedTransaction = true;
        }

        public void Dispose()
        {
            if (!_isClosedTransaction)
            {
                try
                {
                    CommitTransaction();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Commit Transaction failed in the dispose method");
                }
            }

            _dbconnection = null;
            _dbTransaction = null;
        }
    }

}
