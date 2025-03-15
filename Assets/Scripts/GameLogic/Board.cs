using System.Collections.Generic;
using UnityEngine;

namespace GameLogic
{
    public class Board : MonoBehaviour
    {
        public List<Region> Regions;
        public List<Nation> Nations;
        public Turn Turn;

        public Board(List<Region> regions, List<Nation> nations, Turn turn)
        {
            Regions = regions;
            Nations = nations;
            Turn = turn;
        }
    }
}