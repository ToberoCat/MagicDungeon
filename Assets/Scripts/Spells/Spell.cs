using UnityEngine;

namespace Spells
{
    public abstract class Spell : ScriptableObject
    {
        public string spellName;
        public Vector2[] path;

        public abstract void CastSpell(Player player, float power);
    }
}