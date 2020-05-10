﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace HunterPie.GUIControls.Custom_Controls {
    /// <summary>
    /// Interaction logic for CNotification.xaml
    /// </summary>
    public partial class CNotification : UserControl {
        
        public ImageSource NIcon
        {
            get { return (ImageSource)GetValue(NIconProperty); }
            set { SetValue(NIconProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NIcon.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NIconProperty =
            DependencyProperty.Register("NIcon", typeof(ImageSource), typeof(CNotification));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CNotification));



        public ImageSource FirstButtonImage
        {
            get { return (ImageSource)GetValue(FirstButtonImageProperty); }
            set { SetValue(FirstButtonImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstButtonImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstButtonImageProperty =
            DependencyProperty.Register("FirstButtonImage", typeof(ImageSource), typeof(CNotification));



        public ImageSource SecondButtonImage
        {
            get { return (ImageSource)GetValue(SecondButtonImageProperty); }
            set { SetValue(SecondButtonImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondButtonImage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondButtonImageProperty =
            DependencyProperty.Register("SecondButtonImage", typeof(ImageSource), typeof(CNotification));

        public Visibility FirstButtonVisibility
        {
            get { return (Visibility)GetValue(FirstButtonVisibilityProperty); }
            set { SetValue(FirstButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FirstButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstButtonVisibilityProperty =
            DependencyProperty.Register("FirstButtonVisibility", typeof(Visibility), typeof(CNotification));
        
        public Visibility SecondButtonVisibility
        {
            get { return (Visibility)GetValue(SecondButtonVisibilityProperty); }
            set { SetValue(SecondButtonVisibilityProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondButtonVisibility.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondButtonVisibilityProperty =
            DependencyProperty.Register("SecondButtonVisibility", typeof(Visibility), typeof(CNotification));



        public string FirstButtonText
        {
            get { return (string)GetValue(FirstButtonTextProperty); }
            set { SetValue(FirstButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for  FirstButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstButtonTextProperty =
            DependencyProperty.Register("FirstButtonText", typeof(string), typeof(CNotification));

        public string SecondButtonText
        {
            get { return (string)GetValue(SecondButtonTextProperty); }
            set { SetValue(SecondButtonTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SecondButtonText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondButtonTextProperty =
            DependencyProperty.Register("SecondButtonText", typeof(string), typeof(CNotification));


        public int ShowTime { get; set; }
        private DispatcherTimer VisibilityTimer;

        public delegate void ClickEvents(object source, RoutedEventArgs args);
        public event ClickEvents OnFirstBttnClick;
        public event ClickEvents OnSecondBttnClick;

        public CNotification() {
            InitializeComponent();
        }

        public void ShowNotification() {
            if (Visibility == Visibility.Visible) return;
            Visibility = Visibility.Visible;
            VisibilityTimer = new DispatcherTimer() {
                Interval = new TimeSpan(0, 0, ShowTime)
            };
            VisibilityTimer.Tick += ChangeVisibility;
            VisibilityTimer.Start();
        }

        private void ChangeVisibility(object source, EventArgs e) {
            VisibilityTimer.Stop();
            Visibility = Visibility.Collapsed;
        }
        
        private void OnFirstButtonClick(object sender, RoutedEventArgs e) => OnFirstBttnClick?.Invoke(this, e);

        private void OnSecondButtonClick(object sender, RoutedEventArgs e) => OnSecondBttnClick?.Invoke(this, e);

        private void OnNotificationClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VisibilityTimer?.Stop();
            Visibility = Visibility.Collapsed;
        }
    }
}
