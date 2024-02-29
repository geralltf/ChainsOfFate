using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChainsOfFate.Gerallt
{
    public interface IFleeAction
    {
        /// <summary>
        /// If the character is able to flee.
        /// (Character usually rolls a dice to determine if to flee)
        /// </summary>
        /// <returns></returns>
        bool Flee();
    }

    public interface IAttackAction
    {
        void Attack(CharacterBase target, WeaponBase weapon);
        void Attack(CharacterBase target, SpellBase spell);
    }

    public interface IResolveAction
    {
        void Encourage(CharacterBase targetCharacter);
        void Taunt(CharacterBase targetCharacter);
    }

    public interface IDefendAction
    {
        void Defend();
        void Defend(float blockPercentage, float totalDamage);
    }

    public interface IInventoryAction
    {
        void UseItem(ItemBase item);
    }

    public interface IDescriptive // Weapons, Spells, Items all implement this to be able to display in UI.
    {
        string GetId();
        string GetName();
        string GetDescription();
        Color GetTint();
    }

    public interface IStat
    {
        string StatName { get; }

        object GetMaximum();
        object GetAbsoluteMaximum();
        
        bool LevelUp(int newLevel, int maxLevels, bool debugOutput = false);
        bool LevelUp(float ratio, bool debugOutput = false);
        void Reset();
        void Replenish();
        CharacterBase GetOwner();
    }
}
