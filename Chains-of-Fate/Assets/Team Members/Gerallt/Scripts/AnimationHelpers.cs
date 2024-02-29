using System.Collections;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public enum AnimationType
    {
        Linear,
        Circular
    }
    
    /// <summary>
    /// Useful Animation helpers that help with animating things that DoTween can't.
    /// E.g. DOTween can't handle destroyed game objects which is why this is even necessary.  
    /// </summary>
    public class AnimationHelpers
    {
        /// <summary>
        /// A coroutine useful for animating an input game object (gameObject) to a new position (endPosition).
        /// </summary>
        public static IEnumerator AnimateObject2D(GameObject gameObject, Vector3 endPosition, AnimationType animationType, float animationSpeed = 1.0f, float animateMinDistanceToEnd = 0.1f)
        {
            float dist;
            bool animating = true;

            while (animating)
            {
                if (gameObject == null)
                {
                    animating = false;
                }
                else
                {
                    RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                    
                    dist = Vector3.Distance(rectTransform.localPosition, endPosition);
                    
                    if (dist < animateMinDistanceToEnd)
                    {
                        animating = false;
                    }
                    else
                    {
                        switch (animationType)
                        {
                            case AnimationType.Linear:
                                rectTransform.localPosition = Vector3.Lerp(rectTransform.localPosition, endPosition, animationSpeed * Time.deltaTime);
                                break;
                            case AnimationType.Circular:
                                rectTransform.localPosition = Vector3.Slerp(rectTransform.localPosition, endPosition, animationSpeed * Time.deltaTime);
                                break;
                        }
                    }
                }
                
                yield return new WaitForEndOfFrame();
            }
        }
    }
}