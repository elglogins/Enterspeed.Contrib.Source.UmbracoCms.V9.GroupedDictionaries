using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Data.Repositories;
using Enterspeed.Source.UmbracoCms.V9.NotificationHandlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Scoping;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Web;
using UmbracoCms.V9.GroupedDictionaries;

namespace UmbracoCms.V9.GroupedDictionaries.NotificationHandlers
{
    public class GroupedDictionaryItemDeletedNotificationHandler : BaseEnterspeedNotificationHandler, INotificationHandler<DictionaryItemDeletedNotification>
    {
        private readonly ILocalizationService _localizationService;

        public GroupedDictionaryItemDeletedNotificationHandler(
          IEnterspeedConfigurationService configurationService,
          IEnterspeedJobRepository enterspeedJobRepository,
          IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
          IUmbracoContextFactory umbracoContextFactory,
          IScopeProvider scopeProvider,
          ILocalizationService localizationService) : base(
                configurationService,
                enterspeedJobRepository,
                enterspeedJobsHandlingService,
                umbracoContextFactory,
                scopeProvider)
        {
            _localizationService = localizationService;
        }

        public void Handle(DictionaryItemDeletedNotification notification)
        {
            if (!IsConfigured())
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            foreach (var language in _localizationService.GetAllLanguages())
            {
                var now = DateTime.UtcNow;
                jobs.Add(new EnterspeedJob
                {
                    EntityId = GroupedDictionaryItemConstants.EntityId,
                    EntityType = EnterspeedJobEntityType.Dictionary,
                    Culture = language.IsoCode,
                    JobType = EnterspeedJobType.Publish,
                    State = EnterspeedJobState.Pending,
                    CreatedAt = now,
                    UpdatedAt = now,
                });
            }

            EnqueueJobs(jobs);
        }
    }
}
