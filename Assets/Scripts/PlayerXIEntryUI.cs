
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class PlayerXIEntryUI : MonoBehaviour
{
    [SerializeField] private Image roleIcon;
    [SerializeField] private TMP_Text playerNameText;
    [SerializeField] private TMP_Text captainIndicatorText;
    [SerializeField] private TMP_Text playerRatingText;
    [SerializeField] private GameObject selectedIndicator;

    public PlayerData Player { get; private set; }

    public void Bind(PlayerData player)
    {
        if (player == null)
            return;

        Player = player;

        if (roleIcon != null)
            roleIcon.sprite = player.RoleIcon;

        if (playerNameText != null)
            playerNameText.text = player.PlayerName;

        if (playerRatingText != null)
            playerRatingText.text = player.OverallRating.ToString();

        if (captainIndicatorText != null)
            captainIndicatorText.gameObject.SetActive(false);

        if (selectedIndicator != null)
            selectedIndicator.SetActive(false);
    }

    public void SetCaptain(bool isCaptain)
    {
        if (captainIndicatorText != null)
        {
            captainIndicatorText.text = isCaptain ? "(C)" : "";
            captainIndicatorText.gameObject.SetActive(isCaptain);
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedIndicator != null)
            selectedIndicator.SetActive(selected);
    }
}
