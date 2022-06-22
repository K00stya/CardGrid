using UnityEngine;

namespace CardGrid.Battle
{
    public class Inventory : Grid
    {
        private CardFactory _cardFactory;
        
        public Inventory(CardFactory cardFactory)
        {
            _cardFactory = cardFactory;
        }
        
        public void FillInventory()
        {
            for (int x = 0; x < SizeX; x++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    var item = _cardFactory.CreateRandomItemCard();
                    item.transform.SetParent(transform);
                }
            }
        }
        
        public void AddItem()
        {
            
        }
    }
}