using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public class PlayerButtonsDefensiveSet : MonoBehaviour
    {
        public PlayerButtons PlayerButtonsParentView;

        public void DefendButton_OnClick()
        {
            var combatGameManager = PlayerButtonsParentView.combatGameManager;
            CharacterBase currentCharacter = combatGameManager.GetCurrentCharacter();
            IDefendAction defendAction = (IDefendAction)currentCharacter;
            
            defendAction?.Defend();
            
            combatGameManager.FinishedTurn(currentCharacter);
            combatGameManager.RaiseDefendEvent(currentCharacter, null);
        }

        public void BackButton_OnClick()
        {
            this.gameObject.SetActive(false);
            PlayerButtonsParentView.view.SetActive(true);
        }
    }
}