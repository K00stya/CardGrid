using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace CardGrid
{
    public partial class CardGridGame
    {
        [Header("Battle")] 
        public CardGameObject CardPrefab;
        public GridGameObject Field;
        public GridGameObject Inventory;
        public Transform Effects;
        public string[] NameEnemies;
        public string[] NameItems;
        [Min(1)] public int StartMaxCellQuantity = 10;
        [Range(0, 1)] public float ChanceItemOnFiled = 0.1f;
        public float SpeedRecession = 0.1f;
        public float SpeedFilling = 0.1f;

        
        //Local fixedUpdate variables, but cached for the glory of the garbage collector
        Ray _ray;
        RaycastHit[] _hits;
        Plane _plane = new Plane(Vector3.up, Vector3.zero);

        CardGameObject _dragGameObjectCard;
        CardGameObject _hitFieldCardOnDrag;
        List<CardGameObject> _highlightCards = new List<CardGameObject>();

        bool _needRecession;

        void FixedUpdate()
        {
            if (!_inputActive) return;
            if (RaycastIsHitCard(out var cardMonobeh))
            {
                DebugSystem.DebugLog($"Raycast hit card {cardMonobeh.CardState.name}" +
                                     $" on {cardMonobeh.CardState.Position}", DebugSystem.Type.PlayerInput);

                if (Input.GetMouseButton(0))
                {
                    //Start drag
                    if (_dragGameObjectCard == null)
                    {
                        _dragGameObjectCard = cardMonobeh;
                    }
                }
                else
                {
                    _inputActive = false;
                    StartCoroutine(EndDrag());
                }
            }

            //Drag
            if (_dragGameObjectCard != null && _plane.Raycast(_ray, out var enter))
            {
                Drag(enter);
            }

            bool RaycastIsHitCard(out CardGameObject cardMonobeh)
            {
                cardMonobeh = null;
                _ray = _camera.ScreenPointToRay(Input.mousePosition);
                _hits = Physics.RaycastAll(_ray);
                return (_hits != null && _hits.Length > 0 &&
                        _hits[0].collider.TryGetComponent<CardGameObject>(out cardMonobeh));
            }
        }

        void Drag(float enter)
        {
            DebugSystem.DebugLog($"Drag card {_dragGameObjectCard.CardState.name}", DebugSystem.Type.PlayerInput);
            var newPosition = _ray.GetPoint(enter);
            _dragGameObjectCard.transform.position = new Vector3(newPosition.x, 1f, newPosition.z);

            Highlight(_dragGameObjectCard);

            void Highlight(CardGameObject activeCard)
            {
                if (_hits == null) return;

                bool dragOnField = false;
                foreach (var obj in _hits)
                {
                    var monobeh = obj.collider.GetComponent<CardGameObject>();
                    if (!monobeh || activeCard == monobeh)
                        continue;
                    if (_hitFieldCardOnDrag == monobeh)
                    {
                        dragOnField = true;
                        continue;
                    }

                    var firstCard = activeCard.CardState;
                    var secondCard = monobeh.CardState;

                    if (firstCard.Grid == CardGrid.Inventory && secondCard.Grid == CardGrid.Field)
                    {
                        dragOnField = true;
                        DisableHighlight();
                        _hitFieldCardOnDrag = monobeh;
                        var cards = GetImpactCards(firstCard, secondCard);
                        foreach (var card in cards)
                        {
                            var highlightGO = card.GameObject;
                            highlightGO.Highlight.SetActive(true);
                            _highlightCards.Add(highlightGO);
                        }
                    }
                }

                //If player not drag on field we disable highlights
                if (!dragOnField)
                {
                    DisableHighlight();
                }

                Card[] GetImpactCards(Card itemCard, Card fieldCard)
                {
                    DebugSystem.DebugLog($"Item {itemCard.name}, impact on {fieldCard.name}" +
                                         $" X:{fieldCard.Position.x} Y:{fieldCard.Position.y}",
                        DebugSystem.Type.Battle);

                    //TODO Push PickUp
                    int[,] attackArray = GetImpactMap<DamageMaps>(itemCard.name);

                    return GetImpactedCards(itemCard.name, fieldCard.Position, attackArray);
                }
            }
        }

        IEnumerator EndDrag()
        {
            if (_dragGameObjectCard == null)
            {
                _inputActive = true;
                yield break;
            }

            if (_highlightCards.Count > 0)
            {
                yield return StartCoroutine(DealImpact());
            }
            else
            {
                DisableHighlight();
                var dragCard = _dragGameObjectCard.CardState;
                if (dragCard.Grid == CardGrid.Field)
                {
                    _dragGameObjectCard.transform.position = Field.GetCellSpacePosition(dragCard.Position);
                }
                else
                {
                    _dragGameObjectCard.transform.position = Inventory.GetCellSpacePosition(dragCard.Position);
                }
            }

            _dragGameObjectCard = null;
            _inputActive = true;
        }

        IEnumerator DealImpact()
        {
            yield return StartCoroutine(UseItemOnFiled(_dragGameObjectCard, _highlightCards));
            var cells = _CommonState.BattleState.Filed.Cells;
            var items = _CommonState.BattleState.Inventory.Items;

            do
            {
                RecessionField(cells);
                yield return StartCoroutine(Filling(cells));
                yield return StartCoroutine(CellsCombinations(cells));

                RecessionInventory(items);
                yield return StartCoroutine(ItemsCombinations(items));
                yield return StartCoroutine(GetNewItemsForField(cells, items));
            } while (_needRecession);

            CheckDefeat(items);
        }

        #region DealImpact
        
        IEnumerator UseItemOnFiled(CardGameObject dragCard, List<CardGameObject> highlightCards)
        {
            Card impactCard = dragCard.CardState;
            Card[] cards = new Card[highlightCards.Count];
            for (int i = 0; i < highlightCards.Count; i++)
            {
                cards[i] = highlightCards[i].CardState;
            }
            DisableHighlight();

            yield return SpawnEffectOnCards(impactCard, cards);

            List<Card> deaths = new List<Card>();
            List<Card> woundeds = new List<Card>(cards);

            ImpactDamageOnField(impactCard.Quantity, cards, ref deaths);
            impactCard.Quantity = 0;
            dragCard.gameObject.SetActive(false);

            int debug = 0;
            do
            {
                if (DebugLoopCount()) yield break;

                do
                {
                    yield return StartCoroutine(
                        ImpactWounded(woundeds.ToArray(),woundeds, deaths));

                    if (DebugLoopCount()) yield break;
                } while (woundeds.Count > 0);

                do
                {
                    yield return StartCoroutine(
                        ImpactDead(deaths, woundeds, deaths));

                    if (DebugLoopCount()) yield break;
                } while (deaths.Count > 0);

            } while (deaths.Count > 0 || woundeds.Count > 0);

            bool DebugLoopCount()
            {
                debug++;
                if (debug > 300)
                {
                    DebugSystem.DebugLog("Infinity impact loop =(", DebugSystem.Type.Error);
                    return true;
                }

                return false;
            }
        }
        
        void RecessionField(Card[,] cards)
        {
            for (int i = 0; i < cards.GetLength(1); i++)
            {
                for (int x = 0; x < cards.GetLength(0); x++)
                {
                    for (int z = cards.GetLength(1) - 1; z >= 0; z--)
                    {
                        int newZ = z - 1;
                        if (cards[x, z].Quantity <= 0 && newZ >= 0)
                        {
                            SwapPositions(cards, cards[x, z].Position, cards[x, newZ].Position);
                        }
                    }
                }
            }

            foreach (var card in cards)
            {
                if (card.Quantity <= 0)
                {
                    card.GameObject.transform.position = Field.GetSpawnPosition(card.Position.x);
                }
                else
                {
                    card.GameObject.transform.DOMove(
                        Field.GetCellSpacePosition(card.Position), SpeedRecession);
                }
            }
        }

        IEnumerator Filling(Card[,] cards)
        {
            for (int z = cards.GetLength(1) - 1; z >= 0; z--)
            {
                for (int x = 0; x < cards.GetLength(0); x++)
                {
                    if (cards[x, z].Quantity <= 0)
                    {
                        ReCreateCard(cards[x, z]);
                        cards[x, z].GameObject.transform.DOMove(
                            Field.GetCellSpacePosition(cards[x, z].Position), SpeedFilling);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(SpeedFilling);
        }
        
        //TODO
        IEnumerator CellsCombinations(Card[,] cards)
        {
            yield return null;
        }
        
        void RecessionInventory(Card[,] items)
        {
            for (int z = 0; z < items.GetLength(1); z++)
            {
                for (int x = 0; x < items.GetLength(0); x++)
                {
                    if (items[x, z].Quantity <= 0)
                    {
                        int newX = x + 1;
                        int newZ = z + 1;
                        if (newX < items.GetLength(0))
                        {
                            SwapPositions(items, items[x, z].Position, items[newX, z].Position);
                        }
                        else if (newZ < items.GetLength(1))
                        {
                            SwapPositions(items, items[x, z].Position, items[0, newZ].Position);
                        }
                    }
                }
            }
        }
        
        //TODO
        IEnumerator ItemsCombinations(Card[,] items)
        {
            yield return null;
        }
        
        /*
        * Instead of removing an object from the inventory and creating a new one on the field,
        * I'll just send the item from the field to the inventory, and the extra item(disabled) to the field
         *
        * Repeats as long as there are items in the bottom line
         * 
        * With a lack of resources, string comparison can also be avoided,
        * for example, by the state of the card (InInventory or OnFiled)
        */
        IEnumerator GetNewItemsForField(Card[,] cells, Card[,] items)
        {
            bool newItems = false;
            int lowerZ = cells.GetLength(1) - 1;
            for (int x = 0; x < cells.GetLength(0); x++)
            {
                var card = cells[x, lowerZ];
                if (NameItems.Contains(card.name))
                {
                    _needRecession = true;
                    newItems = true;
                    yield return StartCoroutine(MoveInventory(x));

                    card.Grid = CardGrid.Inventory;
                    card.Position = new Vector2Int(0, 0);
                    items[0, 0] = card;
                    items[0, 0].GameObject.transform.DOMove(
                        Inventory.GetCellSpacePosition(items[0, 0].Position), SpeedRecession);
                    yield return new WaitForSeconds(SpeedRecession);
                }
            }

            if (!newItems)
            {
                _needRecession = false;
                foreach (var card in items)
                {
                    card.GameObject.transform.DOMove(
                        Inventory.GetCellSpacePosition(card.Position), SpeedRecession);
                }

                yield return new WaitForSeconds(SpeedRecession);
            }

            IEnumerator MoveInventory(int x)
            {
                for (int iz = items.GetLength(1) - 1; iz >= 0; iz--)
                {
                    for (int ix = items.GetLength(0) - 1; ix >= 0; ix--)
                    {
                        int newX = ix + 1;
                        int newZ = iz + 1;
                        if (newX < items.GetLength(0))
                        {
                            items[newX, iz] = items[ix, iz];
                            items[newX, iz].Position = new Vector2Int(newX, iz);
                            items[newX, iz].GameObject.transform.DOMove(
                                Inventory.GetCellSpacePosition(items[newX, iz].Position), SpeedRecession);
                            yield return new WaitForSeconds(SpeedRecession);
                        }
                        else if (newZ < items.GetLength(1))
                        {
                            items[0, newZ] = items[ix, iz];
                            items[0, newZ].Position = new Vector2Int(0, newZ);
                            items[0, newZ].GameObject.transform.DOMove(
                                Inventory.GetCellSpacePosition(items[0, newZ].Position), SpeedRecession);
                            yield return new WaitForSeconds(SpeedRecession);
                        }
                        else
                        {
                            var excessItem = items[ix, iz];
                            excessItem.Grid = CardGrid.Field;
                            excessItem.name = "";
                            excessItem.Quantity = 0;
                            excessItem.GameObject.gameObject.SetActive(false);
                            excessItem.Position = new Vector2Int(x, lowerZ);
                            cells[x, lowerZ] = excessItem;
                        }
                    }
                }
            }
        }
        
        #endregion
        
        IEnumerator ImpactWounded(Card[] woundeds, List<Card> newWoundeds, List<Card> newDeaths)
        {
            foreach (var wounded in woundeds)
            {
                switch (wounded.name)
                {
                    case "Demons":
                        int[,] attackArray = GetImpactMap<DamageMaps>(wounded.name);
                        var cards = GetImpactedCards(wounded.name, wounded.Position, attackArray);
                        SpawnEffectOnCard(wounded);
                        yield return new WaitForSeconds(SpawnEffectOnCards(wounded, cards));
                        newWoundeds.AddRange(cards);
                        ImpactDamageOnField(1, cards, ref newDeaths);
                        break;
                }

                newWoundeds.Remove(wounded);
            }
        }

        IEnumerator ImpactDead(List<Card> deaths, List<Card> newWoundeds, List<Card> newDeaths)
        {
            foreach (var dead in deaths.ToArray())
            {
                switch (dead.name)
                {
                    case "Ghost":
                        int[,] attackArray = GetImpactMap<DamageMaps>(dead.name);
                        var cards = GetImpactedCards(dead.name, dead.Position, attackArray);
                        yield return new WaitForSeconds(SpawnEffectOnCard(dead));
                        newWoundeds.AddRange(cards);
                        ImpactDamageOnField(dead.StartQuantity, cards, ref newDeaths);
                        break;
                }

                newDeaths.Remove(dead);
            }
        }
        
        void ImpactDamageOnField(int damage, Card[] cards, ref List<Card> deaths)
        {
            foreach (var card in cards)
            {
                card.Quantity -= damage;
                if (card.Quantity <= 0)
                {
                    deaths.Add(card);
                    card.GameObject.gameObject.SetActive(false);
                    
                    _CommonState.BattleState.Score += card.StartQuantity;
                    Score.text = _CommonState.BattleState.Score.ToString();
                    if (_CommonState.BattleState.Score > _CommonState.BestScore)
                        _CommonState.BestScore = _CommonState.BattleState.Score;
                    continue;
                }

                card.GameObject.QuantityText.text = card.Quantity.ToString();
            }
        }

        //The X coordinates require a second dimension of the attack map and so,
        //I don't know if this is my mistake or a necessity.
        Card[] GetImpactedCards(string name, Vector2Int position, int[,] attackMap)
        {
            int lenghtX = attackMap.GetLength(1);
            int lenghtZ = attackMap.GetLength(0);
            List<Card> cards = new List<Card>(lenghtX * lenghtZ);

            int offset = 0;
            if (attackMap.Length == 1)
                offset = 0;
            else if (attackMap.Length == 9)
                offset = 1;
            else if (attackMap.Length == 25)
                offset = 2;
            else
                DebugSystem.DebugLog($"Invalid impact map {name}", DebugSystem.Type.Error);

            var startX = position.x - offset;
            var startY = position.y - offset;

            for (int x = startX, i = 0;
                 x < Field.SizeX && i < lenghtX;
                 x++, i++)
            {
                for (int z = startY, j = 0;
                     z < Field.SizeZ && j < lenghtZ;
                     z++, j++)
                {
                    if (x < 0 || z < 0) continue;

                    if (attackMap[j, i] == 0) continue;

                    var cell = _CommonState.BattleState.Filed.Cells[x, z];

                    //If card live she get impact
                    if (cell.Quantity > 0)
                        cards.Add(cell);
                }
            }

            return cards.ToArray();
        }

        /*
        Reflection is a good substitute for a huge dynamic switch,
        but if you are concerned about performance,
        you can reflexive and cache all kinds of attacks at the start.
        */
        int[,] GetImpactMap<T>(string name)
        {
            var filed = typeof(T).GetField(name);
            if (filed != null)
            {
                return (int[,]) filed.GetValue(null);
            }

            DebugSystem.DebugLog($"Invalid impact name requested {name}", DebugSystem.Type.Error);
            return null;
        }

        void SwapPositions(Card[,] cards, Vector2Int first, Vector2Int second)
        {
            (cards[first.x, first.y].Position, cards[second.x, second.y].Position) =
                (cards[second.x, second.y].Position, cards[first.x, first.y].Position);

            (cards[first.x, first.y], cards[second.x, second.y])
                = (cards[second.x, second.y], cards[first.x, first.y]);
        }
        
        void CheckDefeat(Card[,] items)
        {
            bool haveItems = false;
            foreach (var item in items)
            {
                if (item.Quantity > 0)
                {
                    haveItems = true;
                    break;
                }
            }

            if (!haveItems)
            {
                DebugSystem.DebugLog("Defeat", DebugSystem.Type.Battle);
                Defeat.SetActive(true);
                _CommonState.InBattle = false;
                _inputActive = false;
            }
        }

        //It is possible to optimize the loading of the effect
        float SpawnEffectOnCards(Card impactCard, Card[] cards)
        {
            var effect = Resources.Load<ParticleSystem>($"Prefabs/Effects/{impactCard.name}");
            if (effect)
            {
                foreach (var card in cards)
                {
                    var go = Instantiate(effect, Effects);
                    go.transform.position = Field.GetCellSpacePosition(card.Position);
                }

                return effect.main.duration;
            }

            return 0;
        }

        float SpawnEffectOnCard(Card card)
        {
            var effect = Resources.Load<ParticleSystem>($"Prefabs/Effects/{card.name}");
            if (effect)
            {
                var go = Instantiate(effect, Effects);
                go.transform.position = Field.GetCellSpacePosition(card.Position);

                return effect.main.duration;
            }

            return 0;
        }

        void DisableHighlight()
        {
            foreach (var highlighted in _highlightCards)
            {
                highlighted.Highlight.SetActive(false);
            }

            _highlightCards = new List<CardGameObject>();
        }

    }
}