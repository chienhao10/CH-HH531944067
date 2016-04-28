using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace ezEvade
{
    public class HeroInfo
    {
        public AIHeroClient hero;
        public Vector2 serverPos2D;
        public Vector2 serverPos2DExtra;
        public Vector2 serverPos2DPing;
        public Vector2 currentPosition;
        public bool isMoving;
        public float boundingRadius;
        public float moveSpeed;

        public HeroInfo(AIHeroClient hero)
        {
            this.hero = hero;
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            UpdateInfo();
        }

        public void UpdateInfo()
        {
            var extraDelayBuffer = 30; //ObjectCache.menuCache.cache["ExtraPingBuffer"].Cast<Slider>().CurrentValue;

            serverPos2D = hero.ServerPosition.To2D(); //CalculatedPosition.GetPosition(hero, Game.Ping);
            serverPos2DExtra = EvadeUtils.GetGamePosition(hero, Game.Ping + extraDelayBuffer);
            serverPos2DPing = EvadeUtils.GetGamePosition(hero, Game.Ping);
            //CalculatedPosition.GetPosition(hero, Game.Ping + extraDelayBuffer);            
            currentPosition = hero.Position.To2D(); //CalculatedPosition.GetPosition(hero, 0); 
            boundingRadius = hero.BoundingRadius;
            moveSpeed = hero.MoveSpeed;
            isMoving = hero.IsMoving;
        }
    }

    public class CacheDictionary 
    {
        private List<Menu> list = new List<Menu>();

        public void Add(Menu value)
        {
            list.Add(value);
        }

        public ValueBase this[string item]
        {
            get
            {
                foreach (var menu in list)
                {
                    var control = menu[item];

                    if (control != null)
                    {
                        return control;
                    }
                }

                Console.WriteLine("------------> " + item);
                return null;
            }
        }
    }

    public class MenuCache
    {
        public Menu menu;
        public CacheDictionary cache = new CacheDictionary();

        public MenuCache(Menu menu)
        {
            this.menu = menu;

            AddMenuToCache(menu);
        }

        public void AddMenuToCache(Menu newMenu)
        {
            if (newMenu.IsSubMenu)
            {
                cache.Add(newMenu);
            }

            //foreach (var submenu in newMenu.SubMenus)
            //{
            //    cache.Add(submenu);

            //    AddMenuToCache(submenu);
            //}
        }

        //public void AddMenuToCache(Menu newMenu)
        //{
        //    foreach (var item in ReturnAllItems(newMenu))
        //    {
        //        AddMenuItemToCache(item);
        //    }
        //}

        //public void AddMenuItemToCache(ValueBase item)
        //{
        //    if (item != null && !cache.ContainsKey(item.DisplayName))
        //    {
        //        cache.Add(item.SerializationId, item);
        //    }
        //}

        //public static List<ValueBase> ReturnAllItems(Menu menu)
        //{
        //    List<ValueBase> menuList = new List<ValueBase>();

        //    menuList.AddRange(menu.LinkedValues.Values);

        //    foreach (var submenu in menu.SubMenus)
        //    {
        //        menuList.AddRange(ReturnAllItems(submenu));
        //    }

        //    return menuList;
        //}
    }

    public static class ObjectCache
    {
        public static Dictionary<int, Obj_AI_Turret> turrets = new Dictionary<int, Obj_AI_Turret>();

        private static AIHeroClient myHero { get { return ObjectManager.Player; } }

        public static HeroInfo myHeroCache = new HeroInfo(myHero);
        public static MenuCache menuCache = new MenuCache(Evade.menu);

        public static float gamePing = 0;

        static ObjectCache()
        {
            InitializeCache();
            Game.OnUpdate += Game_OnGameUpdate;
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            gamePing = Game.Ping;
        }

        private static void InitializeCache()
        {
            foreach (var obj in ObjectManager.Get<Obj_AI_Turret>())
            {
                if (!turrets.ContainsKey(obj.NetworkId))
                {
                    turrets.Add(obj.NetworkId, obj);
                }
            }
        }
    }
}
