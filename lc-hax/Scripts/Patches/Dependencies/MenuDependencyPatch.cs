using HarmonyLib;
using Hax;

[HarmonyPatch(typeof(QuickMenuManager), nameof(QuickMenuManager.LeaveGameConfirm))]
class MenuDependencyPatch {
    static void Postfix() => State.DisconnectedVoluntarily = true;
}
