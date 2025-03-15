using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class Game : MonoBehaviour
    {
        public Dictionary<Turn, Board> History = new();
        public List<Player> Players;
    
        public Game(List<Player> players)
        {
            Players = players;

            var regions = new List<Region>();
            var nations = new List<Nation>();
            var startDate = new Turn(1900, Seasons.Spring);
        
            var board = new Board(regions, nations, startDate);

            History.Add(startDate, board);
        }
    }
}