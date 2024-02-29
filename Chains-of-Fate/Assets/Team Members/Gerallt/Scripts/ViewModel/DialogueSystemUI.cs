using System;
using System.Collections;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

namespace ChainsOfFate.Gerallt
{
    public class DialogueSystemUI : SingletonBase<DialogueSystemUI>
    {
        [SerializeField] private GameObject view;
        [SerializeField] private Button buttonAddPartyMember;
        [SerializeField] private Button buttonInterruptConversation;
        [SerializeField] private Button buttonAdvanceConversation;
        [SerializeField] private Button buttonTestPopulateOptionsView;
        [SerializeField] private Button buttonTestPlayAllLines;
        [SerializeField] private Button buttonTestResetDialogue;
        [SerializeField] private TextMeshProUGUI textCharacterName;
        [SerializeField] private TextMeshProUGUI textTalkerLine;
        [SerializeField] private GameObject viewButtonOptions;
        [SerializeField] private GameObject optionButtonPrefab;
        [SerializeField] private float nextLineWaitTime = 1.5f;
        [SerializeField] private DialogueUI dialogueUI;
        
        public float openAllowTime = 1.0f;
        public float closeAllowTime = 2.0f;
        public CharacterBase talkingToCharacter;
        public bool playAllLines = false;
        
        private YarnInteractable yarnInteractable;
        private Champion player;
        private DialogueOption[] dialogueOptionsTmp;
        private bool waitingForNextLine = false;
        private bool showingDialogueOptions = false;
        
        private Action lastDialogueLineOnFinished;
        private Action lastDismissalOnComplete;
        
        public void Show(CharacterBase talkingTo)
        {
            talkingToCharacter = talkingTo;
            player = GameManager.Instance.GetPlayer();

            if (dialogueUI == null)
            {
                dialogueUI = FindObjectOfType<DialogueUI>();
            }
            
            yarnInteractable = talkingTo.GetComponent<YarnInteractable>();
            
            SetVisibility(true);
            
            yarnInteractable.StartConversation();
        }

        public void Hide()
        {
            yarnInteractable.EndConversation();

            SetVisibility(false);
        }
        
        public void SetVisibility(bool visibility)
        {
            if (visibility)
            {
                bool showButton = false;
                
                if (talkingToCharacter is Champion)
                {
                    if (!player.partyMembers.Contains(talkingToCharacter as Champion))
                    {
                        showButton = true;
                    }
                }
                
                //buttonAddPartyMember.gameObject.SetActive(showButton);
            }
            
            view.SetActive(visibility);
        }

        public void AddPartyMember()
        {
            Debug.Log("Add Party Member");
            
            yarnInteractable.DisableConversation();
            Hide();

            if (talkingToCharacter is Champion)
            {
                buttonAddPartyMember.gameObject.SetActive(false);
                
                FriendlyNPC friendlyNpc = talkingToCharacter.GetComponent<FriendlyNPC>();

                //friendlyNpc.AddAsPartyMember(player);
            }
        }
        
        public void DialogueStarted()
        {
            Debug.Log("DialogueUI.DialogueStarted(): ");
        }

        public void DialogueComplete()
        {
            Debug.Log("DialogueUI.DialogueComplete(): ");

            if (yarnInteractable.resetDialogWhenComplete)
            {
                TestResetDialogue();
            }
        }

        public void UserRequestedViewAdvancement()
        {
            Debug.Log("DialogueUI.UserRequestedViewAdvancement(): ");
        }
        
        public void RunLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("DialogueUI.RunLine(): ");
            
            Debug.Log(dialogueLine.CharacterName + " says: " + dialogueLine.RawText);
            
            showingDialogueOptions = false;
            
            textCharacterName.text = dialogueLine.CharacterName;
            textTalkerLine.text = dialogueLine.TextWithoutCharacterName.Text;

            if (playAllLines)
            {
                lastDialogueLineOnFinished = onDialogueLineFinished; // Do the next line after a certain amount of time 
                
                NextLine();
            }
            else
            {
                lastDialogueLineOnFinished = onDialogueLineFinished; // Defer line finishing until player presses 'Next' button "Advance Line"
            }
        }

        public void InterruptLine(LocalizedLine dialogueLine, Action onDialogueLineFinished)
        {
            Debug.Log("DialogueUI.InterruptLine(): ");
        }

        public void DismissLine(Action onDismissalComplete)
        {
            Debug.Log("DialogueUI.DismissLine(): ");

            //textTalkerLine.text = string.Empty;

            //onDismissalComplete?.Invoke();

            lastDismissalOnComplete = onDismissalComplete;
        }

