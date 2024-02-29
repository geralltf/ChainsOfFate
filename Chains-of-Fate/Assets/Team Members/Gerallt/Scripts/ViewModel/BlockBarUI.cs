using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using Unity.Mathematics;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class BlockBarUI : MonoBehaviour
    {
        public CombatUI parentView;
        public GameObject view;
        public Transform shieldStart;
        public Transform shieldEnd;
        public BarShield shieldSprite;
        public float animationSpeed = 0.1f;
        public float selectionTime = 5.0f;
        public float loseTime = 10.0f;
        public float guessCooldown = 1.5f;
        public bool animating = false;
        public CharacterBase defendingCharacter;
        public CharacterBase attackingCharacter;
        public bool isTestMode = false;
        public Vector3 orthographicScale = new Vector3(2.0f, 2.0f, 2.0f);
        public Vector3 projectionScale = new Vector3(0.02f, 0.02f, 0.02f);
        public float totalDamageRecieved;
        
        public delegate void WonDelegate(float blockPercentage, bool doCounterAttack);
        
        public event WonDelegate onWonEvent;
        public event Action onLostEvent;

        public AnimationCurve animationCurve;

        private TweenerCore<Vector3, Vector3, VectorOptions> currentTween;
        private CameraFollow cameraFollow;

        private void FixPosition()
        {
            if (!isTestMode)
            {
                Camera _camera = cameraFollow.GetComponent<Camera>();
                Vector3 offset = cameraFollow.GetCenterWorldPosition();
                //offset.z = 0.1f;
                // FindObjectOfType<WorldInfo>().sceneBounds.center
                gameObject.transform.position = offset;
                gameObject.transform.rotation = cameraFollow.transform.rotation;
                    
                if (_camera.orthographic)
                {
                    // float height = _camera.orthographicSize * 2.0f;
                    // float width =  height * Screen.height / Screen.width;
                    // float SIZE_ON_SCREEN = 5.0f;
                    
                    //gameObject.transform.localScale = Vector3.one * width / SIZE_ON_SCREEN;
                    
                    gameObject.transform.localScale = orthographicScale;
                }
                else
                {
                    gameObject.transform.localScale = projectionScale;
                }
            }
        }
        
        public void SetVisibility(bool visibility)
        {
            if (view != null)
            {
                FixPosition();
                
                if (visibility)
                {
                    StartAnim();

                    StartCoroutine(TimeUntilLost());
                }
                else
                {
                    StopAnim();
                }
                
                view.SetActive(visibility);
            }
        }

        private IEnumerator TimeUntilLost()
        {
            yield return new WaitForSeconds(loseTime);

            StopAnim();
            
            onLostEvent?.Invoke();
        }

        public void Guess_ButtonClicked()
        {
            BarHitSquare hit = shieldSprite.currentTarget;
            
            if (hit != null)
            {
                Debug.Log("guess hit! block " + hit.defenseBlockPerentage.ToString() + "%" + (hit.isCounterAttack?"Counter attack" : ""));
                
                onWonEvent?.Invoke(hit.defenseBlockPerentage, hit.isCounterAttack);
            }
        }
        
        public void ResetAnim()
        {
            shieldSprite.transform.position = shieldStart.transform.position;
        }

        public void StartAnim()
        {
            StartAnim_LeftToRight();
        }
        
        public void StartAnim_LeftToRight()
        {
            animating = true;
            ResetAnim();
            
            float time = selectionTime * animationSpeed * 0.5f;
                
            currentTween = shieldSprite.transform.DOLocalMove(shieldEnd.transform.localPosition, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_RightToLeft();
                    
                //Debug.Log("anim left to right done");
            };
            currentTween.SetEase(animationCurve);
        }
        
        public void StartAnim_RightToLeft()
        {
            float time = selectionTime * animationSpeed * 0.5f;
                
            currentTween = shieldSprite.transform.DOLocalMove(shieldStart.transform.localPosition, time, false);
            currentTween.onComplete = () =>
            {
                StartAnim_LeftToRight();
                    
                //Debug.Log("anim right to left done");
            };
            currentTween.SetEase(animationCurve);
        }
        
        public void StopAnim()
        {
            if (currentTween != null)
            {
                currentTween.Kill();
            }
            
            animating = false;
            ResetAnim();
        }

        private void Start()
        {
            cameraFollow = FindObjectOfType<CameraFollow>();

            isTestMode = GameManager.Instance.combatUI.isTestMode;
        }

        private void OnEnable()
        {
            FixPosition();
            
            StartAnim();
        }

        private void OnDisable()
        {
            StopAnim();
        }
    }
}
