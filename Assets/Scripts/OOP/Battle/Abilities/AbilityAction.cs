using Unity.Mathematics;
using UnityEngine;

namespace CardGrid.Battle
{
    public struct AbilityAction
    {
        public AbilityAction(Vector2Int position, int quantity, string[,] mapAbilities, string[] commonAbilities)
        {
            Position = position;
            Quantity = quantity;
            MapAbilities = mapAbilities;
            CommonAbilities = commonAbilities;
        }
        
        public Vector2Int Position;
        public int Quantity;
        public string[,] MapAbilities;
        public string[] CommonAbilities;
    }
}