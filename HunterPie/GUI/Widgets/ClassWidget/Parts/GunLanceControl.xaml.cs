﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HunterPie.Logger;
using GunLance = HunterPie.Core.LPlayer.Jobs.GunLance;
using GunLanceEventArgs = HunterPie.Core.LPlayer.Jobs.GunLanceEventArgs;

namespace HunterPie.GUI.Widgets.ClassWidget.Parts
{
    /// <summary>
    /// Interaction logic for GunLanceControl.xaml
    /// </summary>
    public partial class GunLanceControl : ClassControl
    {

        const string WyvernLoadedColor = "#FFFF8B00";
        const string BigAmmoLoadedColor = "#FF6EB7EB";
        const string BigAmmoNotLoadedColor = "#FFAE0000";

        const string WyvernsfireOnCooldown = "#FFED2F2F";
        const string WyvernsfireReady = "#FF2FED55";

        public double WyvernstakeTimerPercentage
        {
            get { return (double)GetValue(WyvernstakeTimerPercentageProperty); }
            set { SetValue(WyvernstakeTimerPercentageProperty, value); }
        }

        public static readonly DependencyProperty WyvernstakeTimerPercentageProperty =
            DependencyProperty.Register("WyvernstakeTimerPercentage", typeof(double), typeof(GunLanceControl));

        public string WyvernstakeTimer
        {
            get { return (string)GetValue(WyvernstakeTimerProperty); }
            set { SetValue(WyvernstakeTimerProperty, value); }
        }

        public static readonly DependencyProperty WyvernstakeTimerProperty =
            DependencyProperty.Register("WyvernstakeTimer", typeof(string), typeof(GunLanceControl));

        public double WyvernboomPercentage
        {
            get { return (double)GetValue(WyvernboomPercentageProperty); }
            set { SetValue(WyvernboomPercentageProperty, value); }
        }

        public static readonly DependencyProperty WyvernboomPercentageProperty =
            DependencyProperty.Register("WyvernboomPercentage", typeof(double), typeof(GunLanceControl));

        public string WyvernsfireDiamondColor
        {
            get { return (string)GetValue(WyvernsfireDiamondColorProperty); }
            set { SetValue(WyvernsfireDiamondColorProperty, value); }
        }

        public static readonly DependencyProperty WyvernsfireDiamondColorProperty =
            DependencyProperty.Register("WyvernsfireDiamondColor", typeof(string), typeof(GunLanceControl));

        public string BigAmmoShadowColor
        {
            get { return (string)GetValue(BigAmmoShadowColorProperty); }
            set { SetValue(BigAmmoShadowColorProperty, value); }
        }

        public static readonly DependencyProperty BigAmmoShadowColorProperty =
            DependencyProperty.Register("BigAmmoShadowColor", typeof(string), typeof(GunLanceControl));

        public string BigAmmoImage
        {
            get { return (string)GetValue(BigAmmoImageProperty); }
            set { SetValue(BigAmmoImageProperty, value); }
        }

        public static readonly DependencyProperty BigAmmoImageProperty =
            DependencyProperty.Register("BigAmmoImage", typeof(string), typeof(GunLanceControl));

        GunLance Context;

        public GunLanceControl()
        {
            InitializeComponent();
        }

        public void SetContext(GunLance ctx)
        {
            Context = ctx;
            UpdateInformation();
            HookEvents();
        }

        private void UpdateInformation()
        {
            GunLanceEventArgs dummyArgs = new GunLanceEventArgs(Context);
            OnAmmoChange(this, dummyArgs);
            OnBigAmmoChange(this, dummyArgs);
            OnWyvernsFireTimerUpdate(this, dummyArgs);
            OnWyvernstakeBlastTimerUpdate(this, dummyArgs);
        }

