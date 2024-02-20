using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PersistentData
{
    /// <summary>
    /// Holds data for each level
    /// </summary>
    public class LevelData
    {
        public int coins { get; private set; }
        public int levelID { get; private set; }
        public float time { get; private set; }

        public LevelData(int coins, int levelID, float time)
        {
            this.coins = coins;
            this.levelID = levelID;
            this.time = time;
        }
    }

    public static int lastLevelID;

    // this list is retrievable from anywhere and can be easily modified
    public static List<LevelData> Data = new List<LevelData>();

    public static void AddNewData(int coins, int levelID, float time)
    {
        lastLevelID = levelID;

        for(int i = 0; i < Data.Count; i++)
        {
            if (Data[i].levelID == levelID)
            {
                Data[i] = new LevelData(coins, levelID, time);
                return;
            }
        }

        Data.Add(new LevelData(coins, levelID, time));
    }

    public static LevelData GetSceneData(int id)
    {
        for (int i = 0; i < Data.Count; i++)
        {
            if (Data[i].levelID == id)
            {
                return Data[i];
            }
        }
        return null;
    }
}
