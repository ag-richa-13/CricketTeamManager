
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public sealed class PlayerDataGenerator : EditorWindow
{
    [Serializable]
    private class PlayerDefinition
    {
        public string id;
        public string name;
        public string country;
        public PlayerRole role;
        public int overall;
        public int batting;
        public int bowling;

        public PlayerDefinition(
            string id, string name, string country,
            PlayerRole role, int overall, int batting, int bowling)
        {
            this.id = id;
            this.name = name;
            this.country = country;
            this.role = role;
            this.overall = overall;
            this.batting = batting;
            this.bowling = bowling;
        }
    }

    private PlayerDatabase database;

    private DefaultAsset spriteFolder;
    private DefaultAsset outputFolder;

    private Sprite indiaFlag;
    private Sprite southAfricaFlag;
    private Sprite englandFlag;
    private Sprite newZealandFlag;

    private Sprite batterIcon;
    private Sprite bowlerIcon;
    private Sprite allRounderIcon;
    private Sprite wicketkeeperIcon;

    private Vector2 scrollPosition;

    private string statusMessage =
        "Assign the database and sprites, then generate players.";

    private MessageType statusType = MessageType.Info;

    private const string DefaultOutputPath =
        "Assets/Data/GeneratedPlayers";


    private static readonly PlayerDefinition[] Definitions =
    {
    new("IND001", "Shubman Gill", "India", PlayerRole.Batter, 89, 92, 28),
    new("IND002", "Suryakumar Yadav", "India", PlayerRole.Batter, 91, 96, 20),
    new("IND003", "Rishabh Pant", "India", PlayerRole.Wicketkeeper, 88, 90, 15),
    new("IND004", "Sanju Samson", "India", PlayerRole.Wicketkeeper, 75, 89, 10),
    new("IND005", "Jasprit Bumrah", "India", PlayerRole.Bowler, 95, 18, 98),
    new("IND006", "Arshdeep Singh", "India", PlayerRole.Bowler, 72, 20, 87),
    new("IND007", "Hardik Pandya", "India", PlayerRole.AllRounder, 87, 85, 82),
    new("IND008", "Ravindra Jadeja", "India", PlayerRole.AllRounder, 58, 82, 91),

    new("SA001", "Aiden Markram", "South Africa", PlayerRole.Batter, 87, 89, 35),
    new("SA002", "David Miller", "South Africa", PlayerRole.Batter, 77, 94, 15),
    new("SA003", "Quinton de Kock", "South Africa", PlayerRole.Wicketkeeper, 90, 94, 10),
    new("SA004", "Ryan Rickelton", "South Africa", PlayerRole.Wicketkeeper, 64, 87, 10),
    new("SA005", "Kagiso Rabada", "South Africa", PlayerRole.Bowler, 92, 25, 96),
    new("SA006", "Anrich Nortje", "South Africa", PlayerRole.Bowler, 69, 18, 91),
    new("SA007", "Marco Jansen", "South Africa", PlayerRole.AllRounder, 88, 80, 90),
    new("SA008", "George Linde", "South Africa", PlayerRole.AllRounder, 49, 75, 83),

    new("ENG001", "Harry Brook", "England", PlayerRole.Batter, 91, 95, 25),
    new("ENG002", "Joe Root", "England", PlayerRole.Batter, 74, 93, 32),
    new("ENG003", "Jos Buttler", "England", PlayerRole.Wicketkeeper, 92, 97, 10),
    new("ENG004", "Phil Salt", "England", PlayerRole.Wicketkeeper, 67, 92, 10),
    new("ENG005", "Jofra Archer", "England", PlayerRole.Bowler, 89, 30, 94),
    new("ENG006", "Adil Rashid", "England", PlayerRole.Bowler, 71, 20, 90),
    new("ENG007", "Sam Curran", "England", PlayerRole.AllRounder, 86, 83, 85),
    new("ENG008", "Will Jacks", "England", PlayerRole.AllRounder, 54, 88, 74),

    new("NZ001", "Finn Allen", "New Zealand", PlayerRole.Batter, 88, 96, 15),
    new("NZ002", "Daryl Mitchell", "New Zealand", PlayerRole.Batter, 73, 92, 40),
    new("NZ003", "Devon Conway", "New Zealand", PlayerRole.Wicketkeeper, 89, 93, 10),
    new("NZ004", "Tom Latham", "New Zealand", PlayerRole.Wicketkeeper, 62, 87, 10),
    new("NZ005", "Matt Henry", "New Zealand", PlayerRole.Bowler, 87, 20, 92),
    new("NZ006", "Lockie Ferguson", "New Zealand", PlayerRole.Bowler, 68, 18, 93),
    new("NZ007", "Mitchell Santner", "New Zealand", PlayerRole.AllRounder, 87, 78, 91),
    new("NZ008", "Rachin Ravindra", "New Zealand", PlayerRole.AllRounder, 45, 89, 81)
};

    [MenuItem("Tools/Cricket Team/Player Data Generator")]
    private static void OpenWindow()
    {
        PlayerDataGenerator window =
            GetWindow<PlayerDataGenerator>("Player Generator");

        window.minSize = new Vector2(360f, 300f);
        window.Show();
    }

    private void OnEnable()
    {
        if (outputFolder == null)
            outputFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(
                DefaultOutputPath);

        if (spriteFolder == null)
            spriteFolder = AssetDatabase.LoadAssetAtPath<DefaultAsset>(
                "Assets/Sprites");

        if (database == null)
        {
            string[] matches = AssetDatabase.FindAssets(
                "MainPlayerDatabase t:PlayerDatabase");

            if (matches.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(matches[0]);
                database = AssetDatabase.LoadAssetAtPath<PlayerDatabase>(path);
            }
        }
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(
            "Cricket Player Dataset Generator",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "Creates 32 editable PlayerData assets for India, " +
            "South Africa, England and New Zealand. " +
            "Ratings are illustrative game values.",
            MessageType.Info);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            "Database and Folders", EditorStyles.boldLabel);

        database = (PlayerDatabase)EditorGUILayout.ObjectField(
            "Player Database", database,
            typeof(PlayerDatabase), false);

        spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Sprites Folder", spriteFolder,
            typeof(DefaultAsset), false);

        outputFolder = (DefaultAsset)EditorGUILayout.ObjectField(
            "Output Folder", outputFolder,
            typeof(DefaultAsset), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(
            "Country Flags", EditorStyles.boldLabel);

        indiaFlag = (Sprite)EditorGUILayout.ObjectField(
            "India", indiaFlag, typeof(Sprite), false);

        southAfricaFlag = (Sprite)EditorGUILayout.ObjectField(
            "South Africa", southAfricaFlag, typeof(Sprite), false);

        englandFlag = (Sprite)EditorGUILayout.ObjectField(
            "England", englandFlag, typeof(Sprite), false);

        newZealandFlag = (Sprite)EditorGUILayout.ObjectField(
            "New Zealand", newZealandFlag, typeof(Sprite), false);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField(
            "Role Icons", EditorStyles.boldLabel);

        batterIcon = (Sprite)EditorGUILayout.ObjectField(
            "Batter", batterIcon, typeof(Sprite), false);

        bowlerIcon = (Sprite)EditorGUILayout.ObjectField(
            "Bowler", bowlerIcon, typeof(Sprite), false);

        allRounderIcon = (Sprite)EditorGUILayout.ObjectField(
            "All Rounder", allRounderIcon, typeof(Sprite), false);

        wicketkeeperIcon = (Sprite)EditorGUILayout.ObjectField(
            "Wicketkeeper", wicketkeeperIcon, typeof(Sprite), false);

        EditorGUILayout.Space(8);

        EditorGUILayout.HelpBox(
            "Assign the existing sprites by dragging them from " +
            "the Project window. The sprite folder field is " +
            "for reference; flags and icons are assigned explicitly.",
            MessageType.None);

        EditorGUILayout.Space(8);

        EditorGUILayout.LabelField(
            $"Dataset: {Definitions.Length} players",
            EditorStyles.boldLabel);

        if (GUILayout.Button("Generate / Update Players", GUILayout.Height(36)))
            GeneratePlayers();

        EditorGUILayout.Space(8);
        EditorGUILayout.HelpBox(statusMessage, statusType);

        EditorGUILayout.Space(8);
        EditorGUILayout.EndScrollView();
    }

    private void GeneratePlayers()
    {
        if (database == null)
        {
            SetStatus("Assign MainPlayerDatabase first.", MessageType.Error);
            return;
        }

        if (indiaFlag == null || southAfricaFlag == null ||
            englandFlag == null || newZealandFlag == null)
        {
            SetStatus("Assign all four country flag sprites.", MessageType.Error);
            return;
        }

        if (batterIcon == null || bowlerIcon == null ||
            allRounderIcon == null || wicketkeeperIcon == null)
        {
            SetStatus("Assign all four role icons.", MessageType.Error);
            return;
        }

        string outputPath = outputFolder != null
            ? AssetDatabase.GetAssetPath(outputFolder)
            : DefaultOutputPath;

        if (string.IsNullOrEmpty(outputPath) ||
            !AssetDatabase.IsValidFolder(outputPath))
        {
            outputPath = DefaultOutputPath;
        }

        EnsureFolderExists(outputPath);

        var databaseSerialized = new SerializedObject(database);
        SerializedProperty playerList =
            databaseSerialized.FindProperty("players");

        if (playerList == null || !playerList.isArray)
        {
            SetStatus(
                "Could not find the 'players' list in PlayerDatabase.cs.",
                MessageType.Error);
            return;
        }

        var generatedPlayers = new List<PlayerData>();
        var ids = new HashSet<string>();

        foreach (PlayerDefinition definition in Definitions)
        {
            if (!ids.Add(definition.id))
            {
                SetStatus(
                    $"Duplicate ID in generator: {definition.id}",
                    MessageType.Error);
                return;
            }

            string assetPath =
                $"{outputPath}/{definition.id}_{MakeSafeFileName(definition.name)}.asset";

            PlayerData player =
                AssetDatabase.LoadAssetAtPath<PlayerData>(assetPath);

            if (player == null)
            {
                player = CreateInstance<PlayerData>();
                AssetDatabase.CreateAsset(player, assetPath);
            }

            ApplyPlayerData(player, definition);
            generatedPlayers.Add(player);
        }

        // Preserve existing database players not generated by this tool.
        var finalPlayers = new List<PlayerData>();
        var finalIds = new HashSet<string>();

        for (int i = 0; i < playerList.arraySize; i++)
        {
            PlayerData existing =
                playerList.GetArrayElementAtIndex(i).objectReferenceValue
                as PlayerData;

            if (existing == null || ids.Contains(existing.PlayerId))
                continue;

            if (finalIds.Add(existing.PlayerId))
                finalPlayers.Add(existing);
        }

        foreach (PlayerData player in generatedPlayers)
        {
            if (finalIds.Add(player.PlayerId))
                finalPlayers.Add(player);
        }

        databaseSerialized.Update();
        playerList = databaseSerialized.FindProperty("players");
        playerList.arraySize = finalPlayers.Count;

        for (int i = 0; i < finalPlayers.Count; i++)
        {
            playerList.GetArrayElementAtIndex(i).objectReferenceValue =
                finalPlayers[i];
        }

        databaseSerialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(database);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        bool valid = database.ValidateDatabase(out string validationMessage);

        SetStatus(
            valid
                ? $"Success! Generated/updated {generatedPlayers.Count} players. " +
                  $"Database now contains {finalPlayers.Count} players."
                : $"Players generated, but validation failed: {validationMessage}",
            valid ? MessageType.Info : MessageType.Warning);

        Selection.activeObject = database;
        EditorGUIUtility.PingObject(database);
    }

    private void ApplyPlayerData(
        PlayerData player, PlayerDefinition definition)
    {
        SerializedObject serialized = new SerializedObject(player);

        SetString(serialized, "playerId", definition.id);
        SetString(serialized, "playerName", definition.name);

        SerializedProperty roleProperty =
            serialized.FindProperty("role");

        if (roleProperty != null)
            roleProperty.enumValueIndex = (int)definition.role;

        SetInt(serialized, "overallRating", definition.overall);
        SetInt(serialized, "battingRating", definition.batting);
        SetInt(serialized, "bowlingRating", definition.bowling);

        SetSprite(serialized, "countryFlag", GetCountryFlag(definition.country));
        SetSprite(serialized, "roleIcon", GetRoleIcon(definition.role));

        serialized.ApplyModifiedProperties();
        EditorUtility.SetDirty(player);
    }

    private Sprite GetCountryFlag(string country)
    {
        switch (country)
        {
            case "India": return indiaFlag;
            case "South Africa": return southAfricaFlag;
            case "England": return englandFlag;
            case "New Zealand": return newZealandFlag;
            default: return null;
        }
    }

    private Sprite GetRoleIcon(PlayerRole role)
    {
        switch (role)
        {
            case PlayerRole.Batter: return batterIcon;
            case PlayerRole.Bowler: return bowlerIcon;
            case PlayerRole.AllRounder: return allRounderIcon;
            case PlayerRole.Wicketkeeper: return wicketkeeperIcon;
            default: return null;
        }
    }

    private static void SetString(
        SerializedObject obj, string propertyName, string value)
    {
        SerializedProperty property = obj.FindProperty(propertyName);

        if (property != null)
            property.stringValue = value;
    }

    private static void SetInt(
        SerializedObject obj, string propertyName, int value)
    {
        SerializedProperty property = obj.FindProperty(propertyName);

        if (property != null)
            property.intValue = Mathf.Clamp(value, 0, 100);
    }

    private static void SetSprite(
        SerializedObject obj, string propertyName, Sprite value)
    {
        SerializedProperty property = obj.FindProperty(propertyName);

        if (property != null)
            property.objectReferenceValue = value;
    }

    private static string MakeSafeFileName(string playerName)
    {
        foreach (char invalid in Path.GetInvalidFileNameChars())
            playerName = playerName.Replace(invalid, '_');

        return playerName.Replace(' ', '_');
    }

    private static void EnsureFolderExists(string folderPath)
    {
        string[] parts = folderPath.Split('/');
        string currentPath = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string nextPath = currentPath + "/" + parts[i];

            if (!AssetDatabase.IsValidFolder(nextPath))
                AssetDatabase.CreateFolder(currentPath, parts[i]);

            currentPath = nextPath;
        }
    }

    private void SetStatus(string message, MessageType type)
    {
        statusMessage = message;
        statusType = type;
        Repaint();
    }
}
