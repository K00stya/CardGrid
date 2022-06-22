using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CardGrid
{
    public partial class CardGridGame : MonoBehaviour
    {
        public CommonGameSettings CurrentGameSeetings;
        public AudioSource AudioSource;
        
        const string SaveName = "CardGrid";

        PlayerCommonState _CommonState;
        List<CardGameObject> _cardMonobehsPool;
        Camera _camera;
        bool _inputActive = true;

        void Awake()
        {
            _camera = Camera.main;
            DebugSystem.Settings = CurrentGameSeetings.Debug;

            int length = Inventory.SizeX * Inventory.SizeZ + Field.SizeX * Field.SizeZ;
            _cardMonobehsPool = new List<CardGameObject>(length);
        }

        void Start()
        {
            //Subscribe on UI events
            VolumeSlider.onValueChanged.AddListener(value => { _CommonState.Volume = value; });
            LanguageDropdown.onValueChanged.AddListener(language => { ChangeLanguage(language); });

            OpenMenu.onClick.AddListener(() => { GoToMenu(); });
            ToMenuOnDeafeat.onClick.AddListener(() => { GoToMenu(); });

            NewBattleButton.onClick.AddListener(() => { StartNewBattle(); });

            Continue.onClick.AddListener(() => { LoadBattle(); });

            //Try load save
            if (ES3.KeyExists(SaveName))
            {
                //LoadSave
                _CommonState = ES3.Load<PlayerCommonState>(SaveName);
                if (_CommonState == null)
                {
                    DebugSystem.DebugLog("Save can't be load. First active.", DebugSystem.Type.SaveSystem);
                    _CommonState = new PlayerCommonState();
                }
                else
                {
                    DebugSystem.DebugLog("Load exist save", DebugSystem.Type.SaveSystem); 
                }
                BestScore.text = _CommonState.BestScore.ToString();
                AudioSource.volume = _CommonState.Volume;

                Continue.gameObject.SetActive(_CommonState.InBattle);

                //LoadMenu
                Menu.SetActive(true);
                VolumeSlider.SetValueWithoutNotify(_CommonState.Volume);
                LanguageDropdown.SetValueWithoutNotify((int) _CommonState.Language);
            }
            //OpenMenu
            else
            {
                _CommonState = new PlayerCommonState();
                DebugSystem.DebugLog("Save no exist. First active.", DebugSystem.Type.SaveSystem);
                Menu.SetActive(true);
            }
        }
        
        void StartNewBattle()
        {
            DebugSystem.DebugLog("Start new battle", DebugSystem.Type.Battle);
            _CommonState.InBattle = true;
            Menu.SetActive(false);
            BattleUI.SetActive(true);
            Defeat.SetActive(false);
            _inputActive = true;

            //Spawn new filed
            Spawn(out _CommonState.BattleState.Filed.Cells, CardGrid.Field, CreateNewRandomCard);

            //Spawn new inventory
            Spawn(out _CommonState.BattleState.Inventory.Items, CardGrid.Inventory, CreateNewRandomItem);

            void Spawn(out Card[,] cards, CardGrid gridType, Func<Card> createCard)
            {
                GridGameObject gridGameObject;
                if (gridType == CardGrid.Field)
                {
                    gridGameObject = Field;
                }
                else
                {
                    gridGameObject = Inventory;
                }

                cards = new Card[gridGameObject.SizeX, gridGameObject.SizeZ];
                for (int z = 0; z < gridGameObject.SizeZ; z++)
                {
                    for (int x = 0; x < gridGameObject.SizeX; x++)
                    {
                        //Get card
                        var cell = createCard();
                        cell.Grid = gridType;
                        cell.Position = new Vector2Int(x, z);
                        cards[x, z] = cell;
                        var monobeh = SpawnCard(cell, gridGameObject);
                        cell.GameObject = monobeh;
                        monobeh.CardState = cell;
                    }
                }
            }
        }
        
        void LoadBattle()
        {
            //LoadBattle
            Menu.SetActive(false);
            ref BattleState playerBattleState = ref _CommonState.BattleState;

            foreach (var cell in playerBattleState.Filed.Cells)
            {
                var monobeh = SpawnCard(cell, Field);
                cell.GameObject = monobeh;
                monobeh.CardState = cell;
            }

            foreach (var item in playerBattleState.Inventory.Items)
            {
                var monobeh = SpawnCard(item, Inventory);
                item.GameObject = monobeh;
                monobeh.CardState = item;
            }
        }

        void UnLoadCards()
        {
            foreach (var monobeh in _cardMonobehsPool.ToArray())
            {
                Destroy(monobeh.gameObject);
            }

            _cardMonobehsPool.Clear();
        }

        //On android, pauses can become an out
        void OnApplicationPause(bool pauseStatus)
        {
            //Save();
        }

        void OnApplicationQuit()
        {
            Save();
        }

        void Save()
        {
            DebugSystem.DebugLog("Save on pause/out", DebugSystem.Type.SaveSystem);
            Debug.Log(_CommonState);
            ES3.Save(SaveName, _CommonState);
        }

        [MenuItem("CardGrid/DeleteSave")]
        public static void DeleteSave()
        {
            ES3.DeleteFile();
        }

        Card CreateNewRandomCard()
        {
            string newName;
            if (Random.Range(0, 1f) > ChanceItemOnFiled)
            {
                newName = NameEnemies[Random.Range(0, NameEnemies.Length)];
            }
            else
            {
                return CreateNewRandomItem();
            }

            int quantity = Random.Range(1, StartMaxCellQuantity + 1);
            return new Card {name = newName, Quantity = quantity, StartQuantity = quantity};
        }

        void ReCreateCard(Card card)
        {
            string newName;
            if (Random.Range(0, 1f) > ChanceItemOnFiled)
            {
                newName = NameEnemies[Random.Range(0, NameEnemies.Length)];
            }
            else
            {
                newName = NameItems[Random.Range(0, NameItems.Length)];
            }

            int quantity = Random.Range(1, StartMaxCellQuantity + 1);
            card.name = newName;
            card.Quantity = quantity;
            card.StartQuantity = quantity;

            card.GameObject.gameObject.SetActive(true);
            card.GameObject.Sprite.sprite = Resources.Load<Sprite>($"Sprites/Cards/{card.name}");
            card.GameObject.QuantityText.text = card.Quantity.ToString();
            card.GameObject.DebugPosition.text = card.Position.x.ToString() + card.Position.y.ToString();
        }

        Card CreateNewRandomItem()
        {
            return new Card
            {
                name = NameItems[Random.Range(0, NameItems.Length)],
                Quantity = Random.Range(1, StartMaxCellQuantity)
            };
        }

        //Load sprites can be cheaper if you want, i don't
        CardGameObject SpawnCard(Card card, GridGameObject grid)
        {
            CardGameObject cardGameObject = Instantiate(CardPrefab, grid.transform);
            _cardMonobehsPool.Add(cardGameObject);
            cardGameObject.transform.position = grid.GetCellSpacePosition(card.Position);
            cardGameObject.Sprite.sprite = Resources.Load<Sprite>($"Sprites/Cards/{card.name}");
            cardGameObject.QuantityText.text = card.Quantity.ToString();
            cardGameObject.DebugPosition.text = card.Position.x.ToString() + card.Position.y.ToString();
            if (card.Quantity <= 0)
            {
                cardGameObject.gameObject.SetActive(false);
            }

            return cardGameObject;
        }

        void GoToMenu()
        {
            if (!_inputActive) return;
            Save();
            Continue.gameObject.SetActive(_CommonState.InBattle);

            Menu.SetActive(true);
            BestScore.text = _CommonState.BestScore.ToString();
            UnLoadCards();
        }
        
        void ChangeLanguage(int language)
        {
            switch (language)
            {
                case 0:
                    _CommonState.Language = Language.English;
                    break;
                case 1:
                    _CommonState.Language = Language.Russian;
                    break;
            }
        }
    }
}