using Unity.Mathematics;
using UnityEngine;

namespace CardGrid.Battle
{
    public class PickUpItemAbility : Ability
    {
        public PickUpItemAbility(FieldProxy fieldProxy) : base(fieldProxy)
        {}
        
        public override void ExecuteOnCellObject(Vector2Int position)
        {
            
        }
    }
}