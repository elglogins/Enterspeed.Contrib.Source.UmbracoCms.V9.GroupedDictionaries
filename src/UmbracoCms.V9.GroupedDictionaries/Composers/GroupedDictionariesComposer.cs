using Enterspeed.Source.UmbracoCms.V9.Composers;
using Enterspeed.Source.UmbracoCms.V9.DataPropertyValueConverters;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.NotificationHandlers;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Handlers.Dictionaries;
using Enterspeed.Source.UmbracoCms.V9.Handlers.PreviewDictionaries;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using UmbracoCms.V9.GroupedDictionaries.JobHandlers;
using UmbracoCms.V9.GroupedDictionaries.NotificationHandlers;

namespace UmbracoCms.V9.GroupedDictionaries.Composers
{
    [ComposeAfter(typeof(EnterspeedComposer))]
    public class GroupedDictionariesComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // Remove default dictionary item notification handlers
            builder.Services.Remove(builder.Services.FirstOrDefault(f => f.ImplementationType == typeof(EnterspeedDictionaryItemDeletingNotificationHandler)));
            builder.Services.Remove(builder.Services.FirstOrDefault(f => f.ImplementationType == typeof(EnterspeedDictionaryItemSavedNotificationHandler)));

            // Register own dictionary item notification handlers
            builder
            .AddNotificationHandler<DictionaryItemSavedNotification,
                GroupedDictionaryItemSavedNotificationHandler>();

            builder
                .AddNotificationHandler<DictionaryItemDeletedNotification,
                    GroupedDictionaryItemDeletedNotificationHandler>();

            // Remove default dictionary item job handlers
            builder.EnterspeedJobHandlers()
                // Publish
                .Remove<EnterspeedDictionaryItemPublishJobHandler>()
                .Remove<EnterspeedDictionaryItemDeleteJobHandler>()
                // Preview
                .Remove<EnterspeedPreviewDictionaryItemPublishJobHandler>()
                .Remove<EnterspeedPreviewDictionaryItemDeleteJobHandler>();

            // Register own dictionary item job handlers
            builder.EnterspeedJobHandlers()
                .Append<JobHandlers.Preview.GroupedDictionaryItemJobHandler>()
                .Append<JobHandlers.Publish.GroupedDictionaryItemJobHandler>();
        }
    }
}
