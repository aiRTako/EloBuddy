namespace MoonRiven_2.myCommon
{
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Enumerations;

    using SharpDX;

    using System;
    using System.Linq;
    using System.Collections.Generic;

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
            return Player.Instance.Distance(position);
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

        internal static bool IsValidRange(this AttackableUnit unit, float range = float.MaxValue, bool checkTeam = true, Vector3 from = new Vector3())
        {
            if (unit == null || !unit.IsValid || !unit.IsVisible || unit.IsDead || !unit.IsTargetable
                || unit.IsInvulnerable)

            {
                return false;
            }

            if (checkTeam && unit.Team == Player.Instance.Team)
            {
                return false;
            }

            if (unit.Name == "WardCorpse")
            {
                return false;
            }

            var @base = unit as Obj_AI_Base;

            return !(range < float.MaxValue)
                   || !(Vector2.DistanceSquared(
                       (from.To2D().IsValid() ? from : Player.Instance.ServerPosition).To2D(),
                       (@base?.ServerPosition ?? unit.Position).To2D()) > range * range);
        }
    }
}
