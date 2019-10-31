﻿using System;
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
using HunterPie.Core;

namespace HunterPie {
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl {

        public static Settings _Instance;
        public static Settings Instance {
            get {
                if (_Instance == null) {
                    _Instance = new Settings();
                }
                return _Instance;
            }
        }

        public Settings() {
            InitializeComponent();
        }

        static public void RefreshSettingsUI() {
            var settings = UserSettings.PlayerConfig;
            var settingsUI = _Instance.SettingsBox;
            // HunterPie
            settingsUI.enableAutoUpdate.IsChecked = settings.HunterPie.Update.Enabled;
            settingsUI.branchesCombobox.SelectedItem = _Instance.SettingsBox.branchesCombobox.Items.Contains(settings.HunterPie.Update.Branch) ? settings.HunterPie.Update.Branch : "master";
            
            // Rich Presence
            settingsUI.enableRichPresence.IsChecked = settings.RichPresence.Enabled;
            
            // Overlay
            settingsUI.enableOverlay.IsChecked = settings.Overlay.Enabled;
            settingsUI.positionOverlayX.Text = settings.Overlay.Position[0].ToString();
            settingsUI.positionOverlayY.Text = settings.Overlay.Position[1].ToString();
            // Monsters
            settingsUI.enableMonsterComponent.IsChecked = settings.Overlay.MonstersComponent.Enabled;
            settingsUI.positionMonsterCompX.Text = settings.Overlay.MonstersComponent.Position[0].ToString();
            settingsUI.positionMonsterCompY.Text = settings.Overlay.MonstersComponent.Position[1].ToString();

            // Primary Mantle
            settingsUI.enablePrimaryMantle.IsChecked = settings.Overlay.PrimaryMantle.Enabled;
            settingsUI.primMantlePosX.Text = settings.Overlay.PrimaryMantle.Position[0].ToString();
            settingsUI.primMantlePosY.Text = settings.Overlay.PrimaryMantle.Position[1].ToString();
            settingsUI.primMantleColor.Text = settings.Overlay.PrimaryMantle.Color;

            // Secondary Mantle
            settingsUI.enableSecondaryMantle.IsChecked = settings.Overlay.SecondaryMantle.Enabled;
            settingsUI.secMantlePosX.Text = settings.Overlay.SecondaryMantle.Position[0].ToString();
            settingsUI.secMantlePosY.Text = settings.Overlay.SecondaryMantle.Position[1].ToString();
            settingsUI.secMantleColor.Text = settings.Overlay.SecondaryMantle.Color;
        }

    }
}
