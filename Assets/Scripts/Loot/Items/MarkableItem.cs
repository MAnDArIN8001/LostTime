using System;
using DG.Tweening;
using Loot.Systems;
using UnityEngine;

namespace Loot.Items
{
    public class MarkableItem : MonoBehaviour, IMarkable
    {
        [Header("Animation Configuration")] 
        [SerializeField] private float _scaleTime;

        [Space, SerializeField] private Ease _scaleEase;
        
        [Space, SerializeField] private GameObject _suggestionView;

        private Tween _scaleTween;

        private void OnDestroy()
        {
            _scaleTween?.Kill();
        }

        public void ShowMark()
        {
            _scaleTween?.Kill();
            _scaleTween = _suggestionView.transform.DOScale(Vector3.one, _scaleTime).SetEase(_scaleEase);
        }

        public void HideMark()
        {
            _scaleTween?.Kill();
            _scaleTween = _suggestionView.transform.DOScale(Vector3.zero, _scaleTime).SetEase(_scaleEase);
        }
    }
}