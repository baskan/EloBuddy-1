﻿using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using Settings = VodkaGalio.Config.MiscMenu;

namespace VodkaGalio.Modes
{
    public sealed class PermaActive : ModeBase
    {
        static Item HealthPotion;
        static Item CorruptingPotion;
        static Item RefillablePotion;
        static Item TotalBiscuit;

        static PermaActive()
        {
            HealthPotion = new Item(2003, 0);
            TotalBiscuit = new Item(2010, 0);
            CorruptingPotion = new Item(2033, 0);
            RefillablePotion = new Item(2031, 0);
        }

        public override bool ShouldBeExecuted()
        {
            return true;
        }

        public override void Execute()
        {
            if (Player.Instance.IsDead)
            {
                return;
            }
            // KillSteal
            foreach (var enemy in EntityManager.Heroes.Enemies.Where(e => e.IsEnemy && e.IsVisible && !e.IsDead && !e.IsZombie && !e.IsInvulnerable && e.Health > 0))
            {
                if (!isUlting() && Settings.KsQ && SpellManager.Q.IsReady() && Damages.QDamage(enemy) > enemy.Health && SpellManager.Q.IsInRange(enemy))
                {
                    if (enemy.HasBuffOfType(BuffType.SpellImmunity) || enemy.HasBuffOfType(BuffType.SpellShield))
                    {
                        continue;
                    }
                    var pred = Q.GetPrediction(enemy);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Debug.WriteChat("Casting Q in KillSteal on {0}, who has {1} HP", enemy.ChampionName, ""+enemy.Health);
                        Q.Cast(pred.CastPosition);
                        break;
                    }
                }

                if (!isUlting() && Settings.KsE && SpellManager.E.IsReady() && Damages.EDamage(enemy) > enemy.Health && SpellManager.E.IsInRange(enemy))
                {
                    if (enemy.HasBuffOfType(BuffType.SpellImmunity) || enemy.HasBuffOfType(BuffType.SpellShield))
                    {
                        continue;
                    }
                    var pred = E.GetPrediction(enemy);
                    if (pred.HitChance >= HitChance.High)
                    {
                        Debug.WriteChat("Casting E in KillSteal on {0}, who has {1} HP", enemy.ChampionName, "" + enemy.Health);
                        E.Cast(pred.CastPosition);
                        break;
                    }
                }

                if (!isUlting() && SpellManager.Ignite != null && Settings.KsIgnite && SpellManager.Ignite.IsReady() &&
                    Damages.IgniteDmg(enemy) > enemy.Health && SpellManager.Ignite.IsInRange(enemy))
                {
                    Debug.WriteChat("Casting Ignite in KillSteal on {0}, who has {1} HP", enemy.ChampionName, "" + enemy.Health);
                    SpellManager.Ignite.Cast(enemy);
                    break;
                }

                if (!isUlting() && Settings.KsR && SpellManager.R.IsReady() && Damages.RDamage(enemy) > enemy.Health && SpellManager.R.IsInRange(enemy))
                {
                    if (enemy.HasBuffOfType(BuffType.SpellImmunity) || enemy.HasBuffOfType(BuffType.SpellShield))
                    {
                        continue;
                    }
                    Debug.WriteChat("Casting R in KillSteal on {0}, who has {1} HP", enemy.ChampionName, "" + enemy.Health);
                    R.Cast();
                    break;
                }
            }

            // Automatic ult usage
            if (Settings.AutoR && R.IsReady() && !Player.Instance.IsRecalling())
            {
                var enemies = EntityManager.Heroes.Enemies.Where(e => !e.IsDead && !e.IsRecalling() && !e.IsZombie && !e.IsInvulnerable && R.IsInRange(e)).ToList();
                if (enemies.Count() >= Settings.AutoRMinEnemies && Player.Instance.HealthPercent >= Settings.AutoRMinHP)
                {
                    if (W.IsReady() && Player.Instance.Mana >= 160)
                    {
                        W.Cast(Player.Instance);
                    }
                    Debug.WriteChat("AutoCasting R, Enemies in range: {0}", ""+""+enemies.Count());
                    R.Cast();
                    return;
                }
            }

            // Potion manager
            if (Settings.Potion && !Player.Instance.IsInShopRange())
            {
                if (Player.Instance.HealthPercent <= Settings.potionMinHP && !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
                {
                    if (Item.HasItem(HealthPotion.Id) && Item.CanUseItem(HealthPotion.Id))
                    {
                        Debug.WriteChat("Using HealthPotion because below {0}% HP - have {1}% HP", ""+Settings.potionMinHP, String.Format("{0:##.##}", Player.Instance.HealthPercent));
                        HealthPotion.Cast();
                        return;
                    }
                    if (Item.HasItem(TotalBiscuit.Id) && Item.CanUseItem(TotalBiscuit.Id))
                    {
                        Debug.WriteChat("Using TotalBiscuitOfRejuvenation because below {0}% HP - have {1}% HP", "" + Settings.potionMinHP, String.Format("{0:##.##}", Player.Instance.HealthPercent));
                        TotalBiscuit.Cast();
                        return;
                    }
                    if (Item.HasItem(RefillablePotion.Id) && Item.CanUseItem(RefillablePotion.Id))
                    {
                        Debug.WriteChat("Using RefillablePotion because below {0}% HP - have {1}% HP", "" + Settings.potionMinHP, "" + String.Format("{0:##.##}", Player.Instance.HealthPercent));
                        RefillablePotion.Cast();
                        return;
                    }
                    if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                    {
                        Debug.WriteChat("Using CorruptingPotion because below {0}% HP - have {1}% HP", "" + Settings.potionMinHP, "" + String.Format("{0:##.##}", Player.Instance.HealthPercent));
                        CorruptingPotion.Cast();
                        return;
                    }
                }
                if (Player.Instance.ManaPercent <= Settings.potionMinMP && !(Player.Instance.HasBuff("RegenerationPotion") || Player.Instance.HasBuff("ItemMiniRegenPotion") || Player.Instance.HasBuff("ItemCrystalFlask") || Player.Instance.HasBuff("ItemDarkCrystalFlask")))
                {
                    if (Item.HasItem(CorruptingPotion.Id) && Item.CanUseItem(CorruptingPotion.Id))
                    {
                        Debug.WriteChat("Using HealthPotion because below {0}% MP - have {1}% MP", "" + Settings.potionMinMP, "" + String.Format("{0:##.##}", Player.Instance.ManaPercent));
                        CorruptingPotion.Cast();
                        return;
                    }
                }
            }
        }
    }
}