using DisgraceDiscordBot.Entities;
using DisgraceDiscordBot.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace DisgraceDiscordBot.Services
{
    public class ScheduleUpdateService
    {
        DatabaseService _databaseService;
        
        public ScheduleUpdateService(DatabaseService databaseService)
        {
            _databaseService = databaseService;

            // Create & enable a 30 minutes timer 
            Timer scheduleTimer = new Timer(1800000);
            scheduleTimer.Elapsed += OnTimedEvent;
            scheduleTimer.Enabled = true;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            int today = DateTime.UtcNow.Day;

            Country[] entries = _databaseService.GetAllCountries();

            foreach (var entry in entries)
            {
                int lastUpdateDay = TimeUtil.UnixTimeStampToDateTime(entry.LastUpdateTimestamp).Day;

                if (lastUpdateDay < today)
                {
                    // TODO: Config
                    entry.DisgracePoints -= 2;

                    _databaseService.UpdateCountry(entry);
                }
            }
        }
    }
}