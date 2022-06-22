using Unity.Mathematics;
using UnityEngine;

namespace CardGrid.Battle
{
    public abstract class Ability
    {
        protected FieldProxy _fieldProxy;

        public Ability(FieldProxy fieldProxy)
        {
            _fieldProxy = fieldProxy;
        }

        public virtual void ExecuteOnCellObject(Vector2Int position) { }
        public virtual void ExecuteOnCellObject(DamageInfo damage) {}
        public virtual void ExecuteOnCellObject(PushInfo push) {}
    }
}