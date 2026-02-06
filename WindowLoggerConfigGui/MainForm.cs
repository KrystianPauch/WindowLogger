using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;

namespace WindowLoggerConfigGui
{
    public partial class MainForm : Form
    {

        private const string ConfigPath = @"..\..\..\WindowAnalyser\appsettings.json";

        private AppSettings _config;


        private TabControl tabControlMain;
        private DataGridView gridApplications;
        private DataGridView gridExclusions;
        private DataGridView gridCategories;

        public MainForm()
        {

            BuildUserInterface();
        }

        private void BuildUserInterface()
        {
            this.Text = "Edytor Konfiguracji";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterScreen;


            tabControlMain = new TabControl { Dock = DockStyle.Fill };

            TabPage tabApps = new TabPage("Applications");
            TabPage tabExcl = new TabPage("Exclusions");
            TabPage tabCats = new TabPage("Categories");


            gridApplications = CreateGrid();
            gridExclusions = CreateGrid();
            gridCategories = CreateGrid();


            tabApps.Controls.Add(gridApplications);
            tabExcl.Controls.Add(gridExclusions);
            tabCats.Controls.Add(gridCategories);

            tabControlMain.TabPages.Add(tabApps);
            tabControlMain.TabPages.Add(tabExcl);
            tabControlMain.TabPages.Add(tabCats);

            this.Controls.Add(tabControlMain);
        }

        private DataGridView CreateGrid()
        {
            var grid = new DataGridView();
            grid.Dock = DockStyle.Fill;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = SystemColors.ControlLightLight;


            grid.CellFormatting += Grid_CellFormatting;
            grid.CellParsing += Grid_CellParsing;
            grid.DataError += (s, e) => { e.ThrowException = false; };

            return grid;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadConfiguration();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveConfiguration();
            base.OnFormClosing(e);
        }

        private void LoadConfiguration()
        {
            try
            {

                string path = File.Exists(ConfigPath) ? ConfigPath : "appsettings.json";

                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                        AllowTrailingCommas = true
                    };
                    _config = JsonSerializer.Deserialize<AppSettings>(json, options);
                }
                else
                {
                    _config = new AppSettings(); 
                }

                gridApplications.DataSource = new BindingList<ApplicationDefinition>(_config.Applications);
                gridExclusions.DataSource = new BindingList<ExclusionDefinition>(_config.Exclusions);
                gridCategories.DataSource = new BindingList<CategoryDefinition>(_config.Categories);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd wczytywania: " + ex.Message);
                _config = new AppSettings();
            }
        }

        private void SaveConfiguration()
        {
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(_config, options);

                string path = File.Exists(ConfigPath) ? ConfigPath : "appsettings.json";
                File.WriteAllText(path, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd zapisu: " + ex.Message);
            }
        }

        private void Grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value is List<string> list)
            {
                e.Value = string.Join(", ", list);
                e.FormattingApplied = true;
            }
        }

        private void Grid_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.DesiredType == typeof(List<string>))
            {
                string input = e.Value as string;
                if (string.IsNullOrWhiteSpace(input)) e.Value = new List<string>();
                else e.Value = input.Split(',').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToList();
                e.ParsingApplied = true;
            }
        }
    }
}