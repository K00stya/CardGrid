using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CardGrid.Menu
{
    public class MainMenu : MonoBehaviour
    {
        public UnityAction OnContinueBattle;
        public UnityAction OnStartNewBattle;
        public UnityAction<float> OnVolumeChange;
        public UnityAction<int> OnLanguageChange;

        [SerializeField]
        private TextMeshProUGUI BestScore;

        [SerializeField] 
        private Button ContinueBattle;

        [SerializeField] 
        private Button StartNewBattle;

        [SerializeField]
        private Slider Volume;

        [SerializeField]
        private Dropdown Language;

        public void UpdateBestScore(int value)
        {
            BestScore.text = value.ToString();
        }
    }
}