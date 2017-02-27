namespace MoonEzreal
{
    using myCommon;

    using System;
    using System.Linq;

    using SharpDX;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Enumerations;

    internal class Ezreal : Logic
    {
        internal static void Init()
        {
            //Draw the Chat
            Chat.Print("Moon" + Player.Instance.ChampionName + ": Load! Credit: NightMoon", System.Drawing.Color.LightSteelBlue);

            //Init Ezreal SkillSlot
            Q = new Spell.Skillshot(SpellSlot.Q, 1150, SkillShotType.Linear, 250, 2000, 60, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 1255, SkillShotType.Cone, 250, 2000, 60, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 475, SkillShotType.Linear, 650, 1200, 30, DamageType.Magical);
            R = new Spell.Skillshot(SpellSlot.R, 2000, SkillShotType.Linear, 250, 1600, 130, DamageType.Magical);
            EQ = new Spell.Skillshot(SpellSlot.Q, 1150 + 475, SkillShotType.Linear, 900, 2000, 60, DamageType.Physical);

            Q.HaveCollsion(true);
            W.HaveCollsion(false);
            R.HaveCollsion(false);
            EQ.HaveCollsion(true);

            //Init Ezreal Menu
            MenuInit.Init();

            //Init Ezreal Events
            //Game.OnTick += OnTick; // too slow
            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Orbwalker.OnPreAttack += OnPreAttack;
            Orbwalker.OnPostAttack += OnPostAttack;
            Gapcloser.OnGapcloser += OnGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnTick(EventArgs Args)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling())
            {
                return;
            }

            SetSpellMana();

            if (isComboMode)
            {
                Combo();
            }

            if (isHarassMode)
            {
                Harass();
            }

            if (isFarmMode)
            {
                Farm();
            }

            if (R.Level > 0 && R.IsReady() && Player.Instance.Mana > RMana + QMana)
            {
                RLogic();
            }

            KillSteal();
        }

        private static void SetSpellMana()
        {
            if (Q.Level == 0 || !Q.IsReady() || PlayerMana < Q.ManaCost)
            {
                QMana = 0;
            }
            else
            {
                QMana = Q.ManaCost;
            }

            if (W.Level == 0 || !W.IsReady() || PlayerMana < W.ManaCost)
            {
                WMana = 0;
            }
            else
            {
                WMana = W.ManaCost;
            }

            if (E.Level == 0 || !E.IsReady() || PlayerMana < E.ManaCost)
            {
                EMana = 0;
            }
            else
            {
                EMana = E.ManaCost;
            }

            if (R.Level == 0 || !R.IsReady() || PlayerMana < R.ManaCost || Player.Instance.HealthPercent <= 25)
            {
                RMana = 0;
            }
            else
            {
                RMana = R.ManaCost;
            }
        }

        private static void RLogic()
        {
            R.Range = (uint)MenuInit.MaxRRange;

            if (MenuInit.SemiR)
            {
                SemiRLogic();
            }
        }

        private static void SemiRLogic()
        {
            var select = TargetSelector.SelectedTarget;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);

            if (select != null && !select.HaveShiled() && select.IsValidTarget(R.Range))
            {
                var rPred = R.GetPrediction(select);

                if (rPred.HitChance >= HitChance.High)
                {
                    R.Cast(rPred.CastPosition);
                }
            }
            else if (select == null && target != null && !target.HaveShiled() && target.IsValidTarget(R.Range))
            {
                var rPred = R.GetPrediction(target);

                if (rPred.HitChance >= HitChance.High)
                {
                    R.Cast(rPred.CastPosition);
                }
            }
        }

        private static void KillSteal()
        {
            if (MenuInit.KillStealQ && Q.IsReady() && PlayerMana > QMana + EMana)
            {
                foreach (var target in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget(Q.Range) && !x.IsUnKillable() && x.Health < DamageCalculate.GetQDamage(x)))
                {
                    if (target.IsValidTarget(Q.Range))
                    {
                        Q.PredCast(target);
                        return;
                    }
                }
            }

            if (MenuInit.KillStealW && W.IsReady() && PlayerMana > WMana + QMana + EMana)
            {
                foreach (var target in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget(W.Range) && !x.IsUnKillable() && x.Health < DamageCalculate.GetWDamage(x)))
                {
                    if (target.IsValidTarget(W.Range))
                    {
                        W.PredCast(target, false);
                        return;
                    }
                }
            }

            if (MenuInit.KillStealR && R.IsReady() && PlayerMana > RMana + QMana + EMana)
            {
                foreach(var target in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget(R.Range) && killStealMenu.GetBool("KillStealR" + x.ChampionName.ToLower()) && 
                    !x.IsUnKillable() && x.Health < DamageCalculate.GetRDamage(x)))
                {
                    if (target.DistanceToPlayer() < MenuInit.MinRRange)
                    {
                        continue;
                    }

                    var rPred = R.GetPrediction(target);

                    if (rPred.HitChance >= HitChance.High && 
                        target.Health + target.HPRegenRate * 3 < DamageCalculate.GetRDamage(target, rPred.CollisionObjects.Count()))
                    {
                        R.Cast(rPred.CastPosition);
                        return;
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = Orbwalker.GetTarget() as AIHeroClient ?? TargetSelector.GetTarget(EQ.Range, DamageType.Mixed);

            if (target.IsValidTarget(EQ.Range))
            {
                if (MenuInit.ComboQ && Q.IsReady() && target.IsValidTarget(Q.Range))
                {
                    Q.PredCast(target);
                }

                if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    W.PredCast(target, false);
                }

                if (MenuInit.ComboE && E.IsReady() && target.IsValidTarget(EQ.Range))
                {
                    var CastEPos = Player.Instance.Extend(target.Position, 475f);

                    if (MenuInit.ComboESafe)
                    {
                        if (CastEPos.IsUnderTurret() && Player.Instance.HealthPercent <= 60)
                        {
                            return;
                        }

                        if (Player.Instance.CountEnemyChampionsInRange(1200) > 3 && CastEPos.CountEnemyChampionsInRange(600) > 2)
                        {
                            return;
                        }
                    }

                    var useECombo = false;

                    if (target.DistanceToPlayer() > Player.Instance.GetAutoAttackRange() &&
                        !target.IsUnKillable() && Prediction.Health.GetPrediction(target, EQ.CastDelay) > 0)
                    {
                        if (target.Health < DamageCalculate.GetEDamage(target) + Player.Instance.GetAutoAttackDamage(target) &&
                            target.Distance(Game.CursorPos) < Player.Instance.Distance(Game.CursorPos))
                        {
                            useECombo = true;
                        }

                        if (target.Health < DamageCalculate.GetEDamage(target) + DamageCalculate.GetWDamage(target) && W.IsReady() &&
                            target.Distance(Game.CursorPos) + 350 < Player.Instance.Distance(Game.CursorPos))
                        {
                            useECombo = true;
                        }

                        if (target.Health < DamageCalculate.GetEDamage(target) + DamageCalculate.GetQDamage(target) && Q.IsReady() &&
                            target.Distance(Game.CursorPos) + 300 < Player.Instance.Distance(Game.CursorPos))
                        {
                            useECombo = true;
                        }
                    }

                    if (useECombo)
                    {
                        if (MenuInit.ComboEWall)
                        {
                            if (NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Wall &&
                                NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Building &&
                                NavMesh.GetCollisionFlags(CastEPos) != CollisionFlags.Prop)
                            {
                                E.Cast(CastEPos);
                                useECombo = false;
                            }
                        }
                        else
                        {
                            E.Cast(CastEPos);
                            useECombo = false;
                        }
                    }
                }

                if (MenuInit.ComboR && R.IsReady())
                {
                    if (Player.Instance.IsUnderEnemyturret() || Player.Instance.CountEnemyChampionsInRange(900) > 1)
                    {
                        return;
                    }

                    foreach (
                        var rrrTarget in
                        EntityManager.Heroes.Enemies.Where(
                            x =>
                                x.IsValidTarget(R.Range) &&
                                target.DistanceToPlayer() > Player.Instance.GetAutoAttackRange() &&
                                Prediction.Health.GetPrediction(x, 3000) > 0))
                    {
                        if (rrrTarget.IsUnKillable() || rrrTarget.DistanceToPlayer() < MenuInit.MinRRange)
                        {
                            continue;
                        }

                        var rPred = R.GetPrediction(rrrTarget);

                        if (R.GetPrediction(rrrTarget).HitChance < HitChance.High)
                        {
                            return;
                        }

                        var rColCount = rPred.CollisionObjects.Count();

                        if (rrrTarget.Health < DamageCalculate.GetRDamage(rrrTarget) && rrrTarget.DistanceToPlayer() > Q.Range + E.Range / 2)
                        {
                            if (rrrTarget.Health + target.HPRegenRate * 2 < DamageCalculate.GetRDamage(target, rColCount))
                            {
                                R.Cast(rPred.CastPosition);
                            }
                        }

                        if (rrrTarget.IsValidTarget(Q.Range + E.Range) &&
                            DamageCalculate.GetRDamage(rrrTarget, rColCount) + (Q.IsReady() ? DamageCalculate.GetQDamage(rrrTarget) : 0) +
                           (W.IsReady() ? DamageCalculate.GetWDamage(rrrTarget) : 0) > rrrTarget.Health + rrrTarget.HPRegenRate * 2)
                        {
                            R.Cast(rPred.CastPosition);
                        }
                    }
                }
            }
        }

        private static void Harass()
        {
            if (ManaManager.HasEnoughMana(MenuInit.HarassMP))
            {
                if (MenuInit.HarassQ && Q.IsReady())
                {
                    foreach(var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(Q.Range) && 
                                          harassMenu["Harasstarget" + x.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue))
                    {
                        if (target.IsValidTarget(Q.Range))
                        {
                            Q.PredCast(target);
                            return;
                        }
                    }
                }

                if (MenuInit.HarassW && W.IsReady())
                {
                    foreach (var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(W.Range) &&
                                           harassMenu["Harasstarget" + x.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue))
                    {
                        if (target.IsValidTarget(W.Range))
                        {
                            W.PredCast(target, false);
                            return;
                        }
                    }
                }
            }
        }

        private static void Farm()
        {
            var range = (MenuInit.HarassQ && Q.IsReady()) ? Q.Range : (MenuInit.HarassW && W.IsReady()) ? W.Range : 0;

            if (ManaManager.SpellHarass && ManaManager.HasEnoughMana(MenuInit.HarassMP) &&
                range > 0 && Player.Instance.CountEnemyChampionsInRange(range) > 0)
            {
                Harass();
            }
            else if (ManaManager.SpellFarm)
            {
                if (isLaneClearMode && ManaManager.HasEnoughMana(MenuInit.LaneClearMP))
                {
                    LaneClear();
                }

                if (isJungleClearMode && ManaManager.HasEnoughMana(MenuInit.JungleClearMP))
                {
                    JungleClear();
                }

                if (isLastHitMode && ManaManager.HasEnoughMana(MenuInit.LastHitMP))
                {
                    LastHit();
                }
            }
        }

        private static void LaneClear()
        {
            if (MenuInit.LaneClearQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range).ToList();

                if (minions.Any())
                {
                    if (MenuInit.LaneClearQFast)
                    {
                        var minion = minions.Where(x => x.IsValidTarget(Q.Range) && x.Health >= Player.Instance.GetAutoAttackDamage(x)).FirstOrDefault();

                        if (minion != null && minion.IsValidTarget(Q.Range))
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                    else
                    {
                        var minion = minions.Where(x => x.IsValidTarget(Q.Range) &&
                                                        x.Health < DamageCalculate.GetQDamage(x) &&
                                                        Prediction.Health.GetPrediction(x, Q.CastDelay) > 0).FirstOrDefault();

                        if (minion != null && minion.IsValidTarget(Q.Range))
                        {
                            Q.Cast(minion);
                            return;
                        }
                    }
                }
            }
        }

        private static void JungleClear()
        {
            if (MenuInit.JungleClearQ && Q.IsReady())
            {
                var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range, true).ToList();

                if (mobs.Any())
                {
                    Q.Cast(mobs.FirstOrDefault());
                }
            }
        }

        private static void LastHit()
        {
            if (MenuInit.LastHitQ && Q.IsReady())
            {
                var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, Q.Range, true).ToList();

                if (minions.Any())
                {
                    var minion = minions.Where(x => x.DistanceToPlayer() <= Q.Range &&
                                    x.DistanceToPlayer() > Player.Instance.GetAutoAttackRange(x) &&
                                    x.Health < DamageCalculate.GetQDamage(x) && Prediction.Health.GetPrediction(x, Q.CastDelay) > 0).FirstOrDefault();

                    if (minion != null && minion.IsValidTarget(Q.Range))
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (MenuInit.EnabledAntiMelee && E.IsReady() && Player.Instance.HealthPercent <= MenuInit.AntiMeleeHp)
            {
                if (sender != null && sender.IsEnemy && Args.Target != null && Args.Target.IsMe)
                {
                    if (sender.Type == Player.Instance.Type && sender.IsMelee)
                    {
                        E.Cast(Player.Instance.Extend(sender.Position, -E.Range));
                    }
                }
            }
        }

        private static void OnPreAttack(AttackableUnit target, Orbwalker.PreAttackArgs Args)
        {
            if (!isLaneClearMode || !ManaManager.SpellFarm || 
                !ManaManager.HasEnoughMana(MenuInit.LaneClearMP) || 
                !MenuInit.LaneClearW && !W.IsReady() || Player.Instance.CountEnemyChampionsInRange(800) > 0)
            {
                return;
            }

            if (Args.Target.Type != GameObjectType.obj_AI_Turret || Args.Target.Type != GameObjectType.obj_HQ || 
                Args.Target.Type != GameObjectType.obj_Turret)
            {
                return;
            }

            var ally = EntityManager.Heroes.Allies.Where(x => !x.IsDead && x.Health > 0 && x.DistanceToPlayer() <= W.Range);

            if (ally.Any())
            {
                W.Cast(ally.FirstOrDefault().Position);
                return;
            }
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs Args)
        {
            if (target == null || target.IsDead || target.Health <= 0)
            {
                return;
            }

            if (isComboMode && target.Type == GameObjectType.AIHeroClient)
            {
                AfterAACombo(target as AIHeroClient);
            }

            if ((isHarassMode || (isFarmMode && ManaManager.SpellHarass)) && target.Type == GameObjectType.AIHeroClient && ManaManager.HasEnoughMana(MenuInit.HarassMP) && 
                harassMenu["Harasstarget" + ((AIHeroClient)target).ChampionName.ToLower()].Cast<CheckBox>().CurrentValue)
            {
                AfterAAHarass(target as AIHeroClient);
            }

            if (isJungleClearMode && target.Type == GameObjectType.obj_AI_Minion && ManaManager.HasEnoughMana(MenuInit.JungleClearMP))
            {
                AfterAAJungle();
            }
        }

        private static void AfterAACombo(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.IsUnKillable() || !target.IsValidTarget() || target.Health <= 0)
            {
                return;
            }

            if (MenuInit.ComboQ && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.PredCast(target);
            }
            else if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.PredCast(target, false);
            }
        }

        private static void AfterAAHarass(AIHeroClient target)
        {
            if (target == null || target.IsDead || target.IsUnKillable() || !target.IsValidTarget() || target.Health <= 0)
            {
                return;
            }

            if (MenuInit.HarassQ && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.PredCast(target);
            }
            else if(MenuInit.HarassW && W.IsReady() && target.IsValidTarget(W.Range))
            {
                W.PredCast(target, false);
            }
        }

        private static void AfterAAJungle()
        {
            if (MenuInit.JungleClearQ && Q.IsReady())
            {
                var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range, true).ToList();

                if (mobs.Any())
                {
                    Q.Cast(mobs.FirstOrDefault());
                }
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs Args)
        {
            if (!MenuInit.EnabledAnti || !E.IsReady() || PlayerMana < EMana || 
                Player.Instance.HealthPercent > MenuInit.AntiGapCloserHp || sender == null || !sender.IsEnemy)
            {
                return;
            }

            if (miscMenu.GetBool("AntiGapCloserE" + sender.ChampionName.ToLower()))
            {
                if (Args.Target.IsMe || sender.DistanceToPlayer() <= 300 || Args.End.DistanceToPlayer() <= 250)
                {
                    E.Cast(Player.Instance.Extend(sender.Position, -E.Range));
                }
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (!Player.Instance.IsDead && !MenuGUI.IsChatOpen && !Chat.IsOpen)
            {
                if (MenuInit.DrawQ && Q.IsReady())
                {
                    Circle.Draw(Color.Blue, Q.Range, Player.Instance);
                }

                if (MenuInit.DrawW && W.IsReady())
                {
                    Circle.Draw(Color.Yellow, W.Range, Player.Instance);
                }

                if (MenuInit.DrawE && E.IsReady())
                {
                    Circle.Draw(Color.Red, E.Range, Player.Instance);
                }

                if (MenuInit.DrawR && R.IsReady())
                {
                    Circle.Draw(Color.Blue, R.Range, Player.Instance);
                }
            }
        }
    }
}