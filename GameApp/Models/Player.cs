#nullable disable
using System.Collections.Generic;

namespace Game.Models
{
    public enum PlayerClass { None, Swordsman, Archer, Mage }

    public class Player
    {
        public string Name = "Странник";
        public PlayerClass Class = PlayerClass.None;
        public int MaxHealth = 120;
        public int Health = 120;
        public List<string> Inventory = new List<string>();
        public int CurrentCooldown = 0;
        public bool IsFrozen = false;

        public bool IsAlive => Health > 0;
        public void TakeDamage(int damage) { Health -= damage; if (Health < 0) Health = 0; }
        public void Heal(int amount) { Health += amount; if (Health > MaxHealth) Health = MaxHealth; }
        public void ApplyStatsBonus(double percent)
        {
            MaxHealth = (int)(MaxHealth * (1 + percent / 100));
            Health = MaxHealth;
        }
    }
}