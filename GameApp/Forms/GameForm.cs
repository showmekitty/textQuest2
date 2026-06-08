#nullable disable
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Game.Models;
using Game.Game;

namespace Game.Forms
{
    public class GameForm : Form
    {
        private RichTextBox log;
        private Button[] btns;
        private TaskCompletionSource<string> inputTcs;
        private Player player;
        private Random random = new Random();
        private bool gameActive = true;

        public GameForm()
        {
            this.Size = new Size(950, 800);
            this.Text = "Путь странника - Испытание души";
            this.BackColor = Color.Black;
            this.StartPosition = FormStartPosition.CenterScreen;

            log = new RichTextBox();
            log.Location = new Point(20, 20);
            log.Size = new Size(890, 550);
            log.BackColor = Color.Black;
            log.ForeColor = Color.White;
            log.Font = new Font("Consolas", 11);
            log.ReadOnly = true;
            this.Controls.Add(log);

            btns = new Button[5];
            for (int i = 0; i < 5; i++)
            {
                int choice = i + 1;
                btns[i] = new Button();
                btns[i].Text = choice.ToString();
                btns[i].Size = new Size(100, 50);
                btns[i].Location = new Point(20 + i * 120, 600);
                btns[i].BackColor = Color.Gray;
                btns[i].Font = new Font("Arial", 16);
                btns[i].Click += (s, e) => MakeChoice(choice);
                this.Controls.Add(btns[i]);
            }

            Button restart = new Button();
            restart.Text = "Новая игра";
            restart.Size = new Size(150, 40);
            restart.Location = new Point(20, 670);
            restart.BackColor = Color.DarkGreen;
            restart.ForeColor = Color.White;
            restart.Click += (s, e) => RestartGame();
            this.Controls.Add(restart);

            this.Load += async (s, e) => await NewGame();
        }

        private void MakeChoice(int choice)
        {
            if (inputTcs != null && !inputTcs.Task.IsCompleted && gameActive)
                inputTcs.SetResult(choice.ToString());
        }

        private async Task<string> GetInput(string prompt)
        {
            AddText(prompt, Color.Yellow);
            inputTcs = new TaskCompletionSource<string>();
            return await inputTcs.Task;
        }

        private void AddText(string text, Color color)
        {
            if (log.InvokeRequired) { log.Invoke(new Action(() => AddText(text, color))); return; }
            log.SelectionStart = log.TextLength;
            log.SelectionColor = color;
            log.AppendText(text + "\n");
            log.ScrollToCaret();
        }

        private async Task NewGame()
        {
            player = new Player();
            await StartingRoom();
        }

        private async Task StartingRoom()
        {
            AddText("\n╔════════════════════════════════════╗", Color.Magenta);
            AddText("║        КОМНАТА С ПОРТАЛОМ         ║", Color.Magenta);
            AddText("╚════════════════════════════════════╝\n", Color.Magenta);
            AddText("Ты просыпаешься в странной комнате. В центре мерцает портал.", Color.White);
            AddText("Рядом стоит загадочная фигура в капюшоне.\n", Color.Gray);
            AddText("1. Поговорить со странником", Color.White);
            AddText("2. Войти в портал", Color.White);

            string choice = await GetInput("");
            if (choice == "1") await TalkToNPC();
            await EnterPortal();
        }

        private async Task TalkToNPC()
        {
            AddText("\nСтранник: \"С пробуждением, странник.\"", Color.Green);
            AddText("\n1. \"Кто я и как я здесь оказался?\"", Color.White);
            string choice = await GetInput("");
            if (choice == "1")
            {
                AddText("\nСтранник: \"Ты - душа умершего в другом мире человека.\"", Color.Green);
                AddText("А это место - чистилище.\"", Color.Green);
                AddText("\n1. \"Что здесь нужно делать?\"", Color.White);
                choice = await GetInput("");
                if (choice == "1")
                {
                    AddText("\nСтранник: \"Тебе нужно пройти испытания.\"", Color.Green);
                    AddText("Ступай в портал и удачи тебе...\"", Color.Green);
                }
            }
        }

        private async Task EnterPortal()
        {
            AddText("\nТы делаешь шаг в портал... Мир вокруг искажается...\n", Color.Cyan);
            await Task.Delay(1000);
            await ChooseWeapon();
        }

        private async Task ChooseWeapon()
        {
            AddText("\n════════════════════════════════════════", Color.Yellow);
            AddText("           ВЫБЕРИ ОРУЖИЕ", Color.Yellow);
            AddText("════════════════════════════════════════\n", Color.Yellow);
            AddText("1. ⚔️ МЕЧ - мощные атаки", Color.White);
            AddText("2. 🏹 ЛУК - критические выстрелы", Color.White);
            AddText("3. 🔮 ПОСОХ - магия огня и льда\n", Color.White);

            bool chosen = false;
            while (!chosen)
            {
                string choice = await GetInput("Твой выбор (1-3): ");
                if (choice == "1") { player.Class = PlayerClass.Swordsman; AddText("\n⚔️ Ты взял меч! Ты МЕЧНИК!", Color.Cyan); chosen = true; }
                else if (choice == "2") { player.Class = PlayerClass.Archer; AddText("\n🏹 Ты взял лук! Ты ЛУЧНИК!", Color.Cyan); chosen = true; }
                else if (choice == "3") { player.Class = PlayerClass.Mage; AddText("\n🔮 Ты взял посох! Ты МАГ!", Color.Cyan); chosen = true; }
                else AddText("Неверный выбор!", Color.Yellow);
            }
            await Task.Delay(1000);
            await SleepingOrc();
        }

