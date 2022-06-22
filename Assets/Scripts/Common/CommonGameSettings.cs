using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using DebugType = CardGrid.DebugSystem.Type;

namespace CardGrid
{
    [CreateAssetMenu(menuName = "CardGrid/Settings", fileName = "GameSettings")]
    public class CommonGameSettings : ScriptableObject
    {
        public DebugSettings Debug;
        public FrontSettings Front;

        private void Awake()
        {
            
        }

        [Serializable]
        public class DebugSettings
        {
            [Serializable]
            public class DebugChannel
            {
                [NonSerialized]
                public DebugType Type;
                [HideInInspector]
                public string name;
                public bool Active;
            }

            public List<DebugChannel> DebugsChannels = new List<DebugChannel>()
            {
                CreateChannel(DebugType.Battle),
                CreateChannel(DebugType.PlayerInput),
                CreateChannel(DebugType.SaveSystem),
                CreateChannel(DebugType.Error)
            };


            static DebugChannel CreateChannel(DebugType type)
            {
                return new DebugChannel
                {
                    Type = type,
                    name = type.ToString(),
                    Active = true
                };
            }
        }

        [Serializable]
        public class FrontSettings
        {
            public TextTypeSettings[] TextTypeSettingsArray;
            
            [Serializable]
            public struct TextTypeSettings
            {
                public FontType Type;
                public Color DefaultColor;
                public TMP_FontAsset Font;
            }
            
            public enum FontType
            {
                Default,
                Roboto
            }
        }
    }
}