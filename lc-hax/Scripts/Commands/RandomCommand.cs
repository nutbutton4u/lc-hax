using UnityEngine;
using GameNetcodeStuff;

namespace Hax;

public class RandomCommand : ICommand {
    Result TeleportPlayerToRandom(string[] args) {
        if (!Helpers.Extant(Helpers.GetPlayer(args[0]), out PlayerControllerB targetPlayer)) {
            return new Result(message: "Player not found!");
        }

        Helpers.BuyUnlockable(Unlockable.INVERSE_TELEPORTER);
        HaxObjects.Instance?.ShipTeleporters.Renew();

        if (!Helpers.Extant(Helpers.InverseTeleporter, out ShipTeleporter inverseTeleporter)) {
            return new Result(message: "ShipTeleporter not found!");
        }

        if (!Helpers.Extant(Helpers.GetUnlockable(Unlockable.CUPBOARD), out PlaceableShipObject cupboard)) {
            return new Result(message: "Cupboard not found!");
        }

        GameObject previousTeleporterTransform = Helpers.Copy(inverseTeleporter.transform);
        GameObject previousCupboardTransform = Helpers.Copy(cupboard.transform);

        Vector3 teleporterPositionOffset = new(0.0f, 1.5f, 0.0f);
        Vector3 teleporterRotationOffset = new(-90.0f, 0.0f, 0.0f);

        inverseTeleporter.PressTeleportButtonServerRpc();

        _ = Helpers.CreateComponent<TransientBehaviour>()
            .Init(Helpers.PlaceObjectAtTransform(targetPlayer.transform, inverseTeleporter, teleporterPositionOffset, teleporterRotationOffset), 6.0f)
            .Dispose(() => Helpers.PlaceObjectAtTransform(previousTeleporterTransform.transform, inverseTeleporter, teleporterPositionOffset, teleporterRotationOffset).Invoke(0));

        _ = Helpers.CreateComponent<TransientBehaviour>()
            .Init(Helpers.PlaceObjectAtPosition(targetPlayer.transform, cupboard, new Vector3(0.0f, 1.75f, 0.0f), new Vector3(90.0f, 0.0f, 0.0f)), 6.0f)
            .Dispose(() => Helpers.PlaceObjectAtTransform(previousCupboardTransform.transform, cupboard).Invoke(0));

        return new Result(true);
    }

    public void Execute(string[] args) {
        if (args.Length < 1) {
            Console.Print("SYSTEM", "Usage: /random <player>");
            return;
        }

        Result result = this.TeleportPlayerToRandom(args);

        if (!result.Success) {
            Console.Print("SYSTEM", result.Message);
        }
    }
}
