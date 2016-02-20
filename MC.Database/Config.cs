using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MC.Database {
    public class Config {
        public const string MangaPath = @"manga.json";
        public const string SettingsPath = @"settings.json";

        public static JObject CreateMangaConfig() {
            var config = new JObject {
                ["batoto"] = JObject.Parse("{}"),
                ["mangareader"] = JObject.Parse("{}"),
                ["mangastream"] = JObject.Parse("{}"),
                ["mangafox"] = JObject.Parse("{}"),
                ["kissmanga"] = JObject.Parse("{}"),
                ["backlog"] = JObject.Parse("{}")
            };
            File.WriteAllText(MangaPath, config.ToString());
            return config;
        }

        public static JObject GetMangaConfig() {
            if (File.Exists(MangaPath)) {
                var json = JObject.Parse(File.ReadAllText(MangaPath));
                return json;
            }
            return CreateMangaConfig();
        }

        public static JObject CreateConfig() {
            var settingsconfig = new JObject {
                ["settings"] = JObject.Parse(@"{
                                        'batoto': '0',
                                        'batoto_rss': '',
                                        'kissmanga': '0',
                                        'mangafox': '0',
                                        'mangareader': '0',
                                        'mangastream': '0',
                                        'refresh time': '300',
                                        'open links': '1',
                                        'webtoons': '0',
                                        'yomanga': '0',
                                        'notifications': '1',
                                        'debug': '0'}")
            };
            File.WriteAllText(SettingsPath, settingsconfig.ToString());
            return settingsconfig;
        }

        public static JObject GetConfig() {
            if (File.Exists(SettingsPath)) {
                //// read JSON directly from a file
                using (var file = File.OpenText(SettingsPath))
                using (var reader = new JsonTextReader(file)) {
                    var config = (JObject) JToken.ReadFrom(reader);
                    return config;
                }
            }
            return CreateConfig();
        }


        public string Write(string cfg) {
            try {
                var json = JObject.Parse(cfg);
                if (cfg.Contains("\"batoto\": {")
                    && cfg.Contains("\"kissmanga\": {")
                    && cfg.Contains("\"mangafox\": {")
                    && cfg.Contains("\"mangareader\": {")
                    && cfg.Contains("\"mangastream\": {")
                    && cfg.Contains("\"webtoons\": {")
                    && cfg.Contains("\"yomanga\": {")
                    && cfg.Contains("\"backlog\": {")) {
                    File.WriteAllText(MangaPath, cfg);
                    return "Successful import";
                }
                return "Something is missing";
            }
            catch (Exception ex) {
                return ex.Message;
            }
        }
    }
}