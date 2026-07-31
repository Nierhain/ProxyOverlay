using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using ProxyOverlay.Models;

namespace ProxyOverlay.Services;

public sealed class JsonlCardDatabase : ICardDatabase
{
    private readonly string _databasePath;

    public JsonlCardDatabase(string databasePath)
    {
        _databasePath = databasePath;
        var directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory)) Directory.CreateDirectory(directory);
        EnsureSchema();
    }

    public CardRecord? FindByName(string cardName)
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT name, layout, type_line, border_color, frame
            FROM cards
            WHERE name = $name COLLATE NOCASE
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$name", cardName);

        using var reader = command.ExecuteReader();
        return !reader.Read()
            ? null
            : new CardRecord(reader.GetString(0), reader.GetString(1), reader.GetString(2),
                reader.GetString(3), reader.GetString(4));
    }

    public async Task<int> ImportAsync(string jsonlPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(jsonlPath);

        await using var file = File.OpenRead(jsonlPath);
        using var reader = new StreamReader(file);
        using var connection = OpenConnection();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO cards (name, layout, type_line, border_color, frame)
            VALUES ($name, $layout, $type_line, $border_color, $frame)
            ON CONFLICT(name) DO UPDATE SET
                layout = excluded.layout,
                type_line = excluded.type_line,
                border_color = excluded.border_color,
                frame = excluded.frame;
            """;
        var name = command.Parameters.Add("$name", SqliteType.Text);
        var layout = command.Parameters.Add("$layout", SqliteType.Text);
        var typeLine = command.Parameters.Add("$type_line", SqliteType.Text);
        var borderColor = command.Parameters.Add("$border_color", SqliteType.Text);
        var frame = command.Parameters.Add("$frame", SqliteType.Text);

        var imported = 0;
        var lineNumber = 0;
        while (await reader.ReadLineAsync(cancellationToken) is { } line)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line)) continue;

            CardJson? card;
            try
            {
                card = JsonSerializer.Deserialize<CardJson>(line);
            }
            catch (JsonException exception)
            {
                throw new FormatException($"Invalid JSON on line {lineNumber}.", exception);
            }

            if (card is null || string.IsNullOrWhiteSpace(card.Name)) continue;
            name.Value = card.Name;
            layout.Value = card.Layout ?? string.Empty;
            typeLine.Value = card.TypeLine ?? string.Empty;
            borderColor.Value = card.BorderColor ?? string.Empty;
            frame.Value = card.Frame ?? string.Empty;
            command.ExecuteNonQuery();
            imported++;
        }

        transaction.Commit();
        return imported;
    }

    private void EnsureSchema()
    {
        using var connection = OpenConnection();
        using var command = connection.CreateCommand();
        command.CommandText = """
            CREATE TABLE IF NOT EXISTS cards (
                name TEXT NOT NULL PRIMARY KEY COLLATE NOCASE,
                layout TEXT NOT NULL,
                type_line TEXT NOT NULL,
                border_color TEXT NOT NULL,
                frame TEXT NOT NULL
            );
            """;
        command.ExecuteNonQuery();
    }

    private SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection($"Data Source={_databasePath};Pooling=False");
        connection.Open();
        return connection;
    }

    private sealed class CardJson
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("layout")]
        public string? Layout { get; set; }
        [JsonPropertyName("type_line")]
        public string? TypeLine { get; set; }
        [JsonPropertyName("border_color")]
        public string? BorderColor { get; set; }
        [JsonPropertyName("frame")]
        public string? Frame { get; set; }
    }
}
