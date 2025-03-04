using System.Linq;
using Hax;

[Command("/players")]
internal class PlayersCommand : ICommand {
    public void Execute(StringArray args) => Chat.Print(
        string.Join('\n', Helper.Players.Select(player => $"{player.playerClientId}: {player.playerUsername}"))
    );
}
