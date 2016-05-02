namespace Darius
{
    using System;

    using EloBuddy;

    using SharpDX;
    using SharpDX.Direct3D9;

    internal class HpBarIndicator
    {
        //Credits 
        public static Device dxDevice = Drawing.Direct3DDevice;

        public static Line dxLine;

        public AIHeroClient unit { get; set; }

        public float width = 104;

        public float hight = 9;

        public HpBarIndicator()
        {
            dxLine = new Line(dxDevice) { Width = 9 };

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;
        }

        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            dxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            dxLine.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            dxLine.OnLostDevice();
        }

        private Vector2 Offset
        {
            get
            {
                if (this.unit != null)
                {
                    return this.unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public Vector2 startPosition
        {
            get
            {
                return new Vector2(this.unit.HPBarPosition.X + this.Offset.X, this.unit.HPBarPosition.Y + this.Offset.Y);
            }
        }

        private float getHpProc(float dmg = 0)
        {
            float health = ((this.unit.Health - dmg) > 0) ? (this.unit.Health - dmg) : 0;
            return (health / this.unit.MaxHealth);
        }

        private Vector2 getHpPosAfterDmg(float dmg)
        {
            float w = this.getHpProc(dmg) * this.width;
            return new Vector2(this.startPosition.X + w, this.startPosition.Y);
        }

        public void drawDmg(float dmg, System.Drawing.Color color)
        {
            var hpPosNow = this.getHpPosAfterDmg(0);
            var hpPosAfter = this.getHpPosAfterDmg(dmg);

            this.fillHPBar(hpPosNow, hpPosAfter, color);
            //fillHPBar((int)(hpPosNow.X - startPosition.X), (int)(hpPosAfter.X- startPosition.X), color);
        }

        private void fillHPBar(int to, int from, System.Drawing.Color color)
        {
            Vector2 sPos = this.startPosition;

            for (int i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private void fillHPBar(Vector2 from, Vector2 to, System.Drawing.Color color)
        {
            dxLine.Begin();

            dxLine.Draw(
                new[] { new Vector2((int)from.X - 7, (int)from.Y - 11), new Vector2((int)to.X - 7, (int)to.Y - 11) },
                new ColorBGRA(120, 224, 69, 90));
            // Vector2 sPos = startPosition;
            //Drawing.DrawLine((int)from.X, (int)from.Y + 9f, (int)to.X, (int)to.Y + 9f, 9f, color);

            dxLine.End();
        }
    }
}