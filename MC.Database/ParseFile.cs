using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace MC.Database {
    public class ParseFile {
        public const string Path = @"manga.json";
        public const string SettingsPath = @"settings.json";


        public static List<string> GetManga(string site) {
            var jsmanga = new List<string>();
            var conf = Config.GetMangaConfig();
            if (!conf.ToString().Contains(site)) {
                conf[site] = JObject.Parse("{}");
                File.WriteAllText(Path, conf.ToString().ToLower());
                return jsmanga;
            }
            var dick = conf[site].Value<JObject>();
            foreach (var manga in dick) {
                if (site == "webtoons") {
                    var obj = JObject.Parse(manga.Value.ToString());
                    jsmanga.Add(manga.Key + "[]" + obj["chapter"] + "[]" + obj["url"]);
                }
                else {
                    jsmanga.Add(manga.Key + "[]" + manga.Value);
                }
            }
            return jsmanga;
        }

        public static List<string> GetBatotoMangaNames() {
            var jsmanga = new List<string>();
            var conf = Config.GetMangaConfig();
            foreach (var manga in conf["batoto"].Value<JObject>()) {
                jsmanga.Add(manga.Key);
            }

            return jsmanga;
        }

        public static void SetManga(string site, string Name, string Value) {
            var conf = Config.GetMangaConfig();
            try {
                if (site.Equals("webtoons")) {
                    conf[site][Name]["chapter"] = Value;
                }
                else {
                    conf[site][Name] = Value;
                }
            }
            catch (Exception) {
                AddManga(site, Name, Value, "");
            }

            File.WriteAllText(Path, conf.ToString());
        }

        public static string GetValueSettings(string Name) {
            var conf = Config.GetConfig();
            try {
                return conf["settings"][Name].ToString();
            }
            catch (Exception) {
                SetValueSettings(Name, "0");
                return "0";
            }
        }

        public static void SetValueSettings(string Name, string Value) {
            var _Config = Config.GetConfig();
            _Config["settings"][Name] = Value;
            File.WriteAllText(SettingsPath, _Config.ToString());
        }


        public static void AddManga(string site, string name, string chapter, string url) {
            var ch = JObject.Parse("{'chapter': '" + chapter + "', 'url': '" + url + "'}");
            var conf = Config.GetMangaConfig();
            if (site.Equals("webtoons")) {
                conf[site][name] = ch;
            }
            else {
                conf[site][name] = chapter;
            }
            File.WriteAllText(Path, conf.ToString());
            DebugText.Write($"[Debug] Added {site} {name} {chapter} to .json file.");
        }

        public static void RemoveManga(string site, string name) {
            var conf = Config.GetMangaConfig();
            conf[site][name].Parent.Remove();
            File.WriteAllText(Path, conf.ToString());
            DebugText.Write($"[Debug] Removed {name} from Backlog.");
        }


        public static void AddMangatoBacklog(string site, string name, string chapter) {
            var conf = Config.GetMangaConfig();
            try {
                conf[site][name] = chapter;
                File.WriteAllText(Path, conf.ToString());
                DebugText.Write($"[Debug] Added {site} {name} {chapter} to .json file.");
            }
            catch (Exception) {
                var ch = JObject.Parse("{'" + name + "': '" + chapter + "'}");
                conf[site] = ch;
                File.WriteAllText(Path, conf.ToString());
                DebugText.Write($"[Debug] Added {site} {name} {chapter} to .json file.");
            }
        }
    }
}