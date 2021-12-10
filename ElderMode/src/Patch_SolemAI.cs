using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using Microsoft.Xna.Framework;
using SoG;

namespace ElderMode
{
    [HarmonyPatch(typeof(Behaviours.SolemAI))]
    public class Patch_SolemAI
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Behaviours.SolemAI.OnUpdate))]
        internal static void OnUpdatePostfix(Behaviours.SolemAI __instance)
        {
            if (__instance.recHomeRectangle.IsEmpty)
            {
                // Don't act on the original Sol-Gem
                return;
            }

            if (__instance.enCurrentAction == Behaviours.SolemAI.Action.PatternShockwaves_GetToPosition)
            {
                if (__instance.xSolgemOnly.iNumberOfSideShockwaveSmashesInitiated % 2 == 0)
                {
                    Rectangle home = __instance.recHomeRectangle;
                    __instance.xSolgemOnly.v2ShockwaveSideTarget = new Vector2(home.Left + 20, home.Bottom);
                }
                else
                {
                    Rectangle home = __instance.recHomeRectangle;
                    __instance.xSolgemOnly.v2ShockwaveSideTarget = new Vector2(home.Right - 20, home.Bottom);
                }
            }
            else if (__instance.enCurrentAction == Behaviours.SolemAI.Action.HalfmoonUltrablast_GetToPosition)
            {
                Rectangle home = __instance.recHomeRectangle;

                Vector2 target = new Vector2((home.Left + home.Right) / 2, home.Top - 20);
                Vector2 moveDirection = Utility.Normalize(target - __instance.xOwner.xTransform.v2Pos);
                Vector2 undoMoveDirection = Utility.Normalize(new Vector2(320f, 865f));

                if (Utility.IsWithinRange(__instance.xOwner.xRenderComponent.iActiveAnimation, 20, 21))
                {
                    __instance.xOwner.xTransform.v2Pos -= undoMoveDirection * __instance.xOwner.xBaseStats.fMovementSpeed;
                    __instance.xOwner.xTransform.v2Pos += moveDirection * __instance.xOwner.xBaseStats.fMovementSpeed;
                }
                if (Vector2.Distance(target, __instance.xOwner.xTransform.v2Pos) < __instance.xOwner.xBaseStats.fMovementSpeed)
                {
                    __instance.xOwner.xRenderComponent.SwitchAnimation(6);
                    __instance.enCurrentAction = Behaviours.SolemAI.Action.HalfmoonUltrablast_SlamSlamFiesta;
                    __instance.iCounter = 0;
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Behaviours.SolemAI.InstructionsFromServer))]
        internal static void InstructionsFromServerPostfix(Behaviours.SolemAI __instance)
        {
            if (__instance.recHomeRectangle.IsEmpty)
            {
                // Don't act on the original Sol-Gem
                return;
            }

            if (__instance.enCurrentAction == Behaviours.SolemAI.Action.PatternShockwaves_GetToPosition)
            {
                if (__instance.xSolgemOnly.v2ShockwaveSideTarget.X < 300.0f)
                {
                    Rectangle home = __instance.recHomeRectangle;
                    __instance.xSolgemOnly.v2ShockwaveSideTarget = new Vector2(home.Left + 20, home.Bottom);
                }
                else
                {
                    Rectangle home = __instance.recHomeRectangle;
                    __instance.xSolgemOnly.v2ShockwaveSideTarget = new Vector2(home.Right - 20, home.Bottom);
                }
            }
            else if (__instance.enCurrentAction == Behaviours.SolemAI.Action.HalfmoonUltrablast_GetToPosition)
            {
                Rectangle home = __instance.recHomeRectangle;

                Vector2 target = new Vector2((home.Left + home.Right) / 2, home.Top - 20);
                Vector2 moveDirection = Utility.Normalize(target - __instance.xOwner.xTransform.v2Pos);
                Vector2 undoMoveDirection = Utility.Normalize(new Vector2(320f, 865f));

                if (Utility.IsWithinRange(__instance.xOwner.xRenderComponent.iActiveAnimation, 20, 21))
                {
                    __instance.xOwner.xTransform.v2Pos -= undoMoveDirection * __instance.xOwner.xBaseStats.fMovementSpeed;
                    __instance.xOwner.xTransform.v2Pos += moveDirection * __instance.xOwner.xBaseStats.fMovementSpeed;
                }
                if (Vector2.Distance(target, __instance.xOwner.xTransform.v2Pos) < __instance.xOwner.xBaseStats.fMovementSpeed)
                {
                    __instance.xOwner.xRenderComponent.SwitchAnimation(6);
                    __instance.enCurrentAction = Behaviours.SolemAI.Action.HalfmoonUltrablast_SlamSlamFiesta;
                    __instance.iCounter = 0;
                }
            }
        }
    }
}
