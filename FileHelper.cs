using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DanielTerry_Assesment1.Models;

namespace DanielTerry_Assesment1.Helpers
{
    public static class FileHelper
    {
        public static void ExportJson(List<Movie> movies, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true }));

        public static List<Movie> ImportJson(string path) =>
            JsonSerializer.Deserialize<List<Movie>>(File.ReadAllText(path));
    }
}