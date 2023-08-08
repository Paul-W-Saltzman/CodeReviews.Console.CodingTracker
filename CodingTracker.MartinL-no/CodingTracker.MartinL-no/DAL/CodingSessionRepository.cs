﻿using Microsoft.Data.Sqlite;

using CodingTracker.MartinL_no.Models;

namespace CodingTracker.MartinL_no.DAL;

internal class CodingSessionRepository : ICodingSessionRepository
{
    private readonly string ConnString;
    private readonly string DbName;

    public CodingSessionRepository(string connString, string dbName)
    {
        ConnString = connString;
        DbName = dbName;
        CreateTable();
    }

    private void CreateTable()
    {
        using (var connection = new SqliteConnection($"{ConnString}{DbName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS CodingSession (
                    Id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
                    StartTime TEXT NOT NULL,
                    EndTime TEXT NOT NULL
                );
                """;
            command.ExecuteNonQuery();
        }
    }

    public List<CodingSession> GetCodingSessions()
    {
        using (var connection = new SqliteConnection($"{ConnString}{DbName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                SELECT C.Id, C.StartTime, C.EndTime
                FROM CodingSession AS C;
                """;

            using (var reader = command.ExecuteReader())
            {
                var codingSessions = new List<CodingSession>();

                if (!reader.HasRows) return codingSessions;

                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var startTime = DateTime.Parse(reader.GetString(1));
                    var endTime = DateTime.Parse(reader.GetString(2));

                    codingSessions.Add(new CodingSession(id, startTime, endTime));
                }

                return codingSessions;
            }
        }
    }

    public bool InsertCodingSession(CodingSession codingSession)
    {
        using (var connection = new SqliteConnection($"{ConnString}{DbName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO CodingSession (StartTime, EndTime)
                VALUES ($startTime, $endTime)
                """;

            command.Parameters.AddWithValue("$startTime", ToSqLiteDateFormat(codingSession.StartTime));
            command.Parameters.AddWithValue("$endTime", ToSqLiteDateFormat(codingSession.EndTime));

            return command.ExecuteNonQuery() != 0;
        }
    }

    public bool DeleteCodingSession(int id)
    {
        using (var connection = new SqliteConnection($"{ConnString}{DbName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                DELETE FROM CodingSession
                WHERE Id = $id;
                """;

            command.Parameters.AddWithValue("id", id);

            return command.ExecuteNonQuery() != 0;
        }
    }

    public bool UpdateCodingSession(CodingSession codingSession)
    {
        using (var connection = new SqliteConnection($"{ConnString}{DbName}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                UPDATE CodingSession
                SET StartTime = $startTime,
                    EndTime = $endTime
                WHERE
                    Id = $id;
                """;

            command.Parameters.AddWithValue("id", codingSession.Id);
            command.Parameters.AddWithValue("$startTime", ToSqLiteDateFormat(codingSession.StartTime));
            command.Parameters.AddWithValue("$endTime", ToSqLiteDateFormat(codingSession.EndTime));

            return command.ExecuteNonQuery() != 0;
        }
    }

    private string ToSqLiteDateFormat(DateTime dateTime)
    {
        return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
    }
}