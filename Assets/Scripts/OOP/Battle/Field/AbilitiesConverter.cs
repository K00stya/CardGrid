using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

namespace CardGrid.Battle
{
    public class AbilitiesConverter
    {
        private Vector2Int _fieldSize;
        
        
        private string[,] _abilities;
        private int _quantity;
        private Vector2Int _position;


        public DamageInfo[] DamagePositions { get; private set; }
        public PushInfo[] PushPositions { get; private set; }
        public Vector2Int[] PickUpItemsPositions { get; private set; }


        public AbilitiesConverter(Vector2Int fieldSize)
        {
            _fieldSize = fieldSize;
        }

        public void ConvertAbilities(Vector2Int position, int quantity, string[,] abilities)
        {
            _position = position;
            _quantity = quantity;
            _abilities = abilities;
        }

        private bool TryConvertAbilities(Vector2Int position, int quantity, string[,] abilities)
        {
            int xCenter = (int)math.round((abilities.GetLength(0) / 2f));
            for (int x = -xCenter; x < xCenter * 2; x++)
            {
                for (int y = -xCenter; y < xCenter * 2; y++)
                {
                    
                }
            }

            return true;
        }

        private void VerifyCenter(int center)
        {
            if (center % 2 != 1)
            {
                
            }
        }
    }
}