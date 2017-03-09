namespace aiRTako_Kalista
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
    using System.Linq;

    internal class Program
    {
        private static Spell.Skillshot Q, W;
        private static Spell.Active E, R;

        private static int lastWTime, lastETime;

        private static Menu Menu;
        private static Menu ComboMenu;
        private static Menu HarassMenu;
        private static Menu ClearMenu;
        private static Menu MiscMenu;
        private static Menu DrawMenu;

        private static void Main(string[] eventArgs)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs eventArgs)
        {
            if (Player.Instance.Hero != Champion.Kalista)
            {
                return;
            }

            Chat.Print("aiRTako Kalista: Welcome to use my Addon!", System.Drawing.Color.Orange);

            Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 350, 2400, 40, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 5000, SkillShotType.Linear);
            E = new Spell.Active(SpellSlot.E, 950, DamageType.Physical);
            R = new Spell.Active(SpellSlot.R, 1500);

            Menu = MainMenu.AddMenu("aiRTako Kalista", "aiRTako Kalista");

            ComboMenu = Menu.AddSubMenu("Combo", "Combo");
            {
                ComboMenu.AddLabel("Q Settings");
                ComboMenu.Add("ComboQ", new CheckBox("Use Q"));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("W Settings");
                ComboMenu.Add("ComboW", new CheckBox("Use W", false));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("E Settings");
                ComboMenu.Add("ComboE", new CheckBox("Use E"));
                ComboMenu.Add("ComboEUse", new CheckBox("If Can Kill Minion And Slow Target"));
                ComboMenu.Add("ComboEMana", new CheckBox("Save Mana to Cast E"));

                ComboMenu.AddSeparator();

                ComboMenu.AddLabel("Misc Settings");
                ComboMenu.Add("ComboAttack", new CheckBox("Attack Minion To Dash"));
            }

            HarassMenu = Menu.AddSubMenu("Harass", "Harass");
            {
                HarassMenu.AddLabel("Q Settings");
                HarassMenu.Add("HarassQ", new CheckBox("Use Q", false));

                HarassMenu.AddSeparator();

                HarassMenu.AddLabel("E Settings");
                HarassMenu.Add("HarassESlow", new CheckBox("If Can Kill Minion And Slow Target"));
                HarassMenu.Add("HarassECount", new Slider("If Target E Count >= x", 5, 1, 25));

                HarassMenu.AddSeparator();

                HarassMenu.AddLabel("Mana Settings");
                HarassMenu.Add("HarassMana", new Slider("Min Mana %", 60));
            }

            ClearMenu = Menu.AddSubMenu("Clear", "Clear");
            {
                ClearMenu.AddGroupLabel("LaneClear Settings");
                {
                    ClearMenu.AddLabel("E Settings");
                    ClearMenu.Add("LaneClearE", new CheckBox("Use E"));
                    ClearMenu.Add("LaneClearECount", new Slider("Can Kill Minion Count >= x", 5, 1, 25));

                    ClearMenu.AddSeparator();

                    ClearMenu.AddLabel("LaneClear Mana Settings");
                    ClearMenu.Add("LaneClearMana", new Slider("Min Mana %", 60));
                }

                ClearMenu.AddSeparator();

                ClearMenu.AddGroupLabel("JungleClear Settings");
                {
                    ClearMenu.AddLabel("Q Settings");
                    ClearMenu.Add("JungleClearQ", new CheckBox("Use Q"));

                    ClearMenu.AddSeparator();

                    ClearMenu.AddLabel("E Settings");
                    ClearMenu.Add("JungleClearE", new CheckBox("Use E"));

                    ClearMenu.AddSeparator();

                    ClearMenu.AddLabel("JungleClear Mana Settings");
                    ClearMenu.Add("JungleClearMana", new Slider("Min Mana %", 60));
                }

                ManaManager.AddSpellFarm(ClearMenu);
            }

            MiscMenu = Menu.AddSubMenu("Misc", "Misc");
            {
                MiscMenu.AddLabel("E Settings");
                MiscMenu.Add("KillStealE", new CheckBox("KillSteal"));
                MiscMenu.Add("LastHitE", new CheckBox("LastHit"));
                MiscMenu.Add("StealMob", new CheckBox("Steal Dragon/Baron"));

                MiscMenu.AddSeparator();

                MiscMenu.AddLabel("R Settings");
                MiscMenu.Add("AutoSave", new CheckBox("Auto R Save Ally"));
                MiscMenu.Add("AutoSaveHp", new Slider("When Ally Health Percent <= x%", 20, 1));
                MiscMenu.Add("Balista", new CheckBox("Balista"));

                MiscMenu.AddSeparator();

                MiscMenu.AddLabel("Target Settings");
                MiscMenu.Add("Forcus", new CheckBox("Forcus Attack Passive Target"));
            }

            DrawMenu = Menu.AddSubMenu("Drawings", "Drawings");
            {
                DrawMenu.AddLabel("Spell Range");
                DrawMenu.Add("DrawQ", new CheckBox("Draw Q Range", false));
                DrawMenu.Add("DrawE", new CheckBox("Draw E Range"));
                DrawMenu.Add("DrawR", new CheckBox("Draw R Range", false));
                ManaManager.AddDrawFarm(DrawMenu);
                DamageIndicator.AddToMenu(DrawMenu);
            }

            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnUnkillableMinion += Orbwalker_OnUnkillableMinion;
            Orbwalker.OnPreAttack += Orbwalker_OnPreAttack;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs eventArgs)
        {
            if (!E.IsReady())
                return;

            if (ComboMenu["ComboEMana"].Cast<CheckBox>().CurrentValue &&
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                if (eventArgs.Slot == SpellSlot.Q || eventArgs.Slot == SpellSlot.W || eventArgs.Slot == SpellSlot.R)
                {
                    if (Player.Instance.Mana < E.ManaCost)
                        eventArgs.Process = true;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs eventArgs)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling())
                return;

            KillSteal();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                Combo();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                Harass();
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
            {
                Farm();
            }

            StealMob();
            RLogic();
        }

        private static void KillSteal()
        {
            if (MiscMenu["KillStealE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                foreach (
                    var target in
                    EntityManager.Heroes.Enemies.Where(
                        x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker")))
                {
                    if (target.Health > DamageCalculate.GetRealEDamage(target) || DamageCalculate.IsUnKillable(target))
                        continue;

                    if (!target.IsValidTarget(E.Range))
                        continue;

                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                }
            }
        }

        private static void Combo()
        {   
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target.IsValidTarget(Q.Range))
            {
                if (ComboMenu["ComboAttack"].Cast<CheckBox>().CurrentValue &&
                    target.Distance(Player.Instance) >
                    Player.Instance.GetAutoAttackRange(target) + Player.Instance.BoundingRadius)
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .Where(
                                x =>
                                    x != null && x.IsEnemy &&
                                    x.Distance(Player.Instance) <= Player.Instance.GetAutoAttackRange() + 65 &&
                                    !x.Name.ToLower().Contains("plant"))
                            .OrderBy(x => x.Distance(target))
                            .FirstOrDefault();

                    if (minion != null && !minion.IsDead)
                    {
                        Orbwalker.ForcedTarget = minion;
                    }
                }

                if (ComboMenu["ComboE"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) &&
                    target.HasBuff("kalistaexpungemarker") && target.Health < DamageCalculate.GetRealEDamage(target) &&
                    !DamageCalculate.IsUnKillable(target) && TickCount - lastETime >= 500)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                }

                if (ComboMenu["ComboEUse"].Cast<CheckBox>().CurrentValue && E.IsReady() && target.IsValidTarget(E.Range) &&
                    target.HasBuff("kalistaexpungemarker") && TickCount - lastETime >= 500 &&
                    target.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange(target) + 100)
                {
                    var minion =
                        ObjectManager.Get<Obj_AI_Minion>()
                            .FirstOrDefault(
                                x =>
                                    x != null && x.IsEnemy && x.Distance(Player.Instance) <= E.Range &&
                                    x.HasBuff("kalistaexpungemarker") && x.Health < DamageCalculate.GetEDamage(x));

                    if (minion != null && minion.IsValidTarget(E.Range))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                    }
                }

                if (ComboMenu["ComboQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && target.IsValidTarget(Q.Range) &&
                    target.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange(target))
                {
                    var Pred = Q.GetPrediction(target);

                    if (Pred.HitChance == HitChance.High && !Pred.CollisionObjects.Any())
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }

                if (ComboMenu["ComboW"].Cast<CheckBox>().CurrentValue && W.IsReady() && TickCount - lastWTime > 2500)
                {
                    if (NavMesh.IsWallOfGrass(target.ServerPosition, 20) || !target.IsVisible)
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.W, target.ServerPosition);
                    }
                }
            }
        }

        private static void Harass()
        {
            if (Player.Instance.ManaPercent >= HarassMenu["HarassMana"].Cast<Slider>().CurrentValue)
            {
                if (HarassMenu["HarassQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    var target =
                        EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Q.Range))
                            .FirstOrDefault(
                                x =>
                                    Q.GetPrediction(x).HitChance == HitChance.High &&
                                    !Q.GetPrediction(x).CollisionObjects.Any());

                    if (target != null && target.IsValidTarget(Q.Range))
                    {
                        Q.Cast(Q.GetPrediction(target).CastPosition);
                    }
                }

                if (E.IsReady())
                {
                    foreach (
                        var target in
                        EntityManager.Heroes.Enemies.Where(
                            x => x.IsValidTarget(E.Range) && x.HasBuff("kalistaexpungemarker")))
                    {
                        if (!target.IsValidTarget(E.Range))
                            continue;

                        if (HarassMenu["HarassESlow"].Cast<CheckBox>().CurrentValue)
                        {
                            var minion =
                                ObjectManager.Get<Obj_AI_Minion>()
                                    .FirstOrDefault(
                                        x =>
                                            x != null && x.IsEnemy && x.Distance(Player.Instance) <= E.Range &&
                                            x.HasBuff("kalistaexpungemarker") &&
                                            x.Health < DamageCalculate.GetEDamage(x));

                            if (minion != null && minion.IsValidTarget(E.Range))
                                Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                        }

                        if (target.GetBuffCount("kalistaexpungemarker") >= HarassMenu["HarassECount"].Cast<Slider>().CurrentValue)
                            Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                    }
                }
            }
        }

        private static void Farm()
        {
            if (Player.Instance.ManaPercent >= HarassMenu["HarassMana"].Cast<Slider>().CurrentValue && ManaManager.SpellHarass)
            {
                Harass();
            }

            if (ManaManager.SpellFarm)
            {
                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                {
                    LaneClear();
                }

                if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
                {
                    JungleClear();
                }
            }
        }

        private static void LaneClear()
        {
            if (Player.Instance.ManaPercent < ClearMenu["LaneClearMana"].Cast<Slider>().CurrentValue)
                return;

            if (TickCount - lastETime < 1200)
                return;

            if (!ClearMenu["LaneClearE"].Cast<CheckBox>().CurrentValue)
                return;

            var minions =
                EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position,
                    E.Range).ToList();

            if (minions.Count >= ClearMenu["LaneClearECount"].Cast<Slider>().CurrentValue)
            {
                var canKillCount =
                    minions.Count(
                        x => !x.IsDead && x.HasBuff("kalistaexpungemarker") && x.Health < DamageCalculate.GetEDamage(x));

                if (canKillCount >= ClearMenu["LaneClearECount"].Cast<Slider>().CurrentValue)
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
            }
        }

        private static void JungleClear()
        {
            if (Player.Instance.ManaPercent < ClearMenu["JungleClearMana"].Cast<Slider>().CurrentValue)
                return;

            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).ToList();

            if (!mobs.Any())
                return;

            if (ClearMenu["JungleClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
            {
                var mob = mobs.FirstOrDefault();

                if (mob.Distance(Player.Instance) > Player.Instance.GetAutoAttackRange(mob) &&
                    mob.IsValidTarget(Q.Range))
                {
                    if (mob != null)
                        Q.Cast(mob.Position);
                }
            }

            if (ClearMenu["JungleClearE"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                if (mobs.Any(
                    x => !x.IsDead && x.HasBuff("kalistaexpungemarker") && x.Health < DamageCalculate.GetEDamage(x)))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                }
            }
        }

        private static void StealMob()
        {
            if (MiscMenu["StealMob"].Cast<CheckBox>().CurrentValue && E.IsReady())
            {
                var mob =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsMonster && x.Distance(Player.Instance) <= E.Range &&
                                (x.Name.ToLower().Contains("SRU_Baron") || x.Name.ToLower().Contains("SRU_RiftHerald") ||
                                 x.Name.ToLower().Contains("SRU_Dragon")));

                if (mob != null && mob.IsValidTarget(E.Range) && mob.Health < DamageCalculate.GetEDamage(mob))
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.E);
                }
            }
        }

        private static void RLogic()
        {
            if (!R.IsReady())
                return;

            var myDear =
                EntityManager.Heroes.Allies.FirstOrDefault(
                    x => !x.IsDead && !x.IsMe && !x.IsZombie && x.HasBuff("kalistacoopstrikeally"));

            if (myDear != null && myDear.IsVisible && myDear.Distance(Player.Instance) <= R.Range)
            {
                if (MiscMenu["AutoSave"].Cast<CheckBox>().CurrentValue && 
                    Player.Instance.CountEnemyChampionsInRange(R.Range) > 0 &&
                    myDear.CountEnemyChampionsInRange(R.Range) > 0 &&
                    myDear.HealthPercent <= MiscMenu["AutoSaveHp"].Cast<Slider>().CurrentValue)
                {
                    Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                }
                
                if (MiscMenu["Balista"].Cast<CheckBox>().CurrentValue && myDear.ChampionName == "Blitzcrank")
                {
                    if (
                        EntityManager.Heroes.Enemies.Any(
                            x => !x.IsDead && !x.IsZombie && x.IsValidTarget() && x.HasBuff("rocketgrab2")))
                    {
                        Player.Instance.Spellbook.CastSpell(SpellSlot.R);
                    }
                }
            }
        }

        private static void Orbwalker_OnUnkillableMinion(Obj_AI_Base target, Orbwalker.UnkillableMinionArgs eventArgs)
        {
            if (Player.Instance.IsDead || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
            {
                return;
            }

            if (!MiscMenu["LastHitE"].Cast<CheckBox>().CurrentValue || !E.IsReady())
            {
                return;
            }

            var minion = (Obj_AI_Minion)target;

            if (minion != null && minion.IsValidTarget(E.Range) && minion.Health < DamageCalculate.GetEDamage(minion) &&
                Player.Instance.CountEnemyChampionsInRange(600) == 0 && Player.Instance.ManaPercent >= 30)
            {
                E.Cast();
            }
        }

        private static void Orbwalker_OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs eventArgs)
        {
            if (!MiscMenu["Forcus"].Cast<CheckBox>().CurrentValue)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) ||
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
            {
                foreach (var t in EntityManager.Heroes.Enemies.Where(x => !x.IsDead && !x.IsZombie &&
                                                                              x.Distance(Player.Instance) <=
                                                                              Player.Instance.GetAutoAttackRange() + 65 &&
                                                                              x.HasBuff("kalistacoopstrikemarkally")))
                {
                    if (t.IsValidTarget())
                    {
                        Orbwalker.ForcedTarget = t;
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear) || 
                Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear))
            {
                var minion =
                    ObjectManager.Get<Obj_AI_Minion>()
                        .FirstOrDefault(
                            x =>
                                x != null && x.IsEnemy &&
                                x.Distance(Player.Instance) <= Player.Instance.GetAutoAttackRange() + 65 &&
                                !x.Name.ToLower().Contains("plant") && x.HasBuff("kalistacoopstrikemarkally"));

                if (minion != null && minion.IsValidTarget())
                {
                    Orbwalker.ForcedTarget = minion;
                }
            }
        }

        private static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs eventArgs)
        {
            if (target == null)
                return;

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) &&
                target.Type == GameObjectType.AIHeroClient)
            {
                var hero = (AIHeroClient)target;

                if (hero.IsValidTarget(Q.Range) && ComboMenu["ComboQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    var Pred = Q.GetPrediction(hero);

                    if (Pred.HitChance == HitChance.High && !Pred.CollisionObjects.Any())
                    {
                        Q.Cast(Pred.CastPosition);
                    }
                }
            }

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear) && ManaManager.SpellFarm &&
                target.Type == GameObjectType.obj_AI_Minion)
            {
                if (Player.Instance.ManaPercent < ClearMenu["JungleClearMana"].Cast<Slider>().CurrentValue)
                    return;

                var mob = (Obj_AI_Minion)target;

                if (ClearMenu["JungleClearQ"].Cast<CheckBox>().CurrentValue && Q.IsReady() && mob.IsMonster)
                {
                    Q.Cast(mob.Position);
                }
            }
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs eventArgs)
        {
            if (!sender.IsMe)
                return;

            if (eventArgs.SData.Name.Contains("KalistaW"))
            {
                lastWTime = TickCount;
            }

            if (eventArgs.SData.Name.Contains("KalistaExpunge") || eventArgs.SData.Name.Contains("KalistaExpungeWrapper") ||
                eventArgs.SData.Name.Contains("KalistaDummySpell"))
            {
                lastETime = TickCount;
            }
        }

        private static void Drawing_OnDraw(EventArgs eventArgs)
        {
            if (!Player.Instance.IsDead)
            {
                if (DrawMenu["DrawQ"].Cast<CheckBox>().CurrentValue && Q.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Green, Q.Range, Player.Instance);
                }

                if (DrawMenu["DrawE"].Cast<CheckBox>().CurrentValue && E.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Red, E.Range, Player.Instance);
                }

                if (DrawMenu["DrawR"].Cast<CheckBox>().CurrentValue && R.IsReady())
                {
                    Circle.Draw(SharpDX.Color.Blue, R.Range, Player.Instance);
                }
            }
        }

        private static int TickCount => Environment.TickCount;
    }
}
