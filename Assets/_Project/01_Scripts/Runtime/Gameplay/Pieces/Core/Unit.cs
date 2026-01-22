using ForestGambit.Gameplay.Core.Board;
using System.Collections.Generic;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    public class Unit : MonoBehaviour, IUnit
    {
        public string Name => throw new System.NotImplementedException();
        public string Description => throw new System.NotImplementedException();

        private UnitTransform unitTransform;

        public void Awake()
        {
            unitTransform ??= GetComponent<UnitTransform>();
            if (unitTransform == null)
            {
                Debug.LogError("UnitTransform component is missing on Unit.");
            }
        }

        public List<GridCoordinates> GetAvailablePositions()
        {
            throw new System.NotImplementedException();
        }

        public bool CanMoveTo(GridCoordinates destination)
        {
            throw new System.NotImplementedException();
        }

        public bool TryMoveTo(GridCoordinates destination)
        {
            throw new System.NotImplementedException();
        }
    }
}