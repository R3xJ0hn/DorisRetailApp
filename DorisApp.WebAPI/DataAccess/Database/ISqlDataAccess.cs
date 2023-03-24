namespace DorisApp.WebAPI.DataAccess.Database
{
    public interface ISqlDataAccess
    {
        void CommitTransaction();
        Task<int> CountAsync(string tableName);
        void Dispose();
        string GetConnectionString();
        Task<List<T>> LoadDataAsync<T, U>(string storeProcedure, U parameters);
        List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters);
        void RollBackTransaction();
        Task<int> SaveDataAsync<T>(string storeProcedure, T parameters);
        void SaveDataInTransaction<T>(string storeProcedure, T parameters);
        void StartTransaction();
        Task<int> UpdateDataAsync<T>(string storeProcedure, T parameters);
    }
}