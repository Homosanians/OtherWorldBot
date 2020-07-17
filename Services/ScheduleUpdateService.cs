using DisgraceDiscordBot.Entities;
using DisgraceDiscordBot.Utils;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace DisgraceDiscordBot.Services
{
    public class ScheduleUpdateService
    {
        private readonly LogService _logService;
        private readonly DatabaseService _databaseService;

        public ScheduleUpdateService(LogService logService, DatabaseService databaseService)
        {
            _logService = logService;
            _databaseService = databaseService;

            // Create & enable a 30 minutes timer 
            //Timer scheduleTimer = new Timer(1800000);
            Timer scheduleTimer = new Timer(5 * 1000); // ms
            scheduleTimer.Elapsed += OnTimedEvent;
            scheduleTimer.Enabled = true;
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _logService.Log(LogLevel.Info, "ScheduleUpdateService", "Starting update routine...");

            int today = DateTime.UtcNow.Day+1;
            
            var entries = await _databaseService.GetAllCountries();

            foreach (var entry in entries)
            {
                int lastUpdateDay = TimeUtil.UnixTimeStampToDateTime(entry.LastUpdateTimestamp).Day;

                await _databaseService.UpdateCountry(entry);
                if (lastUpdateDay < today)
                {
                    // TODO: Config
                    entry.DisgracePoints -= 2;

                    await _databaseService.UpdateCountry(entry);
                }
            }
        }
    }
}