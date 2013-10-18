﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;
using Windows.Web.Syndication;

namespace Vidyano.WindowsStore.Sample.Background
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            var deferral = taskInstance.GetDeferral();

            // Download the feed.
            var feed = await GetMSDNBlogFeed();

            // Update the live tile with the feed items.
            UpdateTile(feed);

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private static async Task<SyndicationFeed> GetMSDNBlogFeed()
        {
            SyndicationFeed feed = null;

            try
            {
                // Create a syndication client that downloads the feed.  
                var client = new SyndicationClient();
                client.BypassCacheOnRetrieve = true;
                client.SetRequestHeader(customHeaderName, customHeaderValue);

                // Download the feed. 
                feed = await client.RetrieveFeedAsync(new Uri(feedUrl));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }

            return feed;
        }

        private static void UpdateTile(SyndicationFeed feed)
        {
            // Create a tile update manager for the specified syndication feed.
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            // Keep track of the number feed items that get tile notifications. 
            int itemCount = 0;

            // Create a tile notification for each feed item.
            foreach (var item in feed.Items)
            {
                var tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);

                tileXml.GetElementsByTagName(textElementName)[0].InnerText = item.Title.Text;

                // Create a new tile notification. 
                updater.Update(new TileNotification(tileXml));

                // Don't create more than 5 notifications.
                if (itemCount++ > 5) break;
            }
        }

        // Although most HTTP servers do not require User-Agent header, others will reject the request or return 
        // a different response if this header is missing. Use SetRequestHeader() to add custom headers. 
        const string customHeaderName = "User-Agent";
        const string customHeaderValue = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        const string textElementName = "text";
        const string feedUrl = "http://www.vidyano.com/Feed/Rss";
    }
}