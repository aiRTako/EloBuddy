namespace aiRTako_SmiteHelper
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Program
    {
        private static Menu Menu;
        private static Menu ObjectMenu;
        private static Menu SettingMenu;
        private static Menu DrawMenu;

        private static SpellSlot Smite = SpellSlot.Unknown;

        private const float SmiteRange = 600f;

        private static readonly Dictionary<string, string> SmiteObjects =
            new Dictionary<string, string>
            {
                { "Baron Nashor", "SRU_Baron" },
                { "Blue Sentinel", "SRU_Blue" },
                { "Water Dragon", "SRU_Dragon_Water" },
                { "Fire Dragon", "SRU_Dragon_Fire" },
                { "Earth Dragon", "SRU_Dragon_Earth" },
                { "Air Dragon", "SRU_Dragon_Air" },
                { "Elder Dragon", "SRU_Dragon_Elder" },
                { "Red Brambleback", "SRU_Red" },
                { "Rift Herald", "SRU_RiftHerald" },
                { "Crimson Raptor", "SRU_Razorbeak" },
                { "Greater Murk Wolf", "SRU_Murkwolf" },
                { "Gromp", "SRU_Gromp" },
                { "Rift Scuttler", "Sru_Crab" },
                { "Ancient Krug", "SRU_Krug" }
            };

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs eventArgs)
        {
            var slot = Player.Instance.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (slot != null)
                Smite = slot.Slot;

            if (Smite == SpellSlot.Unknown)
            {
                Chat.Print("aiRTako SmiteHelper: You Dont Have Smite Slot. Loading Finish", System.Drawing.Color.Orange);
                return;
            }

            if (Game.MapId != GameMapId.SummonersRift)
            {
                Chat.Print("aiRTako SmiteHelper: Not Support this Map!", System.Drawing.Color.Orange);
                return;
            }

            Chat.Print("aiRTako SmiteHelper: Welcome to Use my Addon. Loading Successful", System.Drawing.Color.Orange);

            Menu = MainMenu.AddMenu("aiRTako SmiteHelper", "aiRTako SmiteHelper");

            ObjectMenu = Menu.AddSubMenu("Smite Target", "Smite Target");
            {
                foreach (var item in SmiteObjects)
                {
                    ObjectMenu.Add($"{item.Value}", new CheckBox($"{item.Key}"));
                }
            }

            SettingMenu = Menu.AddSubMenu("Smite Setting", "Smite Setting");
            {
                SettingMenu.AddLabel("Global Settings");
                SettingMenu.Add("Enabled", new KeyBind("Enbaled", true, KeyBind.BindTypes.PressToggle, 'M'));

                SettingMenu.AddSeparator();

                SettingMenu.AddLabel("Combo Settings");
                SettingMenu.Add("Combo", new CheckBox("Enbaled in Combo Mode"));
                SettingMenu.Add("ComboSave", new CheckBox("Save 1 Ammo"));
                SettingMenu.Add("ComboKey", new KeyBind("ComboKey", false, KeyBind.BindTypes.HoldActive, 32));
                SettingMenu.AddSeparator();

                SettingMenu.AddLabel("KillSteal Settings");
                SettingMenu.Add("KillSteal", new CheckBox("Enbaled in KillSteal"));
            }

            DrawMenu = Menu.AddSubMenu("Smite Draw", "Smite Draw");
            {
                DrawMenu.Add("DrawStatus", new CheckBox("Draw Smite Status"));
                DrawMenu.Add("DrawRange", new CheckBox("Draw Smite Range", false));
            }

            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs eventArgs)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling())
                return;

            if (!SettingMenu["Enabled"].Cast<KeyBind>().CurrentValue)
                return;

            if (!Player.GetSpell(Smite).IsReady)
                return;

            Jungle();

            if (SettingMenu["Combo"].Cast<CheckBox>().CurrentValue &&
                SettingMenu["ComboKey"].Cast<KeyBind>().CurrentValue)
                Combo();

            if (SettingMenu["KillSteal"].Cast<CheckBox>().CurrentValue)
                KillSteal();
        }

        private static void Jungle()
        {
            if (!Player.GetSpell(Smite).IsReady)
                return;

            var mobs =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        x =>
                            x != null && x.IsMonster && x.Distance(Player.Instance) <= 900 &&
                            !x.Name.ToLower().Contains("mini") && !x.Name.ToLower().Contains("respawn") &&
                            !x.Name.ToLower().Contains("plant"))
                    .OrderBy(x => x.MaxHealth);

            foreach (var mob in mobs)
            {
                if (mob.Distance(Player.Instance) > SmiteRange)
                    continue;

                if (ObjectMenu[mob.CharData.BaseSkinName] != null &&
                    ObjectMenu[mob.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue &&
                    mob.Health <= Player.Instance.GetSummonerSpellDamage(mob, DamageLibrary.SummonerSpells.Smite) &&
                    mob.Distance(Player.Instance) <= SmiteRange)
                {
                    Player.Instance.Spellbook.CastSpell(Smite, mob);
                    return;
                }
            }
        }

        private static void Combo()
        {
            if (!Player.GetSpell(Smite).IsReady)
                return;

            if (
                !Player.GetSpell(Smite)
                    .Name.Equals("s5_summonersmiteplayerganker", StringComparison.InvariantCultureIgnoreCase) ||
                !Player.GetSpell(Smite)
                    .Name.Equals("s5_summonersmiteduel", StringComparison.InvariantCultureIgnoreCase))
                return;

            if (SettingMenu["ComboSave"].Cast<CheckBox>().CurrentValue && Player.GetSpell(Smite).Ammo <= 1)
                return;

            var target = TargetSelector.GetTarget(SmiteRange, DamageType.True);

            if (target != null && target.IsValidTarget(SmiteRange))
            {
                Player.Instance.Spellbook.CastSpell(Smite, target);
            }
        }

        private static void KillSteal()
        {
            if (!Player.GetSpell(Smite).IsReady)
                return;

            if (!Player.GetSpell(Smite)
                .Name.Equals("s5_summonersmiteplayerganker", StringComparison.InvariantCultureIgnoreCase))
                return;

            var target =
                EntityManager.Heroes.Enemies.FirstOrDefault(
                    x =>
                        x.IsValidTarget(SmiteRange) &&
                        x.Health <= Player.Instance.GetSummonerSpellDamage(x, DamageLibrary.SummonerSpells.Smite) &&
                        !IsUnKillable(x));

            if (target != null && target.IsValidTarget(SmiteRange))
            {
                Player.Instance.Spellbook.CastSpell(Smite, target);
            }
        }

        private static void Drawing_OnDraw(EventArgs eventArgs)
        {
            if (Player.Instance.IsDead)
                return;

            if (DrawMenu["DrawStatus"].Cast<CheckBox>().CurrentValue)
            {
                var MePos = Drawing.WorldToScreen(Player.Instance.Position);
                string str;

                if (SettingMenu["Enabled"].Cast<KeyBind>().CurrentValue)
                {
                    str = !Player.GetSpell(Smite).IsReady ? "Not Ready" : "On";
                }
                else
                {
                    str = "Off";
                }

                Drawing.DrawText(MePos[0] - 68, MePos[1] + 68, System.Drawing.Color.FromArgb(242, 120, 34),
                    "Smite Status:" + str);
            }

            if (DrawMenu["DrawRange"].Cast<CheckBox>().CurrentValue)
            {
                Circle.Draw(Player.GetSpell(Smite).IsReady ? SharpDX.Color.Green : SharpDX.Color.Red, SmiteRange,
                    Player.Instance);
            }
        }

        private static bool IsUnKillable(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.Health <= 0)
            {
                return true;
            }

            if (target.HasBuff("KindredRNoDeathBuff"))
            {
                return true;
            }

            if (target.HasBuff("UndyingRage") && target.GetBuff("UndyingRage").EndTime - Game.Time > 0.3 && target.Health <= target.MaxHealth * 0.10f)
            {
                return true;
            }

            if (target.HasBuff("JudicatorIntervention"))
            {
                return true;
            }

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3 && target.Health <= target.MaxHealth * 0.10f)
            {
                return true;
            }

            if (target.HasBuff("VladimirSanguinePool"))
            {
                return true;
            }

            if (target.HasBuff("ShroudofDarkness"))
            {
                return true;
            }

            if (target.HasBuff("SivirShield"))
            {
                return true;
            }

            if (target.HasBuff("itemmagekillerveil"))
            {
                return true;
            }

            return target.HasBuff("FioraW");
        }
    }
}
