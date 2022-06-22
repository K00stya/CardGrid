using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CardGrid.Battle
{
    public abstract class Card : MonoBehaviour
    {
        public float MoveSpeed = 0.1f;
        
        public Vector3 WorldPosition => transform.position;
        [NonSerialized]
        public Vector2Int FieldPosition;
        [NonSerialized]
        public Action OnDeath;
        
        [SerializeField]
        private SpriteRenderer Sprite;
        [SerializeField]
        private TextMeshProUGUI QuantityText;
        [SerializeField]
        private GameObject HighlightObject;
        [SerializeField]
        private TextMeshProUGUI DebugPosition;

        private int _quantity;

        public int[,] MapDamage;


        public void SetCard(Sprite sprite, int quantity)
        {
            Sprite.sprite = sprite;
            _quantity = quantity;
            QuantityText.text = _quantity.ToString();
            DebugPosition.text = FieldPosition.x.ToString() + FieldPosition.y.ToString();
        }

        public void ChangeQuantity(int value)
        {
            _quantity += value;
            if (_quantity <= 0)
            {
                OnDeath?.Invoke();
            }
            else
            {
                QuantityText.text = _quantity.ToString();
            }
        }

        public void Highlight()
        {
            HighlightObject.SetActive(true);
        }

        public void Dehighlight()
        {
            HighlightObject.SetActive(false);
        }

        public IEnumerator ChangePositionTo(Vector3 worldPosition, Vector2Int filedPosition)
        {
            FieldPosition = filedPosition;
            yield return transform.DOMove(worldPosition, MoveSpeed);
        }
    }
}