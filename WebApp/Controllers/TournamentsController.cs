﻿using KormoranAdminSystemRevamped.Contexts;
using KormoranAdminSystemRevamped.Properties;
using KormoranAdminSystemRevamped.Services;
using KormoranShared.Models;
using KormoranShared.Models.Requests.Tournaments;
using KormoranShared.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KormoranAdminSystemRevamped.Controllers.MatchesController;

namespace KormoranAdminSystemRevamped.Controllers
{
	[Route(@"api/[controller]/[action]")]
	[ApiController]
	public class TournamentsController : ControllerBase
	{
		private readonly KormoranContext _db;
		private readonly ISessionManager _sessionManager;
		public TournamentsController(KormoranContext dp, ISessionManager sessionManager)
		{
			_db = dp;
			_sessionManager = sessionManager;
		}

		[HttpGet]
		public async Task<JsonResult> GetTournaments([FromQuery] int? id)
		{
			try
			{
				var tournamentList = await _db.Tournaments
					.Include(x => x.Teams)
					.Include(x => x.Matches)
					.Include(x => x.Discipline)
					.Include(x => x.State)
					.OrderBy(x => x.TournamentId)
					.ToListAsync();
				if (id.HasValue)
				{
					var tournament = tournamentList.FirstOrDefault(x => x.TournamentId == id.Value);
					if (tournament != null)
					{
						return new JsonResult(new SingleItemResponse<Tournament>
						{
							Data = tournament,
							Message = Resources.operationSuccessfull,
							Error = false
						});
					}
					return new JsonResult(new SingleItemResponse<Tournament>
					{
						Data = null,
						Message = Resources.serverError,
						Error = true
					});
				}


				return new JsonResult(new CollectionResponse<Tournament>()
				{
					Message = Resources.operationSuccessfull,
					Error = false,
					Collection = tournamentList
				});
			}
			catch
			{
				return new JsonResult(new CollectionResponse<Tournament>()
				{
					Message = Resources.serverError,
					Error = true,
					Collection = null
				});
			}
		}

		[HttpPost]
		public async Task<JsonResult> UpdateTournamentBasic([FromBody] UpdateTournamentRequestModel request)
		{
			try
			{
				var tournament = await _db.Tournaments
					.FirstOrDefaultAsync(x => x.TournamentId == request.TournamentId);

				if (tournament == null)
				{
					_db.Tournaments.Add(new Tournament
					{
						Name = request.NewName,
						DisciplineId = request.NewDisciplineId,
						StateId = request.NewStateId
					});
				}
				else
				{
					tournament.Name = request.NewName;
					tournament.StateId = request.NewStateId;
					tournament.DisciplineId = request.NewDisciplineId;
					_db.Tournaments.Update(tournament);
				}
				await _db.SaveChangesAsync();

				return new JsonResult(new BasicResponse
				{
					Error = false,
					Message = Resources.operationSuccessfull
				});
			}
			catch (NullReferenceException)
			{
				return new JsonResult(new BasicResponse() 
				{
					Error = true,
					Message = "Turniej nie został znaleziony"
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new BasicResponse
				{
					Error = true,
					Message = $"Błąd serwera! {ex.Message}"
				});
			}
		}

		[HttpPost]
		public async Task<JsonResult> UpdateTournament([FromBody] TournamentFullUpdateRequestModel request)
		{
			try
			{
				var matchesToAdd = new List<Match>();
				var matchesToUpdate = new List<Match>();
				foreach(var matchData in request.Matches)
				{
					if (matchData.MatchId >= 100000)
					{
						var newMatch = new Match
						{
							TournamentId = matchData.TournamentId,
							MatchId = 0,
							StateId = matchData.StateId,
							Team1Id = matchData.Team1,
							Team2Id = matchData.Team2,
							Team1Score = matchData.Team1Score,
							Team2Score = matchData.Team2Score
						};
						_db.Add(newMatch);
						await _db.SaveChangesAsync();
					}
					else
					{
						var match = await _db.Matches.FirstOrDefaultAsync(x => x.MatchId == matchData.MatchId);
						match.StateId = matchData.StateId;
						match.Team1Id = matchData.Team1;
						match.Team2Id = matchData.Team2;
						match.Team1Score = matchData.Team1Score;
						match.Team2Score = matchData.Team2Score;
						_db.Matches.Update(match);
						await _db.SaveChangesAsync();
					}
				}
				var tournament = await _db.Tournaments
					.FirstOrDefaultAsync(x => x.TournamentId == request.TournamentId);

				tournament.Name = request.NewName;
				tournament.StateId = request.NewStateId;
				tournament.DisciplineId = request.NewDisciplineId;

				_db.Tournaments.Update(tournament);
				await _db.SaveChangesAsync();

				return new JsonResult(new BasicResponse
				{
					Error = false,
					Message = Resources.operationSuccessfull
				});
			}
			catch (NullReferenceException)
			{
				return new JsonResult(new BasicResponse()
				{
					Error = true,
					Message = "Turniej nie został znaleziony"
				});
			}
			catch (Exception ex)
			{
				return new JsonResult(new BasicResponse
				{
					Error = true,
					Message = $"Błąd serwera! {ex.Message}"
				});
			}
		}

		[HttpGet]
		public async Task<JsonResult> GetLeaderboard([FromQuery]int tournamentId)
        {
			var matches = await _db.Matches
				.Include(x => x.Team1)
				.Include(x => x.Team2)
				.Where(x => x.TournamentId == tournamentId)
				.ToListAsync();

			var leaderboardDict = new Dictionary<Team, int>();
			await _db.Teams
				.Where(x => x.TournamentId == tournamentId)
				.ForEachAsync(team => leaderboardDict.Add(team, 0));

			var winners = matches
				.GroupBy(x => x.Winner)
				.Select(x => new LeaderboardEntry
				{
					Team = x.Key,
					Wins = x.Count()
				});

			foreach(var winner in winners)
            {
				leaderboardDict[winner.Team!] = winner.Wins;
            }

			var leaderboard = new List<LeaderboardEntry>();
			foreach(var leaderboardKV in leaderboardDict)
            {
				leaderboard.Add(new LeaderboardEntry
				{
					Team = leaderboardKV.Key,
					Wins = leaderboardKV.Value
				});
            }

            return new JsonResult(new CollectionResponse<LeaderboardEntry>
			{
				Error = false,
				Message = Resources.operationSuccessfull,
				Collection = leaderboard.OrderByDescending(x => x.Wins).ToList()
			});
		}
	}
}
