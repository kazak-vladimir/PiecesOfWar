using ForestGambit.Gameplay.Core.Board;
using System.Collections.Generic;

namespace ForestGambit.Gameplay.Core.Entity
{
    public interface IUnit
    {
        public string Name { get; }
        public string Description { get; }

        public abstract List<GridCoordinates> GetAvailablePositions();
        public abstract bool CanMoveTo(GridCoordinates destination);
        public abstract bool TryMoveTo(GridCoordinates destination);
    }
}