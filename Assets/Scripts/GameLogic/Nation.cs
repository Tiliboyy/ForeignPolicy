using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace GameLogic
{
    public class Nation : MonoBehaviour
    {
        public string Name;
        public string Color;

        public bool IsHuman;
        public Player? Player;

        public List<Region> CoreRegions = new();
        public List<Region> ControlledRegions = new();

        public Nation(string name, string color)
        {
            Name = name;
            Color = color;
        }
    }
} 