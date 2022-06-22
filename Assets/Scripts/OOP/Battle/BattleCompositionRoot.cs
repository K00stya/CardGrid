using CardGrid.Battle;
using UnityEngine;

namespace CardGrid.Battle
{
    public class BattleCompositionRoot : MonoBehaviour
    {
        [SerializeField]
        private BattleUI BattleUI;

        [SerializeField]
        private Field field;

        [SerializeField]
        private Inventory inventory;

        private void Awake()
        {
            IoCContainer.Get<GameCompositionRoot>().InitBattleCompositionRoot(this);
        }

        public void Init()
        {
            
        }

        public void StartBattle()
        {
            
        }
    }
}