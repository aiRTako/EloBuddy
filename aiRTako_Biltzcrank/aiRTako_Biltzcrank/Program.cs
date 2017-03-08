namespace aiRTako_Biltzcrank
{
    using DamageLib;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class Program
    {
        private static Spell.Skillshot Q, R;
        private static Spell.Active W, E;

        private static SpellSlot Smite = SpellSlot.Unknown;

        private static Menu Menu;
        private static Menu ComboMenu;
        private static Menu HarassMenu;
        private static Menu AutoMenu;
        private static Menu JungleStealMenu;
        private static Menu MiscMenu;
        private static Menu DrawMenu;

        private static readonly Dictionary<string, string> SmiteObjects =
            new Dictionary<string, string>
            {
                {"Baron Nashor", "SRU_Baron"},
                {"Blue Sentinel", "SRU_Blue"},
                {"Water Dragon", "SRU_Dragon_Water"},
                {"Fire Dragon", "SRU_Dragon_Fire"},
                {"Earth Dragon", "SRU_Dragon_Earth"},
                {"Air Dragon", "SRU_Dragon_Air"},
                {"Elder Dragon", "SRU_Dragon_Elder"},
                {"Red Brambleback", "SRU_Red"},
                {"Rift Herald", "SRU_RiftHerald"},
                {"Crimson Raptor", "SRU_Razorbeak"},
                {"Greater Murk Wolf", "SRU_Murkwolf"},
                {"Gromp", "SRU_Gromp"},
                {"Rift Scuttler", "Sru_Crab"},
                {"Ancient Krug", "SRU_Krug"}
            };

        private static void Main(string[] eventArgs)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs eventArgs)
        {
            if (Player.Instance.Hero != Champion.Blitzcrank)
            {
                return;
            }

            Chat.Print("aiRTako Blitzcrank: Welcome to use my Addon!", System.Drawing.Color.Orange);

            var slot = Player.Instance.Spellbook.Spells.FirstOrDefault(x => x.Name.ToLower().Contains("smite"));

            if (slot != null)
                Smite = slot.Slot;

            Q = new Spell.Skillshot(SpellSlot.Q, 950, SkillShotType.Linear, 250, 1800, 80, DamageType.Magical);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E);
            R = new Spell.Skillshot(SpellSlot.R, 545, SkillShotType.Circular);

            Menu = MainMenu.AddMenu("aiRTako Biltzcrank", "aiRTako Biltzcrank");

            ComboMenu = Menu.AddSubMenu("Combo", "Combo");
            {
                ComboMenu.AddLabel("Q Target Settings");
                foreach (var target in EntityManager.Heroes.Enemies)
                {
                    ComboMenu.Add("ComboQ" + target.ChampionName, new CheckBox(target.ChampionName));
                }

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("Q Settings");
                ComboMenu.Add("ComboQ", new CheckBox("Use Q"));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("W Settings");
                ComboMenu.Add("ComboW", new CheckBox("Use W", false));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("E Settings");
                ComboMenu.Add("ComboE", new CheckBox("Use E"));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("R Settings");
                ComboMenu.Add("ComboR", new CheckBox("Use R"));
            }

            HarassMenu = Menu.AddSubMenu("Harass", "Harass");
            {
                HarassMenu.AddLabel("Q Target Settings");
                foreach (var target in EntityManager.Heroes.Enemies)
                {
                    HarassMenu.Add("HarassQ" + target.ChampionName, new CheckBox(target.ChampionName));
                }

                HarassMenu.AddSeparator();

                HarassMenu.AddLabel("Q Settings");
                HarassMenu.Add("HarassQ", new CheckBox("Use Q"));

                HarassMenu.AddSeparator();

                HarassMenu.AddLabel("E Settings");
                HarassMenu.Add("HarassE", new CheckBox("Use E"));

                HarassMenu.AddSeparator();

                HarassMenu.AddLabel("Mana Settings");
                HarassMenu.Add("HarassMana", new Slider("Min Mana %", 60));
            }

            AutoMenu = Menu.AddSubMenu("Auto", "Auto");
            {
                AutoMenu.AddLabel("Q Target Settings");
                foreach (var target in EntityManager.Heroes.Enemies)
                {
                    AutoMenu.Add("AutoQ" + target.ChampionName, new CheckBox(target.ChampionName));
                }

                AutoMenu.AddSeparator();

                AutoMenu.AddLabel("Q Settings");
                AutoMenu.Add("AutoQ", new KeyBind("Use Q", false, KeyBind.BindTypes.PressToggle, 'T'));

                AutoMenu.AddSeparator();

                AutoMenu.AddLabel("Mana Settings");
                AutoMenu.Add("AutoMana", new Slider("Min Mana %", 60));
            }

            JungleStealMenu = Menu.AddSubMenu("JungleSteal", "JungleSteal");
            {
                JungleStealMenu.AddLabel("Global Settings");
                JungleStealMenu.Add("JungleEnabled", new CheckBox("Enbaled Jungle Steal"));

                JungleStealMenu.AddSeparator();

                JungleStealMenu.AddLabel("Q Settings");
                JungleStealMenu.Add("JungleQ", new CheckBox("Use Q"));

                JungleStealMenu.AddSeparator();

                JungleStealMenu.AddLabel("R Settings");
                JungleStealMenu.Add("JungleR", new CheckBox("Use R"));

                JungleStealMenu.AddSeparator();

                JungleStealMenu.AddLabel("Other Settings");
                JungleStealMenu.Add("JungleAlly", new CheckBox("If Have Ally In Range Dont Steal"));
                JungleStealMenu.Add("JungleAllyRange", new Slider("Search Ally Range", 500, 200, 920));

                JungleStealMenu.AddSeparator();

                JungleStealMenu.AddLabel("Steal Settings");
                foreach (var item in SmiteObjects)
                {
                    JungleStealMenu.Add($"{item.Value}", new CheckBox($"{item.Key}"));
                }
            }

            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            {
                MiscMenu.AddLabel("Q Settings");
                MiscMenu.Add("SmiteQ", new CheckBox("Smite Q"));
                MiscMenu.Add("InterruptQ", new CheckBox("Interrupt Spell"));
                MiscMenu.Add("MinQRange", new Slider("Min Cast Q Range", 250, 100, 920));

                MiscMenu.AddSeparator();

                MiscMenu.AddLabel("E Settings");
                MiscMenu.Add("AntiGapE", new CheckBox("Anti Gaplcoser"));

                MiscMenu.AddSeparator();

                MiscMenu.AddLabel("R Settings");
                MiscMenu.Add("InterruptR", new CheckBox("Interrupt Spell"));
            }

            DrawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            {
                DrawMenu.AddLabel("Spell Range");
                DrawMenu.Add("DrawQ", new CheckBox("Draw Q Range", false));
                DrawMenu.Add("DrawR", new CheckBox("Draw R Range", false));
                DrawMenu.AddLabel("Auto Q Status");
                DrawMenu.Add("DrawAutoQ", new CheckBox("Draw Auto Q Status"));
                DamageIndicator.AddToMenu(DrawMenu);
            }

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnBuffGain += Obj_AI_Base_OnBuffGain;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs eventArgs)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling())
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (!Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                AutoMenu["AutoQ"].Cast<KeyBind>().CurrentValue && Q.IsReady() &&
                Player.Instance.ManaPercent >= AutoMenu["AutoMana"].Cast<Slider>().CurrentValue)
            {
                AutoQ();
            }

            if (JungleStealMenu["JungleEnabled"].Cast<CheckBox>().CurrentValue)
            {
                JungleSteal();
            }
        }

        private static void Combo()
        {
            if (ComboMenu["ComboQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target.IsValidTarget(Q.Range) &&
                    ComboMenu["ComboQ" + target.ChampionName].Cast<CheckBox>().CurrentValue && !HaveShiled(target))
                {
                    CastQ(target);
                }
            }

            if (ComboMenu["ComboW"].Cast<CheckBox>().CurrentValue && W.IsReady())
            {
                if ((EntityManager.Heroes.Enemies.Any(x => x.IsValidTarget(Q.Range)) && !Q.IsReady()) ||
                    Player.HasBuffOfType(BuffType.Slow))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.W);
                }
            }

            if (ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange() + 65, DamageType.Physical);

                if (target.IsValidTarget() && !Q.IsReady())
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                    Orbwalker.ForcedTarget = target;
                }
            }

            if (ComboMenu["ComboR"].Cast<CheckBox>().CurrentValue && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);

                if (target.IsValidTarget(R.Range) && target.HasBuff("rocketgrab2"))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                }
                else if (target.IsValidTarget(R.Range) && target.Health < DamageCalculate.GetRDamage(target) && !IsUnKillable(target))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                }
                else if (Player.Instance.CountEnemyChampionsInRange(R.Range) >= 3)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                }
                else if (Player.Instance.CountEnemyChampionsInRange(R.Range) >= 2 && Player.Instance.IsMelee)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                }
            }
        }

        private static void Harass()
        {
            if (Player.Instance.ManaPercent < HarassMenu["HarassMana"].Cast<Slider>().CurrentValue)
                return;

            if (!HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue || !Q.IsReady())
                return;

            var targets =
                EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.IsValidTarget(Q.Range) &&
                            HarassMenu["HarassQ" + x.ChampionName].Cast<CheckBox>().CurrentValue && !HaveShiled(x))
                    .OrderBy(TargetSelector.GetPriority)
                    .ToList();

            foreach (var target in targets)
            {
                if (target.IsValidTarget(Q.Range))
                {
                    CastQ(target);
                    return;
                }
            }
        }

        private static void AutoQ()
        {
            foreach (
                var target in
                EntityManager.Heroes.Enemies.Where(
                        x =>
                            x.IsValidTarget(Q.Range) && AutoMenu["AutoQ" + x.ChampionName].Cast<CheckBox>().CurrentValue &&
                            !x.IsValidTarget(MiscMenu["MinQRange"].Cast<Slider>().CurrentValue) && !HaveShiled(x))
                    .OrderBy(TargetSelector.GetPriority))
            {
                if (target.IsValidTarget(Q.Range))
                {
                    CastQ(target);
                    return;
                }
            }
        }

        private static void JungleSteal()
        {
            var mobs =
                ObjectManager.Get<Obj_AI_Minion>()
                    .Where(
                        x =>
                            x != null && x.IsMonster && x.Distance(Player.Instance) <= 950 &&
                            !x.Name.ToLower().Contains("mini") && !x.Name.ToLower().Contains("respawn") &&
                            !x.Name.ToLower().Contains("plant"))
                    .OrderBy(x => x.MaxHealth);

            foreach (var mob in mobs)
            {
                if (mob.Distance(Player.Instance) > (Q.IsReady() ? Q.Range : R.IsReady() ? R.Range : 1000))
                    continue;

                if (JungleStealMenu["JungleAlly"].Cast<CheckBox>().CurrentValue &&
                    EntityManager.Heroes.Allies.Any(
                        x =>
                            !x.IsDead && !x.IsMe &&
                            x.Distance(mob) <= JungleStealMenu["JungleAllyRange"].Cast<Slider>().CurrentValue))
                {
                    continue;
                }

                if (JungleStealMenu[mob.CharData.BaseSkinName] != null &&
                    JungleStealMenu[mob.CharData.BaseSkinName].Cast<CheckBox>().CurrentValue)
                {
                    if (JungleStealMenu["JungleR"].Cast<CheckBox>().CurrentValue &&
                        mob.Distance(Player.Instance) <= R.Range && mob.Health <= DamageCalculate.GetRDamage(mob))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                    }
                    else if (JungleStealMenu["JungleQ"].Cast<CheckBox>().CurrentValue &&
                        mob.Distance(Player.Instance) <= Q.Range && mob.Health <= DamageCalculate.GetQDamage(mob))
                    {
                        var Pred = Q.GetPrediction(mob);

                        if (Pred.HitChance == HitChance.High && !Pred.CollisionObjects.Any())
                            Q.Cast(mob.Position);
                    }
                }
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs eventArgs)
        {
            if (target.Type != GameObjectType.AIHeroClient || !E.IsReady())
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                Orbwalker.ForcedTarget = target;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue &&
                Player.Instance.ManaPercent >= HarassMenu["HarassMana"].Cast<Slider>().CurrentValue)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                Orbwalker.ForcedTarget = target;
            }
        }

        private static void Obj_AI_Base_OnBuffGain(Obj_AI_Base sender, Obj_AI_BaseBuffGainEventArgs eventArgs)
        {
            if (sender == null || sender.Type != GameObjectType.AIHeroClient || !sender.IsEnemy ||
                !eventArgs.Buff.Name.ToLower().Contains("rocketgrab2") || !eventArgs.Buff.Caster.IsMe)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                Orbwalker.ForcedTarget = sender;
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass) &&
                HarassMenu["HarassE"].Cast<CheckBox>().CurrentValue &&
                Player.Instance.ManaPercent >= HarassMenu["HarassMana"].Cast<Slider>().CurrentValue)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                Orbwalker.ForcedTarget = sender;
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs eventArgs)
        {
            if (!MiscMenu["AntiGapE"].Cast<CheckBox>().CurrentValue || !E.IsReady() || sender == null || !sender.IsEnemy)
                return;

            if (Player.Instance.IsInAutoAttackRange(sender) && !HaveShiled(sender))
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                Orbwalker.ForcedTarget = sender;
            }   
        }

        private static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs eventArgs)
        {
            if (sender == null || !sender.IsEnemy || eventArgs == null || sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var target = (AIHeroClient)sender;

            if (eventArgs.DangerLevel >= DangerLevel.High && !HaveShiled(target))
            {
                if (MiscMenu["InterruptR"].Cast<CheckBox>().CurrentValue && R.IsReady() && target.IsValidTarget(R.Range))
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                else if (MiscMenu["InterruptQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range))
                    CastQ(target);
            }
        }

        private static void Drawing_OnDraw(EventArgs eventArgs)
        {
            if (Player.Instance.IsDead)
                return;

            if (DrawMenu["DrawQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                Circle.Draw(SharpDX.Color.Red, Q.Range, Player.Instance);

            if (DrawMenu["DrawR"].Cast<CheckBox>().CurrentValue && R.IsReady())
                Circle.Draw(SharpDX.Color.Blue, R.Range, Player.Instance);

            if (DrawMenu["DrawAutoQ"].Cast<CheckBox>().CurrentValue)
            {
                var MePos = Drawing.WorldToScreen(Player.Instance.Position);
                string str;

                if (AutoMenu["AutoQ"].Cast<KeyBind>().CurrentValue)
                {
                    str = !Q.IsReady() ? "Not Ready" : "On";
                }
                else
                {
                    str = "Off";
                }

                Drawing.DrawText(MePos[0] - 68, MePos[1] + 68, System.Drawing.Color.FromArgb(242, 120, 34),
                    "AutoQ Status:" + str);
            }
        }

        private static void CastQ(AIHeroClient target)
        {
            if (!Q.IsReady() || !target.IsValidTarget(Q.Range) || target.IsValidTarget(MiscMenu["MinQRange"].Cast<Slider>().CurrentValue))
                return;

            if (MiscMenu["SmiteQ"].Cast<CheckBox>().CurrentValue && Smite != SpellSlot.Unknown && Player.GetSpell(Smite).IsReady)
            {
                var Pred = Q.GetPrediction(target);
                var CollsionObj = Pred.CollisionObjects.Where(x => x.Type != GameObjectType.AIHeroClient).ToList();

                switch (CollsionObj.Count)
                {
                    case 0:
                        if (Pred.HitChance >= HitChance.High)
                        {
                            Q.Cast(Pred.CastPosition);
                        }

                        break;
                    case 1:
                        var obj = CollsionObj.FirstOrDefault();

                        if (obj != null && obj.IsValidTarget(600f) &&
                            obj.Health <=
                            Player.Instance.GetSummonerSpellDamage(obj, DamageLibrary.SummonerSpells.Smite))
                        {
                            Q.Cast(Pred.CastPosition);
                            Core.DelayAction(() => Player.Instance.Spellbook.CastSpell(Smite, obj), Game.Ping);
                        }

                        break;
                    default:
                        return;
                }
            }
            else
            {
                var Pred = Q.GetPrediction(target);

                if (Pred.HitChance >= HitChance.High && !Pred.CollisionObjects.Any())
                {
                    Q.Cast(Pred.CastPosition);
                }
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

        private static bool HaveShiled(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.Health <= 0)
            {
                return false;
            }

            if (target.HasBuff("BlackShield"))
            {
                return true;
            }

            if (target.HasBuff("bansheesveil"))
            {
                return true;
            }

            if (target.HasBuff("SivirE"))
            {
                return true;
            }

            if (target.HasBuff("NocturneShroudofDarkness"))
            {
                return true;
            }

            if (target.HasBuff("itemmagekillerveil"))
            {
                return true;
            }

            if (target.HasBuffOfType(BuffType.SpellShield))
            {
                return true;
            }

            return false;
        }
    }
}
