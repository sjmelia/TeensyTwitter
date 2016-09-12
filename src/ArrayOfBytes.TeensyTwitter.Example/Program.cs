namespace ArrayOfBytes.TeensyTwitter.Example
{
    using ArrayOfBytes.OAuth.Client;
    using System;

    public class Program
    {
        public static void Main(string[] args)
        {
            // Replace these with your own values from your Twitter app config.
            OAuthConfig config = new OAuthConfig(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty);
            TwitterClient client = new TwitterClient(config);
            
            // Update status
            //client.UpdateStatus("Hello, World!").Wait();

            // Retrieve all tweets.
            var tweets = client.UserTimelineStatuses().Result;
            foreach (var tweet in tweets)
            {
                Console.WriteLine(tweet.Text);
            }
        }
    }
}
