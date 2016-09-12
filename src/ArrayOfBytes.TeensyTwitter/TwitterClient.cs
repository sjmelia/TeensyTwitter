namespace ArrayOfBytes.TeensyTwitter
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using ArrayOfBytes.OAuth.Client;

    /// <summary>
    /// Teensy client for the Twitter API.
    /// </summary>
    public class TwitterClient
    {
        private readonly HttpClient client;

        private readonly DataContractJsonSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterClient"/> class.
        /// </summary>
        /// <param name="config">OAuth credentials.</param>
        public TwitterClient(OAuthConfig config)
        {
            var handler = new OAuthHttpMessageHandler(config);
            this.client = new HttpClient(handler);

            var serializerSettings = new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("ddd MMM dd HH:mm:ss zz00 yyyy")
            };

            this.serializer = new DataContractJsonSerializer(typeof(IEnumerable<Tweet>), serializerSettings);
        }

        /// <summary>
        /// Update status.
        /// </summary>
        /// <param name="status">Contents of the post.</param>
        /// <returns>Async task.</returns>
        public async Task UpdateStatus(string status)
        {
            var values = new Dictionary<string, string>()
            {
                { "status", status }
            };
            await this.PostParams("https://api.twitter.com/1.1/statuses/update.json", values)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Send a direct message to the given username.
        /// </summary>
        /// <param name="screenName">The user to send the DM to.</param>
        /// <param name="text">Text of the message.</param>
        /// <returns>Async task.</returns>
        public async Task NewDirectMessage(string screenName, string text)
        {
            var values = new Dictionary<string, string>()
            {
                { "screen_name", screenName },
                { "text", text }
            };

            await this.PostParams("https://api.twitter.com/1.1/direct_messages/new.json", values)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get statuses from the user's timeline.
        /// </summary>
        /// <returns>Tweets from the user's timeline.</returns>
        public async Task<IEnumerable<Tweet>> UserTimelineStatuses()
        {
            return await this.GetTweets("https://api.twitter.com/1.1/statuses/user_timeline.json")
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get statuses from the user's home screen timeline.
        /// </summary>
        /// <returns>Tweets from the user's home timeline.</returns>
        public async Task<IEnumerable<Tweet>> HomeTimelineStatuses()
        {
            return await this.GetTweets("https://api.twitter.com/1.1/statuses/home_timeline.json")
                .ConfigureAwait(false);
        }

        private async Task<IEnumerable<Tweet>> GetTweets(string url)
        {
            using (var response = await this.client.GetAsync(url).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new TwitterClientException(response.StatusCode, body);
                }

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    return this.serializer.ReadObject(stream) as IEnumerable<Tweet>;
                }
            }
        }

        private async Task PostParams(string url, Dictionary<string, string> values)
        {
            using (var response = await this.client.PostAsync(url, new FormUrlEncodedContent(values)).ConfigureAwait(false))
            {
                if (!response.IsSuccessStatusCode)
                {
                    var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    throw new TwitterClientException(response.StatusCode, body);
                }
            }
        }

        /// <summary>
        /// Class to represent a tweet.
        /// </summary>
        [DataContract]
        public class Tweet
        {
            /// <summary>
            /// Gets or sets the text of the tweet.
            /// </summary>
            [DataMember(Name = "text")]
            public string Text { get; set; }

            /// <summary>
            /// Gets or sets the tweet's id.
            /// </summary>
            [DataMember(Name = "id")]
            public long Id { get; set; }

            /// <summary>
            /// Gets or sets the date tweet was created, UTC.
            /// </summary>
            [DataMember(Name = "created_at")]
            public DateTime CreatedAt { get; set; }

            /// <summary>
            /// Gets or sets the Twitter user.
            /// </summary>
            [DataMember(Name = "user")]
            public TwitterUser User { get; set; }

            /// <summary>
            /// Class representing a Twitter user.
            /// </summary>
            [DataContract]
            public class TwitterUser
            {
                /// <summary>
                /// Gets or sets the name of the user.
                /// </summary>
                [DataMember(Name = "name")]
                public string Name { get; set; }

                /// <summary>
                /// Gets or sets the user's screen name.
                /// </summary>
                [DataMember(Name = "screen_name")]
                public string ScreenName { get; set; }
            }
        }

        /// <summary>
        /// Exception for API errors.
        /// </summary>
        public class TwitterClientException : Exception
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="TwitterClientException"/> class.
            /// </summary>
            /// <param name="statusCode">The HTTP status code of the response.</param>
            /// <param name="body">Body of the response.</param>
            public TwitterClientException(HttpStatusCode statusCode, string body)
                : base($"Request Failed with code: {statusCode} and body {body}")
            {
            }
        }
    }
}
