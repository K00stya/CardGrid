using Unity.Mathematics;
using UnityEngine;

namespace CardGrid.Battle
{
    public class Field : Grid
    {
        public Field(CardFactory cardFactory)
        {
            
        }
        
        public void DamageCellObjects(DamageInfo[] damageCells)
        {
            foreach (var dc in damageCells)
            {
                TryDamageCellObject(dc.Position, dc.Damage);
            }
        }
        
        public bool TryDamageCellObject(Vector2Int position, int quantity)
        {

            return false;
        }
    }
}