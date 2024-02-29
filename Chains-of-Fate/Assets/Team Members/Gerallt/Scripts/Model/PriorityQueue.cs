using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ChainsOfFate.Gerallt;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

namespace ChainsOfFate.Gerallt
{
    /// <summary>
    /// A circular queue that rotates a priority of characters based on their attributes.
    /// Dequeue never removes an item but usually reassigns it at the end of the queue.
    /// </summary>
    public class PriorityQueue : MonoBehaviour
    {
        [SerializeField] private List<CharacterBase> queue = new List<CharacterBase>();

        /// <summary>
        /// Characters that have had their turns for the current round.
        /// </summary>
        public List<CharacterBase> hadTurns = new List<CharacterBase>();

        [SerializeField] private GameObject nodePrefab;
        [SerializeField] private Transform contentParent;
        [SerializeField] private float animationSpeed = 1.0f;
        [SerializeField] private float animateMinDistanceToEnd = 0.1f;

        public AnimationType animationType = AnimationType.Linear;

        private class QueueOldItem
        {
            public string id;
            public Vector3 position;
        }

        [SerializeField] private float itemSpacing = 60.0f;
        [SerializeField] private float itemOffset = 0.0f;
        [SerializeField] private ItemLayout itemLayout = ItemLayout.ManualHorizontal;

        public enum ItemLayout
        {
            UseScrollView,
            ManualHorizontal,
            ManualVerticalUp,
            ManualVerticalDown
        }

        public int Count => queue.Count;

        public CharacterBase Top()
        {
            return queue.FirstOrDefault();
        }

        public CharacterBase End()
        {
            return queue.LastOrDefault();
        }

        public CharacterBase Next()
        {
            if (Count < 2) return null;
            return queue[2];
        }

        public CharacterBase Dequeue()
        {
            var top = Top();
            if (top != null)
            {
                RemoveTop();
                Add(top); // Re-add character to end of queue.
            }

            return top;
        }

        public void Enqueue(CharacterBase characterBase)
        {
            queue.Add(characterBase);

            characterBase.OnStatChanged += CharacterBase_OnStatChanged;

            // Update UI.
            //UpdateView();
        }

        public void RemoveAt(int index)
        {
            queue.RemoveAt(index);

            // Update UI.
            //UpdateView();
        }

        public void RemoveTop()
        {
            RemoveAt(0);
        }

        public void RemoveEnd()
        {
            //RemoveAt(Count - 1);
            int index = Count - 1;

            queue.RemoveAt(index);

            // Update UI.
            //UpdateView();
        }

        public void Add(CharacterBase newCharacter)
        {
            queue.Add(newCharacter);

            // Update UI.
            //UpdateView();
        }

        public void InsertBeforeTop(CharacterBase newTop)
        {
            int positionId = queue.IndexOf(newTop);

            queue.Insert(0, newTop);

            // Update UI.
            //UpdateView();
        }

        public void InsertAfterTop(CharacterBase second)
        {
            int positionId = queue.IndexOf(second);

            queue.Insert(1, second);

            // Update UI.
            //UpdateView();
        }

        public void InsertBeforeEnd(CharacterBase secondLast)
        {
            queue.Insert(Count - 2, secondLast);

            // Update UI.
            //UpdateView();
        }

        /// <summary>
        /// Skip all other characters until the predicate finds a character that meets the criteria to have its turn. 
        /// </summary>
        /// <param name="predicate"></param>
        public void SkipTo(Predicate<CharacterBase> predicate)
        {
            // Next challenger or champion takes a turn depending on the predicate by skipping to the next one later in the list 

            for (int i = 0; i < Count; i++)
            {
                CharacterBase character = queue[i];

                if (predicate(character) == false)
                {
                    // Reposition character to end of queue
                    queue.RemoveAt(i);
                    queue.Add(character); // Re-add character to end of queue.
                }
                else
                {
                    break;
                }
            }
        }

        public List<CharacterBase> GetChampions()
        {
            return queue.Where(chr => chr is Champion).ToList();
        }

        public List<CharacterBase> GetEnemies()
        {
            return queue.Where(chr => chr is EnemyNPC).ToList();
        }

        public void Sort(CharacterBase changedCharacter = null)
        {
            // TODO: if changedCharacter is specified it should be easier to sort

            //Sort all the characters by their speed priority.
            queue = queue.OrderByDescending(chr => chr.Speed).ToList();

            //UpdateView();
        }

        public void SanityChecks(CharacterBase oldTop, CharacterBase oldEnd)
        {
            bool updateView = false;

            // SANITY CHECK #1
            if (Top() == oldTop)
            {
                // Can't have another turn when character just had a turn.
                RemoveTop();

                if (Count == 1)
                {
                    Add(oldTop);
                }
                else
                {
                    InsertAfterTop(oldTop);
                }

                updateView = true;
            }

            if (Count > 2)
            {
                // SANITY CHECK #2
                if (End() == oldEnd)
                {
                    // Can't not never let a character have a turn.
                    RemoveEnd();

                    if (Count - 2 < 0)
                    {
                        InsertBeforeTop(oldEnd);
                    }
                    else
                    {
                        InsertBeforeEnd(oldEnd);
                    }

                    updateView = true;
                }
            }

            if (updateView)
            {
                //UpdateView();
            }
        }

