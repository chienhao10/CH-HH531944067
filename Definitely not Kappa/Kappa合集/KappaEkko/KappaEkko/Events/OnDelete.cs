namespace KappaEkko.Events
{
    using System;

    using EloBuddy;

    internal class OnDelete
    {
        public static void Delete(GameObject sender, EventArgs args)
        {
            var particle = sender as Obj_GeneralParticleEmitter;
            if (particle != null)
            {
                if (particle.Name.Equals("Ekko_Base_R_TrailEnd.troy"))
                {
                    Spells.EkkoREmitter = null;
                }
            }

            var miss = sender as MissileClient;
            if (miss == null || !miss.IsValid)
            {
                return;
            }

            if (miss.SpellCaster is AIHeroClient && miss.SpellCaster.IsValid && miss.SpellCaster.IsMe
                && (miss.SData.Name == "EkkoQMis" || miss.SData.Name == "EkkoQReturn"))
            {
                Spells.EkkoQMissile = null;
            }
        }
    }
}