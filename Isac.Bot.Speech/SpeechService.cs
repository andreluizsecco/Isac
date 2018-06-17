using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Isac.Bot.Speech
{
    public class SpeechService
    {
        /// <summary>
        /// Gets text from an audio stream.
        /// </summary>
        /// <param name="audiostream"></param>
        /// <returns>Transcribed text. </returns>
        public async Task<string> GetTextFromAudioAsync(Stream audiostream)
        {
            var requestUri = @"	https://speech.platform.bing.com/speech/recognition/interactive/cognitiveservices/v1?language=pt-BR&format=simple";

            using (var client = new HttpClient())
            {
                var token = Authentication.Instance.GetAccessToken();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                using (var binaryContent = new ByteArrayContent(StreamToBytes(audiostream)))
                {
                    binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                    var response = await client.PostAsync(requestUri, binaryContent);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                        throw new Exception($"({response.StatusCode}) {response.ReasonPhrase}");

                    var responseString = await response.Content.ReadAsStringAsync();
                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(responseString);
                        return data.DisplayText;
                    }
                    catch (JsonReaderException ex)
                    {
                        throw new Exception(responseString, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Converts Stream into byte[].
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <returns>Output byte[]</returns>
        private static byte[] StreamToBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