        public List<CharacterBase> ToList()
        {
            return queue;
        }

        private void CharacterBase_OnStatChanged(CharacterBase character, string propertyName, object newValue)
        {
            // if (propertyName == "Speed")
            // {
            //     // Sort queue again everytime a character has its speed changed.
            //     Sort(character);
            // }
        }

        public void Clear()
        {
            // Unsubscribe from character stat updates.
            foreach (CharacterBase character in queue)
            {
                character.OnStatChanged -= CharacterBase_OnStatChanged;
            }

            queue.Clear();

            // Update UI.
            //UpdateView();
        }

        public void Union(List<CharacterBase> other)
        {
            foreach (CharacterBase character in other)
            {
                Enqueue(character);
            }
        }

        public void UpdateView()
        {
            // Visualise current state of queue
            if (!isActiveAndEnabled) return;
            
            List<QueueOldItem> oldPositions = new List<QueueOldItem>();

            // Destroy all node UI instances in content parent view
            for (int idx = 0; idx < contentParent.transform.childCount; idx++)
            {
                Transform child = contentParent.transform.GetChild(idx);
                GameObject nodeInstance = child.gameObject;

                PriorityQueueNode node = nodeInstance.GetComponent<PriorityQueueNode>();

                if (queue.Any(c => c.ID == node.id))
                {
                    RectTransform rectTransform = nodeInstance.GetComponent<RectTransform>();
                    Vector3 pos;

                    if (rectTransform != null)
                    {
                        if (animationType == AnimationType.Circular)
                        {
                            pos = rectTransform.position;
                        }
                        else
                        {
                            pos = rectTransform.localPosition;
                        }

                        oldPositions.Add(new QueueOldItem()
                        {
                            id = node.id,
                            position = pos
                        });
                    }
                }

                Destroy(nodeInstance);
            }

            for (int i = 0; i < queue.Count; i++)
            {
                CharacterBase character = queue[i];

                GameObject nodeInstance = Instantiate(nodePrefab, contentParent);
                Image image = nodeInstance.GetComponentInChildren<Image>();

                SpriteRenderer characterSpriteRenderer = character.GetComponentInChildren<SpriteRenderer>();
                Color representation = character.representation;
                //representation.a = 1;
                
                if (characterSpriteRenderer != null)
                {
                    image.sprite = characterSpriteRenderer.sprite;
                }
                image.color = representation;
                
                nodeInstance.GetComponentInChildren<TextMeshProUGUI>().text = character.CharacterName;
                PriorityQueueNode node = nodeInstance.GetComponent<PriorityQueueNode>();
                node.id = character.ID;

                RectTransform rectTransform = nodeInstance.GetComponent<RectTransform>();

                if (rectTransform != null)
                {
                    if (itemLayout != ItemLayout.UseScrollView)
                    {
                        Vector3 pos;
                        if (animationType == AnimationType.Circular)
                        {
                            pos = rectTransform.position;
                        }
                        else
                        {
                            pos = rectTransform.localPosition;
                        }

                        if (itemLayout == ItemLayout.ManualHorizontal)
                        {
                            pos.x = (i * itemSpacing) +
                                    itemOffset; // Use this if you are not using the scroll rect to position elements.
                            pos.y = 0.0f;
                        }

                        if (itemLayout == ItemLayout.ManualVerticalUp)
                        {
                            pos.y = (i * itemSpacing) +
                                    itemOffset; // Use this if you are not using the scroll rect to position elements.
                            pos.x = 0.0f;
                        }

                        if (itemLayout == ItemLayout.ManualVerticalDown)
                        {
                            pos.y = -(i * itemSpacing) +
                                    itemOffset; // Use this if you are not using the scroll rect to position elements.
                            pos.x = 0.0f;
                        }

                        if (animationType == AnimationType.Circular)
                        {
                            rectTransform.position = pos;
                        }
                        else
                        {
                            rectTransform.localPosition = pos;
                        }
                    }

                    Vector3 newPosition;
                    if (animationType == AnimationType.Circular)
                    {
                        newPosition = rectTransform.position;
                    }
                    else
                    {
                        newPosition = rectTransform.localPosition;
                    }

                    QueueOldItem oldPosition = oldPositions.FirstOrDefault(v => v.id == character.ID);

                    if (oldPosition != null)
                    {
                        // Reset transform to old position

                        if (animationType == AnimationType.Circular)
                        {
                            rectTransform.position = oldPosition.position;
                            StartCoroutine(AnimationHelpers.AnimateObject2D(nodeInstance, newPosition, animationType,
                                animationSpeed, animateMinDistanceToEnd));
                        }
                        else
                        {
                            rectTransform.localPosition = oldPosition.position;
                            StartCoroutine(AnimationHelpers.AnimateObject2D(nodeInstance, newPosition, animationType,
                                animationSpeed, animateMinDistanceToEnd));
                        }
                    }
                }
            }
        }

        private void Start()
        {
            UpdateView();
        }

        private void Update()
        {
        }
    }
}