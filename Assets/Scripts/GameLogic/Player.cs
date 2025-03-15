using UnityEngine;

namespace GameLogic
{
    public class Player : MonoBehaviour
    {
        public readonly string Name;

        public Player(string name)
        {
            Name = name;
        }
    }
}