//Get or Set some datas to Json - XML file

using Assets.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static Assets.Scripts.Tools;

namespace Assets.Scripts.Utility
{
    public static class JsonFileController
    {
        public static int GetHighScore()
        {
            int highScore = GetPlayerInfo().HighScore;
            return highScore;
        }

        public static int GetDistanceHighScore()
        {
            int distanceHighScore = GetPlayerInfo().DistanceHighScore;
            return distanceHighScore;
        }

        public static PlayerInfo[] GetScoreListFromJson()
        {
            PlayerInfo[] playerInfos = null;
            string readPlayerInfo = JsonHelper.ReadDataFromJson(false);
            if (!string.IsNullOrEmpty(readPlayerInfo))
            {
                if (readPlayerInfo.Contains("PlayerName"))
                {
                    playerInfos = JsonHelper.FromJson<PlayerInfo>(readPlayerInfo);
                }
            }
            return playerInfos;
        }

        private static PlayerInfo GetPlayerInfo()
        {
            List<string> keysList = GetKeysArray();
            string playerName = CheckKeyExist(keysList, Key.PlayerName) ? GetKeyValue(Key.PlayerName) : "trancenden";

            int ship = CheckKeyExist(keysList, Key.PlayerShip) ? int.Parse(GetKeyValue(Key.PlayerShip)) : 1;
            PlayerInfo playerInfo = new PlayerInfo();
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            string readPlayerInfo = JsonHelper.ReadDataFromJson(false);
            if (!string.IsNullOrEmpty(readPlayerInfo))
            {
                PlayerInfo[] _tempPlayerInfo = JsonHelper.FromJson<PlayerInfo>(readPlayerInfo);
                playerInfos = _tempPlayerInfo.ToList();
                PlayerInfo currentPlayer = playerInfos.SingleOrDefault(p => p.PlayerName == playerName && p.PlayerShip == ship);
                if (currentPlayer != null)
                {
                    playerInfo = currentPlayer;
                }
            }
            return playerInfo;
        }

        public static void AddHighScoresToJson(int highScore, int highDistanceScore)
        {
            List<string> keysList = GetKeysArray();
            string playerName = CheckKeyExist(keysList, Key.PlayerName) ? GetKeyValue(Key.PlayerName) : "trancenden";
            int ship = CheckKeyExist(keysList, Key.PlayerShip) ? int.Parse(GetKeyValue(Key.PlayerShip)) : 1;
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            string readPlayerInfo = JsonHelper.ReadDataFromJson(false);
            if (!string.IsNullOrEmpty(readPlayerInfo))
            {
                PlayerInfo[] _tempPlayerInfo = JsonHelper.FromJson<PlayerInfo>(readPlayerInfo);
                playerInfos = _tempPlayerInfo.ToList();
                playerInfos.RemoveAll(p => p.PlayerName == playerName && p.PlayerShip == ship);
            }
            PlayerInfo playerInfo = new PlayerInfo()
            {
                PlayerName = playerName,
                PlayerShip = ship,
                HighScore = highScore,
                DistanceHighScore = highDistanceScore
            };
            playerInfos.Add(playerInfo);
            JsonHelper.WriteDataToJson(playerInfos.ToArray(), false);
        }
        public static void ResetScoreData()
        {
            List<PlayerInfo> playerInfos = new List<PlayerInfo>();
            string readPlayerInfo = JsonHelper.ReadDataFromJson(false);
            if (!string.IsNullOrEmpty(readPlayerInfo))
            {
                PlayerInfo[] _tempPlayerInfo = JsonHelper.FromJson<PlayerInfo>(readPlayerInfo);
                playerInfos = _tempPlayerInfo.ToList();
                playerInfos.RemoveRange(0, playerInfos.Count());
            }
            JsonHelper.WriteDataToJson(playerInfos.ToArray(), false);
        }
    }
}
