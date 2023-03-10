namespace DorisApp.WebAPI.DataAccess.Database
{
    public interface ISqlDataAccess
    {
        void CommitTransaction();
        void Dispose();
        string GetConnectionString();
        Task<List<T>> LoadDataAsync<T, U>(string storeProcedure, U parameters);
        Task<List<T>> LoadDataAsync<T>(string storeProcedure);
        List<T> LoadDataInTransaction<T, U>(string storeProcedure, U parameters);
        void RollBackTransaction();
        Task SaveDataAsync<T>(string storeProcedure, T parameters);
        void SaveDataInTransaction<T>(string storeProcedure, T parameters);
        void StartTransaction();
        Task UpdateDataAsync<T>(string storeProcedure, T parameters);
    }
}