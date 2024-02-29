using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace ChainsOfFate.Gerallt
{
    public class FriendlyNPC : MonoBehaviour
    {
        [SerializeField] private string defaultGreeting = "Hi!";
        [SerializeField] private List<string> greetingList;
        [SerializeField] private float movementSpeed = 3.7f;
        [SerializeField] private float minDistanceToPlayer = 0.1f;
        
        /// <summary>
        /// The separation force keeping this NPC away from other party members.
        /// </summary>
        [SerializeField] private float separationForce = 0.4f;
        
        /// <summary>
        /// The minimum distance of separation kept between other party members.
        /// </summary>
        [SerializeField] private float minSeparationDistance = 1.3f;
        
        [SerializeField] private SpriteRenderer friendlySpriteRenderer;

        [SerializeField] private float greetingDisappearTime = 5.0f;
        [SerializeField] private GameObject viewGreeting;
        [SerializeField] private TextMeshPro textMeshProGreeting;

        [SerializeField] private GameObject viewGreetInteractButtons;

        public bool isPartyMember = false;
        public Transform playerTarget;
        public Champion championTarget;
        public NpcState state = NpcState.Idle;

        private Champion champion;
        private PlayerSensor playerSensor;
        private Rigidbody2D rb;
        private bool isGreeting = false;
        private bool inDialogue = false;
        private bool canExitDialogue = false;
        private bool canEnterDialogue = true;
        private YarnInteractable yarnInteractable;
        private bool flipState;

        private Champion player;



        //private float spawnZ; //HACK: 




        public enum NpcState
        {
            Idle,
            GreetPlayer,
            FollowingPlayer,
            TalkingToPlayer
        }

        public void MoveTowardsPlayer()
        {
            Vector2 pos = transform.position;

            Vector2 playerPos = playerTarget.position;
            float distanceToPlayer = Vector2.Distance(pos, playerPos);
            Vector2 directionToPlayer = -(pos - playerPos).normalized;
            Vector2 posDelta = directionToPlayer * movementSpeed * Time.fixedDeltaTime;

            if (distanceToPlayer > minDistanceToPlayer)
            {
                pos.x += posDelta.x;
                pos.y += posDelta.y;
                
                var neighbours = championTarget.partyMembers;

                Vector2 separationFactor = CharacterBase.FlockSeparation(neighbours, minSeparationDistance, pos);

                rb.simulated = true;
                rb.MovePosition(pos + (separationFactor * separationForce));

                UpdateSprite(posDelta);
            }
            else
            {
                rb.simulated = false;
            }
        }

        public void GreetPlayer()
        {
            if (!isGreeting)
            {
                StartCoroutine(GreetingCoroutine());
            }
        }

        public void SetGreetingVisibility(bool visibility)
        {
            if (visibility)
            {
                // Pick a greeting!
                string greeting = defaultGreeting;
                
                if (greetingList.Count > 0)
                {
                    greeting = greetingList[Random.Range(0, greetingList.Count - 1)];
                }
                textMeshProGreeting.text = greeting;
            }
            viewGreeting.SetActive(visibility);
        }

        public void SetGreetInteractionVisibility(bool visibility)
        {
            viewGreetInteractButtons.SetActive(visibility);
        }
        
        private IEnumerator GreetingCoroutine()
        {
            isGreeting = true;
            SetGreetingVisibility(true);
            SetGreetInteractionVisibility(true);
            yield return new WaitForSeconds(greetingDisappearTime);
            SetGreetingVisibility(false);
            SetGreetInteractionVisibility(false);
            isGreeting = false;
            // if (state != NpcState.TalkingToPlayer || state != NpcState.FollowingPlayer)
            // {
            //     state = NpcState.Idle;
            // }
        }
        
        private void UpdateSprite(Vector2 pos)
        {
            if (pos.x < 0)
            {
                flipState = true;
            }
            else
            {
                flipState = false;
            }

            if (flipState != friendlySpriteRenderer.flipX)
            {
                friendlySpriteRenderer.flipX = flipState;
            }

            friendlySpriteRenderer.transform.rotation = Quaternion.identity;
        }
        
        private void Awake()
        {
            playerSensor = GetComponentInChildren<PlayerSensor>();
            rb = GetComponent<Rigidbody2D>();
            champion = GetComponent<Champion>();
            yarnInteractable = GetComponent<YarnInteractable>();
            //spawnZ = transform.position.z; // HACK: 

            SetGreetingVisibility(false);
            SetGreetInteractionVisibility(false);

            player = FindObjectOfType<PlayerController>().GetComponent<Champion>();
            

        }

        private void Update()
        {
            if (playerSensor.DetectedPlayer == null)
            {
                if (state != NpcState.FollowingPlayer)
                {
                    state = NpcState.Idle;
                }
                
                if (inDialogue)
                {
                    StartCoroutine(EndDialogueCoroutine());
                }
                player = FindObjectOfType<PlayerController>().GetComponent<Champion>();
            }

            if (!isPartyMember)
            {
                if (playerSensor.DetectedPlayer != null)
                {
                    switch (state)
                    {
                        case NpcState.Idle:
                            state = NpcState.GreetPlayer;
                            break;
                        case NpcState.GreetPlayer:
                            GreetPlayer();
                            break;
                        case NpcState.FollowingPlayer:
                            break;
                        case NpcState.TalkingToPlayer:
                            break;
                    }
                }
            }

            if ((state is NpcState.GreetPlayer or NpcState.TalkingToPlayer) && yarnInteractable.interactable)
            {
                if (InputSystem.GetDevice<Keyboard>().eKey.isPressed) // TODO: Use proper input system action event
                {
                    if (!inDialogue && canEnterDialogue)
                    {
                        //StartCoroutine(StartDialogueCoroutine());
                        yarnInteractable.StartConversation();
                    }
                    else if (inDialogue && canExitDialogue)
                    {
                       StartCoroutine(EndDialogueCoroutine());
                    }
                }
            }
            
            if(state == NpcState.FollowingPlayer)
            {
                if (playerTarget != null)
                {
                    MoveTowardsPlayer();
                }
            }
        }

        private IEnumerator StartDialogueCoroutine()
        {
            inDialogue = true;
            canExitDialogue = false;
            state = NpcState.TalkingToPlayer;
            
            DialogueSystemUI.Instance.Show(player);

            yield return new WaitForSeconds(DialogueSystemUI.Instance.closeAllowTime);

            canExitDialogue = true;
        }
        
        private IEnumerator EndDialogueCoroutine()
        {
            canEnterDialogue = false;
            DialogueSystemUI.Instance.Hide();
            yield return new WaitForSeconds(DialogueSystemUI.Instance.openAllowTime);
            inDialogue = false;
            canEnterDialogue = true;

            if (!isPartyMember)
            {
                state = NpcState.Idle;
            }
            else
            {
                state = NpcState.FollowingPlayer;
            }
        }

        public void AddAsPartyMember()
        {
            
            
            if (!isPartyMember)
            {
                isPartyMember = true;
                playerTarget = playerSensor.DetectedPlayer.transform;
                championTarget = player;
                
                if (!player.partyMembers.Contains(champion))
                {
                    player.partyMembers.Add(champion);
                }

                DontDestroyOnLoad(gameObject);
            }
        }
    }
}