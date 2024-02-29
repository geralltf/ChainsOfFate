using System.Collections;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class InGameUI : SingletonBase<InGameUI>
    {
        public float inventoryTutorialHideInSeconds = 15.0f;
        
        [SerializeField] private GameObject view;

        public void SetVisibility(bool visibility)
        {
            view.SetActive(visibility);
        }

        public override void Awake()
        {
            base.Awake();
            
            StartCoroutine(ShowTutorial());
        }

        private IEnumerator ShowTutorial()
        {
            SetVisibility(true);
            yield return new WaitForSeconds(inventoryTutorialHideInSeconds);
            SetVisibility(false);
        }
    }
}