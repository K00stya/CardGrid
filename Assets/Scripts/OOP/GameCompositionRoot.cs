using System;
using CardGrid.Battle;
using CardGrid.Menu;
using UnityEngine;

namespace CardGrid
{
    public class GameCompositionRoot : MonoBehaviour
    {
        [SerializeField]
        private MainMenu MainMenu;

        [SerializeField]
        private BattleCompositionRoot BattleCompositionRoot;
        
        private PlayerSettingsState _playerSettings;
        
        
        private void Awake()
        {
            IoCContainer.RegisterInstance(this);

            var playerSetting = new PlayerSettingsState();
            var menu = new Menu.MainMenu();

        }

        public void InitBattleCompositionRoot(BattleCompositionRoot battle)
        {
            
        }
    }
}