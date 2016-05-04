using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
using LeagueSharp.Common.Data;
using ItemData = LeagueSharp.Common.Data.ItemData;
using FioraProject.Evade;

namespace FioraProject
{
    using static Program;
    using static GetTargets;
    using EloBuddy;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Menu;
    #region OtherSkill
    internal class OtherSkill
    {
        private static readonly List<SpellData> Spells = new List<SpellData>();

        // riven variables
        private static int RivenDashTick;
        private static int RivenQ3Tick;
        private static Vector2 RivenDashEnd = new Vector2();
        private static float RivenQ3Rad = 150;

        // fizz variables
        private static Vector2 FizzFishEndPos = new Vector2();
        private static GameObject FizzFishChum = null;
        private static int FizzFishChumStartTick;
        public static Menu evadeMenu;

        internal static void Init()
        {
            LoadSpellData();
            Spells.RemoveAll(i => !HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase)));
            evadeMenu = Program.Menu.AddSubMenu("Block Other skils", "EvadeOthers");
            {
                evadeMenu.Add("W", new CheckBox("Use W"));
                foreach (var spell in Spells.Where(i => HeroManager.Enemies.Any(a => string.Equals(a.ChampionName, i.ChampionName, StringComparison.InvariantCultureIgnoreCase))))
                {
                    evadeMenu.Add(spell.ChampionName + spell.Slot, new CheckBox(spell.ChampionName + " (" + (spell.Slot == SpellSlot.Unknown ? "Passive" : spell.Slot.ToString()) + ")", false));
                }
            }

            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            AIHeroClient.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            CustomEvents.Unit.OnDash += Unit_OnDash;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
                return;
            var caster = missile.SpellCaster as AIHeroClient;
            if (!(caster is AIHeroClient) || caster.Team == Player.Team)
                return;
            if (missile.SData.Name == "FizzMarinerDoomMissile")
            {
                FizzFishEndPos = missile.Position.LSTo2D();
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Fizz_UltimateMissile_Orbit.troy" && FizzFishEndPos.IsValid()
                && sender.Position.LSTo2D().LSDistance(FizzFishEndPos) <= 300)
            {
                FizzFishChum = sender;
                if (Utils.GameTimeTickCount >= FizzFishChumStartTick + 5000)
                    FizzFishChumStartTick = Utils.GameTimeTickCount;
            }
        }

        private static void Unit_OnDash(Obj_AI_Base sender, Dash.DashItem args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            // riven dash
            if (caster.ChampionName == "Riven" && getCheckBoxItem(evadeMenu, "Riven" + SpellSlot.Q))
            {
                RivenDashTick = Utils.GameTimeTickCount;
                RivenDashEnd = args.EndPos;
            }
        }

        private static void AIHeroClient_OnPlayAnimation(Obj_AI_Base sender, GameObjectPlayAnimationEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            // riven Q3
            if (caster.ChampionName == "Riven" && getCheckBoxItem(evadeMenu, "Riven" + SpellSlot.Q) && args.Animation.ToLower() == "spell1c")
            {
                RivenQ3Tick = Utils.GameTimeTickCount;
                if (caster.HasBuff("RivenFengShuiEngine"))
                    RivenQ3Rad = 150;
                else
                    RivenQ3Rad = 225;
            }
            // others
            var spellDatas = Spells.Where(i => caster.ChampionName.ToLowerInvariant() == i.ChampionName.ToLowerInvariant() && evadeMenu[i.ChampionName + i.Slot] != null ? getCheckBoxItem(evadeMenu, i.ChampionName + i.Slot) : false);
            if (!spellDatas.Any())
            {
                return;
            }
            foreach (var spellData in spellDatas)
            {
                //reksaj W
                if (!Player.HasBuff("reksaiknockupimmune") && spellData.ChampionName == "Reksai"
                    && spellData.Slot == SpellSlot.W && args.Animation == "Spell2_knockup")// chua test
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= Player.BoundingRadius + caster.BoundingRadius + caster.AttackRange)
                        SolveInstantBlock();
                    return;
                }
            }
        }
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        private static void AIHeroClient_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            var spellDatas =
               Spells.Where(
                   i =>
                   caster.ChampionName.ToLowerInvariant() == i.ChampionName.ToLowerInvariant()
                   && evadeMenu[i.ChampionName + i.Slot] != null ? getCheckBoxItem(evadeMenu, i.ChampionName + i.Slot) : false);
            if (!spellDatas.Any())
            {
                return;
            }
            foreach (var spellData in spellDatas)
            {
                // auto attack
                if (args.SData.IsAutoAttack() && args.Target != null && args.Target.IsMe)
                {
                    if (spellData.ChampionName == "Jax" && spellData.Slot == SpellSlot.W && caster.HasBuff("JaxEmpowerTwo"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.ChampionName == "Yorick" && spellData.Slot == SpellSlot.Q && caster.HasBuff("YorickSpectral"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.ChampionName == "Poppy" && spellData.Slot == SpellSlot.Q && caster.HasBuff("PoppyDevastatingBlow"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.ChampionName == "Rengar" && spellData.Slot == SpellSlot.Q
                        && (caster.HasBuff("rengarqbase") || caster.HasBuff("rengarqemp")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.ChampionName == "Nautilus" && spellData.Slot == SpellSlot.Unknown
                        && (!Player.HasBuff("nautiluspassivecheck")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.ChampionName == "Udyr" && spellData.Slot == SpellSlot.E && caster.HasBuff("UdyrBearStance")
                        && (Player.HasBuff("udyrbearstuncheck")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    return;
                }
                // aoe
                if (spellData.ChampionName == "Riven" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)// chua test
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= Player.BoundingRadius + caster.BoundingRadius + caster.AttackRange)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.ChampionName == "Diana" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E)// chua test
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= Player.BoundingRadius + 450)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.ChampionName == "Maokai" && spellData.Slot == SpellSlot.R && args.SData.Name == "maokaidrain3toggle")
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= Player.BoundingRadius + 575)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.ChampionName == "Kalista" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E)
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= 950
                        && Player.HasBuff("kalistaexpungemarker"))
                        SolveInstantBlock();
                    return;
                }
                if (spellData.ChampionName == "Kennen" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)// chua test
                {
                    if (Player.Position.LSTo2D().LSDistance(caster.Position.LSTo2D())
                        <= 800
                        && Player.HasBuff("kennenmarkofstorm") && Player.GetBuffCount("kennenmarkofstorm") == 2)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.ChampionName == "Azir" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R)// chua test
                {
                    Vector2 start = caster.Position.LSTo2D().LSExtend(args.End.LSTo2D(), -300);
                    Vector2 end = start.LSExtend(caster.Position.LSTo2D(), 750);
                    Render.Circle.DrawCircle(start.To3D(), 50, Color.Red);
                    Render.Circle.DrawCircle(end.To3D(), 50, Color.Red);
                    float width = caster.Level >= 16 ? 125 * 6 / 2 :
                                caster.Level >= 11 ? 125 * 5 / 2 :
                                125 * 4 / 2;
                    FioraProject.Evade.Geometry.Rectangle Rect = new FioraProject.Evade.Geometry.Rectangle(start, end, width);
                    var Poly = Rect.ToPolygon();
                    if (!Poly.IsOutside(Player.Position.LSTo2D()))
                    {
                        SolveInstantBlock();
                    }
                    return;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.HasBuff("vladimirhemoplaguedebuff") && HeroManager.Enemies.Any(x => x.ChampionName == "Vladimir")
                && getCheckBoxItem(evadeMenu, "Vladimir" + SpellSlot.R))
            {
                var buff = Player.GetBuff("vladimirhemoplaguedebuff");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("zedrdeathmark") && HeroManager.Enemies.Any(x => x.ChampionName == "Zed")
                && getCheckBoxItem(evadeMenu, "Zed" + SpellSlot.R))
            {
                var buff = Player.GetBuff("zedrdeathmark");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("tristanaechargesound") && HeroManager.Enemies.Any(x => x.ChampionName == "Tristana")
                && getCheckBoxItem(evadeMenu, "Tristana" + SpellSlot.E))
            {
                var buff = Player.GetBuff("tristanaechargesound ");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("SoulShackles") && HeroManager.Enemies.Any(x => x.ChampionName == "Morgana")
                && getCheckBoxItem(evadeMenu, "Morgana" + SpellSlot.R))
            {
                var buff = Player.GetBuff("SoulShackles");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("NocturneUnspeakableHorror") && HeroManager.Enemies.Any(x => x.ChampionName == "Nocturne")
                && getCheckBoxItem(evadeMenu, "Nocturne" + SpellSlot.E))
            {
                var buff = Player.GetBuff("NocturneUnspeakableHorror");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("karthusfallenonetarget") && HeroManager.Enemies.Any(x => x.ChampionName == "Karthus")
                && getCheckBoxItem(evadeMenu, "Karthus" + SpellSlot.R))
            {
                var buff = Player.GetBuff("karthusfallenonetarget");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("KarmaSpiritBind") && HeroManager.Enemies.Any(x => x.ChampionName == "Karma")
                && getCheckBoxItem(evadeMenu, "Karma" + SpellSlot.R))
            {
                var buff = Player.GetBuff("KarmaSpiritBind");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if ((Player.HasBuff("LeblancSoulShackle") || (Player.HasBuff("LeblancShoulShackleM")))
                && HeroManager.Enemies.Any(x => x.ChampionName == "Karma")
                && (getCheckBoxItem(evadeMenu, "LeBlanc" + SpellSlot.R) || getCheckBoxItem(evadeMenu, "LeBlanc" + SpellSlot.E)))
            {
                var buff = Player.GetBuff("LeblancSoulShackle");
                if (buff != null)
                {
                    SolveBuffBlock(buff);
                    return;
                }
                var buff2 = Player.GetBuff("LeblancShoulShackleM");
                if (buff2 != null)
                {
                    SolveBuffBlock(buff2);
                    return;
                }
            }

            // jax E
            var jax = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Jax" && x.LSIsValidTarget());

            if (jax != null && jax.HasBuff("JaxCounterStrike") && getCheckBoxItem(evadeMenu, "Jax" + SpellSlot.E))
            {
                var buff = jax.GetBuff("JaxCounterStrike");
                if (buff != null)
                {
                    if ((buff.EndTime - Game.Time) * 1000 <= 650 + Game.Ping && Player.Position.LSTo2D().LSDistance(jax.Position.LSTo2D())
                        <= Player.BoundingRadius + jax.BoundingRadius + jax.AttackRange + 100)
                    {
                        SolveInstantBlock();
                        return;
                    }
                }
            }

            //maokai R
            var maokai = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Maokai" && x.LSIsValidTarget());
            if (maokai != null && maokai.HasBuff("MaokaiDrain3")
                && getCheckBoxItem(evadeMenu, "Maokai" + SpellSlot.R))
            {
                var buff = maokai.GetBuff("MaokaiDrain3");
                if (buff != null)
                {
                    if (Player.Position.LSTo2D().LSDistance(maokai.Position.LSTo2D())
                        <= Player.BoundingRadius + 475)
                        SolveBuffBlock(buff);
                }
            }

            // nautilus R
            if (Player.HasBuff("nautilusgrandlinetarget") && HeroManager.Enemies.Any(x => x.ChampionName == "Nautilus")
                && getCheckBoxItem(evadeMenu, "Nautilus" + SpellSlot.R))
            {
                var buff = Player.GetBuff("nautilusgrandlinetarget");
                if (buff == null)
                    return;
                var obj = ObjectManager.Get<GameObject>().Where(x => x.Name == "GrandLineSeeker").FirstOrDefault();
                if (obj == null)
                    return;
                if (obj.Position.LSTo2D().LSDistance(Player.Position.LSTo2D()) <= 300 + 700 * Game.Ping / 1000)
                {
                    SolveInstantBlock();
                    return;
                }
            }

            //rammus Q
            var ramus = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Rammus" && x.LSIsValidTarget());
            if (ramus != null && getCheckBoxItem(evadeMenu, "Rammus" + SpellSlot.Q))
            {
                var buff = ramus.GetBuff("PowerBall");
                if (buff != null)
                {
                    var waypoints = ramus.GetWaypoints();
                    if (waypoints.Count == 1)
                    {
                        if (Player.Position.LSTo2D().LSDistance(ramus.Position.LSTo2D())
                            <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius)
                        {
                            SolveInstantBlock();
                            return;
                        }
                    }
                    else
                    {
                        if (Player.Position.LSTo2D().LSDistance(ramus.Position.LSTo2D())
                            <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius
                            + ramus.MoveSpeed * (0.5f + Game.Ping / 1000))
                        {
                            if (waypoints.Any(x => x.LSDistance(Player.Position.LSTo2D())
                                <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius + 70))
                            {
                                SolveInstantBlock();
                                return;
                            }
                            for (int i = 0; i < waypoints.Count() - 2; i++)
                            {
                                if (Player.Position.LSTo2D().LSDistance(waypoints[i], waypoints[i + 1], true)
                                    <= Player.BoundingRadius + ramus.BoundingRadius + ramus.AttackRange + 70)
                                {
                                    SolveInstantBlock();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //fizzR
            if (HeroManager.Enemies.Any(x => x.ChampionName == "Fizz") && getCheckBoxItem(evadeMenu, "Fizz" + SpellSlot.R))
            {
                if (FizzFishChum != null && FizzFishChum.IsValid
                    && Utils.GameTimeTickCount - FizzFishChumStartTick >= 1500 - 250 - Game.Ping
                    && Player.Position.LSTo2D().LSDistance(FizzFishChum.Position.LSTo2D()) <= 250 + Player.BoundingRadius)
                {
                    SolveInstantBlock();
                    return;
                }
            }

            //nocturne R
            var nocturne = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Nocturne" && x.LSIsValidTarget());
            if (nocturne != null
                && getCheckBoxItem(evadeMenu, "Nocturne" + SpellSlot.R))
            {
                var buff = Player.GetBuff("nocturneparanoiadash");
                if (buff != null && Player.Position.LSTo2D().LSDistance(nocturne.Position.LSTo2D()) <= 300 + 1200 * Game.Ping / 1000)
                {
                    SolveInstantBlock();
                    return;
                }
            }


            // rivenQ3
            var riven = HeroManager.Enemies.FirstOrDefault(x => x.ChampionName == "Riven" && x.LSIsValidTarget());
            if (riven != null && getCheckBoxItem(evadeMenu, "Riven" + SpellSlot.Q) && RivenDashEnd.IsValid())
            {
                if (Utils.GameTimeTickCount - RivenDashTick <= 100 && Utils.GameTimeTickCount - RivenQ3Tick <= 100
                    && Math.Abs(RivenDashTick - RivenQ3Tick) <= 100 && Player.Position.LSTo2D().LSDistance(RivenDashEnd) <= RivenQ3Rad)
                {
                    SolveInstantBlock();
                    return;
                }
            }

        }
        private static void SolveBuffBlock(BuffInstance buff)
        {
            if (Player.IsDead || Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity)
                || !getCheckBoxItem(evadeMenu, "W") || !W.IsReady())
                return;
            if (buff == null)
                return;
            if ((buff.EndTime - Game.Time) * 1000 <= 250 + Game.Ping)
            {
                var tar = GetTarget(W.Range);
                if (tar.LSIsValidTarget(W.Range))
                    Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                else
                {
                    var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(W.Range));
                    if (hero != null)
                        Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                    else
                        Player.Spellbook.CastSpell(SpellSlot.W, Player.ServerPosition.LSExtend(Game.CursorPos, 100));
                }
            }
        }
        private static void SolveInstantBlock()
        {
            if (Player.IsDead || Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity) || !getCheckBoxItem(evadeMenu, "W") || !W.IsReady()) return;
            var tar = GetTarget(W.Range);
            if (tar.LSIsValidTarget(W.Range))
                Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
            else
            {
                var hero = HeroManager.Enemies.FirstOrDefault(x => x.LSIsValidTarget(W.Range));
                if (hero != null)
                    Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                else
                    Player.Spellbook.CastSpell(SpellSlot.W, Player.ServerPosition.LSExtend(Game.CursorPos, 100));
            }
        }
        private static void LoadSpellData()
        {
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Azir",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Fizz",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jax",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Jax",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Riven",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Diana",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kalista",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karma",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Karthus",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Kennen",
                    Slot = SpellSlot.W,
                });
            //Spells.Add(
            //    new SpellData
            //    {
            //        ChampionName = "Leesin",
            //        Slot = SpellSlot.Q,
            //    });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Maokai",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Morgana",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nautilus",
                    Slot = SpellSlot.Unknown,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nautilus",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nocturne",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rammus",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Rengar",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
            new SpellData
            {
                ChampionName = "Reksai",
                Slot = SpellSlot.W,
            });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Vladimir",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Zed",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Tristana",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Udyr",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Yorick",
                    Slot = SpellSlot.Q,
                });
        }
        private class SpellData
        {
            #region Fields

            public string ChampionName;

            public SpellSlot Slot;

            #endregion

            #region Public Properties


            #endregion
        }
    }
    #endregion OtherSkill
}
