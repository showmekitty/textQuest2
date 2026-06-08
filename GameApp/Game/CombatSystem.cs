#nullable disable
using System;
using System.Drawing;
using System.Threading.Tasks;
using Game.Models;

namespace Game.Game
{
    public class CombatSystem
    {
        private static Random random = new Random();
        private static int burnRemaining = 0;

        public static async Task<bool> FightAsync(Player player, Enemy enemy, Action<string, Color> log, Func<string, Task<string>> getInput)
        {
            player.CurrentCooldown = 0;
            burnRemaining = 0;
            bool playerWon = false;

            log($"\n⚔️ БОЙ: {enemy.Name} ⚔️\n", Color.Red);

            while (player.IsAlive && enemy.IsAlive)
            {
                log($"\n❤️ {player.Health}/{player.MaxHealth} HP | ", Color.Green);
                log($"💀 {enemy.Name}: {enemy.Health}/{enemy.MaxHealth} HP\n", Color.Red);
                if (player.CurrentCooldown > 0)
                    log($"⏳ Усиленная атака через {player.CurrentCooldown} ход\n", Color.Cyan);

                if (player.IsFrozen)
                {
                    log("❄️ Ты заморожен! Пропускаешь ход...\n", Color.Cyan);
                    player.IsFrozen = false;
                    await EnemyTurn(player, enemy, log);
                    continue;
                }

                log("\nТВОЙ ХОД:\n", Color.White);
                ShowPlayerActions(player, log);

                string choice = await getInput("Выбери действие (1-2): ");

                if (choice == "1")
                {
                    PlayerNormalAttack(player, enemy, log);
                }
                else if (choice == "2" && player.CurrentCooldown == 0)
                {
                    PlayerHeavyAttack(player, enemy, log);
                    player.CurrentCooldown = 2;
                }
                else if (choice == "2" && player.CurrentCooldown > 0)
                {
                    log("❌ Способность на перезарядке!\n", Color.Yellow);
                    continue;
                }
                else
                {
                    log("❌ Неверный выбор!\n", Color.Yellow);
                    continue;
                }

                if (player.CurrentCooldown > 0) player.CurrentCooldown--;

                if (!enemy.IsAlive) { playerWon = true; break; }

                await EnemyTurn(player, enemy, log);
                if (!player.IsAlive) { playerWon = false; break; }
            }
            return playerWon;
        }

        private static void ShowPlayerActions(Player player, Action<string, Color> log)
        {
            if (player.Class == PlayerClass.Swordsman)
            {
                log("1. Обычная атака (12 урона)\n", Color.White);
                if (player.CurrentCooldown == 0) log("2. Усиленная атака (28 урона)\n", Color.White);
                else log($"2. Усиленная атака [КД: {player.CurrentCooldown}]\n", Color.Gray);
            }
            else if (player.Class == PlayerClass.Archer)
            {
                log("1. Обычный выстрел (9-13 урона, 10% крит)\n", Color.White);
                if (player.CurrentCooldown == 0) log("2. Выстрел в уязвимое место (16-23 урона)\n", Color.White);
                else log($"2. Выстрел в уязвимое место [КД: {player.CurrentCooldown}]\n", Color.Gray);
            }
            else if (player.Class == PlayerClass.Mage)
            {
                log("1. Огненный шар (12 урона + поджог)\n", Color.White);
                if (player.CurrentCooldown == 0) log("2. Заморозка (враг пропускает ход)\n", Color.White);
                else log($"2. Заморозка [КД: {player.CurrentCooldown}]\n", Color.Gray);
            }
        }

        private static void PlayerNormalAttack(Player player, Enemy enemy, Action<string, Color> log)
        {
            Random rand = new Random();
            if (player.Class == PlayerClass.Swordsman)
            {
                enemy.TakeDamage(12);
                log("🗡️ Обычная атака: 12 урона!\n", Color.Yellow);
            }
            else if (player.Class == PlayerClass.Archer)
            {
                int damage = rand.Next(9, 14);
                if (rand.Next(100) < 10)
                {
                    damage = rand.Next(16, 24);
                    log($"🎯 КРИТИЧЕСКИЙ ВЫСТРЕЛ! {damage} урона!\n", Color.DarkOrange);
                }
                else log($"🏹 Обычный выстрел: {damage} урона!\n", Color.Yellow);
                enemy.TakeDamage(damage);
            }
            else if (player.Class == PlayerClass.Mage)
            {
                enemy.TakeDamage(12);
                burnRemaining = 2;
                log("🔥 Огненный шар: 12 урона + поджог на 2 хода!\n", Color.Red);
            }
        }

        private static void PlayerHeavyAttack(Player player, Enemy enemy, Action<string, Color> log)
        {
            Random rand = new Random();
            if (player.Class == PlayerClass.Swordsman)
            {
                enemy.TakeDamage(28);
                log("⚡ УСИЛЕННАЯ АТАКА: 28 урона!\n", Color.DarkRed);
            }
            else if (player.Class == PlayerClass.Archer)
            {
                int damage = rand.Next(16, 24);
                enemy.TakeDamage(damage);
                log($"🎯 ВЫСТРЕЛ В УЯЗВИМОЕ МЕСТО: {damage} урона!\n", Color.DarkOrange);
            }
            else if (player.Class == PlayerClass.Mage)
            {
                enemy.IsFrozen = true;
                log("❄️ ЗАМОРОЗКА! Враг пропустит следующий ход!\n", Color.Cyan);
            }
        }

        private static async Task EnemyTurn(Player player, Enemy enemy, Action<string, Color> log)
        {
            Random rand = new Random();
            if (burnRemaining > 0)
            {
                enemy.TakeDamage(1);
                log($"🔥 Горение: 1 урон! (осталось {burnRemaining - 1})\n", Color.Red);
                burnRemaining--;
            }
            if (!enemy.IsAlive) return;
            if (enemy.IsFrozen)
            {
                log("❄️ Враг заморожен! Пропускает ход!\n", Color.Cyan);
                enemy.IsFrozen = false;
                return;
            }
            enemy.ReduceCooldowns();
            log($"\nХОД {enemy.Name.ToUpper()}:\n", Color.Red);
            if (enemy.HasStun && enemy.CurrentHeavyCooldown == 0 && rand.Next(100) < enemy.StunChance)
            {
                player.TakeDamage(enemy.StunDamage);
                log($"💫 {enemy.Name} оглушил тебя! -{enemy.StunDamage} HP!\n", Color.DarkRed);
                enemy.CurrentHeavyCooldown = 5;
                return;
            }
            if (enemy.CurrentHeavyCooldown == 0 && rand.Next(2) == 0)
            {
                player.TakeDamage(enemy.HeavyDamage);
                log($"💥 {enemy.Name} использует тяжёлую атаку! -{enemy.HeavyDamage} HP!\n", Color.Red);
                enemy.CurrentHeavyCooldown = enemy.HeavyCooldown;
            }
            else
            {
                int damage = rand.Next(enemy.NormalDamageMin, enemy.NormalDamageMax + 1);
                player.TakeDamage(damage);
                log($"👊 {enemy.Name} атакует! -{damage} HP!\n", Color.White);
            }
        }
    }
}