        private async Task SleepingOrc()
        {
            AddText("\n════════════════════════════════════════", Color.DarkRed);
            AddText("             ЗАЛ СПЯЩЕГО ОРКА", Color.DarkRed);
            AddText("════════════════════════════════════════\n", Color.DarkRed);
            AddText("В углу спит огромный орк.", Color.White);

            if (player.Class == PlayerClass.Mage)
            {
                AddText("1. Заморозить и пройти мимо", Color.White);
                AddText("2. Атаковать", Color.White);
                string choice = await GetInput("");
                if (choice == "1")
                {
                    AddText("\n❄️ Ты замораживаешь орка и проходишь мимо!", Color.Cyan);
                    await Task.Delay(1000);
                    await ChestRoom();
                    return;
                }
            }

            AddText("\n⚔️ Ты атакуешь орка! Он просыпается в ярости!\n", Color.Red);
            Enemy orc = new Enemy();
            orc.Name = "Спящий орк";
            orc.Health = 65;
            orc.MaxHealth = 65;
            orc.NormalDamageMin = 6;
            orc.NormalDamageMax = 9;
            orc.HeavyDamage = 18;
            orc.HeavyCooldown = 5;

            bool won = await CombatSystem.FightAsync(player, orc, AddText, GetInput);
            if (won)
            {
                AddText("\n🎉 Ты победил орка! 🎉", Color.Green);
                player.Inventory.Add("Зелье исцеления");
                AddText("📦 Выпало: Зелье исцеления!", Color.Green);
                player.ApplyStatsBonus(25);
                AddText($"✨ Характеристики +25%! HP: {player.MaxHealth}", Color.Green);
                await Task.Delay(1500);
                await ChestRoom();
            }
            else await GameOver(false);
        }

        private async Task ChestRoom()
        {
            AddText("\n════════════════════════════════════════", Color.Yellow);
            AddText("            КОМНАТА С СУНДУКАМИ", Color.Yellow);
            AddText("════════════════════════════════════════\n", Color.Yellow);
            AddText("Перед тобой три сундука.\n", Color.White);

            var chests = new System.Collections.Generic.List<string> { "Бомба", "Зелье исцеления", "Пусто" };
            chests = chests.OrderBy(x => random.Next()).ToList();

            AddText("1 [ ]    2 [ ]    3 [ ]\n", Color.White);
            string choice = await GetInput("Какой открыть? (1-3): ");

            int idx = 0;
            if (choice == "1") idx = 0;
            else if (choice == "2") idx = 1;
            else if (choice == "3") idx = 2;
            else { AddText("Неверный выбор!", Color.Yellow); await BossFight(); return; }

            string content = chests[idx];
            if (content == "Бомба") { AddText("💣 БАБАХ! -12 HP!", Color.Red); player.TakeDamage(12); }
            else if (content == "Зелье исцеления") { AddText("🧪 Зелье добавлено в инвентарь!", Color.Green); player.Inventory.Add("Зелье исцеления"); }
            else AddText("📦 Пусто...", Color.Gray);
            AddText($"❤️ HP: {player.Health}/{player.MaxHealth}\n", Color.Green);
            await Task.Delay(1000);
            await BossFight();
        }

        private async Task BossFight()
        {
            AddText("\n════════════════════════════════════════", Color.DarkRed);
            AddText("          ВЛАСТЕЛИН ПОДЗЕМЕЛИЙ", Color.DarkRed);
            AddText("════════════════════════════════════════\n", Color.DarkRed);
            AddText("Перед тобой ОГРОМНЫЙ ОРК-ВОЖДЬ!\n", Color.White);

            if (player.Inventory.Contains("Зелье исцеления"))
            {
                AddText("Использовать зелье перед боем? (да/нет)", Color.Green);
                string use = await GetInput("");
                if (use.ToLower() == "да")
                {
                    player.Heal(60);
                    player.Inventory.Remove("Зелье исцеления");
                    AddText($"🧪 Выпил зелье! HP: {player.Health}", Color.Green);
                }
            }

            Enemy boss = new Enemy();
            boss.Name = "Огромный Орк-Вождь";
            boss.Health = 100;
            boss.MaxHealth = 100;
            boss.NormalDamageMin = 8;
            boss.NormalDamageMax = 16;
            boss.HeavyDamage = 10;
            boss.HeavyCooldown = 4;
            boss.HasStun = true;
            boss.StunChance = 35;
            boss.StunDamage = 8;

            bool won = await CombatSystem.FightAsync(player, boss, AddText, GetInput);
            await GameOver(won);
        }

        private async Task GameOver(bool won)
        {
            gameActive = false;
            if (won)
            {
                AddText("\n╔════════════════════════════════════════╗", Color.Green);
                AddText("║          ТЫ ПОПАЛ В РАЙ! ПОБЕДА!       ║", Color.Green);
                AddText("╚════════════════════════════════════════╝\n", Color.Green);
            }
            else
            {
                AddText("\n╔════════════════════════════════════════╗", Color.DarkRed);
                AddText("║          ТЫ ПОПАЛ В АД... ПОРАЖЕНИЕ    ║", Color.DarkRed);
                AddText("╚════════════════════════════════════════╝\n", Color.DarkRed);
            }
            AddText("Нажми 'Новая игра' чтобы начать заново", Color.Yellow);
        }

        private void RestartGame()
        {
            log.Clear();
            gameActive = true;
            player = new Player();
            _ = NewGame();
        }
    }
}