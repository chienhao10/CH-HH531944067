#region

using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using EloBuddy;

#endregion

namespace Syndra
{
    public static class OrbManager
    {
        private static readonly Dictionary<Obj_AI_Minion, float> _seeds = new Dictionary<Obj_AI_Minion, float>();
        private static readonly Dictionary<Obj_AI_Minion, float> _k = new Dictionary<Obj_AI_Minion, float>();
        private static readonly Dictionary<Vector3, float> _kConfirmed = new Dictionary<Vector3, float>();
        private static float lastR = 0;
        private static float lastW = 0;
        
        public static void init()
        {
            GameObject.OnCreate += GameObject_OnCreate;
            AIHeroClient.OnProcessSpellCast += Obj_AI_Hero_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Obj_AI_Hero_OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "SyndraQ")
                addK(args.End);
            if (sender.IsMe && args.SData.Name == "SyndraR")
            {
                _seeds.Clear();
                lastR = Game.Time;
            }
            if (sender.IsMe && args.SData.Name == "SyndraW")
            {
                lastW = Game.Time;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Get_Current_Orb(true) != null)
                _seeds[Get_Current_Orb(true)] = lastW;

            for (int i = _seeds.Count - 1; i >= 0; i--)
                if (Game.Time - _seeds.ElementAt(i).Value >= 6)
                        _seeds.Remove(_seeds.ElementAt(i).Key);

            for (int i = _k.Count - 1; i >= 0; i--)
                if (Game.Time - _k.ElementAt(i).Value >= 0.6)
                    _k.Remove(_k.ElementAt(i).Key);

            for (int i = _kConfirmed.Count - 1; i >= 0; i--)
                if (Game.Time - _kConfirmed.ElementAt(i).Value >= 0.6)
                    _kConfirmed.Remove(_kConfirmed.ElementAt(i).Key);
        }


        public static Obj_AI_Minion Grab_Shit(bool orbOnly = false)
        {
            if (!orbOnly)
            {
                var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => x.IsValid && Vector3.Distance(x.Position, Player.Instance.Position) < Program.W.Range);
                if (minion != null)
                    return minion;
            }
            return _seeds.FirstOrDefault().Key;
        }

        public static Obj_AI_Minion Get_Current_Orb(bool orbOnly = false)
        {
            var orb = _seeds.FirstOrDefault(x => x.Key.Team == Player.Instance.Team && x.Key.Name == "Seed" && !x.Key.IsTargetable).Key;
            if (orb != null)
                return orb;
            if (orbOnly)
                return null;
            var minion = ObjectManager.Get<Obj_AI_Minion>().FirstOrDefault(x => Vector3.Distance(Player.Instance.Position, x.Position) < Program.W.Range+50 
                                                                            && !x.IsTargetable && x.Name != "Seed" && x.Name != "k");

            return minion;
        }



        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (!(sender is Obj_AI_Minion) || !sender.IsAlly || (sender.Name != "k" && sender.Name != "Seed"))
                return;

           // Console.WriteLine(sender.Name + " " + Game.Time);
            if (sender.Name == "k" && sender.IsAlly)
            {
                _k.Add((Obj_AI_Minion)sender, Game.Time);
                //Chat.Print("New K " + _k.Count);
            }
            if (sender.Name == "Seed" && sender.IsAlly)
            {
                Obj_AI_Minion orb = (Obj_AI_Minion)sender;
                if (Game.Time - lastR > 0.5)
                    _seeds.Add(orb, Game.Time);
               // Console.WriteLine(_seeds.Count + " " + Game.Time);
                //Console.WriteLine("Added seed");
                //Chat.Print("new seed " + _seeds.Count);
            }
        }

     /*   private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            
            if (!(sender is Obj_AI_Minion))
                return;
            Chat.Print(sender.Name);
            if (_k.Remove((Obj_AI_Minion)sender))
            {
                //Console.WriteLine("Removed Orb" + " " + Game.Time);
                Chat.Print("removing" + _k.Count);
            }
        }*/

        public static List<Obj_AI_Minion> GetAllOrbs()
        {
            List<Obj_AI_Minion> all = new List<Obj_AI_Minion>(_seeds.Keys);
            foreach(var orb in _k)
            {
                foreach (var orbConfirmed in _kConfirmed)
                    if (Vector3.Distance(orb.Key.Position, orbConfirmed.Key) == 0)
                        all.Add(orb.Key);
            }
            //all.AddRange(_kConfirmed.Keys);
            return all;
        }

        public static void addK(Vector3 orb)
        {
            _kConfirmed.Add(orb, Game.Time);
        }
    }
}