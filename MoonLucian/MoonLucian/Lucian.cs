namespace MoonLucian
{
    using myCommon;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Enumerations;

    using SharpDX;

    using System;
    using System.Linq;

    internal class Lucian : Logic
    {
        internal static void Init()
        {
            //Draw the Chat
            Chat.Print("Moon" + Player.Instance.ChampionName + ": Load! Credit: NightMoon", System.Drawing.Color.LightSteelBlue);

            //Init Lucian SkillSlot
            Q = new Spell.Targeted(SpellSlot.Q, 650, DamageType.Physical);
            QExtend = new Spell.Skillshot(SpellSlot.Q, 900, SkillShotType.Linear, 350, int.MaxValue, 25, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 1000, SkillShotType.Linear, 300, 1600, 80, DamageType.Magical);
            E = new Spell.Skillshot(SpellSlot.E, 425, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 100, 2500, 110, DamageType.Physical);
            R1 = new Spell.Skillshot(SpellSlot.R, 1200, SkillShotType.Linear, 100, 2500, 110, DamageType.Physical);

            QExtend.HaveCollsion(false);
            W.HaveCollsion(true);
            R.HaveCollsion(true);
            R1.HaveCollsion(false);

            //Init Youmuu
            Youmuus = new Item(ItemId.Youmuus_Ghostblade);

            //Init Lucian Menu
            MenuInit.Init();

            //Init Lucian Events
            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Orbwalker.OnPostAttack += OnPostAttack;
            Gapcloser.OnGapcloser += OnGapcloser;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnTick(EventArgs Args)
        {
            if (Player.Instance.IsDead || Player.Instance.IsRecalling())
            {
                havePassive = false;
                return;
            }

            if (TickCount - lastCastTime >= 3100)
            {
                havePassive = false;
            }

            SetSpellMana();

            if (Player.Instance.HasBuff("LucianR"))
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                return;
            }

            if (Player.Instance.HasBuff("LucianR") || Player.Instance.IsDashing())
            {
                return;
            }

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

            if (R.Level > 0 && R.IsReady() && Player.Instance.Mana > RMana + QMana + WMana + EMana)
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
            if (MenuInit.SemiR)
            {
                SemiRLogic();
            }
        }

        private static void SemiRLogic()
        {
            var select = TargetSelector.SelectedTarget;
            var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

            if (select != null && select.IsValidTarget(R.Range))
            {
                R1.Cast(select);
                return;
            }

            if (select == null && target != null && !target.HaveShiled() && target.IsValidTarget(R.Range))
            {
                R1.Cast(target);
            }
        }

        private static void KillSteal()
        {
            if (MenuInit.KillStealQ && Q.IsReady() && PlayerMana > QMana + EMana)
            {
                foreach (var target in EntityManager.Heroes.Enemies
                    .Where(x => x.IsValidTarget(QExtend.Range) && !x.IsUnKillable() && x.Health < DamageCalculate.GetQDamage(x)))
                {
                    QLogic(target);
                    return;
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
        }

        private static void Combo()
        {
            if (MenuInit.ComboEDash && E.IsReady())
            {
                var target = TargetSelector.GetTarget(Player.Instance.GetAutoAttackRange() + E.Range, DamageType.Physical);

                if (target.IsValidTarget(Player.Instance.GetAutoAttackRange() + E.Range) &&
                    !target.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                {
                    ELogic(target, 0);
                }
            }

            if (MenuInit.ComboQExtend && QExtend.IsReady() && !Player.Instance.IsDashing() && !havePassive && !havePassiveBuff)
            {
                var target = TargetSelector.GetTarget(QExtend.Range, DamageType.Physical);

                if (target.IsValidTarget(QExtend.Range) && !target.IsValidTarget(Q.Range))
                {
                    QLogic(target);
                }
            }

            if (MenuInit.ComboR && R.IsReady())
            {
                var target = TargetSelector.GetTarget(R.Range, DamageType.Physical);

                if (target.IsValidTarget(R.Range) && !target.IsUnKillable() && !Player.Instance.IsUnderEnemyturret() &&
                    !target.IsValidTarget(Player.Instance.GetAutoAttackRange()))
                {
                    if (EntityManager.Heroes.Enemies.Any(x => x.NetworkId != target.NetworkId && x.Distance(target) <= 550))
                    {
                        return;
                    }

                    var rAmmo = new float[] { 20, 25, 30 }[Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level - 1];
                    var rDMG = DamageCalculate.GetRDamage(target) * rAmmo;

                    if (target.Health + target.HPRegenRate * 3 < rDMG)
                    {
                        if (target.DistanceToPlayer() <= 800 && target.Health < rDMG * 0.6)
                        {
                            R.Cast(target);
                            return;
                        }

                        if (target.DistanceToPlayer() <= 1000 && target.Health < rDMG * 0.4)
                        {
                            R.Cast(target);
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
                    foreach(var target in EntityManager.Heroes.Enemies.Where(x => x.IsValidTarget(QExtend.Range) && 
                                          harassMenu["Harasstarget" + x.ChampionName.ToLower()].Cast<CheckBox>().CurrentValue))
                    {
                        if (target.IsValidTarget(QExtend.Range))
                        {
                            QLogic(target, MenuInit.HarassQExtend);
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
            var range = MenuInit.HarassQExtend && QExtend.IsReady()
                ? QExtend.Range
                : MenuInit.HarassW && W.IsReady() ? W.Range : MenuInit.HarassQ && Q.IsReady() ? Q.Range : 0;

            if (ManaManager.SpellHarass && ManaManager.HasEnoughMana(MenuInit.HarassMP) && range > 0 && 
                Player.Instance.CountEnemyChampionsInRange(range) > 0)
            {
                Harass();
            }
            else if (ManaManager.SpellFarm)
            {
                if (isLaneClearMode && ManaManager.HasEnoughMana(MenuInit.LaneClearMP))
                {
                    LaneClear();
                }
            }
        }

        private static void LaneClear()
        {
            if (TickCount - lastCastTime < 600 || havePassiveBuff)
            {
                return;
            }

            if (MenuInit.LaneClearQ && Q.IsReady())
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.Position, Q.Range).ToList();

                if (minions.Any())
                {
                    var minion = minions.FirstOrDefault();
                    var qExminions =
                        EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                            Player.Instance.Position, QExtend.Range).ToList();

                    if (minion != null && QExtend.CountHits(qExminions, Player.Instance.Extend(minion.Position, 900)) >= 2)
                    {
                        Q.Cast(minion);
                        return;
                    }
                }
            }

            if (MenuInit.LaneClearW && W.IsReady())
            {
                var minions =
                    EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy,
                        Player.Instance.Position, W.Range).ToList();

                if (minions.Count > 2)
                {
                    var pred = W.GetBestCircularCastPosition(minions);

                    W.Cast(pred.CastPosition);
                }
            }
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                return;
            }

            if (Args.Animation == "Spell1" || Args.Animation == "Spell2")
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs Args)
        {
            if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
            {
                havePassive = true;
                lastCastTime = TickCount;
            }

            if (Args.Slot == SpellSlot.E && !Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.None))
            {
                Orbwalker.ResetAutoAttack();
            }

            if (Args.Slot == SpellSlot.R && Youmuus.IsOwned() && Youmuus.IsReady())
            {
                Youmuus.Cast();
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

            if (sender != null && sender.IsMe)
            {
                if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
                {
                    havePassive = true;
                    lastCastTime = TickCount;
                }
            }
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs Args)
        {
            havePassive = false;

            if (target == null || target.IsDead || target.Health <= 0)
            {
                return;
            }

            if (isComboMode && target.Type == GameObjectType.AIHeroClient)
            {
                if (Youmuus.IsOwned() && Youmuus.IsReady())
                {
                    Youmuus.Cast();
                }

                AfterAACombo(target as AIHeroClient);
            }

            if (isLaneClearMode && ManaManager.SpellFarm && ManaManager.HasEnoughMana(MenuInit.TurretClearMP) && 
                (target.Type == GameObjectType.obj_AI_Turret ||
                target.Type == GameObjectType.obj_Turret || target.Type == GameObjectType.obj_HQ ||
                target.Type == GameObjectType.obj_BarracksDampener))
            {
                AfterAALane(target);
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

            if (MenuInit.ComboEReset && E.IsReady())
            {
                ELogic(target, 1);
            }
            else if (MenuInit.ComboQ && Q.IsReady() && target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            else if (MenuInit.ComboW && W.IsReady() && target.IsValidTarget(W.Range))
            {
                if (MenuInit.ComboWFast)
                {
                    W.Cast(target.Position);
                }
                else
                {
                    W.PredCast(target);
                }
            }
        }

        private static void AfterAALane(AttackableUnit target)
        {
            if (target.Type != GameObjectType.obj_AI_Turret && target.Type != GameObjectType.obj_Turret &&
                target.Type != GameObjectType.obj_HQ && target.Type != GameObjectType.obj_BarracksDampener)
            {
                return;
            }

            if (MenuInit.TurretClearE && E.IsReady())
            {
                E.Cast(Player.Instance.Extend(Game.CursorPos, 130));
            }
            else if (MenuInit.TurretClearW && W.IsReady())
            {
                W.Cast(Game.CursorPos);
            }
        }

        private static void AfterAAJungle()
        {
            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, Q.Range).ToList();

            if (mobs.Any())
            {
                if (MenuInit.JungleClearE && E.IsReady())
                {
                    E.Cast(Player.Instance.Extend(Game.CursorPos, 130));
                }
                else if (MenuInit.JungleClearQ && Q.IsReady())
                {
                    Q.Cast(mobs.FirstOrDefault());
                }
                else if (MenuInit.JungleClearW && W.IsReady())
                {
                    W.Cast(mobs.FirstOrDefault());
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

                if (MenuInit.DrawQExtend && QExtend.IsReady())
                {
                    Circle.Draw(Color.Blue, QExtend.Range, Player.Instance);
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

        private static void QLogic(AIHeroClient target, bool useExtendQ = true)
        {
            if (!Q.IsReady() || target == null || target.IsDead || target.IsUnKillable())
            {
                return;
            }

            if (target.IsValidTarget(Q.Range))
            {
                Q.Cast(target);
            }
            else if (target.IsValidTarget(QExtend.Range) && useExtendQ)
            {
                var collisions = EntityManager.MinionsAndMonsters.EnemyMinions.Where(x => x.IsValidTarget(Q.Range)).ToList();

                if (!collisions.Any())
                {
                    return;
                }

                foreach (var minion in collisions)
                {
                    var qPred = QExtend.GetPrediction(target);
                    var qPloygon = new Geometry.Polygon.Rectangle(Player.Instance.Position, Player.Instance.Position.Extend(minion.Position, QExtend.Range).To3D(), QExtend.Width);

                    if (qPloygon.IsInside(qPred.UnitPosition.To2D()))
                    {
                        Q.Cast(minion);
                        break;
                    }
                }
            }
        }

        private static void ELogic(AIHeroClient target, int count)
        {
            if (!E.IsReady() || target == null || target.IsDead || target.IsUnKillable())
            {
                return;
            }

            switch(count)
            {
                case 0:
                    {
                        if (target.DistanceToPlayer() <= Player.Instance.GetAutoAttackRange(target) || 
                            target.DistanceToPlayer () > Player.Instance.GetAutoAttackRange(target) + E.Range)
                        {
                            return;
                        }

                        var dashPos = Player.Instance.Extend(Game.CursorPos, E.Range);

                        if ((NavMesh.GetCollisionFlags(dashPos).HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(dashPos).HasFlag(CollisionFlags.Building)) &&
                            MenuInit.ComboEWall)
                        {
                            return;
                        }

                        if (dashPos.CountEnemyChampionsInRange(500) >= 3 && dashPos.CountAllyChampionsInRange(400) < 3 &&
                            MenuInit.ComboESafe)
                        {
                            return;
                        }

                        if (Player.Instance.DistanceToMouse() > Player.Instance.GetAutoAttackRange() &&
                            target.Distance(dashPos) < Player.Instance.GetAutoAttackRange())
                        {
                            E.Cast(dashPos);
                        }
                    }
                    break;
                case 1:
                {
                        var dashRange = comboMenu["ComboEShort"].Cast<CheckBox>().CurrentValue
                            ? (Player.Instance.DistanceToMouse() > Player.Instance.GetAutoAttackRange() ? E.Range : 130)
                            : E.Range;
                        var dashPos = Player.Instance.Extend(Game.CursorPos, dashRange);

                        if ((NavMesh.GetCollisionFlags(dashPos).HasFlag(CollisionFlags.Wall) ||
                            NavMesh.GetCollisionFlags(dashPos).HasFlag(CollisionFlags.Building)) &&
                            MenuInit.ComboEWall)
                        {
                            return;
                        }

                        if (dashPos.CountEnemyChampionsInRange(500) >= 3 && dashPos.CountAllyChampionsInRange(400) < 3 &&
                            MenuInit.ComboESafe)
                        {
                            return;
                        }

                        E.Cast(dashPos);
                    }
                    break;
            }
        }
    }
}