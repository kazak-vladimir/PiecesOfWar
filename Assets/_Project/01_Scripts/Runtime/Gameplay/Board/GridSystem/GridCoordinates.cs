using System;

namespace ForestGambit.Gameplay.Core.Board
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

        public bool Equals(GridCoordinates other) => X == other.X && Y == other.Y;

        public override bool Equals(object obj) => obj is GridCoordinates other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X}, {Y})";

        public static bool operator ==(GridCoordinates a, GridCoordinates b) => a.Equals(b);

        public static bool operator !=(GridCoordinates a, GridCoordinates b) => !a.Equals(b);

        public static GridCoordinates operator +(GridCoordinates a, GridCoordinates b) =>
            new GridCoordinates(a.X + b.X, a.Y + b.Y);

        public static GridCoordinates operator -(GridCoordinates a, GridCoordinates b) =>
            new GridCoordinates(a.X - b.X, a.Y - b.Y);

        public static GridCoordinates operator *(GridCoordinates a, int scalar) =>
            new GridCoordinates(a.X * scalar, a.Y * scalar);

        public static GridCoordinates operator /(GridCoordinates a, int scalar) =>
            new GridCoordinates(a.X / scalar, a.Y / scalar);
    }
}