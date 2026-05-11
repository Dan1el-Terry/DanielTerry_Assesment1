using DanielTerry_Assesment1.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DanielTerry_Assesment1.Helpers
{
    /// <summary>
    /// Static helper for reading and writing data to JSON files.
    /// Handles both the movie list and the notifications log.
    /// </summary>
    public static class FileHelper
    {
        // Shared serialiser options — pretty-print so exported files are human-readable
        private static readonly JsonSerializerOptions _options =
            new JsonSerializerOptions { WriteIndented = true };


        // ── MOVIES ───────────────────────────────────────────────────────────────

        /// <summary>
        /// Serialises the list of movies to a JSON file at the given path.
        /// Overwrites the file if it already exists.
        /// </summary>
        public static void ExportJson(List<Movie> movies, string path)
        {
            string json = JsonSerializer.Serialize(movies, _options);
            File.WriteAllText(path, json);
        }

        /// <summary>
        /// Reads a JSON file and deserialises it into a list of Movie objects.
        /// Returns an empty list (not null) if the file is empty or malformed.
        /// </summary>
        public static List<Movie> ImportJson(string path)
        {
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
        }


        // ── NOTIFICATIONS ─────────────────────────────────────────────────────────

        /// <summary>
        /// Exports the notifications log (generated when queued borrowers are assigned
        /// a returned movie) to a separate JSON file.
        /// This satisfies the brief requirement: "add notifications to a queue to be exported".
        /// </summary>
        public static void ExportNotifications(List<string> notifications, string path)
        {
            string json = JsonSerializer.Serialize(notifications, _options);
            File.WriteAllText(path, json);
        }
    }
}