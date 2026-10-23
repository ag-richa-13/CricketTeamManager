
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "PlayerDatabase",
    menuName = "Cricket Team/Player Database"
)]
public class PlayerDatabase : ScriptableObject
{
    [Header("All Owned Players")]
    [SerializeField] private List<PlayerData> players = new();

    public IReadOnlyList<PlayerData> Players => players;

    public bool TryGetPlayer(string playerId, out PlayerData player)
    {
        player = null;

        if (string.IsNullOrWhiteSpace(playerId))
            return false;

        foreach (PlayerData item in players)
        {
            if (item != null && item.PlayerId == playerId)
            {
                player = item;
                return true;
            }
        }

        return false;
    }

    public bool ValidateDatabase(out string error)
    {
        var ids = new HashSet<string>();

        if (players.Count < 20)
        {
            error = "The database must contain at least 20 players.";
            return false;
        }

        int wicketkeepers = 0;
        int bowlers = 0;

        foreach (PlayerData player in players)
        {
            if (player == null)
            {
                error = "The database contains an empty player reference.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(player.PlayerId))
            {
                error = $"Player '{player.PlayerName}' has no ID.";
                return false;
            }

            if (!ids.Add(player.PlayerId))
            {
                error = $"Duplicate player ID: {player.PlayerId}";
                return false;
            }

            if (string.IsNullOrWhiteSpace(player.PlayerName))
            {
                error = $"Player '{player.PlayerId}' has no name.";
                return false;
            }

            if (player.CountryFlag == null)
            {
                error = $"Player '{player.PlayerName}' has no country flag.";
                return false;
            }

            if (player.RoleIcon == null)
            {
                error = $"Player '{player.PlayerName}' has no role icon.";
                return false;
            }

            if (player.Role == PlayerRole.Wicketkeeper)
                wicketkeepers++;

            if (player.Role == PlayerRole.Bowler)
                bowlers++;
        }

        if (wicketkeepers < 2)
        {
            error = "Add at least 2 wicketkeepers.";
            return false;
        }

        if (bowlers < 6)
        {
            error = "Add at least 6 bowlers.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
