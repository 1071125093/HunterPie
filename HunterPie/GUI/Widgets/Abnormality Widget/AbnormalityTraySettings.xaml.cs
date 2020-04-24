﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using HunterPie.Core;
using HunterPie.Core.LPlayer;
using HunterPie.GUI.Widgets.Abnormality_Widget.Parts;

namespace HunterPie.GUI.Widgets.Abnormality_Widget {
    /// <summary>
    /// Interaction logic for AbnormalityTraySettings.xaml
    /// </summary>
    public partial class AbnormalityTraySettings : WidgetSettings {
        readonly List<AbnormalitySettingControl> abnormalityControls = new List<AbnormalitySettingControl>();
        int buffTrayIndex;

        public AbnormalityTraySettings(int trayIndex = 0) {
            InitializeComponent();
            buffTrayIndex = trayIndex;
            WindowTitle.Text = $"Settings: {UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].Name}";

            PopulateAbnormalities();
            ConfigureWindow();
        }

        private void PopulateAbnormalities() {
            PopulateHuntingHornBuffs();
            PopulateOrchestraBuffs();
            PopulateDebuffs();
            PopulateConsumableBuffs();
            PopulateGearBuffs();
        }

        private void ConfigureWindow() {
            EnableName.IsEnabled = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].ShowNames;
            OrientationSwitcher.IsEnabled = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].Orientation == "Horizontal";
            EnableTimeLeftSwitcher.IsEnabled = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].ShowTimeLeftText;
            TimerTextFormatBox.SelectedIndex = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].TimeLeftTextFormat;
            BackgroundOpacitySlider.Value = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].BackgroundOpacity;
        }

        private void PopulateHuntingHornBuffs() =>
            PopulateAbnormalities(AbnormalityData.HuntingHornAbnormalities, HuntingHornBuffs);

        private void PopulateOrchestraBuffs() =>
            PopulateAbnormalities(AbnormalityData.PalicoAbnormalities, PalicoBuffs);

        private void PopulateDebuffs() => PopulateAbnormalities(AbnormalityData.BlightAbnormalities, Debuffs);

        private void PopulateConsumableBuffs() =>
            PopulateAbnormalities(AbnormalityData.MiscAbnormalities, ConsumableBuffs);

        private void PopulateGearBuffs() => PopulateAbnormalities(AbnormalityData.GearAbnormalities, GearBuffs);

        private void PopulateAbnormalities(IEnumerable<AbnormalityInfo> abnormalities, Panel panel)
        {
            foreach (AbnormalityInfo abnormality in abnormalities)
            {
                string name = GStrings.GetAbnormalityByID(abnormality.Type, abnormality.Id, 0);
                bool isEnabled = UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].AcceptedAbnormalities.Contains(abnormality.InternalId);
                ImageSource icon = (ImageSource)FindResource(abnormality.IconName);
                icon?.Freeze();

                AbnormalitySettingControl settingsControl = new AbnormalitySettingControl();
                settingsControl.SetAbnormalityInfo(icon, name, abnormality.InternalId, isEnabled);
                abnormalityControls.Add(settingsControl);
                panel.Children.Add(settingsControl);
            }
        }

        private void OnCloseButtonClick(object sender, MouseButtonEventArgs e) => Close();

        private void OnDragWindow(object sender, MouseButtonEventArgs e) => DragMove();

        private void OnSaveButtonClick(object sender, RoutedEventArgs e) {
            string[] enabledAbnormalities = abnormalityControls
                .Where(a => a.IsEnabled)
                .Select(a => a.InternalID)
                .ToArray();

            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].ShowNames = EnableName.IsEnabled;
            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].AcceptedAbnormalities = enabledAbnormalities;
            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].Orientation = OrientationSwitcher.IsEnabled ? "Horizontal" : "Vertical";
            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].ShowTimeLeftText = EnableTimeLeftSwitcher.IsEnabled;
            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].TimeLeftTextFormat = (byte)TimerTextFormatBox.SelectedIndex;
            UserSettings.PlayerConfig.Overlay.AbnormalitiesWidget.BarPresets[buffTrayIndex].BackgroundOpacity = (float)BackgroundOpacitySlider.Value;
            UserSettings.SaveNewConfig();
        }

        private void OnSelectAllButtonClick(object sender, RoutedEventArgs e) => ToggleAllAbnormalitiesInTab(true);

        private void OnUnselectAllButtonClick(object sender, RoutedEventArgs e) => ToggleAllAbnormalitiesInTab(false);

        private void ToggleAllAbnormalitiesInTab(bool enable)
        {
            ContentControl selectedAbnormalityContainer = (ContentControl)AbnormalitySelectionContainer.SelectedContent;
            Panel selectedAbnormalityPanel = (Panel)selectedAbnormalityContainer.Content;
            
            foreach (AbnormalitySettingControl abnormalityDisplay in selectedAbnormalityPanel.Children.Cast<AbnormalitySettingControl>()) {
                abnormalityDisplay.IsEnabled = enable;
            }
        }

        private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            HuntingHornBuffs.Children.Clear();
            PalicoBuffs.Children.Clear();
            Debuffs.Children.Clear();
            ConsumableBuffs.Children.Clear();
            abnormalityControls.Clear();
            GearBuffs.Children.Clear();
            abnormalityControls.Clear();
        }

    }
}
