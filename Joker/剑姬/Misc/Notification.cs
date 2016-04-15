using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using JokerFioraBuddy.Properties;
using EloBuddy;
using EloBuddy.SDK.Rendering;
using SharpDX;
using Color = System.Drawing.Color;

namespace JokerFioraBuddy.Misc
{
    static class Notification
    {
        static List<NotificationModel> _notifications = new List<NotificationModel>();
        public static readonly TextureLoader TextureLoader = new TextureLoader();
        private static Sprite MainBar { get; set; }
        private static readonly Text Text = new Text("", new Font(FontFamily.GenericSansSerif, 12, FontStyle.Regular)) { Color = Color.White };

        static Notification()
        {
            TextureLoader.Load("notification", Resources.notification);
        }

        public static void DrawNotification(NotificationModel notification)
        {
            _notifications.Add(notification);

            MainBar = new Sprite(() => TextureLoader["notification"]);

            Init();
            Drawing.OnEndScene += OnDraw;
        }

        private static void Init()
        {
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += OnDomainUnload;
        }

        private static void OnDraw(EventArgs args)
        {
            if (_notifications.Count != 0)
            {
                var lastNotifPos = new Vector2();
                var auxNotifications = new List<NotificationModel>();

                foreach (var notificationModel in _notifications)
                {
                    var diffTime = (notificationModel.StartTimer + notificationModel.AnimationTimer + notificationModel.ShowTimer) - Game.Time;

                    var animationEnd = notificationModel.StartTimer + notificationModel.AnimationTimer - Game.Time;

                    var diffPos = 0f;

                    if (animationEnd > 0)
                    {
                        diffPos = 200 * animationEnd;
                    }

                    if (diffTime > 0)
                    {
                        if (lastNotifPos.Equals(new Vector2()))
                        {
                            var pos = new Vector2(Drawing.Width - 930, y: Drawing.Height / 13.5f - diffPos - 15);
                            MainBar.Draw(pos);
                            lastNotifPos = pos;
                            Text.TextValue = notificationModel.ShowText;

                            var vector1 = new Vector2(Text.Bounding.Width + 25, Text.Bounding.Height + 400);
                            var vector2 = new Vector2(Resources.notification.Size.Width,
                                Resources.notification.Size.Height);

                            pos += (vector2 - vector1) / 2;

                            Text.Position = pos;
                            Text.Color = notificationModel.Color;
                            Text.Draw();
                        }
                        else
                        {
                            var pos = new Vector2(lastNotifPos.X, y: lastNotifPos.Y);
                            MainBar.Draw(pos);
                            lastNotifPos = pos;
                            Text.TextValue = notificationModel.ShowText;

                            var vector1 = new Vector2(Text.Bounding.Width, Text.Bounding.Height);
                            var vector2 = new Vector2(Resources.notification.Size.Width,
                                Resources.notification.Size.Height);

                            pos += (vector2 - vector1) / 2;

                            Text.Position = pos;
                            Text.Color = notificationModel.Color;
                            Text.Draw();
                        }
                    }
                    else
                    {
                        auxNotifications.Add(notificationModel);
                    }
                }

                if (auxNotifications.Count > 0)
                {
                    _notifications = _notifications.Except(auxNotifications).ToList();
                }
            }
            else
            {
                Drawing.OnDraw -= OnDraw;
            }
        }

        public static bool IsDivisble(int x, int n)
        {
            return (x % n) == 0;
        }

        private static void OnDomainUnload(object sender, EventArgs e)
        {
            TextureLoader.Dispose();
        }

    }
}
