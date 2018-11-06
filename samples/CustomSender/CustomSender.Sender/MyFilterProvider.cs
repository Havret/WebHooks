using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebHooks;

namespace CustomSender.Sender
{
    public class MyFilterProvider : IWebHookFilterProvider
    {
        private readonly Collection<WebHookFilter> filters = new Collection<WebHookFilter>
        {
            new WebHookFilter { Name = "event1", Description = "This event happened."},
            new WebHookFilter { Name = "event2", Description = "This event happened."},
        };

        public Task<Collection<WebHookFilter>> GetFiltersAsync()
        {
            return Task.FromResult(this.filters);
        }
    }
}
