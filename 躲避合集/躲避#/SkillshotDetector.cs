using System;
using System.Collections.Generic;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu.Values;

namespace EvadePlus
{
    public class SkillshotDetector
    {
        #region "Events"

        public delegate void OnSkillshotDetectedDelegate(EvadeSkillshot skillshot, bool isProcessSpell);

        public event OnSkillshotDetectedDelegate OnSkillshotDetected;

        public delegate void OnSkillshotDeletedDelegate(EvadeSkillshot skillshot);

        public event OnSkillshotDeletedDelegate OnSkillshotDeleted;

        public delegate void OnUpdateSkillshotsDelegate(EvadeSkillshot skillshot, bool remove, bool isProcessSpell);

        public event OnUpdateSkillshotsDelegate OnUpdateSkillshots;

        public delegate void OnSkillshotActivationDelegate(EvadeSkillshot skillshot);

        public event OnSkillshotActivationDelegate OnSkillshotActivation;

        #endregion

        public DetectionTeam Detection;

        public readonly List<EvadeSkillshot> DetectedSkillshots = new List<EvadeSkillshot>();

        public IEnumerable<EvadeSkillshot> ActiveSkillshots
        {
            get { return DetectedSkillshots.Where(c => EvadeMenu.IsSkillshotEnabled(c) && c.IsValid && c.IsActive); }
        }

        public bool EnableFoWDetection
        {
            get { return EvadeMenu.MainMenu["fowDetection"].Cast<CheckBox>().CurrentValue; }
        }

        public bool LimitDetectionRange
        {
            get { return EvadeMenu.MainMenu["limitDetectionRange"].Cast<CheckBox>().CurrentValue; }
        }

        public int SkillshotActivationDelay
        {
            get { return EvadeMenu.MainMenu["skillshotActivationDelay"].Cast<Slider>().CurrentValue; }
        }

        public bool EnableSpellDetection
        {
            get { return EvadeMenu.MainMenu["processSpellDetection"].Cast<CheckBox>().CurrentValue; }
        }

        public SkillshotDetector(DetectionTeam detection = DetectionTeam.EnemyTeam)
        {
            Detection = detection;

            Game.OnTick += OnTick;
            GameObject.OnCreate += GameObjectOnCreate;
            GameObject.OnDelete += GameObjectOnDelete;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Spellbook.OnStopCast += OnStopCast;
            Drawing.OnDraw += OnDraw;
        }

        public void AddSkillshot(EvadeSkillshot skillshot, bool isProcessSpell = false, bool triggerEvent = true)
        {
            if (LimitDetectionRange && !skillshot.SpellData.IsGlobal &&
                skillshot.GetPosition().Distance(Player.Instance, true) > (2*skillshot.SpellData.Range).Pow())
            {
                return;
            }

            if (SkillshotActivationDelay <= 10)
                skillshot.IsActive = true;

            DetectedSkillshots.Add(skillshot);

            if (triggerEvent && EvadeMenu.IsSkillshotEnabled(skillshot))
            {
                if (OnSkillshotDetected != null)
                    OnSkillshotDetected(skillshot, isProcessSpell);

                if (OnUpdateSkillshots != null)
                    OnUpdateSkillshots(skillshot, false, isProcessSpell);
            }
        }

        public bool IsValidTeam(GameObjectTeam team)
        {
            if (team == GameObjectTeam.Unknown)
                return true;

            switch (Detection)
            {
                case DetectionTeam.AllyTeam:
                    return team.IsAlly();
                case DetectionTeam.EnemyTeam:
                    return team.IsEnemy();
                case DetectionTeam.AnyTeam:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return true;
        }

        private void OnTick(EventArgs args)
        {
            foreach (var skillshot in DetectedSkillshots.Where(v => !v.IsValid))
            {
                if (!skillshot.CastComplete)
                {
                    if (OnSkillshotDeleted != null)
                        OnSkillshotDeleted(skillshot);

                    if (OnUpdateSkillshots != null)
                        OnUpdateSkillshots(skillshot, true, false);
                }

                skillshot.OnDispose();
            }

            DetectedSkillshots.RemoveAll(v => !v.IsValid);

            foreach (var c in DetectedSkillshots)
            {
                if (!c.IsActive && Environment.TickCount >= c.TimeDetected + SkillshotActivationDelay)
                {
                    c.IsActive = true;

                    if (OnSkillshotActivation != null)
                    {
                        OnSkillshotActivation(c);
                    }
                }
            }

            foreach (var c in DetectedSkillshots)
                c.OnTick();
        }

        private void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            //Chat.Print(args.SData.Name);

            if (!EnableSpellDetection)
            {
                return;
            }

            if (args.End.Distance(sender, true) < 10.Pow())
            {
                return;
            }

            var skillshot =
                SkillshotDatabase.Database.FirstOrDefault(
                    evadeSkillshot => evadeSkillshot.SpellData.SpellName == args.SData.Name);

            if (skillshot != null && IsValidTeam(sender.Team))
            {
                var nSkillshot = skillshot.NewInstance();
                nSkillshot.SkillshotDetector = this;
                nSkillshot.Caster = sender;
                nSkillshot.CastArgs = args;
                nSkillshot.SData = args.SData;
                nSkillshot.Team = Utils.GetGameObjectTeam(sender);

                nSkillshot.OnCreate(null);
                nSkillshot.OnSpellDetection(sender, args);
                AddSkillshot(nSkillshot, true);
            }
        }

