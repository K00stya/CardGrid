using TMPro;

namespace CardGrid
{
    public class CustomText : TextMeshProUGUI
    {
        public CommonGameSettings.FrontSettings.TextTypeSettings TextSettings;

        protected override void Awake()
        {
            font = TextSettings.Font;
            color = TextSettings.DefaultColor;
            base.Awake();
        }
    }
}