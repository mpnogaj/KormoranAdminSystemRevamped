﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using KormoranMobile.Maui.Helpers;
using KormoranMobile.Maui.Services;
using KormoranMobile.Maui.ViewModels.Abstraction;
using KormoranMobile.Maui.ViewModels.Commands;
using KormoranShared.Models;
using KormoranShared.Models.Responses;
using Refit;
using System.Collections.ObjectModel;

namespace KormoranMobile.Maui.ViewModels
{
    public class TournamentsPageViewModel : ViewModelBase
    {
        private IKormoranServer _kormoranServer;
        private ObservableCollection<Tournament> _tournaments;
        private bool _isRefreshing;
        private AsyncRelayCommand _showServerPopupCommand;
        private AsyncRelayCommand _refreshTournamentsListCommand;


        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ObservableCollection<Tournament> Tournaments
        {
            get => _tournaments;
            private set => SetProperty(ref _tournaments, value);
        }

        public AsyncRelayCommand ShowServerPopupCommand => _showServerPopupCommand;
        public AsyncRelayCommand RefreshTournamentsListCommand => _refreshTournamentsListCommand;

        public TournamentsPageViewModel()
        {
            _isRefreshing = false;
            _tournaments = new();

            _showServerPopupCommand = new(async () =>
            {
                var res = await Application.Current.MainPage.DisplayPromptAsync(
                    "Ustawienia", "Adres serwera",
                    initialValue: Preferences.Get(ServerHelper.AddressKey, string.Empty));
                if (res == null) return;
                Preferences.Set(ServerHelper.AddressKey, res);
                await SetupServer();
            });

            _refreshTournamentsListCommand = new(RefreshTournamentsList);

            SetupServer().Wait();
            //RefreshTournamentsList().Wait();
        }

        private async Task RefreshTournamentsList()
        {
            if (_kormoranServer == null || !ServerHelper.AddressSet)
            {
                await Toast.Make("Nie ustawiono adresu serwera!").Show();
                return;
            }
            try
            {
                CollectionResponse<Tournament> response =
                                await _kormoranServer.GetTournaments();
                if (response.Error)
                {
                    await Toast.Make(response.Message, ToastDuration.Long).Show();
                }
                else
                {
                    Tournaments = new(response.Collection);
                }
            }
            catch (Exception ex)
            {
                await Toast.Make(ex.Message, ToastDuration.Long).Show();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task SetupServer()
        {
            if (!ServerHelper.AddressSet)
            {
                await Toast.Make("Nie ustawiono adresu serwera!").Show();
                _kormoranServer = null;
            }
            else
            {
                _kormoranServer = RestService.For<IKormoranServer>(new HttpClient
                {
                    BaseAddress = new Uri(ServerHelper.ServerAddress),
                    Timeout = TimeSpan.FromSeconds(8)
                });
            }
        }
    }
}