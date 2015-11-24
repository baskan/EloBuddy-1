﻿using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = VodkaDrMundo.Config.ModesMenu.Harass;
using SettingsCombo = VodkaDrMundo.Config.ModesMenu.Combo;
using SettingsHealth = VodkaDrMundo.Config.HealthManagerMenu;

namespace VodkaDrMundo.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            if (Settings.UseQ && Q.IsReady() && PlayerHealth >= SettingsHealth.MinQHealth)
            {
                var target = TargetSelector.GetTarget(SettingsCombo.MaxQDistance, DamageType.Magical);
                if (target == null)
                {
                    return;
                }
                var pred = Q.GetPrediction(target);
                if (pred.HitChance >= HitChance.Medium)
                {
                    Q.Cast(pred.CastPosition);
                    Debug.WriteChat("Casting Q in Harass, Target: {0}, Distance: {0}", target.ChampionName, "" + Player.Instance.Distance(target));
                }
            }
            if (Settings.UseW && W.IsReady() && !WActive && PlayerHealth >= SettingsHealth.MinWHealth)
            {
                var enemy =
                    EntityManager.Heroes.Enemies
                        .FirstOrDefault(e => !e.IsDead && e.Health > 0 && e.IsVisible && e.IsValidTarget() && _Player.Distance(e) < W.Range);
                if (enemy != null)
                {
                    W.Cast();
                    Debug.WriteChat("Casting W in Combo");
                }
            }
            if (Settings.UseW && W.IsReady() && !WActive)
            {
                var enemies = EntityManager.Heroes.Enemies
                        .Count(e => !e.IsDead && e.Health > 0 && e.IsVisible && e.IsValidTarget() && _Player.Distance(e) < W.Range + 200);
                W.Cast();
                Debug.WriteChat("Turning W off in Harass, because enemy moved out of range.");
            }
        }
    }
}
