using System;

namespace GameLogic
{
    public enum Seasons
    {
        Spring,
        Fall,
        Winter,
    }

    public readonly struct Turn : IEquatable<Turn>
    {
        public Turn(uint year, Seasons season)
        {
            Year = year;
            Season = season;
        }

        private uint Year { get; }
        private Seasons Season { get; }

        public bool Equals(Turn other)
        {
            return Year == other.Year && Season == other.Season;
        }

        public override bool Equals(object obj)
        {
            return obj is Turn other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Year, (int)Season);
        }
    }
}