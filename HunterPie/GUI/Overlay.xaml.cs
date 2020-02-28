﻿using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using HunterPie.Core;
using HunterPie.Memory;
using System.Windows.Controls.Primitives;

namespace HunterPie.GUI {
    /// <summary>
    /// Interaction logic for Overlay.xaml
    /// </summary>
    public partial class Overlay : Window, IDisposable {
        public bool IsDisposed { get; private set; }
        Game ctx;
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int style);

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        public Overlay(Game Context) {
            ctx = Context;
            SetRenderMode();
            InitializeComponent();
            SetWidgetsContext();
            SetOverlaySize();
            SetWindowFlags();
        }

        private void SetRenderMode() {
            if (!UserSettings.PlayerConfig.Overlay.EnableHardwareAcceleration) {
                RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
            }
        }

        private void SetWidgetsContext() {
            this.MonstersWidget.SetContext(ctx);
            this.PrimaryMantle.SetContext(ctx.Player.PrimaryMantle);
            this.SecondaryMantle.SetContext(ctx.Player.SecondaryMantle);
            this.HarvestBoxComponent.SetContext(ctx.Player);
            this.DPSMeter.SetContext(ctx);
        }

        public void HookEvents() {
            Scanner.OnGameFocus += OnGameFocus;
            Scanner.OnGameUnfocus += OnGameUnfocus;
        }

        private void UnhookEvents() {
            Scanner.OnGameFocus -= OnGameFocus;
            Scanner.OnGameUnfocus -= OnGameUnfocus;
        }

        public void Destroy() {
            this.MonstersWidget.UnhookEvents();
            this.PrimaryMantle.UnhookEvents();
            this.SecondaryMantle.UnhookEvents();
            this.HarvestBoxComponent.UnhookEvents();
            this.DPSMeter.DestroyPlayerComponents();
            this.DPSMeter.UnhookEvents();
            this.UnhookEvents();
            this.ctx = null;
        }
        
        public void GlobalSettingsEventHandler(object source, EventArgs e) {
            this.ToggleOverlay(source, e);
            this.ChangeHarvestBoxPosition(source, e);
            this.ChangeMonsterComponentPosition(source, e);
            this.ChangePrimaryMantlePosition(source, e);
            this.ChangeSecondaryMantlePosition(source, e);
            this.ChangePrimaryMantleColor(source, e);
            this.ChangeSecondaryMantleColor(source, e);
            this.ChangeDPSMeterPosition(source, e);
        }

        private void SetWindowFlags() {
            // flags to make overlay click-through
            int WS_EX_TRANSPARENT = 0x20;
            int WS_EX_TOPMOST = 0x8;
            int WS_EX_TOOLWINDOW = 0x80; // Flag to hide overlay from ALT+TAB
            int GWL_EXSTYLE = (-20);

            var wnd = GetWindow(this);
            IntPtr hwnd = new WindowInteropHelper(wnd).EnsureHandle();
            // Get overlay flags
            int Styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            // Apply new flags
            SetWindowLong(hwnd, GWL_EXSTYLE, Styles | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT | WS_EX_TOPMOST);
        }

