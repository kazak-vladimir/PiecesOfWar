using System;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Board
{
    /// <summary>
    /// Represents a position on the game grid.
    /// Immutable struct for grid coordinates (x, y).
    /// </summary>
    [Serializable]
    public struct GridCoordinates : IEquatable<GridCoordinates>
    {
        public int x;
        public int y;

        public static float CellSize => 1f;

        public GridCoordinates(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static GridCoordinates Zero => new GridCoordinates(0, 0);

        public bool Equals(GridCoordinates other) => x == other.x && y == other.y;

        public override bool Equals(object obj) => obj is GridCoordinates other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(x, y);

        public override string ToString() => $"({x}, {y})";

        public static bool operator ==(GridCoordinates a, GridCoordinates b) => a.Equals(b);

        public static bool operator !=(GridCoordinates a, GridCoordinates b) => !a.Equals(b);

        public static GridCoordinates operator +(GridCoordinates a, GridCoordinates b) =>
            new GridCoordinates(a.x + b.x, a.y + b.y);

        public static GridCoordinates operator -(GridCoordinates a, GridCoordinates b) =>
            new GridCoordinates(a.x - b.x, a.y - b.y);

        public static GridCoordinates operator *(GridCoordinates a, int scalar) =>
            new GridCoordinates(a.x * scalar, a.y * scalar);

        public static GridCoordinates operator /(GridCoordinates a, int scalar) =>
            new GridCoordinates(a.x / scalar, a.y / scalar);

        public static GridCoordinates GetGridCoordinatesFromWorldPosition(Vector3 worldPosition) =>
            new GridCoordinates(
                Mathf.RoundToInt(worldPosition.x * CellSize),
                Mathf.RoundToInt(worldPosition.z * CellSize)
            );

        public static Vector3 ToWorldPosition(GridCoordinates gridCoordinates) =>
            new(gridCoordinates.x * CellSize, 0, gridCoordinates.y * CellSize);

        public static implicit operator Vector3(GridCoordinates gridCoordinates) =>
            new Vector3(gridCoordinates.x * CellSize, 0, gridCoordinates.y * CellSize);

        public static implicit operator GridCoordinates(Vector3 worldPosition) =>
            new GridCoordinates(
                Mathf.RoundToInt(worldPosition.x / CellSize),
                Mathf.RoundToInt(worldPosition.z / CellSize)
            );
    }
}