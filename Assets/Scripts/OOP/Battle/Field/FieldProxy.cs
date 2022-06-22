using Unity.Mathematics;
using UnityEngine;

namespace CardGrid.Battle
{
    public class FieldProxy
    {
        
        private Field _field;
        private AbilitiesConverter _abilitiesConverter;

        public FieldProxy(Field filed)
        {
            _field = filed;
        }
        
        //Here queue abilities
        public void UseAbilitiesOnField(AbilityAction abilityAction)
        {
            _abilitiesConverter.ConvertAbilities(
                abilityAction.Position, abilityAction.Quantity, abilityAction.MapAbilities);
            DamageCellObjects(_abilitiesConverter.DamagePositions);
        }

        public void DamageCellObject(Vector2Int position, int quantity)
        {
            
            _field.TryDamageCellObject(position, quantity);
        }
        
        private void DamageCellObjects(DamageInfo[] damageCells)
        {
            
            
            _field.DamageCellObjects(damageCells);
        }
    }
}