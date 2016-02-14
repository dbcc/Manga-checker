using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading;
using System.Threading.Tasks;
using MC.Models;

namespace MC.Database {
    public class Sqlite {
        private static readonly Dictionary<string, string> Sites = new Dictionary<string, string> {
            {"Mangafox", "http://mangafox.me/"},
            {"Mangahere", "http://mangahere.co/"},
            {"Mangareader", "http://www.mangareader.net/"},
            {"Mangastream", "http://mangastream.com/"},
            {"Batoto", "http://bato.to/"},
            {"Webtoons", "http://www.webtoons.com/"},
            {"YoManga", "http://yomanga.co/"},
            {"Kissmanga", "http://kissmanga.com/"},
            {"Backlog", "/"}
        };


        private static string[] _otherTables = {"link_collections", "settings"};

        // public static void test() {
        // SetupDatabase();
        // PopulateDb();
        // UpdateManga("Mangafox", "akame ga kill", "64", "www.bitch.com");
        // GetMangas("mangafox");
        // DebugText.Write(GetMangaInfo("batoto", "Prison School").Name);
        // }
        public static void PopulateDb() {
            try {
                foreach (var site in Sites) {
                    foreach (var manga in ParseFile.GetManga(site.Key.ToLower())) {
                        var chna = manga.Split(new[] { "[]" }, StringSplitOptions.None);
                        AddManga(site.Key.ToLower(), chna[0], chna[1], chna.Length.Equals(2) ? "placeholder" : chna[2],
                            DateTime.Now);
                        Thread.Sleep(100);
                    }
                }
            } catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void SetupDatabase() {
            try {
                SQLiteConnection.CreateFile("MangaDB.sqlite");

                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();

                foreach (var site in Sites) {
                    var sql =
                        $@"CREATE TABLE '{site.Key.ToLower()
                            }' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'last_update'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200),
	                        'rss_url'	varchar(200)
                        );";

                    // var sql =
                    // $"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";
                    var command = new SQLiteCommand(sql, mDbConnection);
                    command.ExecuteNonQuery();
                }

                new SQLiteCommand(@"
                        CREATE TABLE 'settings' (
	                        'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'link'	varchar(200),
	                        'active' INTEGER NOT NULL DEFAULT (0),
	                        'added'	datetime NOT NULL DEFAULT (datetime())
                        );", mDbConnection).ExecuteNonQuery();

                foreach (var keyValuePair in Sites) {
                    new SQLiteCommand(
                        $"INSERT INTO settings (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                    Thread.Sleep(100);
                }

                new SQLiteCommand(
                    $"INSERT INTO settings (name, active) VALUES ('open links', 0)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, active) VALUES ('refresh time', 300)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link) VALUES ('batoto_rss', '/')",
                    mDbConnection).ExecuteNonQuery();

                new SQLiteCommand(
                    @"CREATE TABLE 'link_collection' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'site'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'added'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200) NOT NULL
                        );",
                    mDbConnection).ExecuteNonQuery();

                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            GetAllTables();
        }

        public static bool AddManga(string site, string name, string chapter, string rss, DateTime date,
            string link = "placeholder") {
            var mangas = GetMangaNameList(site);
            mangas = mangas.ConvertAll(i => i.ToLower());

            if (mangas.Contains(name.ToLower())) {
                return false;
            }
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"insert into {site} (name, chapter, last_update, link, rss_url) values ('{name.Replace("'", "''")}', '{chapter}', datetime('{date.ToString("yyyy-MM-dd HH:mm:ss")}'), '{link}', '{rss}')";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return false;
            }
            return true;
        }

