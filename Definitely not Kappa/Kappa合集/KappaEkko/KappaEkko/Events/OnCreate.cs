namespace KappaEkko.Events
{
    using System;

    using EloBuddy;

    internal class OnCreate
    {
        public static void Create(GameObject sender, EventArgs args)
        {
            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Spells.EkkoREmitter = particle;
                }
            }

            var miss = sender as MissileClient;
            if (miss != null && miss.IsValid)
            {
                if (miss.SpellCaster.IsMe && miss.SpellCaster.IsValid && (miss.SData.Name == "EkkoQMis" || miss.SData.Name == "EkkoQReturn"))
                {
                    Spells.EkkoQMissile = miss;
                }
            }
        }
    }
}