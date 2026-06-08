#nullable disable
namespace Game.Models
{
    public class Enemy
    {
        public string Name = "";
        public int Health;
        public int MaxHealth;
        public int NormalDamageMin;
        public int NormalDamageMax;
        public int HeavyDamage;
        public int HeavyCooldown;
        public int CurrentHeavyCooldown = 0;
        public bool HasStun = false;
        public int StunChance = 0;
        public int StunDamage = 0;
        public bool IsFrozen = false;

        public bool IsAlive => Health > 0;
        public void TakeDamage(int damage) { Health -= damage; if (Health < 0) Health = 0; }
        public void ReduceCooldowns() { if (CurrentHeavyCooldown > 0) CurrentHeavyCooldown--; }
    }
}