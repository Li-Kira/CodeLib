using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class TestDataPersistence : MonoBehaviour
{
    private IDataService m_JsonDataService = new JsonDataService();

    public bool EncryptionEnabled;
    public TMP_InputField SourceInputField;
    public TMP_InputField TargetInputField;
    public TMP_Text SaveTimeText;
    public TMP_Text LoadTimeText;

    public void ToggleEncryption(bool ToggleState)
    {
        EncryptionEnabled = ToggleState;
    }
    
    private void Awake()
    {
        Debug.Log(Application.persistentDataPath);
        SourceInputField.text = JsonConvert.SerializeObject(m_PlayerState, Formatting.Indented);
        
        // 使用下面代码生成密钥
        // using Aes aesProvider = Aes.Create();
        // Debug.Log($"Initialization Vector: {Convert.ToBase64String(aesProvider.IV)}");
        // Debug.Log($"Key: {Convert.ToBase64String(aesProvider.Key)}");
    }

    public void SerializeJson()
    {
        long startTime = DateTime.Now.Ticks;
        
        if (m_JsonDataService.SaveData("/player-state.json", m_PlayerState, EncryptionEnabled))
        {
            // 保存Json文件
            long saveTime = DateTime.Now.Ticks - startTime;
            SaveTimeText.text = $"Save Time: {(saveTime / 1000f):N4} ms";
            
            // 读取Json
            startTime = DateTime.Now.Ticks;
            try
            {
                PlayerState data = m_JsonDataService.LoadData<PlayerState>("/player-state.json", EncryptionEnabled);
                long loadTime = DateTime.Now.Ticks - startTime;

                TargetInputField.text = "Load file form: " + Application.persistentDataPath + "\n"  + JsonConvert.SerializeObject(data, Formatting.Indented);
                LoadTimeText.text = $"Save Time: {(loadTime / 1000f):N4} ms";
            }
            catch (Exception e)
            {
                Debug.LogError("Could not read file!");
                TargetInputField.text = "<color=#ff0000>Error Reading file</color>";
                throw;
            }
        }
        else
        {
            TargetInputField.text = "<color=#ff0000>Error saving data</color>";
        }
    }
    
    public async void SerializeJsonAsync()
    {
        long startTime = DateTime.Now.Ticks;
        
        if (await m_JsonDataService.SaveDataAsync("/player-state.json", m_PlayerState, EncryptionEnabled))
        {
            // 保存Json文件
            long saveTime = DateTime.Now.Ticks - startTime;
            SaveTimeText.text = $"Save Time: {(saveTime / 1000f):N4} ms";
            
            // 读取Json
            startTime = DateTime.Now.Ticks;
            try
            {
                PlayerState data = await m_JsonDataService.LoadDataAsync<PlayerState>("/player-state.json", EncryptionEnabled);
                long loadTime = DateTime.Now.Ticks - startTime;

                TargetInputField.text = "Load file form: " + Application.persistentDataPath + "\n"  + JsonConvert.SerializeObject(data, Formatting.Indented);
                LoadTimeText.text = $"Load Time: {(loadTime / 1000f):N4} ms";
            }
            catch (Exception e)
            {
                Debug.LogError("Could not read file!");
                TargetInputField.text = $"<color=#ff0000>Error Reading file: {e.Message}</color>";
                throw;
            }
        }
        else
        {
            TargetInputField.text = "<color=#ff0000>Error saving data</color>";
        }
    }
    
    private PlayerState m_PlayerState = new PlayerState
    {
        Health = 100,
        Name = "Llama Lover",
        BaseAttackSpeed = 1.0f,
        Level = 1,
        HasUnlockedSomething = false,
        HasUnlockedSomethingElse = false,
        Equipment = new EquipmentData
        {
            Head = new EquipmentItem
            {
                Defense = 1,
                FireResist = 0,
                PoisonResist = 0,
                ElectricResist = 0,
                WaterResist = 0,
                EquippedSlot = 0,
                Name = "Tiara",
                Rarity = 0,
                Value = 5,
                Quantity = 1
            }
        }
    };
    
    
    
    
    
    
    
    
}