        public void RunOptions(DialogueOption[] dialogueOptions, Action<int> onOptionSelected)
        {
            Debug.Log("DialogueUI.RunOptions(): ");

            playAllLines = false;
            showingDialogueOptions = true;
            //lastDismissalOnComplete = null;
            
            PopulateView(dialogueOptions, onOptionSelected);

            dialogueOptionsTmp = dialogueOptions; // Store temporarily for testing
        }

        public void NodeStart(string nodeName)
        {
            if (nodeName.ToUpper().Contains("START")) // HACK: Need to distinguish between different node types in a better way
            {
                //NextLineNoWait();
                // Advance to initial list of options
                //yarnInteractable.OnViewRequestedInterrupt();
            }
        }
        
        public void ClearView()
        {
            // Clear dialogue button options view.
            for (int i = 0; i < viewButtonOptions.transform.childCount; i++)
            {
                Transform child = viewButtonOptions.transform.GetChild(i);
                
                Destroy(child.gameObject);
            }
        }
        
        public void PopulateView(DialogueOption[] dialogueOptions, [CanBeNull] Action<int> onOptionSelected)
        {
            ClearView();
            
            // Display dialogue options as buttons on screen. Make them not interactable if they aren't available. (I think that's how that works) 
            foreach (DialogueOption dialogueOption in dialogueOptions)
            {
                GameObject optionButtonInstance = Instantiate(optionButtonPrefab, viewButtonOptions.transform);

                Button optionButton = optionButtonInstance.GetComponent<Button>();
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = dialogueOption.Line.TextWithoutCharacterName.Text;

                optionButton.interactable = dialogueOption.IsAvailable;
                optionButton.onClick.AddListener(() =>
                {
                    // Tell Yarn to select the specified dialogue option.
                    onOptionSelected(dialogueOption.DialogueOptionID);
                    
                    // HACK: Yarn needs to provide a better way to identify options by name or ID.
                    if (dialogueOption.TextID == "line:Assets/Dialogue/COF-Maria.yarn-MariaStart-2"
                        || dialogueOption.TextID == "line:Assets/Dialogue/COF-Bann'jo.yarn-BannjoStart-11")
                    {
                        Hide();
                    }

                    if (dialogueOption.TextID == "line:Assets/Dialogue/COF-Maria.yarn-MariaStart-8"
                        || dialogueOption.TextID == "line:Assets/Dialogue/COF-Bann'jo.yarn-BannjoStart-17") // HACK: Yarn needs to provide a better way to identify options by name or ID. So if script line numbers change the code doesn't have to
                    {
                        AddPartyMember();
                    }

                    ClearView();
                });
            }
        }
        
        public void InterruptLine()
        {
           // dialogueUI.InterruptConversation();
        }
        public void NextLine()

        {
            //if (!waitingForNextLine)
            {
                StartCoroutine(NextLineCoroutine());
            }
        }
        
        public void NextLineNoWait()
        {
            // Only continue dialogue if yarn Can Continue i.e. not showing dialogue options

            if (!showingDialogueOptions)
            {
                // Tell YarnSpinner that the dialogue has finished.
                lastDialogueLineOnFinished?.Invoke();
                //lastDialogueLineOnFinished = null;  
            }
        }

        public void PopulateOptionsView_OnClick()
        {
            if (dialogueOptionsTmp != null)
            {
                PopulateView(dialogueOptionsTmp, null);
            }
        }
        
        private void PlayAllLines_OnClick()
        {
            playAllLines = true;

            NextLine();
        }
        
        private void NextLineWithNoWait()
        {
            playAllLines = false;
            
            NextLineNoWait();
        }

        private void TestResetDialogue()
        {
            ClearView();
            
            yarnInteractable.EndConversation();
            yarnInteractable.StartConversation();
        }
        
        private IEnumerator NextLineCoroutine()
        {
            waitingForNextLine = true;
            yield return new WaitForSeconds(nextLineWaitTime);
            if (!showingDialogueOptions)
            {
                lastDismissalOnComplete?.Invoke();
                NextLineNoWait();
            }
            waitingForNextLine = false;
        }
        
        public override void Awake()
        {
            base.Awake();

            SetVisibility(false);
            
            buttonAddPartyMember.onClick.AddListener(AddPartyMember);
            buttonInterruptConversation.onClick.AddListener(InterruptLine);
            buttonAdvanceConversation.onClick.AddListener(NextLineWithNoWait);
            buttonTestPopulateOptionsView.onClick.AddListener(PopulateOptionsView_OnClick);
            buttonTestPlayAllLines.onClick.AddListener(PlayAllLines_OnClick);
            buttonTestResetDialogue.onClick.AddListener(TestResetDialogue);
        }
    }
}