        private void HookEvents()
        {
            Context.OnAmmoChange += OnAmmoChange;
            Context.OnBigAmmoChange += OnBigAmmoChange;
            Context.OnTotalAmmoChange += OnAmmoChange;
            Context.OnTotalBigAmmoChange += OnBigAmmoChange;
            Context.OnWyvernsFireTimerUpdate += OnWyvernsFireTimerUpdate;
            Context.OnWyvernstakeBlastTimerUpdate += OnWyvernstakeBlastTimerUpdate;
            Context.OnWyvernstakeStateChanged += OnBigAmmoChange;
        }

        public override void UnhookEvents()
        {
            Context.OnAmmoChange -= OnAmmoChange;
            Context.OnBigAmmoChange -= OnBigAmmoChange;
            Context.OnTotalAmmoChange -= OnAmmoChange;
            Context.OnTotalBigAmmoChange -= OnBigAmmoChange;
            Context.OnWyvernsFireTimerUpdate -= OnWyvernsFireTimerUpdate;
            Context.OnWyvernstakeBlastTimerUpdate -= OnWyvernstakeBlastTimerUpdate;
            Context.OnWyvernstakeStateChanged -= OnBigAmmoChange;
            Context = null;
            base.UnhookEvents();
        }

        private void OnWyvernstakeBlastTimerUpdate(object source, GunLanceEventArgs args)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                WyvernstakeTimerPercentage = args.WyvernstakeBlastTimer / 120;
                WyvernstakeTimer = args.WyvernstakeBlastTimer > 60 ? TimeSpan.FromSeconds(args.WyvernstakeBlastTimer).ToString("m\\:ss") :
                TimeSpan.FromSeconds(args.WyvernstakeBlastTimer).ToString("ss");
            }));
        }

        private void OnWyvernsFireTimerUpdate(object source, GunLanceEventArgs args)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                WyvernsfireDiamondColor = args.WyvernsFireTimer <= 0 ? WyvernsfireReady : WyvernsfireOnCooldown;
                WyvernboomPercentage = 1 - args.WyvernsFireTimer / 120;
            }));
        }

        private void OnBigAmmoChange(object source, GunLanceEventArgs args)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                if (args.HasWyvernstakeLoaded)
                {
                    WyvernstakeTimerPercentage = 1;
                }
                BigAmmoImage = args.HasWyvernstakeLoaded ? "pack://siteoforigin:,,,/HunterPie.Resources/UI/Class/GLanceWyvernstake.png" :
                args.BigAmmo == 0 ? "pack://siteoforigin:,,,/HunterPie.Resources/UI/Class/GLanceBAmmoEmpty.png" : "pack://siteoforigin:,,,/HunterPie.Resources/UI/Class/GLanceBAmmo.png";
                BigAmmoShadowColor = args.HasWyvernstakeLoaded ? WyvernLoadedColor : args.BigAmmo == 0 ? BigAmmoNotLoadedColor : BigAmmoLoadedColor;

            }));
        }

        private void OnAmmoChange(object source, GunLanceEventArgs args)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, new Action(() =>
            {
                DrawAmmo(args.Ammo, args.TotalAmmo - args.Ammo);
            }));
        }

        private void DrawAmmo(int full, int empty)
        {
            AmmoHolder.Children.Clear();
            for (int i = 0; i < full; i++)
            {
                Image img = new Image
                {
                    Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/HunterPie.Resources/UI/Class/GLanceAmmo.png", UriKind.Absolute)),
                    Height = 19
                };
                img.Source.Freeze();
                AmmoHolder.Children.Add(img);
            }
            for (int i = 0; i < empty; i++)
            {
                Image img = new Image()
                {
                    Source = new BitmapImage(new Uri("pack://siteoforigin:,,,/HunterPie.Resources/UI/Class/GLanceAmmoEmpty.png", UriKind.RelativeOrAbsolute)),
                    Height = 19
                };
                img.Source.Freeze();
                AmmoHolder.Children.Add(img);
            }
        }

    }
}
