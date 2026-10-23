
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerCardUI : MonoBehaviour
{
    [Header("Player Information")]
    [SerializeField] private Image countryFlag;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text playerRoleText;
    [SerializeField] private TMP_Text overallRatingText;
    [SerializeField] private TMP_Text battingRatingText;
    [SerializeField] private TMP_Text bowlingRatingText;

    [Header("Indicators")]
    [SerializeField] private GameObject captainIndicator;
    [SerializeField] private GameObject selectedIndicator;

    public PlayerData Player { get; private set; }

    public void Bind(PlayerData player)
    {
        if (player == null)
        {
            Debug.LogError("PlayerCardUI: Player data is null.", this);
            return;
        }

        Player = player;

        if (countryFlag != null)
        {
            countryFlag.sprite = player.CountryFlag;
            countryFlag.enabled = player.CountryFlag != null;
        }

        if (playerNameText != null)
            playerNameText.text = player.PlayerName;

        if (playerRoleText != null)
            playerRoleText.text = GetRoleName(player.Role);

        if (overallRatingText != null)
            overallRatingText.text = player.OverallRating.ToString();

        if (battingRatingText != null)
            battingRatingText.text = player.BattingRating.ToString();

        if (bowlingRatingText != null)
            bowlingRatingText.text = player.BowlingRating.ToString();

        SetSelected(false);
        SetCaptain(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(selected);
    }

    public void SetCaptain(bool isCaptain)
    {
        if (captainIndicator != null)
            captainIndicator.SetActive(isCaptain);
    }

    private static string GetRoleName(PlayerRole role)
    {
        return role switch
        {
            PlayerRole.AllRounder => "All Rounder",
            PlayerRole.Wicketkeeper => "Wicketkeeper",
            _ => role.ToString()
        };
    }
}
