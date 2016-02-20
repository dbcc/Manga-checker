using Manga_checker.Properties;
using MC.Database;

namespace Manga_checker.Handlers {
    internal class StartupInit {
        public static void Setup() {
            Settings.Default.SettingBatoto = ParseFile.GetValueSettings("batoto");
            Settings.Default.SettingMangastream = ParseFile.GetValueSettings("mangastream");
            Settings.Default.SettingMangareader = ParseFile.GetValueSettings("mangareader");
            Settings.Default.SettingMangafox = ParseFile.GetValueSettings("mangafox");
            Settings.Default.SettingKissmanga = ParseFile.GetValueSettings("kissmanga");
            Settings.Default.SettingWebtoons = ParseFile.GetValueSettings("webtoons");
            Settings.Default.SettingYomanga = ParseFile.GetValueSettings("yomanga");
            Settings.Default.SettingOpenLinks = ParseFile.GetValueSettings("open links");
            Settings.Default.SettingBatotoRSS = ParseFile.GetValueSettings("batoto_rss");
            Settings.Default.SettingRefreshTime = int.Parse(ParseFile.GetValueSettings("refresh time"));
            //Settings.Default.Save();
        }
    }
}