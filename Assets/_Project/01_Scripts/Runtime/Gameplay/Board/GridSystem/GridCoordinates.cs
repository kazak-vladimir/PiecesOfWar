using System;

namespace PiecesOfWar.Gameplay.Board
{
    /// <summary>
    /// Represents a position on the game grid.
    /// Immutable struct for grid coordinates (X, Y).
    /// </summary>
    [Serializable]
    public struct GridCoordinates : IEquatable<GridCoordinates>
    {
        public int X;
        public int Y;

        public GridCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static GridCoordinates Zero => new GridCoordinates(0, 0);
        
        public bool Equals(GridCoordinates other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoordinates other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(GridCoordinates a, GridCoordinates b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(GridCoordinates a, GridCoordinates b)
        {
            return !a.Equals(b);
        }
    }
}
