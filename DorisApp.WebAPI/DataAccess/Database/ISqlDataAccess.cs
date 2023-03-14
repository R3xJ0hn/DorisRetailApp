namespace DorisApp.WebAPI.DataAccess.Database
{
    public interface ISqlDataAccess
    {
        void CommitTransaction();
        Task<int> CountPageAsync(string tableName);
        void Dispose();
        string GetConnectionString();
        Task<List<T>> LoadDataAsync<T, U>(string storeProcedure, U parameters);
        List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters);
        void RollBackTransaction();
        Task SaveDataAsync<T>(string storeProcedure, T parameters);
        void SaveDataInTransaction<T>(string storeProcedure, T parameters);
        void StartTransaction();
        Task UpdateDataAsync<T>(string storeProcedure, T parameters);
    }
}