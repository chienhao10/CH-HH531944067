using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using SharpDX;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;

    internal static class Program
    {
        #region Static Fields

        public static Vector2 JumpPos;
        private static readonly bool castWardAgain = true;
        private static Spell.Targeted Q = new Spell.Targeted(SpellSlot.Q, 675);
        private static Spell.Active W = new Spell.Active(SpellSlot.W, 375);
        private static Spell.Targeted E = new Spell.Targeted(SpellSlot.E, 700);
        private static Spell.Active R = new Spell.Active(SpellSlot.R, 550);
        private static Spell.Targeted ignite;
        private static Menu config;
        private static SpellSlot igniteSlot;
        private static int lastPlaced;
        private static long lastECast;
        private static Vector3 lastWardPos;
        private static float rStart;
        private static float wcasttime;

        private static CheckBox _smartR = new CheckBox("使用智能 R", false);
        private static CheckBox _wardJump = new CheckBox("连招进行跳眼");
        private static ComboBox _harassMode = new ComboBox("骚扰模式", 1, new string[] { "只 Q", "Q+W", "Q+E+W" });
        private static CheckBox _harassQ = new CheckBox("自动骚扰 Q", false);
        private static CheckBox _harassW = new CheckBox("自动骚扰 W");
        private static CheckBox _harassE = new CheckBox("塔下不E");
        private static CheckBox _laneQ = new CheckBox("农兵 Q");
        private static CheckBox _laneW = new CheckBox("农兵 W");
        private static CheckBox _laneE = new CheckBox("农兵 E", false);
        private static CheckBox _laneCQ = new CheckBox("清线 Q");
        private static CheckBox _laneCW = new CheckBox("清线 W");
        private static CheckBox _laneCE = new CheckBox("清线 E", false);
        private static CheckBox _jungleQ = new CheckBox("清野 Q");
        private static CheckBox _jungleW = new CheckBox("清野 W");
        private static CheckBox _jungleE = new CheckBox("清野 E");
        private static CheckBox _ksSmart = new CheckBox("抢头");
        private static CheckBox _ksR = new CheckBox("抢头结束 R");
        private static CheckBox _drawingQ = new CheckBox("显示 Q 范围", false);
        private static CheckBox _drawingW = new CheckBox("显示 W 范围", false);
        private static CheckBox _drawingE = new CheckBox("显示 E 范围");
        private static CheckBox _drawingR = new CheckBox("显示 R 范围", false);
        private static CheckBox _legitE = new CheckBox("人性化 E", false);
        private static Slider _legitEDelay = new Slider("人性化 E 延迟", 1000, 0, 2000);
        private static KeyBind _wardJumpKey = new KeyBind("跳眼延迟", false, KeyBind.BindTypes.HoldActive, 'A');
        private static CheckBox _jumpMax  = new CheckBox("跳眼至最远距离", true);
        private static CheckBox _jumpMinions = new CheckBox("E 至小兵", true);
        private static CheckBox _jumpMouse = new CheckBox("E 至鼠标", true);
        private static CheckBox _jumpChampions = new CheckBox("E 至英雄", true);
        private static CheckBox _ignite = new CheckBox("使用 点燃", true);

        #endregion


        private static void BeforeAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (args.Target.IsMe)
            {
                args.Process = !HasRBuff();
            }
        }

        private static void CastE(Obj_AI_Base unit)
        {
            var playLegit = _legitE.CurrentValue;
            var legitCastDelay = _legitEDelay.CurrentValue;

            if (playLegit)
            {
                if (Environment.TickCount > lastECast + legitCastDelay)
                {
                    E.Cast(unit);
                    lastECast = Environment.TickCount;
                }
            }
            else
            {
                E.Cast(unit);
                lastECast = Environment.TickCount;
            }
        }

        private static void CastEWard(Obj_AI_Base obj)
        {
            Program.E.Cast(obj);
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget() || HasRBuff())
                return;

            var rdmg = Player.Instance.GetSpellDamage(target, SpellSlot.R);

            if (Q.IsInRange(target))
            {
                if (Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }
                if (E.IsReady())
                {
                    CastE(target);
                    return;
                }
            }
            else
            {
                if (E.IsReady())
                {
                    CastE(target);
                    return;
                }
                if (Q.IsReady())
                {
                    Q.Cast(target);
                    return;
                }
            }

            if (W.IsReady() && W.IsInRange(target))
            {
                W.Cast();
                return;
            }

            if(GetComboDamage(target) > target.Health)
            {
                if (igniteSlot != SpellSlot.Unknown)
                    ignite.Cast(target);
            }

            //Smart R
            if (_smartR.CurrentValue)
            {
                if (R.IsReady() && target.Health - rdmg < 0 && !E.IsReady())
                {
                    R.Cast();

                    rStart = Environment.TickCount;
                }
            }
            else if (R.IsReady() && !E.IsReady())
            {
                Orbwalker.DisableAttacking = true;
                Orbwalker.DisableMovement = true;
                R.Cast();

                rStart = Environment.TickCount;
            }

            
        }

        private static void Drawings(EventArgs args)
        {
            var drawQ = _drawingQ.CurrentValue;
            var drawW = _drawingW.CurrentValue;
            var drawE = _drawingE.CurrentValue;
            var drawR = _drawingR.CurrentValue;

            if (drawQ)
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
            if (drawW)
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
            if (drawE)
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
            if (drawR)
                Circle.Draw(Q.IsReady() ? Color.Green : Color.Red, Q.Range, Player.Instance.Position);
        }

        private static void Farm()
        {
            foreach (var minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range).OrderBy(x => x.Health))
            {
                var qdmg = Player.Instance.GetSpellDamage(minion, SpellSlot.Q);
                var wdmg = Player.Instance.GetSpellDamage(minion, SpellSlot.W);
                var edmg = Player.Instance.GetSpellDamage(minion, SpellSlot.E);
                var markDmg = Player.Instance.CalculateDamageOnUnit(
                    minion,
                    DamageType.Magical,
                    Player.Instance.FlatMagicDamageMod * 0.15f + Player.Instance.Level * 15);

                //Killable with Q
                if (minion.Health - qdmg <= 0 && minion.Distance(Player.Instance.ServerPosition) <= Q.Range
                    && Q.IsReady() && _laneQ.CurrentValue)
                {
                    Q.Cast(minion);
                }

                if (minion.Health - wdmg <= 0 && minion.Distance(Player.Instance.ServerPosition) <= W.Range
                    && W.IsReady() && _laneW.CurrentValue)
                {
                    Q.Cast();
                    return;
                }

                if (minion.Health - edmg <= 0 && minion.Distance(Player.Instance.ServerPosition) <= E.Range
                    && E.IsReady() && _laneE.CurrentValue)
                {
                    CastE(minion);
                    return;
                }

                if (minion.Health - wdmg - qdmg <= 0 && minion.Distance(Player.Instance.ServerPosition) <= W.Range
                    && Q.IsReady() && W.IsReady()
                    && _laneQ.CurrentValue && _laneW.CurrentValue)
                {
                    Q.Cast(minion);
                    W.Cast();
                    return;
                }

                if (minion.Health - wdmg - qdmg - markDmg <= 0
                    && minion.Distance(Player.Instance.ServerPosition) <= W.Range && Q.IsReady()
                    && W.IsReady() && _laneQ.CurrentValue
                    && _laneW.CurrentValue)
                {
                    Q.Cast(minion);
                    W.Cast();
                    return;
                }

                if (minion.Health - wdmg - qdmg - markDmg - edmg <= 0
                    && minion.Distance(Player.Instance.ServerPosition) <= W.Range && E.IsReady()
                    && Q.IsReady() && W.IsReady()
                    && _laneQ.CurrentValue && _laneW.CurrentValue
                    && _laneE.CurrentValue)
                {
                    CastE(minion);
                    Q.Cast(minion);
                    W.Cast();
                    return;
                }
            }
        }

        private static InventorySlot FindBestWardItem()
        {
            var wardIds = new[] { ItemId.Warding_Totem_Trinket, ItemId.Sightstone, ItemId.Ruby_Sightstone, ItemId.Vision_Ward, ItemId.Greater_Stealth_Totem_Trinket };
            var slot = Player.Instance.InventoryItems.FirstOrDefault(a => wardIds.Contains(a.Id) && a.IsWard && a.CanUseItem());
            if (slot == default(InventorySlot))
            {
                return null;
            }

            var sdi = GetItemSpell(slot);

            if (sdi != default(SpellDataInst) && sdi.State == SpellState.Ready)
            {
                return slot;
            }
            return slot;
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!E.IsReady() || !(sender is Obj_AI_Minion) || Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }

            if (Environment.TickCount >= lastPlaced + 300)
            {
                return;
            }
            var ward = (Obj_AI_Minion)sender;

            if (ward.Name.ToLower().Contains("ward"))
            {
                E.Cast(ward);
            }
        }

        private static float GetComboDamage(Obj_AI_Base enemy)
        {
            float damage = 0;

            if (Q.IsReady())
            {
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.Q);
            }

            if (W.IsReady())
            {
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.W);
            }

            if (E.IsReady())
            {
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.E);
            }

            if (R.IsReady())
            {
                damage += Player.Instance.GetSpellDamage(enemy, SpellSlot.R);
            }

            if (igniteSlot != SpellSlot.Unknown && Player.Instance.Spellbook.CanUseSpell(igniteSlot) == SpellState.Ready)
            {
                damage += (float)Player.Instance.GetSummonerSpellDamage(enemy, DamageLibrary.SummonerSpells.Ignite);
            }

            return damage;
        }

        private static SpellDataInst GetItemSpell(InventorySlot invSlot)
        {
            return Player.Instance.Spellbook.Spells.FirstOrDefault(spell => (int)spell.Slot == invSlot.Slot + 4);
        }

        private static void Harass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValidTarget())
            {
                return;
            }

            var menuItem = _harassMode.CurrentValue;

            switch (menuItem)
            {
                case 0:
                    if (Q.IsReady())
                        Q.Cast(target);
                    break;
                case 1:
                    if (Q.IsReady())
                        Q.Cast(target);
                    if (W.IsReady() && target.Distance(Player.Instance) <= W.Range)
                        W.Cast();
                    break;
                case 2:
                    if (Q.IsReady())
                        Q.Cast(target);
                    if (E.IsReady())
                        if (_harassE.CurrentValue)
                            if (target.IsUnderHisturret())
                                return;
                        CastE(target);
                    if (W.IsReady() && target.Distance(Player.Instance) <= W.Range)
                        W.Cast();
                    break;
            }
        }

        private static bool HasRBuff()
        {
            return Player.HasBuff("KatarinaR") || Player.HasBuff("KatarinaRSound");
        }

        private static void JungleClear()
        {
            var mobs = EntityManager.MinionsAndMonsters.GetJungleMonsters(Player.Instance.Position, E.Range).OrderByDescending(x => x.MaxHealth).ToArray();
            if (mobs.Length <= 0)
                return;

            var mob = mobs[0];
            if (mob == null)
            {
                return;
            }

            if (_jungleQ.CurrentValue && Q.IsReady())
                Q.Cast(mob);

            if (_jungleW.CurrentValue && W.IsReady() && W.IsInRange(mob))
                W.Cast();

            if (_jungleE.CurrentValue && E.IsReady())
                E.Cast(mob);
        }

        private static void KillSteal()
        {
            if (_ksSmart.CurrentValue)
            {
                if (HasRBuff() && !_ksR.CurrentValue)
                    return;

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => !x.IsDead && x.IsEnemy && x.Distance(Player.Instance) < E.Range).OrderBy(x => x.Health))
                {
                    float damageCanBeDone =
                        (Q.IsReady() ? Player.Instance.GetSpellDamage(enemy, SpellSlot.Q) : 0) +
                        (W.IsReady() ? Player.Instance.GetSpellDamage(enemy, SpellSlot.W) : 0) +
                        (E.IsReady() ? Player.Instance.GetSpellDamage(enemy, SpellSlot.E) : 0);
                    if (damageCanBeDone > enemy.Health)
                    {
                        if (E.IsReady())
                            E.Cast(enemy);

                        if (Q.IsReady())
                            Q.Cast(enemy);

                        if (W.IsReady() && enemy.Distance(Player.Instance) < W.Range)
                            W.Cast();
                        return;
                    }
                }
            }
        }

        private static void Laneclear()
        {
            var useQ = _laneCQ.CurrentValue;
            var useW = _laneCW.CurrentValue;
            var useE = _laneCE.CurrentValue;

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.Position, E.Range).OrderBy(x => x.Health).ToArray();
            if (minions.Length <= 0)
                return;

            if (Q.IsReady() && useQ)
            {
                    Q.Cast(minions[0]);
                    return;
            }

            if (useW && W.IsReady() && W.IsInRange(minions.FirstOrDefault())) //check
            {
                if (minions.Length > 2)
                {
                    W.Cast();
                    return;
                }
            }

            if (useE && E.IsReady())
            {
                    CastE(minions[0]);
                    return;
            }
        }

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoad;
        }

        private static void MenuLoad()
        {
            config = MainMenu.AddMenu("皇族卡特", "RoyalKatarina");
            Menu Menu = config.AddSubMenu("Laning Settings");
            Menu.AddLabel("连招设置");
            Menu.Add("_smartRR", _smartR);
            //Menu.Add("_wardJumpR", _wardJump);
            if (igniteSlot != SpellSlot.Unknown)
            {
                Menu.Add("_igniteR", _ignite);
            }
            Menu.AddLabel("骚扰设置");
            Menu.Add("_harassModeR", _harassMode);
            Menu.Add("_harassE", _harassE);
            Menu.Add("_harassQR", _harassQ);
            Menu.Add("_harassWR", _harassW);
            Menu.AddLabel("尾兵设置");
            Menu.Add("_laneQR", _laneQ);
            Menu.Add("_laneWR", _laneW);
            Menu.Add("_laneER", _laneE);
            Menu.AddLabel("清线设置");
            Menu.Add("_laneCQR", _laneCQ);
            Menu.Add("_laneCWR", _laneCW);
            Menu.Add("_laneCER", _laneCE);
            Menu.AddLabel("清野设置");
            Menu.Add("_jungleQR", _jungleQ);
            Menu.Add("_jungleWR", _jungleW);
            Menu.Add("_jungleER", _jungleE);
            Menu.AddLabel("抢头设置");
            Menu.Add("_ksSmartR", _ksSmart);
            Menu.Add("_ksRR", _ksR);

            Menu MenuMisc = config.AddSubMenu("杂项");
            MenuMisc.AddLabel("E 设置");
            MenuMisc.Add("_legitER", _legitE);
            MenuMisc.Add("_legitEDelayR", _legitEDelay);
            MenuMisc.AddLabel("跳眼设置");
            MenuMisc.Add("_wardJumpKeyR", _wardJumpKey);
            MenuMisc.Add("_jumpMax", _jumpMax);
            MenuMisc.Add("_jumpMouseR", _jumpMouse);
            MenuMisc.Add("_jumpMinionsR", _jumpMinions);
            MenuMisc.Add("_jumpChampionsR", _jumpChampions);

            Menu Drawings = config.AddSubMenu("线圈显示");
            Drawings.AddLabel("Drawing settings");
            Drawings.Add("_drawingQR", _drawingQ);
            Drawings.Add("_drawingWR", _drawingW);
            Drawings.Add("_drawingER", _drawingE);
            Drawings.Add("_drawingRR", _drawingR);
        }

        private static void Obj_AI_Base_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe || args.SData.Name != "KatarinaR" || !Player.HasBuff("katarinarsound"))
            {
                return;
            }

            Orbwalker.DisableMovement = false;
            Orbwalker.DisableAttacking = false;
        }

        private static void Obj_AI_Hero_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe && Environment.TickCount < rStart + 300 && args.Order == GameObjectOrder.MoveTo)
            {
                args.Process = false;
            }
        }

        private static void OnAutoHarass()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null || !target.IsValid || Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo) || HasRBuff())
            {
                return;
            }

            var useQ = _harassQ.CurrentValue;
            var useW = _harassW.CurrentValue;

            if (Q.IsReady() && target.IsValidTarget() && useQ)
            {
                Q.Cast(target);
            }

            if (W.IsReady() && target.IsValidTarget(W.Range) && useW)
            {
                W.Cast();
            }
        }

        private static void OnLoad(EventArgs args)
        {
            if (ObjectManager.Player.ChampionName != "Katarina")
            {
                return;
            }
            igniteSlot = Player.Instance.GetSpellSlotFromName("summonerdot");
            if (igniteSlot != SpellSlot.Unknown)
            {
                ignite = new Spell.Targeted(igniteSlot, 600);
            }
            MenuLoad();
            Drawing.OnDraw += Drawings;
            Player.OnIssueOrder += Obj_AI_Hero_OnIssueOrder;
            GameObject.OnCreate += GameObject_OnCreate;
            Obj_AI_Base.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Orbwalker.OnPreAttack += BeforeAttack;
            Drawing.OnEndScene += Drawing_OnEndScene;
            Game.OnTick += OnUpdate;
        }

        static void Drawing_OnEndScene(EventArgs args)
        {
            foreach (var unit in EntityManager.Heroes.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
            {
                var damage = GetComboDamage(unit);
                if (damage <= 0)
                    continue;

                var damagePercentage = ((unit.TotalShieldHealth() - damage) > 0 ? (unit.TotalShieldHealth() - damage) : 0) /
                                       (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);
                var currentHealthPercentage = unit.TotalShieldHealth() / (unit.MaxHealth + unit.AllShield + unit.AttackShield + unit.MagicShield);

                var startPoint = new Vector2((int)(unit.HPBarPosition.X + damagePercentage * 106), (int)unit.HPBarPosition.Y + 10);
                var endPoint = new Vector2((int)(unit.HPBarPosition.X + currentHealthPercentage * 106) + 1, (int)unit.HPBarPosition.Y + 10);

                var colorH = System.Drawing.Color.FromArgb(Color.Yellow.A - 120, Color.Yellow.R,
                    Color.Yellow.G, Color.Yellow.B);

                Drawing.DrawLine(startPoint, endPoint, 9.8f, colorH);
            }
        }

        private static void OnUpdate(EventArgs args)
        {
            if (Player.Instance.IsDead)
            {
                return;
            }

            if (HasRBuff())
            {
                Orbwalker.DisableMovement = true;
                Orbwalker.DisableAttacking = true;
            }
            else
            {
                Orbwalker.DisableMovement = false;
                Orbwalker.DisableAttacking = false;
            }

            switch (Orbwalker.ActiveModesFlags)
            {
                case Orbwalker.ActiveModes.Combo:
                    Combo();
                    break;
                case Orbwalker.ActiveModes.Harass:
                    Harass();
                    break;
                case Orbwalker.ActiveModes.LaneClear | Orbwalker.ActiveModes.JungleClear:
                    Laneclear();
                    JungleClear();
                    break;
                case Orbwalker.ActiveModes.LastHit:
                    Farm();
                    break;
            }

            KillSteal();

            if (_wardJumpKey.CurrentValue)
            {
                WardjumpToMouse();
            }


            OnAutoHarass();
        }

        private static void Orbwalk(Vector3 pos, AIHeroClient target = null)
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, pos);
        }

        private static void WardJump(
            Vector3 pos,
            bool m2M = true,
            bool maxRange = true,
            bool reqinMaxRange = false,
            bool minions = true,
            bool champions = true)
        {
            if (!E.IsReady())
            {
                return;
            }
            var basePos = Player.Instance.Position.To2D();
            var newPos = (pos.To2D() - Player.Instance.Position.To2D());

                if (reqinMaxRange)
                {
                    JumpPos = pos.To2D();
                }
                else if (maxRange || Player.Instance.Distance(pos) > 600)
                {
                    JumpPos = basePos + (newPos.Normalized() * (600));
                }
                else
                {
                    JumpPos = basePos + (newPos.Normalized() * (Player.Instance.Distance(pos)));
                }
            if (m2M)
            {
                Orbwalk(pos);
            }
            if (!E.IsReady() || reqinMaxRange && Player.Instance.Distance(pos) > E.Range)
            {
                return;
            }

            if (minions || champions)
            {
                if (champions)
                {
                    var champs = (from champ in ObjectManager.Get<AIHeroClient>()
                                  where
                                      champ.IsAlly && champ.Distance(Player.Instance) < E.Range
                                      && champ.Distance(pos) < 200 && !champ.IsMe
                                  select champ).ToList();
                    if (champs.Count > 0 && E.IsReady())
                    {
                        if (500 >= Environment.TickCount - wcasttime || !E.IsReady())
                        {
                            return;
                        }

                        CastEWard(champs[0]);
                        return;
                    }
                }
                if (minions)
                {
                    var minion2 = (from minion in ObjectManager.Get<Obj_AI_Minion>()
                                   where
                                       minion.IsAlly && minion.Distance(Player.Instance) < E.Range
                                       && minion.Distance(pos) < 200 && !minion.Name.ToLower().Contains("ward")
                                   select minion).ToList();
                    if (minion2.Count > 0)
                    {
                        if (500 >= Environment.TickCount - wcasttime || !E.IsReady())
                        {
                            return;
                        }

                        CastEWard(minion2[0]);
                        return;
                    }
                }
            }

            var isWard = false;
            foreach (var ward in ObjectManager.Get<Obj_AI_Base>())
            {
                if (ward.IsAlly && ward.Name.ToLower().Contains("ward") && ward.Distance(JumpPos) < 200)
                {
                    isWard = true;
                    if (500 >= Environment.TickCount - wcasttime || !E.IsReady())
                    {
                        return;
                    }

                    CastEWard(ward);
                    wcasttime = Environment.TickCount;
                }
            }

            if (!isWard && castWardAgain)
            {
                var ward = FindBestWardItem();
                if (ward == null || !E.IsReady())
                {
                    return;
                }

                Player.Instance.Spellbook.CastSpell(ward.SpellSlot, JumpPos.To3D());
                lastWardPos = JumpPos.To3D();
            }
        }

        private static void WardjumpToMouse()
        {
            WardJump(
                Game.CursorPos,
                _jumpMouse.CurrentValue,
                _jumpMax.CurrentValue,
                false,
                _jumpMinions.CurrentValue,
                _jumpChampions.CurrentValue);
        }

    }
