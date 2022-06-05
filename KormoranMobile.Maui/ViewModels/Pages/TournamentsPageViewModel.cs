﻿using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using KormoranMobile.Maui.Helpers;
using KormoranMobile.Maui.Services;
using KormoranMobile.Maui.ViewModels.Abstraction;
using KormoranMobile.Maui.ViewModels.Commands;
using KormoranMobile.Maui.Views;
using KormoranMobile.Maui.Views.Pages;
using KormoranMobile.Maui.Views.Popups;
using KormoranShared.Models;
using KormoranShared.Models.Responses;
using Refit;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace KormoranMobile.Maui.ViewModels.Pages
{
	public class TournamentsPageViewModel : ViewModelBase
	{
		private IKormoranServer _kormoranServer;
		private ObservableCollection<Tournament> _tournaments;
		private bool _isRefreshing;
		private AsyncRelayCommand _showServerPopupCommand;
		private AsyncRelayCommand _showLoginPageCommand;
		private AsyncRelayCommand<Tournament> _itemTappedCommand;
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
		public AsyncRelayCommand ShowLoginPageCommand => _showLoginPageCommand;
		public AsyncRelayCommand<Tournament> ItemTappedCommand => _itemTappedCommand;
		public AsyncRelayCommand RefreshTournamentsListCommand => _refreshTournamentsListCommand;

		public TournamentsPageViewModel()
		{
			_isRefreshing = false;
			_tournaments = new();

			_showServerPopupCommand = new(async () =>
			{
				var res = await Application.Current.MainPage.ShowPopupAsync(new SettingsPopup());
				if(res != null)
				{
					Preferences.Set(ServerHelper.AddressKey, (string)res);
					await SetupServer();
				}
			});

			_showLoginPageCommand = new(async () =>
			{
				await Application.Current.MainPage.ShowPopupAsync(new LoginPopup());
			}, () => _kormoranServer != null);

			_itemTappedCommand = new(async (tournament) =>
			{
				try
				{
					await Shell.Current.GoToAsync($"{nameof(MatchesPage)}?tournament={tournament}");
				}
				catch (Exception ex)
				{
					Debug.WriteLine(ex.Message);
				}
			});
			_refreshTournamentsListCommand = new(RefreshTournamentsList,
				() => IsRefreshing == false && _kormoranServer != null);

			try
			{
				SetupServer().Wait();
			}
			catch
			{
				_kormoranServer = null;
				Preferences.Remove(ServerHelper.AddressKey);
			}
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
				IsRefreshing = true;
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

		private async Task<bool> SetupServer()
		{
			if (!ServerHelper.AddressSet)
			{
				await Toast.Make("Nie ustawiono adresu serwera!").Show();
				_kormoranServer = null;
				return false;
			}
			else
			{
				try
				{
					_kormoranServer = RestService.For<IKormoranServer>(ServerHelper.DefaultHttpClient);
				}
				catch
				{
					_kormoranServer = null;
					await Toast.Make("Ustawiony adres jest nie poprawny!").Show();
				}
			}
			_showLoginPageCommand.RaiseCanExecuteChanged();
			_refreshTournamentsListCommand.RaiseCanExecuteChanged();
			return _kormoranServer != null;
		}
	}
}