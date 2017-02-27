namespace MoonLucian.myCommon
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    using SharpDX;

    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Enumerations;

    internal static class Extensions
    {
        // Menu Extensions
        internal static void AddLine(this Menu mainMenu, string spellName)
        {
            mainMenu.AddGroupLabel(spellName + " Settings");
        }

        internal static void AddText(this Menu mainMenu, string disableName)
        {
            mainMenu.AddGroupLabel(disableName);
        }

        internal static void AddBool(this Menu mainMenu, string name, string disableName, bool isEnabled = true)
        {
            mainMenu.Add(name, new CheckBox(disableName, isEnabled));
        }

        internal static void AddSlider(this Menu mainMenu, string name, string disableName, int defalutValue = 0, int minValue = 0, int maxValue = 100)
        {
            mainMenu.Add(name, new Slider(disableName, defalutValue, minValue, maxValue));
        }

        internal static void AddList(this Menu mainMenu, string name, string disableName, string[] list, int defaultIndex = 0)
        {
            mainMenu.Add(name, new ComboBox(disableName, list, defaultIndex));
        }

        internal static void AddKey(this Menu mainMenu, string name, string disableName, KeyBind.BindTypes keyBindType, uint defaultKey1 = 27, bool isEnabled = false)
        {
            mainMenu.Add(name, new KeyBind(disableName, isEnabled, keyBindType, defaultKey1));
        }

        internal static bool GetBool(this Menu mainMenu, string name)
        {
            return mainMenu[name].Cast<CheckBox>().CurrentValue;
        }

        internal static bool GetKey(this Menu mainMenu, string name)
        {
            return mainMenu[name].Cast<KeyBind>().CurrentValue;
        }

        internal static int GetSlider(this Menu mainMenu, string name)
        {
            return mainMenu[name].Cast<Slider>().CurrentValue;
        }

        internal static int GetList(this Menu mainMenu, string name)
        {
            return mainMenu[name].Cast<ComboBox>().CurrentValue;
        }

        //Distance Extensions
        internal static float DistanceToPlayer(this Obj_AI_Base source)
        {
            return Player.Instance.Distance(source);
        }

        internal static float DistanceToPlayer(this Vector3 position)
        {
            return position.To2D().DistanceToPlayer();
        }

        internal static float DistanceToPlayer(this Vector2 position)
        {
            return ObjectManager.Player.Distance(position);
        }

        internal static float DistanceToMouse(this Obj_AI_Base source)
        {
            return Game.CursorPos.Distance(source.Position);
        }

        internal static float DistanceToMouse(this Vector3 position)
        {
            return position.To2D().DistanceToMouse();
        }

        internal static float DistanceToMouse(this Vector2 position)
        {
            return Game.CursorPos.Distance(position.To3D());
        }

        //Status Extensions
        internal static bool HaveShiled(this AIHeroClient target)
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

        internal static bool CanMoveMent(this AIHeroClient target)
        {
            return !(target.MoveSpeed < 50) && !target.IsStunned && !target.HasBuffOfType(BuffType.Stun) &&
                   !target.HasBuffOfType(BuffType.Fear) && !target.HasBuffOfType(BuffType.Snare) &&
                   !target.HasBuffOfType(BuffType.Knockup) && !target.HasBuff("Recall") &&
                   !target.HasBuffOfType(BuffType.Knockback)
                   && !target.HasBuffOfType(BuffType.Charm) && !target.HasBuffOfType(BuffType.Taunt) &&
                   !target.HasBuffOfType(BuffType.Suppression) && (!target.Spellbook.IsCharging
                                                                   || target.IsMoving) &&
                   !target.HasBuff("zhonyasringshield") && !target.HasBuff("bardrstasis");
        }

        internal static bool IsUnKillable(this AIHeroClient target)
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

            if (target.HasBuff("ChronoShift") && target.GetBuff("ChronoShift").EndTime - Game.Time > 0.3  && target.Health <= target.MaxHealth * 0.10f)
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

        //Spell Extensions
        internal static void HaveCollsion(this Spell.Skillshot spell, bool haveCollsion, int count = 0)
        {
            if (haveCollsion)
            {
                spell.AllowedCollisionCount = count;
            }
            else
            {
                spell.AllowedCollisionCount = int.MaxValue;
            }
        }

        internal static void PredCast(this Spell.Skillshot spell, AIHeroClient target, bool checkCollsion = true, HitChance hitchance = HitChance.High)
        {
            if (target == null || !target.IsValidTarget(spell.Range) || target.IsUnKillable() || target.Health <= 0)
            {
                return;
            }

            var pred = spell.GetPrediction(target);

            if (checkCollsion)
            {
                if (pred.HitChance >= hitchance && !pred.CollisionObjects.Any())
                {
                    spell.Cast(pred.CastPosition);
                }
            }
            else
            {
                if (pred.HitChance >= hitchance)
                {
                    spell.Cast(pred.CastPosition);
                }
            }
        }

        internal static int CountHits(this Spell.Skillshot spell, List<Obj_AI_Minion> units, Vector3 castPosition)
        {
            var points = units.Select(unit => spell.GetPrediction(unit).UnitPosition).ToList();
            return spell.CountHits(points, castPosition);
        }

        internal static int CountHits(this Spell.Skillshot spell, List<Vector3> points, Vector3 castPosition)
        {
            return points.Count(point => spell.WillHit(point, castPosition));
        }

        internal static bool WillHit(this Spell.Skillshot spell, Obj_AI_Base unit, Vector3 castPosition, int extraWidth = 0)
        {
            var unitPosition = spell.GetPrediction(unit);
            return unitPosition.HitChance >= HitChance.High
                   && spell.WillHit(unitPosition.UnitPosition, castPosition, extraWidth);
        }

        internal static bool WillHit(this Spell.Skillshot spell, Vector3 point, Vector3 castPosition, int extraWidth = 0)
        {
            if (point.To2D().Distance(castPosition.To2D(), Player.Instance.Position.To2D(), true, true) < Math.Pow(spell.Width + extraWidth, 2))
            {
                return true;
            }

            return false;
        }

        //Vector Extensions
        internal static Vector3 Extend(this Obj_AI_Base v, Vector3 to, float distance)
        {
            return v.Position + distance * (to - v.Position).Normalized();
        }
    }
}
