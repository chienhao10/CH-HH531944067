using System;
using System.Drawing;
using LeagueSharp.Common;
using VayneHunter_Reborn.External;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace VayneHunter_Reborn.Utility.MenuUtility
{
    class MenuGenerator
    {

        public static Menu comboMenu, harassMenu, farmMenu, miscMenu, drawMenu;

        public static void OnLoad()
        {
            var RootMenu = Variables.Menu;

            comboMenu = RootMenu.AddSubMenu("[VHR] 连招", "dz191.vhr.combo");
            comboMenu.AddGroupLabel("蓝量控制器");
            comboMenu.AddManaLimiter(Enumerations.Skills.Q, "combo");
            comboMenu.AddManaLimiter(Enumerations.Skills.E, "combo");
            comboMenu.AddManaLimiter(Enumerations.Skills.R, "combo");
            comboMenu.AddSeparator();
            comboMenu.AddSkill(Enumerations.Skills.Q, "combo");
            comboMenu.AddSkill(Enumerations.Skills.E, "combo");
            comboMenu.AddSkill(Enumerations.Skills.R, "combo", false);
            comboMenu.AddSeparator();
            comboMenu.Add("dz191.vhr.combo.r.minenemies", new Slider("最低附近敌人数量使用 R", 2, 1, 5));
            comboMenu.Add("dz191.vhr.combo.q.2wstacks", new CheckBox("只对两层W目标使用 Q", false));

            harassMenu = RootMenu.AddSubMenu("[VHR] 骚扰", "dz191.vhr.mixed");
            harassMenu.AddGroupLabel("蓝量控制器");
            harassMenu.AddManaLimiter(Enumerations.Skills.Q, "harass");
            harassMenu.AddManaLimiter(Enumerations.Skills.E, "harass");
            harassMenu.AddSeparator();
            harassMenu.AddSkill(Enumerations.Skills.Q, "harass");
            harassMenu.AddSkill(Enumerations.Skills.E, "harass");
            harassMenu.AddSeparator();
            harassMenu.Add("dz191.vhr.mixed.q.2wstacks", new CheckBox("只对两层W目标使用 Q"));
            harassMenu.Add("dz191.vhr.mixed.ethird", new CheckBox("使用 E 触发第三W"));

            farmMenu = RootMenu.AddSubMenu("[VHR] 农兵", "dz191.vhr.farm");
            farmMenu.AddSkill(Enumerations.Skills.Q, "laneclear");
            farmMenu.AddManaLimiter(Enumerations.Skills.Q, "清线", 45, true);
            farmMenu.AddSeparator();
            farmMenu.AddSkill(Enumerations.Skills.Q, "lasthit");
            farmMenu.AddManaLimiter(Enumerations.Skills.Q, "尾兵", 45, true);
            farmMenu.AddSeparator();
            farmMenu.Add("dz191.vhr.farm.condemnjungle", new CheckBox("使用 E 晕眩野怪", true));
            farmMenu.Add("dz191.vhr.farm.qjungle", new CheckBox("对野怪使用 Q", true));

            miscMenu = RootMenu.AddSubMenu("[VHR] 杂项", "dz191.vhr.misc");
            miscMenu.AddGroupLabel("杂项 - Q ");
            miscMenu.Add("dz191.vhr.misc.condemn.qlogic", new ComboBox("Q 逻辑", 0, "重生", "正常", "风筝近程", "Kurisu"));
            miscMenu.Add("dz191.vhr.mixed.mirinQ", new CheckBox("尝试 Q 至墙上 (Mirin 模式)", true));
            miscMenu.Add("dz191.vhr.misc.tumble.smartq", new CheckBox("尝试 QE"));
            miscMenu.Add("dz191.vhr.misc.tumble.noaastealthex", new KeyBind("隐身时不普攻按键", false, KeyBind.BindTypes.PressToggle, 'K'));
            miscMenu.Add("dz191.vhr.misc.tumble.noaastealthex.hp", new Slider("当血量低于 x 时", 35, 0, 100));
            miscMenu.Add("dz191.vhr.misc.tumble.ijava", new CheckBox("iJava 隐身")); //Done
            miscMenu.Add("dz191.vhr.misc.tumble.noaastealth.duration", new Slider("等待时间 (iJava模式下使用)", 700, 0, 1000));
            miscMenu.Add("dz191.vhr.misc.tumble.noqenemies", new CheckBox("不Q进敌人"));
            miscMenu.Add("dz191.vhr.misc.tumble.noqenemies.old", new CheckBox("不Q进敌人 （旧版）"));
            miscMenu.Add("dz191.vhr.misc.tumble.dynamicqsafety", new CheckBox("使用 安全动态Q距离"));
            miscMenu.Add("dz191.vhr.misc.tumble.qspam", new CheckBox("无视 Q 冷却检查"));
            miscMenu.Add("dz191.vhr.misc.tumble.qinrange", new CheckBox("Q 抢头", true));
            miscMenu.Add("dz191.vhr.misc.tumble.noaa.enemies", new Slider("最低 X 敌人附近隐身时普攻", 3, 2, 5));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("杂项 - E (定墙)");
            miscMenu.Add("dz191.vhr.misc.condemn.condemnmethod", new ComboBox("定墙模式", 0, "VH 革命", "VH 重生", "神射手/Gosu", "Shine#"));
            miscMenu.Add("dz191.vhr.misc.condemn.pushdistance", new Slider("E 推行距离", 420, 350, 470));
            miscMenu.Add("dz191.vhr.misc.condemn.accuracy", new Slider("准确度 (只用于革命模式)", 45, 1, 65));
            miscMenu.Add("dz191.vhr.misc.condemn.enextauto", new KeyBind("下一普攻后 E 按键", false, KeyBind.BindTypes.PressToggle, 'T'));
            miscMenu.Add("dz191.vhr.misc.condemn.flashcondemn", new KeyBind("闪现E", false, KeyBind.BindTypes.HoldActive, 'W'));
            miscMenu.Add("dz191.vhr.misc.condemn.onlystuncurrent", new CheckBox("只允许当前目标"));
            miscMenu.Add("dz191.vhr.misc.condemn.autoe", new CheckBox("自动 E"));
            miscMenu.Add("dz191.vhr.misc.condemn.eks", new CheckBox("智能 E 抢头"));
            miscMenu.Add("dz191.vhr.misc.condemn.noeaa", new Slider("当目标可被 X 下击杀不使用E", 1, 0, 4));
            miscMenu.Add("dz191.vhr.misc.condemn.trinketbush", new CheckBox("E 后自动草丛插眼", true));
            miscMenu.Add("dz191.vhr.misc.condemn.lowlifepeel", new CheckBox("低血量 E 防守"));
            miscMenu.Add("dz191.vhr.misc.condemn.condemnflag", new CheckBox("E 至皇子旗子", true));
            miscMenu.Add("dz191.vhr.misc.condemn.noeturret", new CheckBox("敌人塔下不 E"));
            miscMenu.Add("dz191.vhr.misc.condemn.repelflash", new CheckBox("对闪现的敌人使用 E"));
            miscMenu.Add("dz191.vhr.misc.condemn.repelkindred", new CheckBox("使用 E 推出千玗大招"));
            miscMenu.AddSeparator();
            miscMenu.AddGroupLabel("杂项 - 一般");
            miscMenu.Add("dz191.vhr.misc.general.antigp", new CheckBox("防突进"));
            miscMenu.Add("dz191.vhr.misc.general.interrupt", new CheckBox("技能打断", true));
            miscMenu.Add("dz191.vhr.misc.general.antigpdelay", new Slider("防突进延迟 (毫秒)", 0, 0, 1000));
            miscMenu.Add("dz191.vhr.misc.general.specialfocus", new CheckBox("集火 2 层W 目标"));
            miscMenu.Add("dz191.vhr.misc.general.reveal", new CheckBox("反隐身 (真眼 / 扫)"));
            miscMenu.Add("dz191.vhr.misc.general.disablemovement", new CheckBox("屏蔽走砍移动", false));
            miscMenu.Add("dz191.vhr.misc.general.disableattk", new CheckBox("屏蔽走砍攻击", false));

            drawMenu = RootMenu.AddSubMenu("[VHR] 线圈", "dz191.vhr.draw");
            drawMenu.Add("dz191.vhr.draw.spots", new CheckBox("显示位置", false));
            drawMenu.Add("dz191.vhr.draw.range", new CheckBox("显示 敌人范围", false));
            drawMenu.Add("dz191.vhr.draw.condemn", new CheckBox("显示 定墙 方格", false));
            drawMenu.Add("dz191.vhr.draw.qpos", new CheckBox("重生Q 位置 (调试)", false));

            //  CustomAntigapcloser.BuildMenu(RootMenu);
            DZLib.Core.DZAntigapcloser.BuildMenu(RootMenu, "[VHR] AntiGapclosers List", "dz191.vhr.agplist");
        }
    }
}