        public static void DeleteManga(MangaModel item) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"DELETE FROM {item.Site.ToLower()} WHERE id = {item.Id}";
                var command = new SQLiteCommand(sql, mDbConnection);
                command.ExecuteNonQuery();
                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static void UpdateManga(string site, string name, string chapter, string link, DateTime date,
            bool linkcol = true) {
            try {
                name = name.Replace("'", "''");
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                mDbConnection.Open();
                var sql =
                    $"UPDATE {site.ToLower()} SET chapter = '{chapter}', link = '{link}', last_update = datetime('{date.ToString("yyyy-MM-dd HH:mm:ss")}') WHERE name = '{name}'";
                new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();

                if (!site.ToLower().Equals("backlog") && linkcol) {
                    new SQLiteCommand(
                        $@"INSERT INTO link_collection (name, chapter, added, link, site) VALUES ('{name}', '{chapter
                            }', (datetime()), '{link}', '{site
                                .ToLower()}')", mDbConnection).ExecuteNonQuery();
                }

                DebugText.Write($"{mDbConnection.Changes} rows affected ");
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }

        public static List<MangaModel> GetMangas(string site) {
            var mangas = new List<MangaModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = (DateTime) reader["last_update"]
                                });
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mangas;
        }
        public static async Task<List<MangaModel>> GetMangasAsync(string site) {
            var mangas = new List<MangaModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (await reader.ReadAsync()) {
                                mangas.Add(new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = (DateTime) reader["last_update"]
                                });
                            }
                        }
                    }
                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mangas;
        }

        public static MangaModel GetMangaInfo(string site, string name) {
            try {
                MangaModel m;
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM {site.ToLower()} WHERE name = '{name}'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                m = new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = site,
                                    Link = reader["link"].ToString(),
                                    RssLink = reader["rss_url"].ToString(),
                                    Date = (DateTime) reader["last_update"]
                                };
                                mDbConnection.Close();
                                return m;
                            }
                        }
                    }
                }

                return m = new MangaModel();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
                return new MangaModel();
            }
        }

        public static List<string> GetMangaNameList(string site) {
            var mangas = new List<string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT name FROM {site.ToLower()}";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(reader["name"].ToString());
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mangas;
        }

        public static List<string> GetAllTables() {
            var tables = new List<string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();

                    // var sql = "SELECT name FROM sqlite_sequence";
                    var sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                tables.Add(reader.GetString(0));
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            // DebugText.Write($"tables\n{string.Join("\n", tables)}");
            return tables;
        }

        public static void UpdateDatabase() {
            var existingTables = GetAllTables();
            Thread.Sleep(100);
            if (!existingTables.Contains("settings")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(@"
                        CREATE TABLE 'settings' (
	                        'id'	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'link'	varchar(200),
	                        'active' INTEGER NOT NULL DEFAULT (0),
	                        'added'	datetime NOT NULL DEFAULT (datetime())
                        );", mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table settings to Database");
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link, active) VALUES ('open links', '/',  0)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, link, active) VALUES ('refresh time', '/', 300)",
                    mDbConnection).ExecuteNonQuery();
                new SQLiteCommand(
                    $"INSERT INTO settings (name, active, link) VALUES ('batoto_rss', 300, '/')",
                    mDbConnection).ExecuteNonQuery();
            }

            foreach (var keyValuePair in Sites) {
                if (!existingTables.Contains(keyValuePair.Key.ToLower())) {
                    var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                    mDbConnection.Open();
                    var sql =
                        $@"CREATE TABLE '{keyValuePair.Key.ToLower()
                            }' (
	                    'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                    'name'	varchar(200) NOT NULL,
	                    'chapter'	varchar(200) NOT NULL,
	                    'last_update'	datetime NOT NULL DEFAULT (datetime()),
	                    'link'	varchar(200),
	                    'rss_url'	varchar(200)
                    );";

                    // var sql =
                    // $"create table {site.ToLower()} (name varchar(50) not null, chapter varchar(20) not null, last_update varchar(100), link varchar(200), rss_url varchar(200))";
                    new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
                    new SQLiteCommand(
                        $"INSERT INTO settings (name, link, active) VALUES ('{keyValuePair.Key.ToLower()}', '{keyValuePair.Value}', 0)",
                        mDbConnection).ExecuteNonQuery();
                    DebugText.Write($"Added table {keyValuePair.Key} to Database");
                    mDbConnection.Close();
                }

                Thread.Sleep(100);
            }

            if (!existingTables.Contains("link_collection")) {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;");
                mDbConnection.Open();
                new SQLiteCommand(
                    @"CREATE TABLE 'link_collection' (
	                        'id'	INTEGER PRIMARY KEY AUTOINCREMENT,
	                        'name'	varchar(200) NOT NULL,
	                        'site'	varchar(200) NOT NULL,
	                        'chapter'	varchar(200) NOT NULL,
	                        'added'	datetime NOT NULL DEFAULT (datetime()),
	                        'link'	varchar(200) NOT NULL
                        );",
                    mDbConnection).ExecuteNonQuery();
                DebugText.Write($"Added table link_collections to Database");
            }
        }

        public static List<MangaModel> GetHistory() {
            var mangas = new List<MangaModel>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM link_collection";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                mangas.Add(new MangaModel {
                                    Id = reader.GetInt32(0),
                                    Name = reader["name"].ToString(),
                                    Chapter = reader["chapter"].ToString(),
                                    Site = reader["site"].ToString(),
                                    Link = reader["link"].ToString(),
                                    Date = (DateTime) reader["added"]
                                });
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return mangas;
        }

        public static Dictionary<string, string> GetSettings() {
            var settings = new Dictionary<string, string>();
            try {
                using (var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;")) {
                    mDbConnection.Open();
                    var sql = $"SELECT * FROM settings";
                    using (var command = new SQLiteCommand(sql, mDbConnection)) {
                        using (var reader = command.ExecuteReader()) {
                            while (reader.Read()) {
                                if (reader["name"].ToString() == "batoto_rss") {
                                    settings[reader["name"].ToString()] = reader["link"].ToString();
                                }
                                else {
                                    settings[reader["name"].ToString()] = reader["active"].ToString();
                                }
                            }
                        }
                    }

                    mDbConnection.Close();
                }
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }

            return settings;
        }


        public static void UpdateSetting(string site, string active) {
            try {
                var mDbConnection = new SQLiteConnection("Data Source=MangaDB.sqlite;Version=3;");
                string sql;
                mDbConnection.Open();
                if (site == "batoto_rss") {
                    sql =
                        $"UPDATE settings SET link = '{active}', added = (datetime()) WHERE name = '{site}'";
                    new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
                    mDbConnection.Close();
                    return;
                }

                sql =
                    $"UPDATE settings SET active = {int.Parse(active)}, added = (datetime()) WHERE name = '{site}'";
                new SQLiteCommand(sql, mDbConnection).ExecuteNonQuery();
                mDbConnection.Close();
            }
            catch (Exception e) {
                DebugText.Write(e.Message);
            }
        }
    }
}