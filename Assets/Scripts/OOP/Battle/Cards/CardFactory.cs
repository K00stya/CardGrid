using UnityEngine;

namespace CardGrid.Battle
{
    public class CardFactory : MonoBehaviour
    {
        private string[] EnemiesNames = { "Ghost", "Demons"};
        private string[] ItemsNames = { "Book", "Hammer", "Claws"};
        
        private FieldProxy _field;

        private Enemy _enemyPrefab;
        private Item _itemPrefab;
        private float _chanceItem;
        private int MinQuantityCard = 1;
        private int MaxQuantityCard = 10;

        public CardFactory(FieldProxy field)
        {
            _field = field;
        }
        
        public Card CreateRandomCard()
        {
            if (Random.value <= _chanceItem)
            {
                return CreateRandomItemCard();
            }
            else
            {
                return CreateRandomEnemy();
            }
        }

        public Card CreateRandomItemCard()
        {
            var name = ItemsNames[Random.Range(0, ItemsNames.Length)];
            var card = CreateCard(name);
            switch (name)
            {
                case "Hammer":
                    card.MapDamage = new int[,]
                    {
                        {0, 0, 1, 0, 0},
                        {0, 1, 1, 1, 0},
                        {1, 1, 1, 1, 1},
                        {0, 1, 1, 1, 0},
                        {0, 0, 1, 0, 0}
                    };
                    break;
                case "Claws":
                    card.MapDamage = new int[,]
                    {
                        {0, 0, 0},
                        {1, 1, 1},
                        {0, 0, 0}
                    };
                    break;
                case "Book":
                    card.MapDamage = new int[,]
                    {
                        {1, 0, 1},
                        {0, 0, 0},
                        {1, 0, 1}
                    };
                    break;
            }

            return card;
        }

        public Card CreateRandomEnemy()
        {
            var name = EnemiesNames[Random.Range(0, EnemiesNames.Length)];
            var card = CreateCard(name);
            switch (name)
            {
                case "Ghost":
                    card.MapDamage = new int[,]
                    {
                        {1, 1, 1},
                        {1, 0, 1},
                        {1, 1, 1}
                    };
                    break;
                case "Demons":
                    card.MapDamage = new int[,]
                    {
                        {0, 1, 0},
                        {1, 0, 1},
                        {0, 1, 0}
                    };
                    break;
            }

            return card;
        }

        private Card CreateCard(string name)
        {
            Card card = Instantiate(_itemPrefab);
            var sprite = Resources.Load<Sprite>($"Sprites/Cards/{name}");
            card.SetCard(sprite, Random.Range(MinQuantityCard, MaxQuantityCard));
            return card;
        }
    }
}