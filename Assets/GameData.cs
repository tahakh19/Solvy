using System.Collections.Generic;
using System;

[Serializable] // kari makone custome data stracture haye man qabel khondan shan
public class LevelData
{
    public int levelNumber;
    public long startTime;
    public long endTime;
    public List<string> clickEvents;
    public List<int> clickSequence;

    public LevelData(int level)
    {
        levelNumber = level;
        startTime = 0;
        endTime = 0;
        clickEvents = new List<string>();
        clickSequence = new List<int>();
    }
}

[Serializable]
public class Profile
{
    public string profileName; 
    
    public List<LevelData> levels;

    public Profile()
    {
        levels = new List<LevelData>();
    }
}
