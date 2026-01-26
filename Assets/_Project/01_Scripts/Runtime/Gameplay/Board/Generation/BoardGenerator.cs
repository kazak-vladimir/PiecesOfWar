using UnityEngine;
using System;

namespace ForestGambit.Gameplay.Core.Board.Generation
{
    /// <summary>
    /// Responsible for generating the game world.
    /// </summary>
    public class BoardGenerator : MonoBehaviour
    {
        [Serializable]
        public struct BoardBounds { public GridCoordinates from, to; }

        [SerializeField] private BoardBounds boardBounds;
        [SerializeField] private GameObject darkTile, brightTile;

        public BoardBounds GetBoardBounds => boardBounds;

        private void OnValidate()
        {
            if (boardBounds.to.x < boardBounds.from.x)
                boardBounds.to.x = boardBounds.from.x;
            if (boardBounds.to.y < boardBounds.from.y)
                boardBounds.to.y = boardBounds.from.y;
        }

        public void Awake()
        {
            GenerateBoard();
        }

        public void GenerateBoard()
        {
            for (int x = boardBounds.from.x; x <= boardBounds.to.x; x++)
            {
                for (int y = boardBounds.from.y; y <= boardBounds.to.y; y++)
                {
                    GridCoordinates coords = new GridCoordinates(x, y);
                    GameObject tilePrefab = ((x + y) & 1) == 1 ? brightTile : darkTile;
                    Instantiate(tilePrefab, coords, Quaternion.identity, transform);
                }
            }
        }

        public void ClearBoard()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }

        public void RegenerateBoard()
        {
            ClearBoard();
            GenerateBoard();
        }

        public void OnDrawGizmos()
        {
            float halfCell = GridCoordinates.CellSize / 2f;

            Gizmos.color = Color.green;
            Vector3 fromWorld = new Vector3(boardBounds.from.x - halfCell, 0, boardBounds.from.y - halfCell);
            Vector3 toWorld = new Vector3(boardBounds.to.x + halfCell, 0, boardBounds.to.y + halfCell);
            Vector3 size = toWorld - fromWorld;
            Gizmos.DrawWireCube(fromWorld + size / 2, size);
        }
    }
}
