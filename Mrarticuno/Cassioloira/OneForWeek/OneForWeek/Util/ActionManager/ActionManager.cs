using System;
using EloBuddy;
using OneForWeek.Model;
using OneForWeek.Model.ActionQueue;

namespace OneForWeek.Util.ActionManager
{
    static class ActionManager
    {
        public static void EnqueueAction(ActionQueueList list, Func<bool> preCondition, Action comboAction, Func<bool> conditionToRemove)
        {
            list.Add(new ActionQueueItem()
            {
                Time = Game.Time,
                PreConditionFunc = preCondition,
                ComboAction = comboAction,
                ConditionToRemoveFunc = conditionToRemove
            });
        }

        public static bool ExecuteNextAction(ActionQueueList list)
        {
            if (list.Count > 0)
            {
                if (Game.Time > list[0].Time + 2F)
                {
                    list.Remove(list[0]);
                    return true;
                }

                if (list[0].PreConditionFunc.Invoke())
                {
                    list[0].ComboAction.Invoke();
                }

                if (list[0].ConditionToRemoveFunc.Invoke() || Game.Time > list[0].Time + 1.5F)
                {
                    list.Remove(list[0]);
                    if (list.Count > 0)
                    {
                        var nextItem = list[0];
                        nextItem.Time = Game.Time;
                        list[0] = nextItem;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
