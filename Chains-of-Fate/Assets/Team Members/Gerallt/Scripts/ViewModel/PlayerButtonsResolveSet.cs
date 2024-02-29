using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsResolveSet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void ResolveEncourageButton_OnClick()
        {
            Debug.Log("Test resolve action - encourage");
            
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IResolveAction resolveAction = (IResolveAction)currentCharacter;

            if (resolveAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                resolveAction.Encourage(target); 
                
                combatGameManager.FinishedTurn(currentCharacter);
                combatGameManager.RaiseResolveEncourageEvent(currentCharacter, target);
            }
        }
        
        public void ResolveTauntButton_OnClick()
        {
            Debug.Log("Test resolve action - taunt");
            
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IResolveAction resolveAction = (IResolveAction)currentCharacter;

            if (resolveAction != null)
            {
                CharacterBase target = combatGameManager.attackTarget;
                
                resolveAction.Taunt(target); 
                
                combatGameManager.FinishedTurn(currentCharacter);
                combatGameManager.RaiseResolveTauntEvent(currentCharacter, target);
            }
        }
        
        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
        
        public void OnEnable()
        {

        }

        public void OnDisable()
        {
            
        }
        
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
    }

}