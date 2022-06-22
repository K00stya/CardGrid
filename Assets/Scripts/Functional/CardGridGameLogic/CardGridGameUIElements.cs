using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardGrid
{
    public partial class CardGridGame
    {
        [Header("Menu")] public GameObject Menu;
        public TextMeshProUGUI BestScore;
        public Button Continue;
        public Button NewBattleButton;
        public Slider VolumeSlider;
        public TMP_Dropdown LanguageDropdown;

        [Header("Battle UI")] 
        public GameObject BattleUI;
        public TextMeshProUGUI Score;
        public Button OpenMenu;
        public GameObject Defeat;
        public Button ToMenuOnDeafeat;
    }
}