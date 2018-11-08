using Microsoft.Bot.Connector;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Isac.Bot.Services
{
    public class Audio
    {
        public async Task<Stream> GetAudioStream(ConnectorClient connector, Attachment audioAttachment)
        {
            using (var httpClient = new HttpClient())
            {
                // The Skype attachment URLs are secured by JwtToken,
                // you should set the JwtToken of your bot as the authorization header for the GET request your bot initiates to fetch the image.
                // https://github.com/Microsoft/BotBuilder/issues/662
                var uri = new Uri(audioAttachment.ContentUrl);
                if (uri.Host.EndsWith("skype.com") && uri.Scheme == "https")
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetTokenAsync(connector));
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
                }

                var stream = await httpClient.GetStreamAsync(uri);
                if (audioAttachment.ContentType.Equals("audio/ogg"))
                    return ConvertOggToWav(stream);
                return stream;
            }
        }

        /// <summary>
        /// Gets the JwT token of the bot. 
        /// </summary>
        /// <param name="connector"></param>
        /// <returns>JwT token of the bot</returns>
        private async Task<string> GetTokenAsync(ConnectorClient connector)
        {
            var credentials = connector.Credentials as MicrosoftAppCredentials;
            if (credentials != null)
                return await credentials.GetTokenAsync();
            return null;
        }

        private Stream ConvertOggToWav(Stream stream)
        {
            var path = $@"{AppContext.BaseDirectory}Data\Audio\";
            var name = Guid.NewGuid().ToString();

            var source = path + name + ".ogg";
            var destination = source.Replace("ogg", "wav");

            Directory.CreateDirectory(path);

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                File.WriteAllBytes(source, ms.ToArray());
            }

            var bs = File.ReadAllBytes(source);
            var psi = new ProcessStartInfo();
            psi.FileName = $@"{AppContext.BaseDirectory}Tools\opusdec.exe";
            psi.Arguments = $"--rate 16000 \"{source}\" \"{destination}\"";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;

            var process = Process.Start(psi);
            process.WaitForExit((int)TimeSpan.FromSeconds(60).TotalMilliseconds);
            var bytes = File.ReadAllBytes(destination);

            Directory.Delete(path, true);

            return new MemoryStream(bytes);
        }
    }
}