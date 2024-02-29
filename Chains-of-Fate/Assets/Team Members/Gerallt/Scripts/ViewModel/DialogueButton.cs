using System;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class DialogueButton : MonoBehaviour
    {
        [SerializeField] private float minWidth;
        [SerializeField] private float minHeight;
        
        private RectTransform _rectTransform;
        private Vector2 localScale;
        private Rect defaultRect;
        
        private float _minWidth;
        private float _minHeight;
        
        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();

            localScale = _rectTransform.localScale;

            defaultRect = _rectTransform.rect;
        }
        
        private void FixedUpdate()
        {
            float w = minWidth;
            float h = minHeight;
            if (w == 0) w = 1;
            if (h == 0) h = 1;
            bool update = false;
            
            if (!Mathf.Approximately(_minWidth, w))
            {
                _minWidth = w;
                
                update = true;
            }
            
            if (!Mathf.Approximately(_minHeight, w))
            {
                _minHeight = h;
                
                update = true;
            }

            if (update)
            {
                //_rectTransform.localScale = new Vector2(localScale.x * w, localScale.y * h);
                Rect rect = _rectTransform.rect;
                
                if (_rectTransform.rect.height < h)
                {
                    rect.height = h;
                }
                
                if (_rectTransform.rect.width < w)
                {
                    rect.width = w;
                }
            }
        }
    }
}