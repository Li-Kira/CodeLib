using System.Threading.Tasks;

public interface IDataService
{
    bool SaveData<T>(string RelativePath, T Data, bool Encrypted);
    T LoadData<T>(string RelativePath, bool Encrypted);

    Task<bool> SaveDataAsync<T>(string RelativePath, T Data, bool Encrypted);
    Task<T> LoadDataAsync<T>(string RelativePath, bool Encrypted);

}
