
using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TeamManager : MonoBehaviour
{
    public const int MaxPlayers = 11;

    [SerializeField] private PlayerDatabase playerDatabase;

    private readonly List<PlayerData> selectedPlayers = new();
    private readonly HashSet<string> selectedIds = new();

    private string captainId;

    public event Action TeamChanged;

    public IReadOnlyList<PlayerData> SelectedPlayers => selectedPlayers;

    public PlayerData Captain =>
        selectedPlayers.Find(p => p.PlayerId == captainId);

    public int SelectedCount => selectedPlayers.Count;

    public int WicketkeeperCount =>
        selectedPlayers.FindAll(
            p => p.Role == PlayerRole.Wicketkeeper).Count;

    public int BowlerCount =>
        selectedPlayers.FindAll(
            p => p.Role == PlayerRole.Bowler).Count;

    public int TeamRating
    {
        get
        {
            if (selectedPlayers.Count == 0)
                return 0;

            int total = 0;

            foreach (PlayerData player in selectedPlayers)
                total += player.OverallRating;

            return (int)Math.Floor(
                (double)total / selectedPlayers.Count + 0.5);
        }
    }

    public bool IsValid =>
        SelectedCount == MaxPlayers &&
        WicketkeeperCount >= 1 &&
        BowlerCount >= 4 &&
        Captain != null;

    private void Start()
    {
        if (playerDatabase == null)
        {
            Debug.LogError(
                "TeamManager: Assign PlayerDatabase in Inspector.", this);
            return;
        }

        if (!playerDatabase.ValidateDatabase(out string error))
            Debug.LogError($"Invalid player database: {error}", this);
    }

    public bool IsSelected(string playerId) =>
        selectedIds.Contains(playerId);

    public bool IsCaptain(string playerId) =>
        !string.IsNullOrEmpty(captainId) && captainId == playerId;

    public bool TryAddPlayer(PlayerData player, out string message)
    {
        if (player == null || string.IsNullOrWhiteSpace(player.PlayerId))
        {
            message = "Invalid player data.";
            return false;
        }

        if (IsSelected(player.PlayerId))
        {
            message = "Player is already selected.";
            return false;
        }

        if (selectedPlayers.Count >= MaxPlayers)
        {
            message = "Team is full. Maximum 11 players allowed.";
            return false;
        }

        selectedPlayers.Add(player);
        selectedIds.Add(player.PlayerId);

        message = $"{player.PlayerName} added.";
        TeamChanged?.Invoke();
        return true;
    }

    public bool TryRemovePlayer(string playerId, out string message)
    {
        int index = selectedPlayers.FindIndex(
            p => p.PlayerId == playerId);

        if (index < 0)
        {
            message = "Player is not in the Playing XI.";
            return false;
        }

        PlayerData player = selectedPlayers[index];
        selectedPlayers.RemoveAt(index);
        selectedIds.Remove(playerId);

        if (IsCaptain(playerId))
            captainId = null;

        message = $"{player.PlayerName} removed.";
        TeamChanged?.Invoke();
        return true;
    }

    public bool TrySetCaptain(string playerId, out string message)
    {
        if (!IsSelected(playerId))
        {
            message = "Select the player before assigning captain.";
            return false;
        }

        if (IsCaptain(playerId))
        {
            message = "This player is already captain.";
            return true;
        }

        captainId = playerId;
        message = $"{Captain.PlayerName} is now captain.";

        TeamChanged?.Invoke();
        return true;
    }

    public List<string> GetValidationIssues()
    {
        var issues = new List<string>();

        if (SelectedCount != MaxPlayers)
            issues.Add($"Select 11 players ({SelectedCount}/11).");

        if (WicketkeeperCount < 1)
            issues.Add("Select at least 1 wicketkeeper.");

        if (BowlerCount < 4)
            issues.Add($"Select at least 4 bowlers ({BowlerCount}/4).");

        if (Captain == null)
            issues.Add("Select a captain.");

        return issues;
    }
}
