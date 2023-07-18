using LiteNetLib.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace rater193.scb.global
{
    [System.Serializable]
    public class PlayerStats
    {
        public int Money = 0;

        public void WriteTo(NetDataWriter writer)
		{
            //Writing order
            writer.Put(Money);
		}

        public PlayerStats ReadFrom(NetDataReader reader)
		{
            PlayerStats stats = new PlayerStats();

            //Reading order
            stats.Money = reader.GetInt();

            return stats;
		}
    }
}
