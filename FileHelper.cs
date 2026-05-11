using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using WpfApp1.ProjectFiles.Models;

namespace WpfApp1.ProjectFiles.Helpers
{
    public static class FileHelper
    {
        public static void ExportJson(List<Movie> movies, string path) =>
            File.WriteAllText(path, JsonSerializer.Serialize(movies, new JsonSerializerOptions { WriteIndented = true }));

        public static List<Movie> ImportJson(string path) =>
            JsonSerializer.Deserialize<List<Movie>>(File.ReadAllText(path));
    }
}