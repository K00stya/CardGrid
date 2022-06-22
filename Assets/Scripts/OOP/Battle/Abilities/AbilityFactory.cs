
namespace CardGrid.Battle
{
    public class AbilityFactory
    {
        private FieldProxy _fieldProxy;

        public Ability CreateAbility(string name)
        {
            Ability ability = null;
            switch (name)
            {
                case "Damage":
                    ability = new DamageAbility(_fieldProxy);
                    break;
                
                case "Push":
                    ability = new PushAbility(_fieldProxy);
                    break;
                
                case "PickUpItem":
                    ability = new PickUpItemAbility(_fieldProxy);
                    break;
                
                default:
                    
                    
                    break;
            }

            return ability;
        }
    }
}