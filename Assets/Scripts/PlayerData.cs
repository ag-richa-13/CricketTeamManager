
using UnityEngine;

public enum PlayerRole
{
    Batter,
    Bowler,
    AllRounder,
    Wicketkeeper
}

[CreateAssetMenu(
    fileName = "NewPlayer",
    menuName = "Cricket Team/Player Data"
)]
public class PlayerData : ScriptableObject
{
    [Header("Player Information")]
    [SerializeField] private string playerId;
    [SerializeField] private string playerName;
    [SerializeField] private PlayerRole role;

    [Header("Ratings (0-100)")]
    [Range(0, 100)]
    [SerializeField] private int overallRating;

    [Range(0, 100)]
    [SerializeField] private int battingRating;

    [Range(0, 100)]
    [SerializeField] private int bowlingRating;

    [Header("Visuals")]
    [SerializeField] private Sprite countryFlag;
    [SerializeField] private Sprite roleIcon;

    public string PlayerId => playerId;
    public string PlayerName => playerName;
    public PlayerRole Role => role;

    public int OverallRating => overallRating;
    public int BattingRating => battingRating;
    public int BowlingRating => bowlingRating;

    public Sprite CountryFlag => countryFlag;
    public Sprite RoleIcon => roleIcon;
}
