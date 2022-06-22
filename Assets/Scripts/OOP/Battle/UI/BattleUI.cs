using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CardGrid.Battle
{
    public class BattleUI : MonoBehaviour
    {
        public UnityAction OnMenuButtonClick;
        
        [SerializeField]
        private Button MenuButton;

        [SerializeField]
        private Button MenuButtonOnDefeat;

        [SerializeField]
        private TextMeshProUGUI Score;

        private void Awake()
        {
            MenuButton.onClick.AddListener(OnMenuButtonClick);
            MenuButtonOnDefeat.onClick.AddListener(OnMenuButtonClick);
        }

        public void UpdateScore(int value)
        {
            Score.text = value.ToString();
        }
    }
}