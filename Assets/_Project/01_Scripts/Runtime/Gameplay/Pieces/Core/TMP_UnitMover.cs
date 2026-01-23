using ForestGambit.Gameplay.Core.Board;
using ForestGambit.Gameplay.Core.Entity;
using UnityEngine;

public class TMP_UnitMover : MonoBehaviour
{
    [SerializeField] private UnitTransform unitTransform;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            unitTransform.MoveBy(new GridCoordinates(0, 1));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            unitTransform.MoveBy(new GridCoordinates(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            unitTransform.MoveBy(new GridCoordinates(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            unitTransform.MoveBy(new GridCoordinates(1, 0));
        }
    }
}
