using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GameNetcodeStuff;

namespace Hax;

internal static class Chat {
    internal static event Action<string>? OnExecuteCommandAttempt;

    static Dictionary<string, ICommand> Commands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<CommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<CommandAttribute>().Syntax,
                type => (ICommand)Activator.CreateInstance(type)
            );

    static Dictionary<string, ICommand> DebugCommands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<DebugCommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<DebugCommandAttribute>().Syntax,
                type => (ICommand)new DebugCommand((ICommand)Activator.CreateInstance(type))
            );

    static Dictionary<string, ICommand> PrivilegeCommands { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(type => typeof(ICommand).IsAssignableFrom(type))
            .Where(type => type.GetCustomAttribute<PrivilegedCommandAttribute>() is not null)
            .ToDictionary(
                type => type.GetCustomAttribute<PrivilegedCommandAttribute>().Syntax,
                type => (ICommand)new PrivilegedCommand((ICommand)Activator.CreateInstance(type))
            );

    internal static void Announce(string announcement, bool keepHistory = false) {
        if (Helper.LocalPlayer is not PlayerControllerB player) return;
        if (Helper.HUDManager is not HUDManager hudManager) return;

        string actualHistory = string.Join('\n', hudManager.ChatMessageHistory.Where(message =>
            !message.StartsWith("<color=#FF0000>USER</color>: <color=#FFFF00>'") &&
            !message.StartsWith("<color=#FF0000>SYSTEM</color>: <color=#FFFF00>'")
        ));

        string chatText = keepHistory ? $"{actualHistory}\n<color=#7069ff>{announcement}</color>" : announcement;

        hudManager.AddTextToChatOnServer(
            $"</color>\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n{chatText}<color=#FFFFFF00>",
            player.PlayerIndex()
        );
    }

    internal static void Clear() => Chat.Announce("");

    internal static void Print(string name, string? message, bool isSystem = false) {
        if (string.IsNullOrWhiteSpace(message) || Helper.HUDManager is not HUDManager hudManager) return;
        _ = hudManager.Reflect().InvokeInternalMethod("AddChatMessage", message, name);

        if (!isSystem && hudManager.localPlayer.isTypingChat) {
            hudManager.localPlayer.isTypingChat = false;
            hudManager.typingIndicator.enabled = false;
            hudManager.chatTextField.text = "";
            hudManager.PingHUDElement(hudManager.Chat, 1.0f, 1.0f, 0.2f);
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    internal static void Print(string? message) => Chat.Print("SYSTEM", message, true);

    internal static void ExecuteCommand(string command) {
        Chat.Print("USER", command);
        Chat.OnExecuteCommandAttempt?.Invoke(command);
        Chat.ExecuteCommand(command.Split(' '));
    }

    internal static void ExecuteCommand(StringArray args) {
        if (args.Length is 0) {
            Chat.Print("Usage: /<command> <args>");
            return;
        }

        ICommand? command =
            Chat.Commands.GetValue(args[0]) ??
            Chat.PrivilegeCommands.GetValue(args[0]) ??
            Chat.DebugCommands.GetValue(args[0]);

        if (command is null) {
            Chat.Print("The command is not found!");
            return;
        }

        command.Execute(args[1..]);
    }
}