        private void GameObjectOnCreate(GameObject sender, EventArgs args)
        {
            //if (Utils.GetTeam(sender) == Utils.PlayerTeam())
            //    Chat.Print("create {0} {1} {2} {3}", sender.Team, sender.GetType().ToString(), Utils.GetGameObjectName(sender), sender.Index);

            if (!(sender is Obj_GeneralParticleEmitter))
            {
                var skillshot =
                    EvadeMenu.MenuSkillshots.Values.FirstOrDefault(
                        evadeSkillshot => evadeSkillshot.SpellData.MissileSpellName == Utils.GetGameObjectName(sender));

                if (skillshot != null)
                {
                    var nSkillshot = skillshot.NewInstance();
                    nSkillshot.SkillshotDetector = this;
                    nSkillshot.SpawnObject = sender;
                    nSkillshot.Team = Utils.GetGameObjectTeam(sender);
                    nSkillshot.OnCreate(sender);

                    if (IsValidTeam(nSkillshot.Team) && (EnableFoWDetection || !nSkillshot.IsFromFow()))
                    {
                        var triggerEvent = true;
                        var missile = (sender as MissileClient);

                        if (missile != null)
                        {
                            var castSkillshot =
                                DetectedSkillshots.FirstOrDefault(
                                    c => c.Caster != null && c.Caster.IndexEquals(missile.SpellCaster));

                            if (castSkillshot != null)
                            {
                                nSkillshot.TimeDetected = castSkillshot.TimeDetected;
                                nSkillshot.IsActive = castSkillshot.IsActive;
                                castSkillshot.CastComplete = true;
                                triggerEvent = false;
                            }
                        }

                        AddSkillshot(nSkillshot, false, triggerEvent);
                    }
                }
            }


            foreach (var c in DetectedSkillshots)
                c.OnCreateObject(sender);
        }

        private void GameObjectOnDelete(GameObject sender, EventArgs args)
        {
            //if (Utils.GetTeam(sender) == Utils.PlayerTeam())
            //    Chat.Print("delete {0} {1} {2} {3}", sender.Team, sender.GetType().ToString(), Utils.GetGameObjectName(sender), sender.Index);

            foreach (
                var c in DetectedSkillshots.Where(v => v.SpawnObject != null && v.SpawnObject.IndexEquals(sender)))
            {
                if (c.OnDelete(sender))
                    c.IsValid = false;
            }

            foreach (var c in DetectedSkillshots)
                c.OnDeleteObject(sender);
        }

        private void OnStopCast(Obj_AI_Base sender, SpellbookStopCastEventArgs args)
        {
            if (sender == null)
            {
                return;
            }

            if (args.ForceStop || args.StopAnimation)
            {
                foreach (
                    var c in
                        DetectedSkillshots.Where(
                            v => v.SpawnObject == null && v.Caster != null && v.Caster.IndexEquals(sender)))
                {
                    c.IsValid = false;
                }
            }
        }

        private void OnDraw(EventArgs args)
        {
            if (EvadeMenu.DrawMenu["disableAllDrawings"].Cast<CheckBox>().CurrentValue)
            {
                return;
            }

            foreach (var c in DetectedSkillshots)
                if (EvadeMenu.IsSkillshotDrawingEnabled(c))
                    c.OnDraw();
        }
    }

    public enum DetectionTeam
    {
        AllyTeam,
        EnemyTeam,
        AnyTeam,
    }
}
