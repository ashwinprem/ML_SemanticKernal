using Microsoft.SemanticKernel;
using SimpleFeedReader;
using System.ComponentModel;

namespace SemanticKernel.Plugins;
public class NewsPlugin
{
    [KernelFunction("get_news")]
    [Description("Gets new items for today's date.")]
    [return: Description("A list of current news stories.")]
    public async Task<List<FeedItem>> GetNews(Kernel kernel, string category)
    {
        var reader = new FeedReader();
        var feed = await reader.RetrieveFeedsAsync(
            [$"https://rss.nytimes.com/services/xml/rss/nyt/{category}.xml"]);

        return feed
            .Take(5)  // Show only 5 items
            .ToList(); // Convert to a list
    }
}
