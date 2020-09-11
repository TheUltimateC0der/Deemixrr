namespace Deemixrr.Configuration
{
    public class DelayConfiguration
    {
        public int ImportArtistsBackgroundJob_ExecuteDelay { get; set; }

        public int CheckArtistForUpdatesBackgroundJob_GetTrackCountDelay { get; set; }
        public int CheckArtistForUpdatesBackgroundJob_ExecuteDelay { get; set; }

        public int CheckPlaylistForUpdatesBackgroundJob_ExecuteDelay { get; set; }

        public int CreateArtistBackgroundJob_FromPlaylistDelay { get; set; }
        public int CreateArtistBackgroundJob_FromUserDelay { get; set; }
        public int CreateArtistBackgroundJob_FromCsvDelay { get; set; }

        public int CreatePlaylistBackgroundJob_FromCsvDelay { get; set; }
    }
}