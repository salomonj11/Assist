﻿using Assist.MVVM.ViewModel;
using Assist.Settings;

using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Serilog;

namespace Assist.MVVM.View.Settings.SettingPages
{
    /// <summary>
    /// Interaction logic for Settings_General.xaml
    /// </summary>
    public partial class Settings_General : Page
    {

        public Settings_General()
        {
            DataContext = AssistSettings.Current;
            InitializeComponent();
        }

        private async void Settings_General_Loaded(object sender, RoutedEventArgs e)
        {
            WindowSizeComboBox.SelectedIndex = (int)AssistSettings.Current.Resolution;
            LanguageChangeComboBox.SelectedIndex = (int)AssistSettings.Current.Language;
            SoundVol_Slider.Value = AssistSettings.Current.SoundVolume;
            SoundVol_Label.Content = Convert.ToInt32(SoundVol_Slider.Value * 100) + "%";
            AccountSelectToggle.IsChecked = AssistSettings.Current.UseAccountLaunchSelection;
            UpgradeBTN.Visibility = await CheckUpgrade();
        }

        #region Language Selection Settings

        private bool _initialLanguageSelectionChange = true;

        private async Task<Visibility> CheckUpgrade()
        {
            string upgradeURL = "https://api.assistapp.dev/config/upgrade";
            var c = new HttpClient();
            var r = await c.GetAsync(upgradeURL);
            if(!r.IsSuccessStatusCode)
                return Visibility.Collapsed;
            else
            {
                return Visibility.Visible;
            }
        }

        private void LanguageChangeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_initialLanguageSelectionChange)
            {
                _initialLanguageSelectionChange = false;
                return;
            }

            AssistSettings.Current.Language = (Enums.ELanguage)LanguageChangeComboBox.SelectedIndex;
            App.ChangeLanguage();
            System.Windows.Forms.Application.Restart();
            System.Windows.Application.Current.Shutdown();
        }

        #endregion

        #region Window Size Settings

        private async void WindowSizeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(WindowSizeComboBox.SelectedIndex == (int)AssistSettings.Current.Resolution)
                return;

            Trace.WriteLine(WindowSizeComboBox.SelectedIndex);
            AssistSettings.Current.Resolution = (Enums.EWindowSize) WindowSizeComboBox.SelectedIndex;
            AssistApplication.AppInstance.OpenAssistMainWindowToSettings();
        }



        #endregion

        private void SoundVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            AssistSettings.Current.SoundVolume = SoundVol_Slider.Value;
            SoundVol_Label.Content = Convert.ToInt32(SoundVol_Slider.Value * 100) + "%";
        }

        private void AccountSelectToggle_Changed(object sender, RoutedEventArgs e)
        {
            if (AccountSelectToggle.IsChecked is null || AccountSelectToggle.IsChecked is false)
                AssistSettings.Current.UseAccountLaunchSelection = false;
            else
                AssistSettings.Current.UseAccountLaunchSelection = true;

            Log.Information("Value of AccountSelectToggle {sad}", AssistSettings.Current.UseAccountLaunchSelection);
        }
    }
}
