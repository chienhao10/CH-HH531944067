namespace StreamBuddy
{
    using System;
    using System.Linq;

    using System.Collections.Generic;
    using EloBuddy;
    using EloBuddy.SDK;
    using EloBuddy.SDK.Enumerations;
    using EloBuddy.SDK.Events;
    using EloBuddy.SDK.Menu;
    using EloBuddy.SDK.Menu.Values;
    using EloBuddy.SDK.Rendering;
    using SharpDX;     
                          /// <summary>
                          /// The program.
                          /// </summary>
    internal class Program
    {
        #region Static Fields
        /// <summary>
        /// Gets the value of FakeCliks Enable
        /// </summary>
        public static bool Enabled
        {
            get { return fakeclick["Enable"].Cast<CheckBox>().CurrentValue; }
        }
        /// <summary>
        /// If the user is attacking
        /// Currently used for the second style of fake clicks
        /// </summary>
        public static bool attacking = false;

        /// <summary>
        /// The delta t for click frequency
        /// </summary>
        public static float deltaT = 0.15f;

        /// <summary>
        /// Testing unit for EB.
        /// </summary>
        public static AttackableUnit unit = null;


        /// <summary>
        /// The last endpoint the player was moving to.
        /// </summary>
        public static Vector3 lastEndpoint;

        /// <summary>
        /// The last order the player had.
        /// </summary>
        public static GameObjectOrder lastOrder = 0;

        /// <summary>
        /// The time of the last order the player had.
        /// </summary>
        public static float lastOrderTime;

        /// <summary>
        /// The last time a click was done.
        /// </summary>
        public static float lastTime;

        /// <summary>
        /// The Player.
        /// </summary>
        public static AIHeroClient player;

        /// <summary>
        /// The Random number generator
        /// </summary>
        public static Random r = new Random();

        /// <summary>
        /// The root menu.
        /// </summary>
        public static Menu root, fakeclick;

        #endregion

        #region Methods

        /// <summary>
        /// The move fake click after attacking
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        /// <param name="target">
        /// The target.
        /// </param>
        public static void Orbwalker_OnPostAttack(AttackableUnit target, EventArgs args)
        {
            
            attacking = false;
            var t = target as AIHeroClient;
            if (t != null && unit.IsMe)
            {
                Hud.ShowClick(ClickType.Move, RandomizePosition(t.Position));
            }
        }

        /// <summary>
        /// The angle between two vectors.
        /// </summary>
        /// <param name="a">
        /// The first vector.
        /// </param>
        /// <param name="b">
        /// The second vector.
        /// </param>
        /// <returns>
        /// The Angle between two vectors
        /// </returns>
        public static float AngleBetween(Vector3 a, Vector3 b)
        {
            var dotProd = Vector3.Dot(a, b);
            var lenProd = a.Length() * b.Length();
            var divOperation = dotProd / lenProd;
            return (float)(Math.Acos(divOperation) * (180.0 / Math.PI));
        }

        /// <summary>
        /// The before attack fake click.
        /// Currently used for the second style of fake clicks
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Orbwalker_PreAttack(AttackableUnit target, Orbwalker.PreAttackArgs args)
        {
            if (fakeclick["clicktype2"].Cast<CheckBox>().CurrentValue && args.Process)
            {
                Hud.ShowClick(ClickType.Attack, RandomizePosition(args.Target.Position));
                attacking = true;
            }
        }

        /// <summary>
        /// The fake click before you cast a spell
        /// </summary>
        /// <param name="s">
        /// The Spell Book.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void BeforeSpellCast(Spellbook s, SpellbookCastSpellEventArgs args)
        {
            var target = args.Target;

            if (target == null)
            {
                return;
            }

            if (target.Position.Distance(player.Position) >= 5f)
            {
                Hud.ShowClick(ClickType.Attack, args.Target.Position);
            }
        }

        /// <summary>
        /// The on new path fake.
        /// Currently used for the second style of fake clicks
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void DrawFake(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender.IsMe && lastTime + deltaT < Game.Time && args.Path.LastOrDefault() != lastEndpoint
                && args.Path.LastOrDefault().Distance(player.ServerPosition) >= 5f
                && fakeclick["Enable"].Cast<CheckBox>().CurrentValue
                && fakeclick["clicktype2"].Cast<CheckBox>().CurrentValue)
            {
                lastEndpoint = args.Path.LastOrDefault();
                if (!attacking)
                {
                    Hud.ShowClick(ClickType.Move, Game.CursorPos);
                }
                else
                {
                    Hud.ShowClick(ClickType.Attack, Game.CursorPos);
                }

                lastTime = Game.Time;
            }
        }

        /// <summary>
        /// The main entry point.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Main()
        {
            float z = 0.75f;
            int a = (int)Math.Round(z);
            Console.WriteLine(a);


            float y = 0.23f;
            int b = (int)Math.Round(y);
            Console.WriteLine(b);


            float x = 0.21f;
            int c = (int)Math.Round(x);
            Console.WriteLine(c);
            Loading.OnLoadingComplete += OnLoad;
            Game.OnUpdate += OnUpdate;
           // Hacks.DisableDrawings = true;
          //  Hacks.DisableRangeIndicator = true;
           // Hacks.IngameChat = false;
          //  Hacks.RenderWatermark = false;
         //   Hacks.TowerRanges = false;
         //   Hacks.Console = false;
        }

        /// <summary>
        /// The OnIssueOrder event delegate.
        /// Currently used for the first style of fake clicks
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void Player_OnIssueOrder(Obj_AI_Base sender, PlayerIssueOrderEventArgs args)
        {
            if (sender.IsMe
                && (args.Order == GameObjectOrder.MoveTo || args.Order == GameObjectOrder.AttackUnit
                    || args.Order == GameObjectOrder.AttackTo && args.Process)
                && lastOrderTime + r.NextFloat(deltaT, deltaT + .2f) < Game.Time
                && fakeclick["Enable"].Cast<CheckBox>().CurrentValue
                && fakeclick["clicktype1"].Cast<CheckBox>().CurrentValue)
            {
                var vect = args.TargetPosition;
                vect.Z = player.Position.Z;
                if (args.Order == GameObjectOrder.AttackUnit || args.Order == GameObjectOrder.AttackTo)
                {
                    Hud.ShowClick(ClickType.Attack, RandomizePosition(vect));
                }
                else
                {
                    Hud.ShowClick(ClickType.Move, vect);
                }

                lastOrderTime = Game.Time;
            }
        }

        /// <summary>
        /// The OnLoad event delegate
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void OnLoad(EventArgs args)
        {
            root = MainMenu.AddMenu("FakeClicksBuddy", "FakeClicksBuddy");
            root.AddGroupLabel("StreamSettings");
            root.AddLabel("BY MOSTLYPRIDE w00t");
            root.Add("stream", new CheckBox("Soon BIK", false));

            fakeclick = root.AddSubMenu("fakeclick", "Fake Clicks");

            fakeclick.AddGroupLabel("Fake Clicks Settings");
            fakeclick.Add("Enable", new CheckBox("Enable Fake Clicks", true));
            fakeclick.AddLabel("Click Settings 1 = w evade | 2 = Without Evade");
            fakeclick.Add("clicktype1", new CheckBox("With Evade", false));
            fakeclick.Add("clicktype2", new CheckBox("Without Evade", true));
            


            player = ObjectManager.Player;

            Obj_AI_Base.OnNewPath += DrawFake;
            Orbwalker.OnPreAttack += Orbwalker_PreAttack;
            Spellbook.OnCastSpell += BeforeSpellCast;
            Orbwalker.OnPostAttack += Orbwalker_OnPostAttack;
            Player.OnIssueOrder += Player_OnIssueOrder;
        }

        /// <summary>
        /// Shows the click.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="type">The type.</param>
        private static void ShowClick(Vector3 position, ClickType type)
        {
            if (!Enabled)
            {
                return;
            }

            Hud.ShowClick(type, position);
        }

        /// <summary>
        /// The OnUpdate event delegate.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        public static void OnUpdate(EventArgs args)
        {
            /*
            if (root["stream"].Cast<CheckBox>().CurrentValue)
            {

                Hacks.DisableDrawings = true;
                Hacks.DisableRangeIndicator = true;
                Hacks.IngameChat = true;
                Hacks.RenderWatermark = false;
                Hacks.TowerRanges = false;
                Hacks.Console = false;
            
            }
            else
            {
                Hacks.DisableDrawings = false;
                Hacks.DisableRangeIndicator = false;
                Hacks.IngameChat = false;
                Hacks.RenderWatermark = true;
                Hacks.TowerRanges = true;
                Hacks.Console = true;
                
            }
        
            */
        }
        
        /// <summary>
        /// The RandomizePosition function to randomize click location.
        /// </summary>
        /// <param name="input">
        /// The input Vector3.
        /// </param>
        /// <returns>
        /// A Vector within 100 units of the unit
        /// </returns>
        public static Vector3 RandomizePosition(Vector3 input)
        {
            if (r.Next(2) == 0)
            {
                input.X += r.Next(100);
            }
            else
            {
                input.Y += r.Next(100);
            } 
            return input;
        }

        #endregion
    }
}