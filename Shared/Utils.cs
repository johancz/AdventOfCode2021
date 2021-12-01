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

        private static async Task<string[]> ReadInputFile(string inputFilePath)
        {
            return await File.ReadAllLinesAsync(inputFilePath);
        }
    }
}
