using Hax;

[Command("/unlock")]
internal class UnlockCommand : ICommand, ISecureGate {
    public void Execute(StringArray _) {
        this.SetSecureDoorState(true);
        Helper.FindObjects<DoorLock>()
              .ForEach(door => door.UnlockDoorSyncWithServer());

        Chat.Print("All doors unlocked!");
    }
}
