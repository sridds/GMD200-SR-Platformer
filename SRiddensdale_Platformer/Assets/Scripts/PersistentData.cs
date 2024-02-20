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

        public LevelData(int coins, int levelID)
        {
            this.coins = coins;
            this.levelID = levelID;
        }
    }

    // this list is retrievable from anywhere and can be easily modified
    public static List<LevelData> Data = new List<LevelData>();

    public static void AddNewData(int coins, int levelID)
    {
        for(int i = 0; i < Data.Count; i++)
        {
            if (Data[i].levelID == levelID)
            {
                Data[i] = new LevelData(coins, levelID);
                return;
            }
        }

        Data.Add(new LevelData(coins, levelID));
    }
}
