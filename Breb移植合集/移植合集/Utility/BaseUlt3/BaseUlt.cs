using EloBuddy.SDK.Menu.Values;
using EloBuddy.SDK.Menu;
using EloBuddy;
using LeagueSharp.Common;
using SharpDX.Direct3D9;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using Collision = LeagueSharp.Common.Collision;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Spell = LeagueSharp.Common.Spell;
using Utility = LeagueSharp.Common.Utility;


namespace BaseUlt3
{
    /*
     * fixed? use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
     * Fadeout even normal recall finishes
     * 
     * @beaving why doesn't team baseult work? did it require packets? we tried yesterday(before l# was fully updated) and my friend ulted, my character was about to ult and then after it saw ez ult it canceled I was playing jinx. WE lost a free kill lol because of it. Does it work? 
     * 
     * Hello beaving , iam a great fan of your scripts especially the baseult one , i can see u have did a great work in ur scripts , but while i was playing with baseult3 an idea came up to my mind , A script that can detect position of enemies in a circular shape
Example:http://imgur.com/2BGvB2C (sry for bad drawing )
The idea where the lines come from is that u can calculate how far they are from base (enemySpawnPos) , so is it possible to make a script that can just show the position of the enemy while recalling i guess it would help , especially if u can show these lines even when they're not recalling (not sure if it's possible tho ) that would help so much in ganks and other stuff , thanks for your time , and i hope you give me your opinion about this script , have a nice day 
     ---> draw growing circle as soon as enemies go into fow. if they start recalling, dont increase the circle range. if they finished -> reset. some time limit too?
     
     */

    internal class BaseUlt
    {
        private readonly Menu Menu;
        private readonly Menu TeamUlt;
        private readonly Menu DisabledChampions;
        private readonly Menu Notifications;

        private readonly Spell Ultimate;
        private int LastUltCastT;

        private readonly List<AIHeroClient> Allies;

        public List<EnemyInfo> EnemyInfo;

        public Dictionary<int, int> RecallT = new Dictionary<int, int>();

        private readonly Vector3 EnemySpawnPos;

        private readonly Font Text;

        private static readonly float BarX = Drawing.Width * 0.425f;
        private readonly float BarY = Drawing.Height * 0.80f;
        private static readonly int BarWidth = (int)(Drawing.Width - 2 * BarX);
        private readonly int BarHeight = 6;
        private readonly int SeperatorHeight = 5;
        private static readonly float Scale = (float)BarWidth / 8000;

        public static bool getCheckBoxItem(Menu m, string item)
        {
            return m[item].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderItem(Menu m, string item)
        {
            return m[item].Cast<Slider>().CurrentValue;
        }

        public static bool getKeyBindItem(Menu m, string item)
        {
            return m[item].Cast<KeyBind>().CurrentValue;
        }

        public static int getBoxItem(Menu m, string item)
        {
            return m[item].Cast<ComboBox>().CurrentValue;
        }

        public BaseUlt()
        {
            Menu = MainMenu.AddMenu("BaseUlt3", "BaseUlt3");
            Menu.Add("showRecalls", new CheckBox("Show Recalls"));
            Menu.Add("baseUlt", new CheckBox("Base Ult"));
            Menu.Add("checkCollision", new CheckBox("Check Collision"));
            Menu.Add("panicKey", new KeyBind("No Ult while SBTW", false, KeyBind.BindTypes.HoldActive, 32));
            Menu.Add("regardlessKey", new KeyBind("No timelimit (hold)", false, KeyBind.BindTypes.HoldActive, 17)); //17 == ctrl

            var heroes = ObjectManager.Get<AIHeroClient>().ToList();
            var enemies = heroes.Where(x => x.IsEnemy).ToList();
            Allies = heroes.Where(x => x.IsAlly).ToList();

            EnemyInfo = enemies.Select(x => new EnemyInfo(x)).ToList();

            bool compatibleChamp = IsCompatibleChamp(ObjectManager.Player.ChampionName);

            TeamUlt = Menu.AddSubMenu("Team Baseult Friends", "TeamUlt");
            DisabledChampions = Menu.AddSubMenu("Disabled Champion targets", "DisabledChampions");

            if (compatibleChamp)
            {
                foreach (AIHeroClient champ in Allies.Where(x => !x.IsMe && IsCompatibleChamp(x.ChampionName)))
                    TeamUlt.Add(champ.ChampionName, new CheckBox("Ally with baseult: " + champ.ChampionName, false));

                foreach (AIHeroClient champ in enemies)
                    DisabledChampions.Add(champ.ChampionName, new CheckBox("Don't shoot: " + champ.ChampionName, false));
            }

            Notifications = Menu.AddSubMenu("Notifications", "Notifications");
            Notifications.Add("notifRecFinished", new CheckBox("Recall finished"));
            Notifications.Add("notifRecAborted", new CheckBox("Recall aborted"));

            var objSpawnPoint = ObjectManager.Get<Obj_SpawnPoint>().FirstOrDefault(x => x.IsEnemy);
            if (objSpawnPoint != null)
                EnemySpawnPos = objSpawnPoint.Position; //ObjectManager.Get<GameObject>().FirstOrDefault(x => x.Type == GameObjectType.obj_SpawnPoint && x.IsEnemy).Position;

            Ultimate = new Spell(SpellSlot.R);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 13, Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });

