using ForestGambit.Gameplay.Core.Board;
using System.Collections.Generic;
using UnityEngine;

namespace ForestGambit.Gameplay.Core.Entity
{
    public class Unit : MonoBehaviour, IUnit
    {
        [SerializeField] private string unitName;
        [SerializeField, TextArea] private string description;

        private UnitTransform unitTransform;

        public string Name => unitName;
        public string Description => description;

        private void Awake()
        {
            unitTransform = GetComponent<UnitTransform>();

            if (unitTransform == null)
            {
                Debug.LogError("UnitTransform component is missing on Unit", this);
            }
        }

        public virtual List<GridCoordinates> GetAvailablePositions()
        {
            // Override in derived classes to implement specific movement patterns
            return new List<GridCoordinates>();
        }

        public virtual bool CanMoveTo(GridCoordinates destination)
        {
            // Override in derived classes to implement movement rules
            return false;
        }

        public virtual bool TryMoveTo(GridCoordinates destination)
        {
            if (!CanMoveTo(destination))
                return false;

            unitTransform.MoveTo(destination);
            return true;
        }
    }
}