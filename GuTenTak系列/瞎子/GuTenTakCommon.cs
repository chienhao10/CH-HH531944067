using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using System;
using GuTenTak.LeeSin;

namespace GuTenTak.LeeSin
{
    internal class Common : Program
    {

        private static readonly string[] QSpellNames = { string.Empty, "BlindMonkQOne", "BlindMonkQTwo" };

        //c
        public static void Combo()
        {
            var Target = TargetSelector.GetTarget(Q1.Range, DamageType.Physical);
            if (Target == null) return;
            var useQ = ModesMenu1["ComboQ"].Cast<CheckBox>().CurrentValue;
            var useQ2 = ModesMenu1["ComboQ2"].Cast<CheckBox>().CurrentValue;
            var Qp = Q1.GetPrediction(Target);
            if (!Target.IsValid()) return;
            if (Q1.IsReady() && useQ && !Target.IsInvulnerable && Q1.Name.Equals("BlindMonkQOne", StringComparison.InvariantCultureIgnoreCase))
            {
                if (Qp.HitChance >= HitChance.High)
                {
                    Q1.Cast(Qp.CastPosition);
                }
            }

            if (Q2.IsReady() && useQ2 && !Target.IsInvulnerable && Q2.Name.Equals("blindmonkqtwo", StringComparison.InvariantCultureIgnoreCase))
            {
                Q2.Cast();
            }

        }
    }
}