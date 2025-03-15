using UnityEngine;

namespace GameLogic
{
    public abstract class Unit : MonoBehaviour
    {
        public Nation Owner;
        public Region Region;

        protected Unit(Nation nation, Region region)
        {
            Owner = nation;
            Region = region;
        }

        public abstract bool IsArmy { get; }
        public bool IsFleet => !IsArmy;
    }

    public class Army : Unit
    {
        public Army(Nation nation, Region region) : base(nation, region) { }

        public override bool IsArmy => true;
    }

    public class Fleet : Unit
    {
        public Fleet(Nation nation, Region region) : base(nation, region) { }

        public override bool IsArmy => false;
    }
}