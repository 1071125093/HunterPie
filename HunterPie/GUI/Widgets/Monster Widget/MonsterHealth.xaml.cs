﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Timer = System.Threading.Timer;
using HunterPie.Core;
using HunterPie.GUIControls.Custom_Controls;

namespace HunterPie.GUI.Widgets {
    /// <summary>
    /// Interaction logic for MonsterHealth.xaml
    /// </summary>
    public partial class MonsterHealth : UserControl {

        private Monster Context;
        private Timer VisibilityTimer; // TODO: Implement Visibility timer to hide monsters that aren't active

        // Animations
        private Storyboard ANIM_ENRAGEDICON;

        public MonsterHealth() {
            InitializeComponent();
        }

        ~MonsterHealth() {
            ANIM_ENRAGEDICON = null;
        }

        public void SetContext(Monster ctx) {
            Context = ctx;
            HookEvents();
            LoadAnimations();
            if (Context.Name != null) {
                UpdateMonsterInfo();
            } else { this.Visibility = Visibility.Collapsed; } 
        }

        private void Dispatch(Action function) {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, function);
        }

        private void HookEvents() {
            Context.OnMonsterSpawn += OnMonsterSpawn;
            Context.OnMonsterDespawn += OnMonsterDespawn;
            Context.OnMonsterDeath += OnMonsterDespawn;
            Context.OnHPUpdate += OnMonsterUpdate;
            Context.OnEnrage += OnEnrage;
            Context.OnUnenrage += OnUnenrage;
            Context.OnEnrageTimerUpdate += OnEnrageTimerUpdate;
            Context.OnTargetted += OnMonsterTargetted;
        }

        public void UnhookEvents() {
            Context.OnMonsterSpawn -= OnMonsterSpawn;
            Context.OnMonsterDespawn -= OnMonsterDespawn;
            Context.OnMonsterDeath -= OnMonsterDespawn;
            Context.OnHPUpdate -= OnMonsterUpdate;
            Context.OnEnrage -= OnEnrage;
            Context.OnUnenrage -= OnUnenrage;
            Context.OnEnrageTimerUpdate -= OnEnrageTimerUpdate;
            Context.OnTargetted -= OnMonsterTargetted;
            Context = null;
        }

        private void UpdateMonsterInfo() {
            // Used when starting HunterPie for the first time, since the events won't be triggered
            this.Visibility = Visibility.Visible;
            this.MonsterName.Text = Context.Name;

            // Update monster health

            MonsterHealthBar.MaxSize = this.Width * 0.7833333333333333;
            MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
            SetMonsterHealthBarText(Context.CurrentHP, Context.TotalHP);

            SwitchSizeBasedOnTarget();

            // Parts
            this.MonsterPartsContainer.Children.Clear();
            foreach (Part mPart in Context.Parts) {
                Monster_Widget.Parts.MonsterPart PartDisplay = new Monster_Widget.Parts.MonsterPart();
                PartDisplay.SetContext(mPart, this.MonsterPartsContainer.ItemWidth);
                this.MonsterPartsContainer.Children.Add(PartDisplay);
            }

            // Enrage
            if (Context.IsEnraged) {
                ANIM_ENRAGEDICON.Begin(this.MonsterHealthBar, true);
                ANIM_ENRAGEDICON.Begin(this.HealthBossIcon, true);
                EnrageTimerText.Visibility = Visibility.Visible;
                EnrageTimerText.Text = $"{Context.EnrageTimerStatic - Context.EnrageTimer:0}s";
            }

            // Set monster crown
            this.MonsterCrown.Source = Context.Crown == null ? null : (ImageSource)FindResource(Context.Crown);
            this.MonsterCrown.Visibility = Context.Crown == null ? Visibility.Collapsed : Visibility.Visible;
            Weaknesses.Children.Clear(); // Removes every weakness icon
            if (Context.Weaknesses == null) return;
            foreach (string Weakness in Context.Weaknesses.Keys) {
                ImageSource img = this.Resources[Weakness] as ImageSource;
                img.Freeze();
                WeaknessDisplay MonsterWeaknessDisplay = new WeaknessDisplay {
                    Icon = img,
                    Width = 20,
                    Height = 20
                };
                Weaknesses.Children.Add(MonsterWeaknessDisplay);
            }
        }

        private void LoadAnimations() {
            ANIM_ENRAGEDICON = FindResource("ANIM_ENRAGED") as Storyboard;
        }

        private void OnMonsterTargetted(object source, EventArgs args) {
            Dispatch(() => {
                SwitchSizeBasedOnTarget();
            });
        }

        private void OnEnrage(object source, MonsterUpdateEventArgs args) {
            this.Dispatch(() => {
                ANIM_ENRAGEDICON.Begin(this.MonsterHealthBar, true);
                ANIM_ENRAGEDICON.Begin(this.HealthBossIcon, true);
            });
        }

        private void OnEnrageTimerUpdate(object source, MonsterUpdateEventArgs args) {
            if (Context == null) return;
            int EnrageTimer = (int)Context.EnrageTimerStatic - (int)Context.EnrageTimer;
            Dispatch(() => {
                this.EnrageTimerText.Visibility = EnrageTimer > 0 ? Visibility.Visible : Visibility.Hidden;
                this.EnrageTimerText.Text = $"{EnrageTimer}";
            });
        }

        private void OnUnenrage(object source, MonsterUpdateEventArgs args) {
            this.Dispatch(() => {
                ANIM_ENRAGEDICON.Remove(this.MonsterHealthBar);
                ANIM_ENRAGEDICON.Remove(this.HealthBossIcon);
            });
        }

