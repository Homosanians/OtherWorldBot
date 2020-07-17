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
        private readonly ConfigService _configService;
        private readonly DatabaseService _databaseService;

        public ScheduleUpdateService(LogService logService, ConfigService configService, DatabaseService databaseService)
        {
            _logService = logService;
            _configService = configService;
            _databaseService = databaseService;

            // Create & enable the timer 
            Timer scheduleTimer = new Timer(_configService.BotConfig.UpdateRateInMinutes * 60 * 1000); // ms by default
            scheduleTimer.Elapsed += OnTimedEvent;
            scheduleTimer.Enabled = true;
        }

        private async void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            _logService.Log(LogLevel.Info, "ScheduleUpdateService", "Starting update routine...");

            int today = DateTime.UtcNow.Day;
            
            var entries = await _databaseService.GetAllCountriesAsync();

            foreach (var entry in entries)
            {
                int lastUpdateDay = TimeUtil.UnixTimeStampToDateTime(entry.LastUpdateTimestamp).Day;

                if (lastUpdateDay != today)
                {
                    entry.DisgracePoints -= _configService.BotConfig.UpdateDecreaseValue;

                    await _databaseService.UpdateCountryAsync(entry);
                }
            }
        }

        public TimeSpan GetTimeTillUpdate()
        {
            DateTime now = DateTime.UtcNow;
            DateTime tomorrow = now.AddDays(1).Date;

            return tomorrow - now;
        }
    }
}