using HarmonyLib;
using Kitchen;
using System;
using System.Reflection;
using KitchenWalkThisWay.Components;
using KitchenWalkThisWay.Systems;
using Unity.Entities;

namespace KitchenWalkThisWay.Patches
{
    [HarmonyPatch]
    public class GroupHandleGoingToTablePatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod()
        {
            Type type = AccessTools.FirstInner(typeof(GroupHandleGoingToTable), t => t.Name.Contains($"c__DisplayClass_OnUpdate_LambdaJob0"));
            return AccessTools.FirstMethod(type, method => method.Name.Contains("OriginalLambdaBody"));
        }

        [HarmonyPrefix]
        static bool OriginalLambdaBody_Prefix(in DynamicBuffer<CGroupMember> group)
        {
            foreach (CGroupMember cGroupMember in group)
            {
                if (StaticHelper.HasComponent<CPathIntercept>(cGroupMember) &&
                    !StaticHelper.HasComponent<CCompletedIntercept>(cGroupMember))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
