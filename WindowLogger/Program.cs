using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WindowLoggerConfig
{
    // === MODELE DANYCH ===
    public class ConfigData
    {
        public List<AppEntry> applications { get; set; } = new();
        public List<ExclusionContainer> exclusions { get; set; } = new();
        public List<CategoryEntry> categories { get; set; } = new();
    }

    public class AppEntry
    {
        public string name { get; set; } = "";
        public List<string> include { get; set; } = new();
        public List<string> exclude { get; set; } = new();
    }

    public class ExclusionContainer
    {
        public List<string> include { get; set; } = new();
    }

    public class CategoryEntry
    {
        public string name { get; set; } = "";
        public List<string> includeApplications { get; set; } = new();
        public List<string> excludeApplications { get; set; } = new();
    }

    // === GŁÓWNY PROGRAM ===
    class Program
    {
        static string jsonPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        static ConfigData config = new ConfigData();

        static void Main(string[] args)
        {
            LoadConfig();

            while (true)
            {
                     Console.Clear();
                    Console.WriteLine("=== CONFIGURATION EDITOR (SAFE MODE) ===");
                    Console.WriteLine("1. Applications");
                    Console.WriteLine("2. Exclusions");
                    Console.WriteLine("3. Categories");
                    Console.WriteLine("4. SAVE AND EXIT");
                    Console.WriteLine("5. Exit without saving");
                    Console.WriteLine("========================================");
                    Console.Write("Select an option: ");

                var key = Console.ReadLine();
                switch (key)
                {
                    case "1": EditApps(); break;
                    case "2": EditExclusions(); break;
                    case "3": EditCategories(); break;
                    case "4": SaveConfig(); return;
                    case "5": return;
                }
            }
        }

        static void LoadConfig()
        {
            if (!File.Exists(jsonPath))
            {
                config.exclusions.Add(new ExclusionContainer());
                return;
            }
            try
            {
                string json = File.ReadAllText(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                config = JsonSerializer.Deserialize<ConfigData>(json, options) ?? new ConfigData();
                if (config.exclusions.Count == 0) config.exclusions.Add(new ExclusionContainer());
            }
            catch { }
        }

        static void SaveConfig()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(config, options);
                File.WriteAllText(jsonPath, json);
                Console.WriteLine("Saved successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Save error: " + ex.Message);
                Console.ReadKey();
            }
        }

        static void EditApps()
        {
            Console.Clear();
            Console.WriteLine("--- APPLICATIONS ---");
            for (int i = 0; i < config.applications.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {config.applications[i].name}");
            }
            Console.WriteLine("\n[A] Add | [R] Remove | [Enter] Back");
            var cmd = Console.ReadLine()?.ToUpper();
            if (cmd == "A")
            {
                Console.Write("Name: ");
                string n = Console.ReadLine();
                config.applications.Add(new AppEntry { name = n, include = new List<string> { n } });
            }
            else if (cmd == "R" && config.applications.Count > 0) config.applications.RemoveAt(config.applications.Count - 1);
        }

        static void EditExclusions()
        {
            Console.Clear();
            Console.WriteLine("--- EXCLUSIONS ---");
            var list = config.exclusions[0].include;
            Console.WriteLine(string.Join(", ", list));
            Console.WriteLine("\n[A] Add | [C] Clear | [Enter] Back");
            var cmd = Console.ReadLine()?.ToUpper();
            if (cmd == "A") { Console.Write("Word: "); list.Add(Console.ReadLine()); }
            else if (cmd == "C") list.Clear();
        }

        static void EditCategories()
        {
            Console.Clear();
            Console.WriteLine("--- CATEGORIES ---");
            for (int i = 0; i < config.categories.Count; i++) Console.WriteLine($"{i+1}. {config.categories[i].name}");
            Console.WriteLine("\n[A] Add | [Enter] Back");
            if (Console.ReadLine()?.ToUpper() == "A") { Console.Write("Name: "); config.categories.Add(new CategoryEntry { name = Console.ReadLine() }); }
        }
    }
}