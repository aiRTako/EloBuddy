namespace MoonRiven_2
{
    using myCommon;

    using System;

    using SharpDX;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Rendering;
    using EloBuddy.SDK.Enumerations;

    internal class Riven : Logic
    {
        internal static void Init()
        {
            //Draw the Chat
            Chat.Print("Moon" + Player.Instance.ChampionName + ": Load! Credit: NightMoon", System.Drawing.Color.LightSteelBlue);

            //Init Riven SkillSlot
            Q = new Spell.Skillshot(SpellSlot.Q, 325, SkillShotType.Circular, 250, 2200, 100, DamageType.Physical);
            W = new Spell.Active(SpellSlot.W, 260);
            E = new Spell.Skillshot(SpellSlot.E, 312, SkillShotType.Linear, 100);
            R = new Spell.Active(SpellSlot.R, 600);
            R1 = new Spell.Skillshot(SpellSlot.R, 900, SkillShotType.Cone, 250, 1600, 40, DamageType.Physical);

            //Init Riven SummonerSpell
            Ignite = Player.Instance.GetSpellSlotFromName("SummonerDot");
            Flash = Player.Instance.GetSpellSlotFromName("SummonerFlash");

            //Init Riven Menu
            MenuInit.Init();

            //Init Riven Events
            Game.OnUpdate += OnTick;
            Obj_AI_Base.OnPlayAnimation += OnPlayAnimation;
            Spellbook.OnCastSpell += OnCastSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Obj_AI_Base.OnSpellCast += OnSpellCast;
            Orbwalker.OnPostAttack += OnPostAttack;
            Gapcloser.OnGapcloser += OnGapcloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Drawing.OnDraw += OnDraw;
        }

        private static void OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            var time = 0;

            switch (Args.Animation)
            {
                case "Spell1a": //Q1
                    time = 351;
                    qStack = 1;
                    lastQTime = TickCount;
                    break;
                case "Spell1b": //Q2
                    time = 351;
                    qStack = 2;
                    lastQTime = TickCount;
                    break;
                case "Spell1c": //Q3
                    time = 451;
                    qStack = 0;
                    lastQTime = TickCount;
                    break;
                //case "Spell2": //W
                //    time = 50;
                //    break;
                //case "Spell4a": //R1
                //    time = 50;
                //    break;
                //case "Spell4b": //R2
                //    time = 180;
                //    break;
                default:
                    time = 0;
                    break;
            }

            if (isFleeMode)
                return;

            if (time > 0)
            {
                if (MenuInit.manualCancel || !isNoneMode)
                {
                    if (MenuInit.manualCancelPing)
                    {
                        if (time - Game.Ping > 0)
                        {
                            Core.DelayAction(Cancel, time - Game.Ping);
                        }
                        else
                        {
                            Core.DelayAction(Cancel, 1);
                        }
                    }
                    else
                    {
                        Core.DelayAction(Cancel, time);
                    }
                }
            }
        }

        private static void Cancel()
        {
            //Player.DoEmote(Emote.Dance);
            Orbwalker.ResetAutoAttack();

            if (Orbwalker.GetTarget() != null && !Orbwalker.GetTarget().IsDead)
            {
                Orbwalker.OrbwalkTo(Player.Instance.Position.Extend(Orbwalker.GetTarget().Position, +10).To3D());
            }
            else
            {
                Orbwalker.OrbwalkTo(Game.CursorPos);
            }
        }

        private static void OnTick(EventArgs Args)
        {
            if (Player.Instance.IsDead)
            {
                myTarget = null;
                qStack = 0;
                return;
            }

            if (Player.Instance.IsRecalling())
                return;

            Mode.None.Init();

            if (isComboMode)
            {
                if (MenuInit.BurstEnabledKey)
                {
                    Mode.Burst.InitTick();
                }
                else
                {
                    Mode.Combo.InitTick();
                }
            }

            if (isHarassMode)
            {
                Mode.Harass.InitTick();
            }

            if (isLaneClearMode && ManaManager.SpellFarm)
            {
                Mode.LaneClear.InitTick();
            }

            if (isJungleClearMode && ManaManager.SpellFarm)
            {
                Mode.JungleClear.InitTick();
            }

            if (isFleeMode)
            {
                Mode.Flee.InitTick();
            }

            Mode.KillSteal.InitTick();
        }

        private static void OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs Args)
        {
            if (Args.Slot == SpellSlot.Q || Args.Slot == SpellSlot.W || Args.Slot == SpellSlot.E)
            {
                lastCastTime = TickCount;
            }
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (sender.IsMe)
            {
                if (isComboMode)
                {
                    if (MenuInit.BurstEnabledKey)
                    {
                        Mode.Burst.InitSpellCast(Args);
                    }
                    else
                    {
                        Mode.Combo.InitSpellCast(Args);
                    }
                }
            }

            if (sender.IsEnemy && sender.Type == GameObjectType.AIHeroClient && MenuInit.evadeELogic && E.IsReady())
            {
                EvadeLogic(sender as AIHeroClient, Args);
            }
        }

        private static void EvadeLogic(AIHeroClient target, GameObjectProcessSpellCastEventArgs Args)
        {
            if (eMenu[target.ChampionName + "Skill" + Args.SData.Name] == null || !eMenu.GetBool(target.ChampionName + "Skill" + Args.SData.Name))
            {
                return;
            }

            if (Args.SData.TargettingType == SpellDataTargetType.Unit && Args.Target != null && Args.Target.IsMe)
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.E, Game.CursorPos);
            }
            //else if (Args.SData.TargettingType != SpellDataTargetType.Unit)
            //{
            //    //Use the Evade Logic?
            //}
        }

        private static void OnSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
                return;

            if (isComboMode)
            {
                if (MenuInit.BurstEnabledKey)
                {
                    Mode.Burst.InitSpellCast(Args);
                }
                else
                {
                    Mode.Combo.InitSpellCast(Args);
                }
            }
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs Args)
        {
            Orbwalker.ForcedTarget = null;

            if (target == null || target.IsAlly)
                return;

            if (isComboMode && target.Type == GameObjectType.AIHeroClient)
            {
                if (MenuInit.BurstEnabledKey)
                {
                    Mode.Burst.InitAfterAttack(target);
                }
                else
                {
                    Mode.Combo.InitAfterAttack(target);
                }
            }

            if (isHarassMode && target.Type == GameObjectType.AIHeroClient)
            {
                Mode.Harass.InitAfterAttack(target);
            }

            if (isLaneClearMode && ManaManager.SpellFarm)
            {
                Mode.LaneClear.InitAfterAttack(target);
            }

            if (isJungleClearMode && target.Team == GameObjectTeam.Neutral && ManaManager.SpellFarm)
            {
                Mode.JungleClear.InitAfterAttack(target);
            }
        }

        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs Args)
        {
            if (!MenuInit.AntiGapcloserW || !W.IsReady() || sender == null || !sender.IsEnemy || !sender.IsValidTarget())
            {
                return;
            }

            if (Args.Target.IsMe || sender.DistanceToPlayer() <= W.Range || Args.End.DistanceToPlayer() <= W.Range)
            {
                if (!sender.HaveShiled())
                    Player.Instance.Spellbook.CastSpell(SpellSlot.W);
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base sender, Interrupter.InterruptableSpellEventArgs Args)
        {
            if (!MenuInit.InterruptW || sender == null || !sender.IsEnemy || Args == null || 
                !W.IsReady() || sender.Type != GameObjectType.AIHeroClient)
            {
                return;
            }

            var target = (AIHeroClient) sender;

            if (target.IsValidRange(W.Range) && Args.DangerLevel >= DangerLevel.High && !target.HaveShiled())
            {
                Player.Instance.Spellbook.CastSpell(SpellSlot.W);
            }
        }

        private static void OnDraw(EventArgs Args)
        {
            if (Player.Instance.IsDead || MenuGUI.IsChatOpen || Chat.IsOpen)
                return;

            if (MenuInit.DrawW && W.IsReady())
            {
                Circle.Draw(Color.Yellow, W.Range, Player.Instance);
            }

            if (MenuInit.DrawE && E.IsReady())
            {
                Circle.Draw(Color.Red, E.Range, Player.Instance);
            }

            if (MenuInit.DrawR1 && Player.GetSpell(SpellSlot.R).IsReady)
            {
                Circle.Draw(Color.Orange, R1.Range, Player.Instance);
            }

            if (MenuInit.DrawR && Player.GetSpell(SpellSlot.R).Level > 0)
            {
                var MePos = Drawing.WorldToScreen(Player.Instance.Position);

                Drawing.DrawText(MePos[0] - 57, MePos[1] + 68, System.Drawing.Color.FromArgb(242, 120, 34),
                    "Combo R1:" + (MenuInit.ComboR1 ? "On" : "Off"));
            }

            if (MenuInit.DrawBurst && Player.GetSpell(SpellSlot.R).Level > 0)
            {
                var MePos = Drawing.WorldToScreen(Player.Instance.Position);
                string str;

                if (MenuInit.BurstEnabledKey)
                {
                    str = !R.IsReady() ? "Not Ready" : "On";
                }
                else
                {
                    str = "Off";
                }

                Drawing.DrawText(MePos[0] - 57, MePos[1] + 88, System.Drawing.Color.FromArgb(242, 120, 34),
                    "Burst Fire:" + str);
            }
        }
    }
}
