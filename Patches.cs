using System;
using System.Collections.Generic;
using BoplFixedMath;
using HarmonyLib;
using UnityEngine;

namespace DoubleJump
{
    internal class Patches
    {
        [HarmonyPatch(typeof(SlimeController), "OldUpdate")]
        class Patch1
        {
            private static Dictionary<SlimeController, int> nJumps = new Dictionary<SlimeController, int>();
            private static bool wasGrounded;
            static void Prefix(SlimeController __instance)
            {
                wasGrounded = __instance.GetComponent<PlayerPhysics>().IsGrounded();
            }
            static void Postfix(SlimeController __instance)
            {
                Player player = PlayerHandler.Get().GetPlayer(__instance.playerNumber);
                PlayerPhysics playerPhysics = __instance.GetComponent<PlayerPhysics>();
                if (!nJumps.ContainsKey(__instance)) nJumps.Add(__instance, Plugin.maxNJumps.Value);
                if (playerPhysics.IsGrounded()) nJumps[__instance] = Plugin.maxNJumps.Value;
                if (player.jumpButton_PressedThisFrame() && nJumps[__instance] > 0 && !wasGrounded)
                {
                    nJumps[__instance]--;
                    bool flag = playerPhysics.getAttachedGround() == null ? false : playerPhysics.getAttachedGround().currentNormal(__instance.body).y < 0L;
                    Traverse.Create(__instance).Field("wasUpsideDown").SetValue(flag);
                    playerPhysics.Jump();
                }
            }
        }

        [HarmonyPatch(typeof(PlayerPhysics), "Jump")]
        class Patch2
        {
            static bool Prefix(PlayerPhysics __instance)
            {
                __instance.jumpedThisFrame = true;
                Vec2 vec;
                bool isGrounded = Traverse.Create(__instance).Field("isGrounded").GetValue() is true;
                PlayerBody body = __instance.GetComponent<PlayerBody>();
                if (!isGrounded)
                {
                    vec = body.Velocity;
                    vec.y = Fix.One;
                    vec.x = Fix.Min(Fix.One, Fix.Max((Fix)(-1), vec.x));
                    Vec2 v = new Vec2(vec.x, Fix.Zero);
                    Vec2 v2 = Vec2.up;
                    Fix fix = (vec.y + Fix.One) / __instance.jumpNormalScaleFactor;
                    fix = Fix.Min(Fix.One, fix);
                    fix = Fix.Max(Fix.Zero, fix);
                    v2 *= fix;
                    v2 += v * __instance.jumpExtraXStrength;
                    Vec2 v3 = new Vec2(vec.y, -vec.x) * __instance.groundedSpeed;
                    body.selfImposedVelocity = v3 * __instance.jumpKeptMomentum + v2 * __instance.jumpStrength;
                    body.position = body.position + body.selfImposedVelocity * __instance.extraJumpTeleportMultiplier;
                    __instance.transform.position = (Vector3)body.position;
                    GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("slimeJump" + __instance.GetComponent<IPlayerIdHolder>().GetPlayerId());
                    return false;
                }
                return true;
            }
        }
    }
}