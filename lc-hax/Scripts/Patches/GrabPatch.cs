#pragma warning disable IDE1006

using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;

namespace Hax;

[HarmonyPatch(typeof(PlayerControllerB), "SetHoverTipAndCurrentInteractTrigger")]
class GrabPatch {
    static void Postfix(
        ref int ___interactableObjectsMask,
        ref float ___grabDistance
    ) {
        ___interactableObjectsMask = LayerMask.GetMask(["Props", "InteractableObject"]);
        ___grabDistance = float.MaxValue;
    }
}