        private void OnMonsterDespawn(object source, EventArgs args) {
            this.Dispatch(() => {
                this.MonsterCrown.Visibility = Visibility.Collapsed;
                this.Visibility = Visibility.Collapsed;
                this.Weaknesses.Children.Clear();
                foreach (Monster_Widget.Parts.MonsterPart Part in MonsterPartsContainer.Children) {
                    //Part.UnhookEvents();
                }
                MonsterPartsContainer.Children.Clear();
            });
        }

        private void OnMonsterSpawn(object source, MonsterSpawnEventArgs args) {
            this.Dispatch(() => {
                this.Visibility = Visibility.Visible;
                this.MonsterName.Text = args.Name;

                // Update monster health
                MonsterHealthBar.MaxSize = this.Width * 0.7833333333333333;
                MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
                SetMonsterHealthBarText(args.CurrentHP, args.TotalHP);

                SwitchSizeBasedOnTarget();

                // Parts
                this.MonsterPartsContainer.Children.Clear();
                foreach (Part mPart in Context.Parts) {
                    Monster_Widget.Parts.MonsterPart PartDisplay = new Monster_Widget.Parts.MonsterPart();
                    PartDisplay.SetContext(mPart, this.MonsterPartsContainer.ItemWidth);
                    this.MonsterPartsContainer.Children.Add(PartDisplay);
                }

                // Set monster crown
                this.MonsterCrown.Source = args.Crown == null ? null : (ImageSource)FindResource(args.Crown);
                this.MonsterCrown.Visibility = args.Crown == null ? Visibility.Collapsed : Visibility.Visible;
                Weaknesses.Children.Clear(); // Removes every weakness icon
                foreach (string Weakness in Context.Weaknesses.Keys) {
                    ImageSource img = this.Resources[Weakness] as ImageSource;
                    img.Freeze();
                    WeaknessDisplay MonsterWeaknessDisplay = new WeaknessDisplay {
                        Icon = img,
                        Width = 20,
                        Height = 20
                    };
                    Weaknesses.Children.Add(MonsterWeaknessDisplay);
                }
            });
        }

        private void OnMonsterUpdate(object source, MonsterUpdateEventArgs args) {
            this.Dispatch(() => {
                this.MonsterHealthBar.MaxHealth = args.TotalHP;
                this.MonsterHealthBar.Health = args.CurrentHP;
                SetMonsterHealthBarText(args.CurrentHP, args.TotalHP);
            });
        }

        private void SetMonsterHealthBarText(float hp, float max_hp) {
            this.HealthText.Text = $"{hp:0}/{max_hp:0} ({hp / max_hp * 100:0}%)";
        }

        public void SwitchSizeBasedOnTarget() {
            switch(UserSettings.PlayerConfig.Overlay.MonstersComponent.ShowMonsterBarMode) {
                case 0: // Default
                    ShowAllMonstersAtOnce();
                    break;
                case 1: // Show all but highlight target
                    ShowAllButFocusTarget();
                    break;
                case 2: // Show only target
                    ShowOnlyTargetMonster();
                    break;
            }
        }

        #region Monster bar modes

        // Show all monsters at once
        private void ShowAllMonstersAtOnce() {
            if (this.Context != null && this.Context.IsAlive) this.Visibility = Visibility.Visible;
            else { this.Visibility = Visibility.Collapsed; }
            this.Width = 300;
            MonsterAilmentsContainer.ItemWidth = (this.Width - 2) / 2;
            MonsterPartsContainer.ItemWidth = (this.Width - 2) / 2;
            UpdatePartHealthBarSizes(MonsterPartsContainer.ItemWidth);
            MonsterHealthBar.MaxSize = 231;
            MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
            this.Opacity = 1;
        }

        // Only show one monster
        private void ShowOnlyTargetMonster() {
            if (Context == null || !Context.isTarget) { this.Visibility = Visibility.Collapsed; }
            else {      
                this.Visibility = Visibility.Visible;
                this.Width = 500;
                MonsterAilmentsContainer.ItemWidth = (this.Width - 2) / 2;
                MonsterPartsContainer.ItemWidth = (this.Width - 2) / 2;
                UpdatePartHealthBarSizes(MonsterPartsContainer.ItemWidth);
                this.Opacity = 1;
                MonsterHealthBar.MaxSize = this.Width * 0.9;
                MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
            }
        }

        // Show all monsters but highlight only target
        private void ShowAllButFocusTarget() {
            if (this.Context != null && this.Context.IsAlive) this.Visibility = Visibility.Visible;
            else { this.Visibility = Visibility.Collapsed; }
            if (!Context.isTarget) {
                this.Width = 240;
                // Parts
                MonsterAilmentsContainer.ItemWidth = (this.Width - 2) / 2;
                MonsterPartsContainer.ItemWidth = (this.Width - 2) / 2;
                UpdatePartHealthBarSizes(MonsterPartsContainer.ItemWidth);
                // Monster Bar
                MonsterHealthBar.MaxSize = this.Width * 0.8;
                MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
                this.Opacity = 0.5;
            } else {
                this.Width = 320;
                // Parts
                MonsterAilmentsContainer.ItemWidth = (this.Width - 2) / 2;
                MonsterPartsContainer.ItemWidth = (this.Width - 2) / 2;
                UpdatePartHealthBarSizes(MonsterPartsContainer.ItemWidth);
                // Monster Bar
                MonsterHealthBar.MaxSize = this.Width * 0.8;
                MonsterHealthBar.UpdateBar(Context.CurrentHP, Context.TotalHP);
                this.Opacity = 1;
            }
        }
        #endregion

        #region Parts

        private void UpdatePartHealthBarSizes(double NewSize) {
            foreach (Monster_Widget.Parts.MonsterPart part in MonsterPartsContainer.Children) {
                part.UpdateHealthBarSize(NewSize);
            }
        }

        #endregion
    }
}
