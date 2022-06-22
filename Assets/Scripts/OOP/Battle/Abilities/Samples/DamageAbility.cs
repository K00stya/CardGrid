
namespace CardGrid.Battle
{
    public class DamageAbility : Ability
    {
        public DamageAbility(FieldProxy fieldProxy) : base(fieldProxy)
        {}

        public override void ExecuteOnCellObject(DamageInfo damage)
        {
            _fieldProxy.DamageCellObject(damage.Position, damage.Damage);
        }
    }
}