using iNKORE.UI.WPF.Modern;
using iNKORE.UI.WPF.Modern.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PdkBot.Pages
{
    public partial class SettingsPage : iNKORE.UI.WPF.Modern.Controls.Page
    {
        public SettingsPage()
        {
            InitializeComponent();
            Loaded += SettingsPage_Loaded;
        }

        private void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            var themeName = Params.Other.GetApplicationThemeName();
            cmbTheme.SelectedIndex = themeName == "Dark" ? 0 : 1;
        }

        private void cmbTheme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selected = e.AddedItems[0] as ComboBoxItem;
                if (selected.Content.ToString() == "深色")
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                    Params.Other.SetApplicationThemeName("Dark");
                }
                else
                {
                    ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                    Params.Other.SetApplicationThemeName("Light");
                }
            }
        }


    }
}
