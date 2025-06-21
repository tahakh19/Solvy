using UnityEngine;
using System;
using System.IO;

public class GameDataManager : MonoBehaviour
{
    public Profile playerProfile;
    private LevelData currentLevelData;

    private string userSaveFilePath;

    void Awake()
    {
        SetupProfile();
    }

    private void SetupProfile()
    {
        string username = "DefaultUser";
        if (!string.IsNullOrEmpty(GameSession.Username))
        {
            username = GameSession.Username;
        }

      
        string basePath = @"SolvyJsonsFile";
        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }
        userSaveFilePath = Path.Combine(basePath, $"{username}.json");

        if (File.Exists(userSaveFilePath))
        {
            string json = File.ReadAllText(userSaveFilePath);
            playerProfile = JsonUtility.FromJson<Profile>(json);
        }
        else
        {
            playerProfile = new Profile();
            playerProfile.profileName = username;
        }
    }


    public void StartLevel(int levelNum)
    {
        currentLevelData = new LevelData(levelNum);
        currentLevelData.startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Debug.Log($"Level {levelNum} started for user '{playerProfile.profileName}'.");
    }

    public void RecordClick(int pieceIndex)
    {
        if (currentLevelData == null) return;
        long clickTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        currentLevelData.clickEvents.Add($"c1, {clickTimestamp}");
        currentLevelData.clickSequence.Add(pieceIndex);
    }

    public void EndLevel()
    {
        if (currentLevelData == null) return;
        
        currentLevelData.endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        Debug.Log($"Level {currentLevelData.levelNumber} ended.");

      
        playerProfile.levels.RemoveAll(level => level.levelNumber == currentLevelData.levelNumber);
        
        playerProfile.levels.Add(currentLevelData);

        SaveProfile();
        currentLevelData = null;
    }

    private void SaveProfile()
    { 
  
        string json = JsonUtility.ToJson(playerProfile, true);
        File.WriteAllText(userSaveFilePath, json);
        Debug.Log($"Profile for '{playerProfile.profileName}' saved to: {userSaveFilePath}");
    }
}
