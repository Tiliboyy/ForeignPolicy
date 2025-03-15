using System.Collections.Generic;
using UnityEngine;

#nullable enable

namespace GameLogic
{
    public class Region : MonoBehaviour
    {
        public string ShortName;
        public string Name;
        public bool IsSupplyHub;
        public bool IsOcean;
        public bool IsCoast;
    
        public List<Region> Connections = new();
    
        public Nation? Owner { get; set; }
        public Unit? Unit { get; set; }

        public Region(string shortName, string name, bool isSupplyHub=false, bool isOcean=false, bool isCoast=false, Nation? owner=null, Unit? unit=null)
        {
            ShortName = shortName;
            Name = name;
            IsSupplyHub = isSupplyHub;
            IsOcean = isOcean;
            IsCoast = isCoast;
        
            Owner = owner;
            Unit = unit;
        
            if (owner != null)
                owner.ControlledRegions.Add(this);
        }
    
        public void AddConnections(Region[] regions)
        {
            foreach (var region in regions)
            {
                Connections.Add(region);
                region.Connections.Add(this);
            }
        }
    }
}