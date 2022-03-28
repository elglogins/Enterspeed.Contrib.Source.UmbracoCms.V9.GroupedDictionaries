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
    public class GroupedDictionaryItemSavedNotificationHandler : BaseEnterspeedNotificationHandler,
        INotificationHandler<DictionaryItemSavedNotification>
    {
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedConfigurationService _enterspeedConfigurationService;

        public GroupedDictionaryItemSavedNotificationHandler(
            IEnterspeedConfigurationService configurationService,
            ILocalizationService localizationService,
            IEnterspeedJobRepository enterspeedJobRepository,
            IEnterspeedJobsHandlingService enterspeedJobsHandlingService,
            IUmbracoContextFactory umbracoContextFactory,
            IScopeProvider scopeProvider,
            IAuditService auditService) : base(
            configurationService,
            enterspeedJobRepository,
            enterspeedJobsHandlingService,
            umbracoContextFactory,
            scopeProvider,
            auditService)
        {
            _localizationService = localizationService;
            _enterspeedConfigurationService = configurationService;
        }

        public void Handle(DictionaryItemSavedNotification notification)
        {
            var stateConfigurations = new Dictionary<EnterspeedContentState, bool>()
            {
                { EnterspeedContentState.Preview, _enterspeedConfigurationService.IsPreviewConfigured() },
                { EnterspeedContentState.Publish, _enterspeedConfigurationService.IsPublishConfigured() },
            };
            if (stateConfigurations.All(a => !a.Value))
            {
                return;
            }

            var jobs = new List<EnterspeedJob>();
            var now = DateTime.UtcNow;

            foreach (var destination in stateConfigurations.Where(w => w.Value))
            {
                foreach (var language in _localizationService.GetAllLanguages())
                {
                    jobs.Add(
                        new EnterspeedJob
                        {
                            EntityId = GroupedDictionaryItemConstants.EntityId,
                            EntityType = EnterspeedJobEntityType.Dictionary,
                            Culture = language.IsoCode,
                            JobType = EnterspeedJobType.Publish,
                            State = EnterspeedJobState.Pending,
                            CreatedAt = now,
                            UpdatedAt = now,
                            ContentState = destination.Key
                        });
                }
            }

            EnqueueJobs(jobs);
        }
    }
}