using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

namespace CardGrid
{
    public class CardGameObject : MonoBehaviour
    {
        public Card CardState;
        
        public SpriteRenderer Sprite;
        public TextMeshProUGUI QuantityText;
        public GameObject Highlight;
        public TextMeshProUGUI DebugPosition;
    }
}