        public void Dispatch(Action function) {
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, function);
        } 

        private void SetOverlaySize() {
            /* Looks for all monitors available and sets the overlay to the total width, height */
            double Width = 0;
            double Height = 0;
            int MonitorX = -1;
            int MonitorY = -1;
            foreach (Screen Monitor in Screen.AllScreens) {
                if (MonitorX != Monitor.Bounds.X) {
                    Width += Monitor.Bounds.Width;
                    MonitorX = Monitor.Bounds.X;
                }
                if (MonitorY != Monitor.Bounds.Y) {
                    Height += Monitor.Bounds.Height;
                    MonitorY = Monitor.Bounds.Y;
                }
            }
            this.Width = Width;
            this.Height = Height;
        }

        private void OnGameUnfocus(object source, EventArgs args) {
            if (UserSettings.PlayerConfig.Overlay.HideWhenGameIsUnfocused)
                Dispatch(() => { this.Hide(); });
        }

        private void OnGameFocus(object source, EventArgs args) {
            if (UserSettings.PlayerConfig.Overlay.Enabled)
                Dispatch(() => { this.Show(); });
        }

        public void ChangeMonsterComponentPosition(object source, EventArgs e) {
            
            bool ContainerEnabled = UserSettings.PlayerConfig.Overlay.MonstersComponent.Enabled;
            bool MonsterWeaknessEnabled = UserSettings.PlayerConfig.Overlay.MonstersComponent.ShowMonsterWeakness;
            double X = UserSettings.PlayerConfig.Overlay.MonstersComponent.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.MonstersComponent.Position[1];
            double Left = MonstersWidget.Margin.Left;
            double Top = MonstersWidget.Margin.Top;
            double Right = MonstersWidget.Margin.Right;
            double Bottom = MonstersWidget.Margin.Bottom;
            Dispatch(() => {
                if (X != Left || Y != Top) {
                    MonstersWidget.Margin = new Thickness(X, Y, Right, Bottom);
                }
                if (ContainerEnabled) {
                    MonstersWidget.Visibility = Visibility.Visible;
                } else {
                    MonstersWidget.Visibility = Visibility.Hidden;
                }

                MonstersWidget.UpdateMonstersWidgetsSettings(MonsterWeaknessEnabled);
            }); 
        }

        /* Positions and enable/disable components */

        public void ToggleOverlay(object source, EventArgs e) {
            double X = UserSettings.PlayerConfig.Overlay.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.Position[1];
            Dispatch(() => {
                if (UserSettings.PlayerConfig.Overlay.Enabled && Scanner.IsForegroundWindow) this.Show();
                else if (UserSettings.PlayerConfig.Overlay.Enabled && !UserSettings.PlayerConfig.Overlay.HideWhenGameIsUnfocused) this.Show();
                else if (UserSettings.PlayerConfig.Overlay.Enabled && !Scanner.IsForegroundWindow) this.Hide();
                else if (!UserSettings.PlayerConfig.Overlay.Enabled) this.Hide();
                this.Top = Y;
                this.Left = X;
            });
        }
        
        public void ChangePrimaryMantlePosition(object source, EventArgs e) {
            double X = UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.PrimaryMantle.Position[1];
            double Left = PrimaryMantle.Margin.Left;
            double Top = PrimaryMantle.Margin.Top;
            double Right = PrimaryMantle.Margin.Right;
            double Bottom = PrimaryMantle.Margin.Bottom;
            Dispatch(() => {
                if (X != Left || Y != Top) {
                    PrimaryMantle.Margin = new Thickness(X, Y, Right, Bottom);
                }
                if (PrimaryMantle.IsVisible && !UserSettings.PlayerConfig.Overlay.PrimaryMantle.Enabled) {
                    PrimaryMantle.Visibility = Visibility.Hidden;
                } else if (UserSettings.PlayerConfig.Overlay.PrimaryMantle.Enabled) {
                    PrimaryMantle.Visibility = Visibility.Visible;
                }
            });
        }

        public void ChangeDPSMeterPosition(object source, EventArgs e) {
            double X = UserSettings.PlayerConfig.Overlay.DPSMeter.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.DPSMeter.Position[1];
            double Left = DPSMeter.Margin.Left;
            double Top = DPSMeter.Margin.Top;
            double Right = DPSMeter.Margin.Right;
            double Bottom = DPSMeter.Margin.Bottom;
            Dispatch(() => {
                DPSMeter.UpdatePlayersColor();
                if (X != Left || Y != Top) {
                    DPSMeter.Margin = new Thickness(X, Y, Right, Bottom);
                }
                if (!UserSettings.PlayerConfig.Overlay.DPSMeter.Enabled) {
                    DPSMeter.Visibility = Visibility.Hidden;
                } else if (UserSettings.PlayerConfig.Overlay.DPSMeter.Enabled && DPSMeter.IsActive) {
                    DPSMeter.Visibility = Visibility.Visible;
                }
            });
        }

        public void ChangeHarvestBoxPosition(object source, EventArgs e) {
            double X = UserSettings.PlayerConfig.Overlay.HarvestBoxComponent.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.HarvestBoxComponent.Position[1];
            double Left = HarvestBoxComponent.Margin.Left;
            double Top = HarvestBoxComponent.Margin.Top;
            double Right = HarvestBoxComponent.Margin.Right;
            double Bottom = HarvestBoxComponent.Margin.Bottom;
            Dispatch(() => {
                if (X != Left || Y != Top) {
                    HarvestBoxComponent.Margin = new Thickness(X, Y, Right, Bottom);
                }
                if (HarvestBoxComponent.IsVisible && !UserSettings.PlayerConfig.Overlay.HarvestBoxComponent.Enabled) {
                    HarvestBoxComponent.Visibility = Visibility.Hidden;
                } else if (UserSettings.PlayerConfig.Overlay.HarvestBoxComponent.Enabled) {
                    if (ctx.Player.inHarvestZone) {
                        HarvestBoxComponent.Visibility = Visibility.Visible;
                    } else {
                        HarvestBoxComponent.Visibility = Visibility.Hidden;
                    }
                }
            });
        }

        private RadialGradientBrush DonutBrush(Color customColor) {
            RadialGradientBrush brush = new RadialGradientBrush {
                Center = new Point(13, 13),
                GradientOrigin = new Point(13, 13),
                MappingMode = BrushMappingMode.Absolute,
                RadiusX = 13,
                RadiusY = 13
            };
            // Add the colors to make a donut
            brush.GradientStops.Add(new GradientStop(Colors.Transparent, 0.4));
            brush.GradientStops.Add(new GradientStop(customColor, 0.4));
            return brush;
        }

        public void ChangePrimaryMantleColor(object source, EventArgs e) {
            string newColor = UserSettings.PlayerConfig.Overlay.PrimaryMantle.Color;
            if (PrimaryMantle.MantleCooldown.Fill.ToString() == newColor) {
                return;
            }
            Color primaryColor = (Color)ColorConverter.ConvertFromString(newColor);
            Brush primaryColorBrush = new SolidColorBrush(primaryColor);
            Dispatch(() => {
                PrimaryMantle.MantleCooldown.Fill = DonutBrush(primaryColor);
                PrimaryMantle.MantleBorder.BorderBrush = primaryColorBrush;
            });
        }

        public void ChangeSecondaryMantlePosition(object source, EventArgs e) {
            double X = UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[0];
            double Y = UserSettings.PlayerConfig.Overlay.SecondaryMantle.Position[1];
            double Left = SecondaryMantle.Margin.Left;
            double Top = SecondaryMantle.Margin.Top;
            double Right = SecondaryMantle.Margin.Right;
            double Bottom = SecondaryMantle.Margin.Bottom;
            Dispatch(() => {
                if (X != Left || Y != Top) {
                    SecondaryMantle.Margin = new Thickness(X, Y, Right, Bottom);
                }
                if (SecondaryMantle.IsVisible && !UserSettings.PlayerConfig.Overlay.SecondaryMantle.Enabled) {
                    SecondaryMantle.Visibility = Visibility.Hidden;
                } else if (UserSettings.PlayerConfig.Overlay.SecondaryMantle.Enabled) {
                    SecondaryMantle.Visibility = Visibility.Visible;
                }
            });
        }
        
        public void ChangeSecondaryMantleColor(object source, EventArgs e) {
            string newColor = UserSettings.PlayerConfig.Overlay.SecondaryMantle.Color;
            if (SecondaryMantle.MantleCooldown.Fill.ToString() == newColor) {
                return;
            }
            Color secondaryColor = (Color)ColorConverter.ConvertFromString(newColor);
            Brush secondaryColorBrush = new SolidColorBrush(secondaryColor);
            Dispatch(() => {
                SecondaryMantle.MantleCooldown.Fill = DonutBrush(secondaryColor);
                SecondaryMantle.MantleBorder.BorderBrush = secondaryColorBrush;
            });
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this.UnhookEvents();
                this.Destroy();
            }
            this.Close();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
