using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable

namespace GameLogic
{
    public enum OrderType
    {
        Hold,
        Move,
        Support,
        Convoy,
        Build,
        Disband
    }

    public class Order : MonoBehaviour
    {
        public bool IsValid { get; private set; } = false;

        private readonly Nation Initiator;
        private readonly Unit Unit;
        private readonly Region Origin;
        private readonly Region Destination;
        private readonly Region? Target;
        private readonly OrderType OrderType;
    
        private int Strength;
    
        private static List<Order> AllOrders = new();

        public Order(Nation initiator, Unit unit, OrderType orderType, Region tile)
        {
            if (orderType != OrderType.Build && orderType != OrderType.Disband && orderType != OrderType.Hold)
                throw new ArgumentException($"Order type {orderType} is not supported.");

            Initiator = initiator;
            Unit = unit;
            Origin = tile;
            Destination = tile;
            Target = null;
            OrderType = orderType;
        
            AllOrders.Add(this);
        }
    
        public Order(Nation initiator, Unit unit, OrderType orderType, Region origin, Region destination, Region? target=null)
        {
            if ((orderType == OrderType.Support || orderType == OrderType.Convoy) && target == null)
                throw new ArgumentException("Convoy and Support orders need a Target.");
        
            Initiator = initiator;
            Unit = unit;
            Origin = origin;
            Destination = destination;
            Target = target;
            OrderType = orderType;
        
            AllOrders.Add(this);
        }
    
    
        public static void ValidateAllOrders()
        {
            EnsureLegalOrders();
            ResolveCombatOrders();
        }
    
        private static bool IsSupportCut(Order supportOrder)
        {
            return AllOrders.Any(o => o.OrderType == OrderType.Move && o.Destination == supportOrder.Origin);
        }
        private static bool IsConvoyDisrupted(Order convoyOrder)
        {
            return AllOrders.Any(o => o.OrderType == OrderType.Move && o.Destination == convoyOrder.Origin);
        }
        private static bool IsValidConvoyChain(Order moveOrder)
        {
            if (moveOrder.OrderType != OrderType.Move || !moveOrder.Unit.IsArmy)
                return false;

            var visited = new HashSet<Region>();
            var queue = new Queue<Region>();

            queue.Enqueue(moveOrder.Origin);
            visited.Add(moveOrder.Origin);

            while (queue.Count > 0)
            {
                var currentRegion = queue.Dequeue();
        
                var convoys = AllOrders
                    .Where(o => o.OrderType == OrderType.Convoy &&
                                o.Origin == currentRegion &&
                                o.Target == moveOrder.Origin &&
                                !IsConvoyDisrupted(o))
                    .Select(o => o.Destination);

                foreach (var convoyDestination in convoys)
                {
                    if (visited.Contains(convoyDestination)) 
                        continue;
                
                    if (convoyDestination == moveOrder.Destination)
                        return true;
                
                    visited.Add(convoyDestination);
                    queue.Enqueue(convoyDestination);
                }
            }

            return false;
        }
    
        private static void EnsureLegalOrders()
        {
            foreach (var order in AllOrders.ToList())
            {
                if (order.OrderType == OrderType.Move && !(order.Origin.Connections.Contains(order.Destination) || IsValidConvoyChain(order)))
                    AllOrders.Remove(order);
            
                if (order.OrderType == OrderType.Support && !order.Origin.Connections.Contains(order.Destination))
                    AllOrders.Remove(order);

                if (order.OrderType == OrderType.Build && (!order.Initiator.CoreRegions.Contains(order.Destination) || order.Destination.Unit != null))
                    AllOrders.Remove(order);
            }
        }
    
        private static void ResolveCombatOrders()
        {
            var movesByDestination = AllOrders
                .Where(o => o.OrderType == OrderType.Move || o.OrderType == OrderType.Hold)
                .GroupBy(o => o.Destination)
                .ToDictionary(g => g.Key, g => g.ToList());
        
            foreach (var entry in movesByDestination)
            {
                var destination = entry.Key;
                var competingOrders = entry.Value;
     
                if (competingOrders.Count == 1)
                    competingOrders[0].IsValid = true;
                else
                    ResolveConflict(competingOrders);
            }
        }

        private static void ResolveConflict(List<Order> competingOrders)
        {
            foreach (var orderStrength in competingOrders)
                orderStrength.Strength = CalculateStrength(orderStrength);

            var maxStrength = competingOrders.Max(o => o.Strength);
            var strongestOrders = competingOrders.Where(o => o.Strength == maxStrength).ToList();

            if (strongestOrders.Count == 1)
                strongestOrders[0].IsValid = true;
        }

        private static int CalculateStrength(Order order)
        {
            var supportOrders = AllOrders
                .Where(o => o.OrderType == OrderType.Support && o.Target == order.Origin && o.Destination == order.Destination)
                .ToList();

            var strength = supportOrders.Count(supportOrder => !IsSupportCut(supportOrder));

            return order.OrderType == OrderType.Hold ? strength * 2 + 1 : strength * 2;
        }
    
    
        public override string ToString()
        {
            var unitName = Unit.IsArmy ? "A" : "F";

            return OrderType switch
            {
                OrderType.Hold    => $"{unitName} {Origin.ShortName} H",
                OrderType.Move    => $"{unitName} {Origin.ShortName} -> {Destination.ShortName}",
                OrderType.Support => $"{unitName} {Origin.ShortName} S {Target!.ShortName} -> {Destination.ShortName}",
                OrderType.Convoy  => $"{unitName} {Origin.ShortName} C {Target!.ShortName} -> {Destination.ShortName}",
                OrderType.Build   => $"Build {unitName} {Origin.ShortName}",
                OrderType.Disband => $"Disband {unitName} {Origin.ShortName}",
                _ => "?"
            };
        }
    }
}