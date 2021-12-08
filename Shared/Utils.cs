using HtmlAgilityPack;
using System.Net;

namespace Shared
{
    public static class Utils
    {
        public static async Task<string> GetSessionCookie(string url)
        {
            string sessionCookie = await File.ReadAllTextAsync(Data.SesssionCookiePath);
            return sessionCookie;
        }

        public static async Task<string[]> GetInput(int year, int day, bool overwrite = false)
        {
            string sessionCookie = await GetSessionCookie(Data.SesssionCookiePath);
            string inputFilePath = Path.Combine(Data.InputFileDirPath, $"input-{year}-{day}.txt");
            bool fileExists = File.Exists(inputFilePath);
            string[] inputData = fileExists ? await ReadInputFile(inputFilePath) : Array.Empty<string>();

            if (overwrite || !fileExists || fileExists && string.IsNullOrWhiteSpace(string.Concat(inputData)))
            {
                // Request stuff
                var uri = new Uri("https://adventofcode.com");
                var cookies = new CookieContainer();
                cookies.Add(uri, new Cookie("session", sessionCookie));
                using var handler = new HttpClientHandler();
                handler.CookieContainer = cookies;
                using var client = new HttpClient(handler);
                client.BaseAddress = uri;
                using var response = await client.GetAsync($"/{year}/day/{day}/input");
                using var stream = await response.Content.ReadAsStreamAsync();

                // File stuff
                string? directoryName = Path.GetDirectoryName(inputFilePath);

                if (string.IsNullOrEmpty(directoryName))
                {
                    throw new Exception($"DirectoryName for {inputFilePath} is 'null' or 'string.Empty'");
                }

                Directory.CreateDirectory(directoryName);
                using var file = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

                await stream.CopyToAsync(file);
                var a = await response.Content.ReadAsStringAsync();
                inputData = (await response.Content.ReadAsStringAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
            }

            return inputData;
        }

        public static async Task<string> ReadHtmlPage(string inputFilePath)
        {
            return await File.ReadAllTextAsync(inputFilePath);
        }

        public static async Task<string[]> ReadInputFile(string inputFilePath)
        {
            return await File.ReadAllLinesAsync(inputFilePath);
        }

        public async static Task<string> GetExampleInput(int year, int day, bool overwrite = false)
        {
            string inputFilePath = Path.Combine(Data.InputFileDirPath, $"input-{year}-{day}-page.html");
            bool fileExists = File.Exists(inputFilePath);
            string inputData = fileExists ? await ReadHtmlPage(inputFilePath) : "";

            if (overwrite || !fileExists || fileExists && string.IsNullOrWhiteSpace(inputData))
            {
                // Web scraping stuff
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load($"https://adventofcode.com/{year}/day/{day}");

                // File stuff
                string? directoryName = Path.GetDirectoryName(inputFilePath);

                if (string.IsNullOrEmpty(directoryName))
                {
                    throw new Exception($"DirectoryName for {inputFilePath} is 'null' or 'string.Empty'");
                }

                Directory.CreateDirectory(directoryName);
                using var file = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
                using var writer = new StreamWriter(file);

                await writer.WriteAsync(doc.DocumentNode.OuterHtml);
                inputData = doc.DocumentNode.OuterHtml;
            }

            // Scrape until it finds the first 'pre > code' node and return its "InnerText".

            var doc2 = new HtmlDocument();
            doc2.LoadHtml(inputData);

            var articleNode = doc2.DocumentNode.SelectSingleNode("/html/body/main/article");

            if (articleNode == null)
            {
                throw new Exception($"Could not find the <article> node in today's page source.");
            }

            var codeNode = articleNode.ChildNodes.FirstOrDefault(x => x.Name == "pre" && x.FirstChild?.Name == "code")?.FirstChild;

            if (codeNode == null)
            {
                throw new Exception($"Could not find a single 'pre > code' node in today's page source.");
            }

            return codeNode.InnerText;
        }





        //public static async Task<T[]> GetInput<T>(int year, int day, bool overwrite = false)
        //{
        //    string sessionCookie = await GetSessionCookie(Data.SesssionCookiePath);
        //    string inputFilePath = Path.Combine(Data.InputFileDirPath, $"input-{year}-{day}.txt");
        //    bool fileExists = File.Exists(inputFilePath);
        //    T[] inputData = fileExists ? await ReadInputFile<T>(inputFilePath) : Array.Empty<T>();

        //    if (overwrite || !fileExists || fileExists && string.IsNullOrWhiteSpace(string.Concat(inputData)))
        //    {
        //        // Request stuff
        //        var uri = new Uri("https://adventofcode.com");
        //        var cookies = new CookieContainer();
        //        cookies.Add(uri, new Cookie("session", sessionCookie));
        //        using var handler = new HttpClientHandler();
        //        handler.CookieContainer = cookies;
        //        using var client = new HttpClient(handler);
        //        client.BaseAddress = uri;
        //        using var response = await client.GetAsync($"/{year}/day/{day}/input");
        //        using var stream = await response.Content.ReadAsStreamAsync();
        //        //var stream = await response.Content.ReadAsByteArrayAsync();

        //        // File stuff
        //        string? directoryName = Path.GetDirectoryName(inputFilePath);

        //        if (string.IsNullOrEmpty(directoryName))
        //        {
        //            throw new Exception($"DirectoryName for {inputFilePath} is 'null' or 'string.Empty'");
        //        }

        //        Directory.CreateDirectory(directoryName);
        //        using var file = new FileStream(inputFilePath, FileMode.Create, FileAccess.Write, FileShare.None);

        //        await stream.CopyToAsync(file);
        //        var a = await response.Content.ReadAsStringAsync();
        //        inputData = (await response.Content.ReadAsStringAsync()).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        //    }

        //    return inputData;
        //}

        //private static async Task<T[]> ReadInputFile<T>(string inputFilePath)
        //{
        //    return await File.ReadAllLinesAsync(inputFilePath);
        //}
    }
}
