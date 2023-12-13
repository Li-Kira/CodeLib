using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class JsonDataService : IDataService
{
    private string KEY = "ftnaNJUGsEhPeiOhXccX4002e42any9bopSvlFvCUS8=";
    private string IV = "zQYakjdZCTPdYAaahGMaTg==";

    public bool SaveData<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath; 
        
        try 
        {
            // 如果是第一次，则按下面逻辑创建文件，如果不是第一次，则删除文件重写写入
            if (File.Exists(path))
            {
                Debug.Log("Data exists");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing File for the first time!");
            }
            
            using (FileStream stream = File.Create(path))
            {
                if (Encrypted)
                {
                    WriteEncryptedData(Data, stream);
                }
                else
                {
                    stream.Close();
                    File.WriteAllText(path, JsonConvert.SerializeObject(Data));
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to save data due to {e.Message} {e.StackTrace}");
            return false;
        }
    }
    
    public T LoadData<T>(string RelativePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Can't find file at {path}. File does not exist!");
            throw new FileNotFoundException($"Can't find file at {path}.");
        }

        try
        {
            T data;

            if (Encrypted)
            {
                data = ReadEncryptedData<T>(path);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data due to: {e.Message}  {e.StackTrace}");
            throw;
        }
        
    }
    
    private void WriteEncryptedData<T>(T data, FileStream stream)
    {
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);
        
        using ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor();
        using CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);
        cryptoStream.Write(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data)));
    }

    private T ReadEncryptedData<T>(string path)
    {
        byte[] fileBytes = File.ReadAllBytes(path);
        
        using Aes aesProvider = Aes.Create();
        aesProvider.Key = Convert.FromBase64String(KEY);
        aesProvider.IV = Convert.FromBase64String(IV);

        using ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV);
        
        using MemoryStream decryptionStream = new MemoryStream(fileBytes);
        using CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read);
        using StreamReader reader = new StreamReader(cryptoStream);

        string result = reader.ReadToEnd();
        return JsonConvert.DeserializeObject<T>(result);
    }

    /// <summary>
    /// 异步操作
    /// </summary>
    public async Task<bool> SaveDataAsync<T>(string RelativePath, T Data, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath; 

        try 
        {
            if (File.Exists(path))
            {
                Debug.Log("Data exists");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Writing File for the first time!");
            }

            using (FileStream stream = File.Create(path))
            {
                if (Encrypted)
                {
                    await WriteEncryptedDataAsync(Data, stream);
                }
                else
                {
                    stream.Close();
                    await File.WriteAllTextAsync(path, JsonConvert.SerializeObject(Data));
                }
            }

            Debug.Log("Data saved successfully.");
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Unable to save data: {e.Message}\n{e.StackTrace}");
            return false;
        }
    }
    
    public async Task<T> LoadDataAsync<T>(string RelativePath, bool Encrypted)
    {
        string path = Application.persistentDataPath + RelativePath;

        if (!File.Exists(path))
        {
            Debug.LogError($"Can't find file at {path}. File does not exist!");
            throw new FileNotFoundException($"Can't find file at {path}.");
        }

        try
        {
            T data;

            if (Encrypted)
            {
                data = await ReadEncryptedDataAsync<T>(path);
            }
            else
            {
                data = JsonConvert.DeserializeObject<T>(await File.ReadAllTextAsync(path));
            }

            Debug.Log("Data loaded successfully.");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data: {e.Message}\n{e.StackTrace}");
            throw;
        }
    }
    
    private async Task WriteEncryptedDataAsync<T>(T data, FileStream stream)
    {
        using (Aes aesProvider = Aes.Create())
        {
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
            using (CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write))
            {
                byte[] dataBytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data));
                await cryptoStream.WriteAsync(dataBytes, 0, dataBytes.Length);
            }
        }
    }

    private async Task<T> ReadEncryptedDataAsync<T>(string path)
    {
        byte[] fileBytes = await File.ReadAllBytesAsync(path);

        using (Aes aesProvider = Aes.Create())
        {
            aesProvider.Key = Convert.FromBase64String(KEY);
            aesProvider.IV = Convert.FromBase64String(IV);

            using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor(aesProvider.Key, aesProvider.IV))
            using (MemoryStream decryptionStream = new MemoryStream(fileBytes))
            using (CryptoStream cryptoStream = new CryptoStream(decryptionStream, cryptoTransform, CryptoStreamMode.Read))
            using (StreamReader reader = new StreamReader(cryptoStream))
            {
                string result = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<T>(result);
            }
        }
    }

}