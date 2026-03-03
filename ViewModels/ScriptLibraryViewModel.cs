using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;

namespace FluentTaskScheduler.ViewModels
{
    public class ScriptTemplateModel
    {
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public string Command { get; set; } = "";
        public string Arguments { get; set; } = "";
        public bool RunAsAdmin { get; set; }

        // Runtime-only, not serialized
        [JsonIgnore] public bool IsUserTemplate { get; set; }
        [JsonIgnore] public Visibility DeleteVisibility => IsUserTemplate ? Visibility.Visible : Visibility.Collapsed;
    }

    public class ScriptLibraryViewModel
    {
        private static readonly string _userTemplatesPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "FluentTaskScheduler", "user_templates.json");

        private static readonly JsonSerializerOptions _json = new() { WriteIndented = true };

        public ObservableCollection<ScriptTemplateModel> Scripts { get; } = new();

        public async Task LoadScriptsAsync()
        {
            if (Scripts.Count > 0) return;

            // Built-in templates
            try
            {
                string json = "";
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "Scripts.json");

                if (File.Exists(fullPath))
                    json = await File.ReadAllTextAsync(fullPath);
                else
                {
                    try
                    {
                        var storageFile = await Package.Current.InstalledLocation.GetFileAsync("Assets\\Scripts.json");
                        json = await Windows.Storage.FileIO.ReadTextAsync(storageFile);
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(json))
                {
                    var data = JsonSerializer.Deserialize<System.Collections.Generic.List<ScriptTemplateModel>>(json);
                    if (data != null)
                        foreach (var item in data) Scripts.Add(item);
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Error loading scripts: {ex}"); }

            // User templates (appended after built-ins)
            LoadUserTemplates();
        }

        public void AddUserTemplate(ScriptTemplateModel model)
        {
            model.IsUserTemplate = true;
            Scripts.Add(model);
            SaveUserTemplates();
        }

        public void DeleteUserTemplate(ScriptTemplateModel model)
        {
            Scripts.Remove(model);
            SaveUserTemplates();
        }

        private void LoadUserTemplates()
        {
            try
            {
                if (!File.Exists(_userTemplatesPath)) return;
                var json = File.ReadAllText(_userTemplatesPath);
                var data = JsonSerializer.Deserialize<System.Collections.Generic.List<ScriptTemplateModel>>(json);
                if (data != null)
                    foreach (var item in data) { item.IsUserTemplate = true; Scripts.Add(item); }
            }
            catch { }
        }

        private void SaveUserTemplates()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_userTemplatesPath)!);
                var userTemplates = Scripts.Where(s => s.IsUserTemplate).ToList();
                File.WriteAllText(_userTemplatesPath, JsonSerializer.Serialize(userTemplates, _json));
            }
            catch { }
        }
    }
}
