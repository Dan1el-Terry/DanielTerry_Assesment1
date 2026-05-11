using DanielTerry_Assesment1.Models;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DanielTerry_Assesment1.Helpers
{
    public static class FileHelper
    {
        private static readonly JsonSerializerOptions _options =
            new JsonSerializerOptions { WriteIndented = true };

        public static void ExportJson(List<Movie> movies, string path)
        {
            var json = JsonSerializer.Serialize(movies, _options);
            File.WriteAllText(path, json);
        }

        public static List<Movie> ImportJson(string path)
        {
            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<Movie>>(json) ?? new List<Movie>();
        }
    }
}