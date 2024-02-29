using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class RoundOverUI : MonoBehaviour
    {
        public CombatUI parentView;
        public GameObject view;
        
        public float animationTime = 1.5f;
        public AnimationCurve animationCurve;
        public Vector3 initialScale =  Vector3.one * 0.1f;
        public Vector3 endScale = Vector3.one;
        public float hideInSeconds = 2.0f;
        public string roundNumberFormat = "Round {0}";
        
        [SerializeField] TextMeshProUGUI roundNumberText;
        
        private void CombatGameManager_OnRoundAdvance(int newRound)
        {
            roundNumberText.text = string.Format(roundNumberFormat, newRound);

            AnimateView();
        }

        private IEnumerator HideView()
        {
            yield return new WaitForSeconds(hideInSeconds);
            view.SetActive(false);
        }
        
        private void AnimateView()
        {
            view.SetActive(true);

            view.transform.localScale = initialScale;
            var currentTween = view.transform.DOScale(endScale, animationTime);
            currentTween.onComplete = () =>
            {
                StartCoroutine(HideView());
            };
            currentTween.SetEase(animationCurve);
        }
        
        private void Awake()
        {
            parentView.combatGameManager.OnRoundAdvance += CombatGameManager_OnRoundAdvance;
        }

        private void OnDestroy()
        {
            parentView.combatGameManager.OnRoundAdvance -= CombatGameManager_OnRoundAdvance;
        }
        
    }

}
