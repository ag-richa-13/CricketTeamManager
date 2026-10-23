
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TeamScreenUI : MonoBehaviour
{
    private enum MedalFilter
    {
        All,
        Gold,
        Silver,
        Bronze
    }

    [Header("Team Logic")]
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private PlayerDatabase playerDatabase;

    [Header("Owned Players")]
    [SerializeField] private GameObject ownedPlayersScrollView;
    [SerializeField] private GameObject ownedPlayersEmptyText;
    [SerializeField] private Transform ownedPlayersContainer;
    [SerializeField] private PlayerCardUI playerCardPrefab;

    [Header("Playing XI")]
    [SerializeField] private GameObject playingXIScrollView;
    [SerializeField] private GameObject playingXIEmptyText;
    [SerializeField] private Transform playingXIContainer;
    [SerializeField] private PlayerXIEntryUI playingXIEntryPrefab;

    [Header("Team Rating and Status")]
    [SerializeField] private TMP_Text teamRatingText;
    [SerializeField] private TMP_Text teamStatusText;

    [Header("Player Counts")]
    [SerializeField] private TMP_Text batterCountText;
    [SerializeField] private TMP_Text bowlerCountText;
    [SerializeField] private TMP_Text allRounderCountText;
    [SerializeField] private TMP_Text wicketkeeperCountText;
    [SerializeField] private TMP_Text totalPlayerCountText;

    [Header("Medal Counts")]
    [SerializeField] private TMP_Text goldCountText;
    [SerializeField] private TMP_Text silverCountText;
    [SerializeField] private TMP_Text bronzeCountText;
    [SerializeField] private TMP_Text allCountText;

    [Header("Medal Filter Buttons")]
    [SerializeField] private Button goldButton;
    [SerializeField] private Button silverButton;
    [SerializeField] private Button bronzeButton;
    [SerializeField] private Button allButton;

    private readonly Dictionary<string, PlayerCardUI> ownedCardUI =
        new Dictionary<string, PlayerCardUI>();

    private MedalFilter currentFilter = MedalFilter.All;
    private bool initialized;

    private void OnEnable()
    {
        if (teamManager != null)
            teamManager.TeamChanged += RefreshAll;

        AddButtonListeners();
    }

    private void Start()
    {
        if (!ReferencesAreValid())
        {
            enabled = false;
            return;
        }

        CreateOwnedPlayerCards();
        initialized = true;
        RefreshAll();
    }

    private void OnDisable()
    {
        if (teamManager != null)
            teamManager.TeamChanged -= RefreshAll;

        RemoveButtonListeners();
    }

    private bool ReferencesAreValid()
    {
        bool valid = true;

        if (teamManager == null || playerDatabase == null)
            valid = false;

        if (ownedPlayersScrollView == null ||
            ownedPlayersEmptyText == null ||
            ownedPlayersContainer == null ||
            playerCardPrefab == null)
            valid = false;

        if (playingXIScrollView == null ||
            playingXIEmptyText == null ||
            playingXIContainer == null ||
            playingXIEntryPrefab == null)
            valid = false;

        if (teamRatingText == null || teamStatusText == null ||
            batterCountText == null || bowlerCountText == null ||
            allRounderCountText == null || wicketkeeperCountText == null ||
            totalPlayerCountText == null)
            valid = false;

        if (goldCountText == null || silverCountText == null ||
            bronzeCountText == null || allCountText == null)
            valid = false;

        if (goldButton == null || silverButton == null ||
            bronzeButton == null || allButton == null)
            valid = false;

        if (!valid)
            Debug.LogError("TeamScreenUI: One or more Inspector references are missing.", this);

        return valid;
    }

    private void AddButtonListeners()
    {
        if (goldButton != null)
            goldButton.onClick.AddListener(ShowGold);

        if (silverButton != null)
            silverButton.onClick.AddListener(ShowSilver);

        if (bronzeButton != null)
            bronzeButton.onClick.AddListener(ShowBronze);

        if (allButton != null)
            allButton.onClick.AddListener(ShowAll);
    }

    private void RemoveButtonListeners()
    {
        if (goldButton != null)
            goldButton.onClick.RemoveListener(ShowGold);

        if (silverButton != null)
            silverButton.onClick.RemoveListener(ShowSilver);

        if (bronzeButton != null)
            bronzeButton.onClick.RemoveListener(ShowBronze);

        if (allButton != null)
            allButton.onClick.RemoveListener(ShowAll);
    }

    private void CreateOwnedPlayerCards()
    {
        ClearChildren(ownedPlayersContainer);
        ownedCardUI.Clear();

        foreach (PlayerData player in playerDatabase.Players)
        {
            if (player == null || string.IsNullOrWhiteSpace(player.PlayerId))
                continue;

            if (ownedCardUI.ContainsKey(player.PlayerId))
            {
                Debug.LogWarning($"Duplicate player ID: {player.PlayerId}");
                continue;
            }

            PlayerCardUI card = Instantiate(playerCardPrefab, ownedPlayersContainer);
            card.Bind(player);

            string playerId = player.PlayerId;
            PlayerGestureHandler gesture = card.GetComponent<PlayerGestureHandler>();

            if (gesture != null)
                gesture.Configure(() => TryAddPlayer(playerId));
            else
                Debug.LogError($"PlayerGestureHandler missing on {player.PlayerName}.", card);

            ownedCardUI.Add(playerId, card);
        }
    }

    private void RefreshAll()
    {
        if (!initialized || teamManager == null)
            return;

        RefreshPlayerCounts();
        RefreshTeamRating();
        RefreshTeamStatus();
        RefreshMedalCounts();
        RefreshOwnedPlayerIndicators();
        RefreshOwnedPlayerFilter();
        RefreshEmptyStates();
        RefreshPlayingXI();
    }

    private void RefreshPlayerCounts()
    {
        int batters = 0;
        int bowlers = 0;
        int allRounders = 0;
        int wicketkeepers = 0;

        foreach (PlayerData player in teamManager.SelectedPlayers)
        {
            if (player == null)
                continue;

            switch (player.Role)
            {
                case PlayerRole.Batter:
                    batters++;
                    break;
                case PlayerRole.Bowler:
                    bowlers++;
                    break;
                case PlayerRole.AllRounder:
                    allRounders++;
                    break;
                case PlayerRole.Wicketkeeper:
                    wicketkeepers++;
                    break;
            }
        }

        batterCountText.text = $"{batters}";
        bowlerCountText.text = $"{bowlers}";
        allRounderCountText.text = $"{allRounders}";
        wicketkeeperCountText.text = $"{wicketkeepers}";
        totalPlayerCountText.text = $"{teamManager.SelectedCount}";
    }

    private void RefreshTeamRating()
    {
        teamRatingText.text = $"Rating: {teamManager.TeamRating}";
    }

    private void RefreshTeamStatus()
    {
        int count = teamManager.SelectedCount;
        int wicketkeepers = 0;
        int specialistBowlers = 0;
        int captains = 0;

        foreach (PlayerData player in teamManager.SelectedPlayers)
        {
            if (player == null)
                continue;

            if (player.Role == PlayerRole.Wicketkeeper)
                wicketkeepers++;

            if (player.Role == PlayerRole.Bowler)
                specialistBowlers++;

            if (teamManager.IsCaptain(player.PlayerId))
                captains++;
        }

        string status;

        if (count == 11 && wicketkeepers >= 1 &&
            specialistBowlers >= 4 && captains == 1)
            status = "TEAM STATUS: READY — Playing XI is complete.";
        else if (count < 11)
            status = $"TEAM STATUS: INCOMPLETE — Select {11 - count} more player(s).";
        else if (count > 11)
            status = "TEAM STATUS: INCOMPLETE — Select 11 players.";
        else if (wicketkeepers < 1)
            status = "TEAM STATUS: INCOMPLETE — Select a wicketkeeper.";
        else if (specialistBowlers < 4)
            status = $"TEAM STATUS: INCOMPLETE — Select {4 - specialistBowlers} more specialist bowler(s).";
        else
            status = "TEAM STATUS: INCOMPLETE — Select a captain.";

        teamStatusText.text = status;
        teamStatusText.color = new Color32(255, 190, 55, 255);
    }

    private void RefreshMedalCounts()
    {
        int gold = 0;
        int silver = 0;
        int bronze = 0;
        int all = 0;

        foreach (PlayerData player in playerDatabase.Players)
        {
            if (player == null)
                continue;

            all++;

            switch (GetMedal(player))
            {
                case MedalFilter.Gold:
                    gold++;
                    break;
                case MedalFilter.Silver:
                    silver++;
                    break;
                case MedalFilter.Bronze:
                    bronze++;
                    break;
            }
        }

        goldCountText.text = $"Gold {gold}";
        silverCountText.text = $"Silver {silver}";
        bronzeCountText.text = $"Bronze {bronze}";
        allCountText.text = $"ALL [{all}]";
    }

    private MedalFilter GetMedal(PlayerData player)
    {
        if (player.OverallRating >= 80)
            return MedalFilter.Gold;

        if (player.OverallRating >= 60)
            return MedalFilter.Silver;

        return MedalFilter.Bronze;
    }

    public void ShowGold()
    {
        ApplyMedalFilter(MedalFilter.Gold);
    }

    public void ShowSilver()
    {
        ApplyMedalFilter(MedalFilter.Silver);
    }

    public void ShowBronze()
    {
        ApplyMedalFilter(MedalFilter.Bronze);
    }

    public void ShowAll()
    {
        ApplyMedalFilter(MedalFilter.All);
    }

    private void ApplyMedalFilter(MedalFilter filter)
    {
        currentFilter = filter;
        RefreshOwnedPlayerFilter();
        RefreshEmptyStates();
    }

    private void RefreshOwnedPlayerFilter()
    {
        if (playerDatabase == null)
            return;

        foreach (PlayerData player in playerDatabase.Players)
        {
            if (player == null)
                continue;

            if (!ownedCardUI.TryGetValue(player.PlayerId, out PlayerCardUI card))
                continue;

            if (card == null)
                continue;

            bool matchesFilter = currentFilter == MedalFilter.All ||
                                 GetMedal(player) == currentFilter;

            card.gameObject.SetActive(matchesFilter);
        }
    }

    private void RefreshOwnedPlayerIndicators()
    {
        foreach (KeyValuePair<string, PlayerCardUI> item in ownedCardUI)
        {
            if (item.Value == null)
                continue;

            item.Value.SetSelected(teamManager.IsSelected(item.Key));
            item.Value.SetCaptain(teamManager.IsCaptain(item.Key));
        }
    }

    private void RefreshEmptyStates()
    {
        int ownedVisibleCount = 0;

        foreach (PlayerData player in playerDatabase.Players)
        {
            if (player == null)
                continue;

            bool matchesFilter = currentFilter == MedalFilter.All ||
                                 GetMedal(player) == currentFilter;

            if (matchesFilter && ownedCardUI.ContainsKey(player.PlayerId))
                ownedVisibleCount++;
        }

        bool hasOwnedPlayers = ownedVisibleCount > 0;
        bool hasXIPlayers = teamManager.SelectedCount > 0;

        ownedPlayersScrollView.SetActive(hasOwnedPlayers);
        ownedPlayersEmptyText.SetActive(!hasOwnedPlayers);

        playingXIScrollView.SetActive(hasXIPlayers);
        playingXIEmptyText.SetActive(!hasXIPlayers);
    }

    private void RefreshPlayingXI()
    {
        ClearChildren(playingXIContainer);

        foreach (PlayerData player in teamManager.SelectedPlayers)
        {
            if (player == null)
                continue;

            PlayerXIEntryUI entry = Instantiate(playingXIEntryPrefab, playingXIContainer);
            entry.Bind(player);
            entry.SetSelected(true);
            entry.SetCaptain(teamManager.IsCaptain(player.PlayerId));

            string playerId = player.PlayerId;
            PlayerGestureHandler gesture = entry.GetComponent<PlayerGestureHandler>();

            if (gesture != null)
            {
                gesture.Configure(
                    () => TryRemovePlayer(playerId),
                    () => TrySetCaptain(playerId),
                    true);
            }
            else
            {
                Debug.LogError($"PlayerGestureHandler missing on {player.PlayerName}.", entry);
            }
        }
    }

    private void TryAddPlayer(string playerId)
    {
        PlayerData player = FindPlayer(playerId);

        if (player == null)
            return;

        if (!teamManager.TryAddPlayer(player, out string message))
            Debug.Log(message);
    }

    private void TryRemovePlayer(string playerId)
    {
        if (!teamManager.TryRemovePlayer(playerId, out string message))
            Debug.Log(message);
    }

    private void TrySetCaptain(string playerId)
    {
        if (!teamManager.TrySetCaptain(playerId, out string message))
            Debug.Log(message);
    }

    private PlayerData FindPlayer(string playerId)
    {
        foreach (PlayerData player in playerDatabase.Players)
        {
            if (player != null && player.PlayerId == playerId)
                return player;
        }

        return null;
    }

    private void ClearChildren(Transform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            GameObject child = container.GetChild(i).gameObject;
            child.SetActive(false);
            Destroy(child);
        }
    }
}