            Teleport.OnTeleport += Obj_AI_Base_OnTeleport;
            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;

            if (compatibleChamp)
                Game.OnUpdate += Game_OnUpdate;
        }

        public bool IsCompatibleChamp(String championName)
        {
            return UltSpellData.Keys.Any(x => x == championName);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            int time = Utils.TickCount;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsVisible))
                enemyInfo.LastSeen = time;

            if (!getCheckBoxItem(Menu, "baseUlt"))
                return;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x => x.Player.IsValid<AIHeroClient>() && !x.Player.IsDead && !getCheckBoxItem(DisabledChampions, x.Player.ChampionName) && x.RecallInfo.status == TeleportStatus.Start && x.RecallInfo.type == TeleportType.Recall).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (Utils.TickCount - LastUltCastT > 15000)
                {
                    HandleUltTarget(enemyInfo);   
                }
            }
        }

        private struct UltSpellDataS
        {
            public int SpellStage;
            public float DamageMultiplicator;
            public float Width;
            public float Delay;
            public float Speed;
            public bool Collision;
        }

        private readonly Dictionary<String, UltSpellDataS> UltSpellData = new Dictionary<string, UltSpellDataS>
        {
            {"Jinx",    new UltSpellDataS { SpellStage = 1, DamageMultiplicator = 1.0f, Width = 140f, Delay = 0600f/1000f, Speed = 1700f, Collision = true}},
            {"Ashe",    new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 130f, Delay = 0250f/1000f, Speed = 1600f, Collision = true}},
            {"Draven",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.7f, Width = 160f, Delay = 0400f/1000f, Speed = 2000f, Collision = true}},
            {"Ezreal",  new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 0.9f, Width = 160f, Delay = 1000f/1000f, Speed = 2000f, Collision = false}},
            {"Karthus", new UltSpellDataS { SpellStage = 0, DamageMultiplicator = 1.0f, Width = 000f, Delay = 3125f/1000f, Speed = 0000f, Collision = false}}
        };

        private bool CanUseUlt(AIHeroClient hero) //use for allies when fixed: champ.Spellbook.GetSpell(SpellSlot.R) = Ready
        {
            return hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready ||
                (hero.Spellbook.GetSpell(SpellSlot.R).Level > 0 && hero.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Surpressed && hero.Mana >= hero.Spellbook.GetSpell(SpellSlot.R).SData.Mana);
        }

        private void HandleUltTarget(EnemyInfo enemyInfo)
        {
            bool ultNow = false;
            bool me = false;

            foreach (AIHeroClient champ in Allies.Where(x => x.IsValid<AIHeroClient>() && !x.IsDead && ((x.IsMe && !x.IsStunned) || getCheckBoxItem(TeamUlt, x.ChampionName)) && CanUseUlt(x)))
            {
                Console.WriteLine("0");
                if (getCheckBoxItem(Menu, "checkCollision") && UltSpellData[champ.ChampionName].Collision && IsCollidingWithChamps(champ, EnemySpawnPos, UltSpellData[champ.ChampionName].Width))
                {
                    Console.WriteLine("1");
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                var timeneeded = GetUltTravelTime(champ, UltSpellData[champ.ChampionName].Speed, UltSpellData[champ.ChampionName].Delay, EnemySpawnPos) - 65;

                Console.WriteLine("2 : " + timeneeded + " | " + enemyInfo.RecallInfo.GetRecallCountdown());
                if (enemyInfo.RecallInfo.GetRecallCountdown() >= timeneeded)
                {
                    Console.WriteLine("2");
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = (float)champ.LSGetSpellDamage(enemyInfo.Player, SpellSlot.R, UltSpellData[champ.ChampionName].SpellStage) * UltSpellData[champ.ChampionName].DamageMultiplicator;
                }
                else if (enemyInfo.RecallInfo.GetRecallCountdown() < timeneeded - (champ.IsMe ? 0 : 125)) //some buffer for allies so their damage isnt getting reset
                {
                    Console.WriteLine("3");
                    enemyInfo.RecallInfo.IncomingDamage[champ.NetworkId] = 0;
                    continue;
                }

                Console.WriteLine("4");

                if (champ.IsMe)
                {
                    me = true;

                    enemyInfo.RecallInfo.EstimatedShootT = timeneeded;

                    Console.WriteLine("Time Needed : " + timeneeded);

                    if ((enemyInfo.RecallInfo.GetRecallCountdown() - timeneeded) < 60)
                    {
                        ultNow = true;   
                    }
                }
            }

            if (me)
            {
                Console.WriteLine("7");
                if (!IsTargetKillable(enemyInfo))
                {
                    enemyInfo.RecallInfo.LockedTarget = false;
                    Console.WriteLine("RETURN 1");
                    return;
                }

                enemyInfo.RecallInfo.LockedTarget = true;

                if (!ultNow || getKeyBindItem(Menu, "panicKey"))
                {
                    Console.WriteLine("RETURN 2");
                    return;
                }
                Console.WriteLine("Casted.");
                Ultimate.Cast(EnemySpawnPos, true);
                LastUltCastT = Utils.TickCount;
            }
            else
            {
                enemyInfo.RecallInfo.LockedTarget = false;
                enemyInfo.RecallInfo.EstimatedShootT = 0;
            }
        }

        private bool IsTargetKillable(EnemyInfo enemyInfo)
        {
            float totalUltDamage = enemyInfo.RecallInfo.IncomingDamage.Values.Sum();
            float targetHealth = GetTargetHealth(enemyInfo, enemyInfo.RecallInfo.GetRecallCountdown());

            if (Utils.TickCount - enemyInfo.LastSeen > 20000 && !getKeyBindItem(Menu, "regardlessKey"))
            {
                if (totalUltDamage < enemyInfo.Player.MaxHealth)
                {
                    Console.WriteLine("This 1");
                    return false;   
                }
            }
            else if (totalUltDamage < targetHealth)
            {
                Console.WriteLine("This 2");
                return false;   
            }

            return true;
        }

        private float GetTargetHealth(EnemyInfo enemyInfo, int additionalTime)
        {
            if (enemyInfo.Player.IsVisible)
            {
                return enemyInfo.Player.Health;
            }
            
            float predictedHealth = enemyInfo.Player.Health + enemyInfo.Player.HPRegenRate * ((Utils.TickCount - enemyInfo.LastSeen + additionalTime) / 1000f);

            return predictedHealth > enemyInfo.Player.MaxHealth ? enemyInfo.Player.MaxHealth : predictedHealth;
        }

        private float GetUltTravelTime(AIHeroClient source, float speed, float delay, Vector3 targetpos)
        {
            if (source.ChampionName == "Karthus")
                return delay * 1000;

            float distance = Vector3.Distance(source.ServerPosition, targetpos);

            float missilespeed = speed;

            if (source.ChampionName == "Jinx" && distance > 1350)
            {
                const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                var acceldifference = distance - 1350f;

                if (acceldifference > 150f) //it only accelerates 150 units
                    acceldifference = 150f;

                var difference = distance - 1500f;

                missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) + difference * 2200f) / distance;
            }

            return (distance / missilespeed + delay) * 1000;
        }

        private bool IsCollidingWithChamps(AIHeroClient source, Vector3 targetpos, float width)
        {
            var input = new PredictionInput
            {
                Radius = width,
                Unit = source,
            };

            input.CollisionObjects[0] = CollisionableObjects.Heroes;

            return Collision.GetCollision(new List<Vector3> { targetpos }, input).Any(); //x => x.NetworkId != targetnetid, hard to realize with teamult
        }

        private void Obj_AI_Base_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var unit = sender as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.IsAlly)
            {
                return;
            }

            var enemyInfo = EnemyInfo.Find(x => x.Player.NetworkId == sender.NetworkId).RecallInfo.UpdateRecall(args.Type, args.Status, args.Duration, args.Start);

            if (args.Type == TeleportType.Recall)
            {
                switch (args.Status)
                {
                    case TeleportStatus.Start:
                        if (getCheckBoxItem(Notifications, "notifRecAborted"))
                        {
                            Chat.Print(enemyInfo.Player.ChampionName + ": Recall STARTED");
                        }
                        break;
                    case TeleportStatus.Abort:
                        if (getCheckBoxItem(Notifications, "notifRecAborted"))
                        {
                            Chat.Print(enemyInfo.Player.ChampionName + ": Recall ABORTED");
                        }
                        break;
                    case TeleportStatus.Finish:
                        if (getCheckBoxItem(Notifications, "notifRecFinished"))
                        {
                            Chat.Print(enemyInfo.Player.ChampionName + ": Recall FINISHED");
                        }
                        break;
                }
            }
        }

        private void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        private void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        private void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            if (!getCheckBoxItem(Menu, "showRecalls") || Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;

            bool indicated = false;

            float fadeout = 1f;
            int count = 0;

            foreach (EnemyInfo enemyInfo in EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0).OrderBy(x => x.RecallInfo.GetRecallCountdown()))
            {
                if (!enemyInfo.RecallInfo.LockedTarget)
                {
                    fadeout = 1f;
                    Color color = Color.White;

                    if (enemyInfo.RecallInfo.WasAborted())
                    {
                        fadeout = enemyInfo.RecallInfo.GetDrawTime() / (float)enemyInfo.RecallInfo.FADEOUT_TIME;
                        color = Color.Yellow;
                    }

                    DrawRect(BarX, BarY, (int)(Scale * enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, Color.FromArgb((int)(100f * fadeout), Color.White));
                    DrawRect(BarX + Scale * enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY - SeperatorHeight, 0, SeperatorHeight + 1, 1, Color.FromArgb((int)(255f * fadeout), color));

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY - SeperatorHeight - Text.Description.Height - 1, new ColorBGRA(color.R, color.G, color.B, (byte)(color.A * fadeout)));
                }
                else
                {
                    if (!indicated && enemyInfo.RecallInfo.EstimatedShootT != 0)
                    {
                        indicated = true;
                        DrawRect(BarX + Scale * enemyInfo.RecallInfo.EstimatedShootT, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight * 2, 2, Color.Orange);
                    }

                    DrawRect(BarX, BarY, (int)(Scale * enemyInfo.RecallInfo.GetRecallCountdown()), BarHeight, 1, Color.FromArgb(255, Color.Red));
                    DrawRect(BarX + Scale * enemyInfo.RecallInfo.GetRecallCountdown() - 1, BarY + SeperatorHeight + BarHeight - 3, 0, SeperatorHeight + 1, 1, Color.IndianRed);

                    Text.DrawText(null, enemyInfo.Player.ChampionName, (int)BarX + (int)(Scale * enemyInfo.RecallInfo.GetRecallCountdown() - (float)(enemyInfo.Player.ChampionName.Length * Text.Description.Width) / 2), (int)BarY + SeperatorHeight + Text.Description.Height / 2, new ColorBGRA(255, 92, 92, 255));
                }

                count++;
            }

            /*
             * Show in a red rectangle right next to the normal bar the names of champs which can be killed (when they are not recalling yet)
             * Requires calculating the damages (make more functions!)
             * 
             * var BaseUltableEnemies = EnemyInfo.Where(x =>
                x.Player.IsValid<AIHeroClient>() &&
                !x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0 && x.RecallInfo.LockedTarget).OrderBy(x => x.RecallInfo.GetRecallCountdown());*/

            if (count > 0)
            {
                if (count != 1) //make the whole bar fadeout when its only 1
                    fadeout = 1f;

                DrawRect(BarX, BarY, BarWidth, BarHeight, 1, Color.FromArgb((int)(40f * fadeout), Color.White));

                DrawRect(BarX - 1, BarY + 1, 0, BarHeight, 1, Color.FromArgb((int)(255f * fadeout), Color.White));
                DrawRect(BarX - 1, BarY - 1, BarWidth + 2, 1, 1, Color.FromArgb((int)(255f * fadeout), Color.White));
                DrawRect(BarX - 1, BarY + BarHeight, BarWidth + 2, 1, 1, Color.FromArgb((int)(255f * fadeout), Color.White));
                DrawRect(BarX + 1 + BarWidth, BarY + 1, 0, BarHeight, 1, Color.FromArgb((int)(255f * fadeout), Color.White));
            }
        }

        public void DrawRect(float x, float y, int width, int height, float thickness, Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }
    }

    internal class EnemyInfo
    {
        public AIHeroClient Player;
        public int LastSeen;

        public RecallInfo RecallInfo;

        public EnemyInfo(AIHeroClient player)
        {
            Player = player;
            RecallInfo = new RecallInfo(this);
        }
    }

    internal class RecallInfo
    {
        public EnemyInfo EnemyInfo;
        public Dictionary<int, float> IncomingDamage; //from, damage
        public TeleportType type;
        public TeleportStatus status;
        public int abduration;
        public int abstart;
        public int duration;
        public int start;
        public bool LockedTarget;
        public float EstimatedShootT;
        public int AbortedT;
        public int FADEOUT_TIME = 3000;

        public RecallInfo(EnemyInfo enemyInfo)
        {
            EnemyInfo = enemyInfo;
            type = TeleportType.Unknown;
            status = TeleportStatus.Unknown;
            duration = 0;
            start = 0;
            abduration = 0;
            abstart = 0;
            IncomingDamage = new Dictionary<int, float>();
        }

        public bool ShouldDraw()
        {
            return IsPorting() || (WasAborted() && GetDrawTime() > 0);
        }

        public bool IsPorting()
        {
            return type == TeleportType.Recall && status == TeleportStatus.Start;
        }

        public bool WasAborted()
        {
            return type == TeleportType.Recall && status == TeleportStatus.Abort;
        }

        public EnemyInfo UpdateRecall(TeleportType t, TeleportStatus s, int dur, int st)
        {
            IncomingDamage.Clear();
            LockedTarget = false;
            EstimatedShootT = 0;

            if (t == TeleportType.Recall && s == TeleportStatus.Abort)
            {
                abduration = dur;
                abstart = st;
                AbortedT = Utils.TickCount;
            }
            else
            {
                AbortedT = 0;
            }

            type = t;
            status = s;
            if (s == TeleportStatus.Start)
            {
                duration = dur;
            }
            start = st;

            Console.WriteLine("" + t + " " + s + " " + duration + " " + start + "");
            return EnemyInfo;
        }

        public int GetDrawTime()
        {
            int drawtime;

            if (WasAborted())
                drawtime = FADEOUT_TIME - (Utils.TickCount - AbortedT);
            else
                drawtime = GetRecallCountdown();

            return drawtime < 0 ? 0 : drawtime;
        }

        public int GetRecallCountdown()
        {
            int time = Environment.TickCount;
            int countdown;

            if (time - AbortedT < FADEOUT_TIME)
            {
                countdown = abduration - (AbortedT - abstart);
                Console.WriteLine("If : " + countdown);
            }
            else if (AbortedT > 0)
            {
                countdown = 0; //AbortedT = 0
                Console.WriteLine("ElseIf : " + countdown);
            }
            else
            {
                countdown = time - (start + duration);
                Console.WriteLine("Else : " + countdown);
            }

            Console.WriteLine(countdown < 0 ? 0 : countdown);
            return countdown < 0 ? 0 : countdown;
            
        }

        public override string ToString()
        {
            String drawtext = EnemyInfo.Player.ChampionName + ": " + status; //change to better string and colored

            float countdown = GetRecallCountdown() / 1000f;

            if (countdown > 0)
                drawtext += " (" + countdown.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + "s)";

            return drawtext;
        }
    }
}
