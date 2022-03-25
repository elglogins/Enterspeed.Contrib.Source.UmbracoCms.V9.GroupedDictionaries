using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Exceptions;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UmbracoCms.V9.GroupedDictionaries;
using UmbracoCms.V9.GroupedDictionaries.Models;

namespace UmbracoCms.V9.GroupedDictionaries.JobHandlers
{
    public class GroupedDictionaryItemJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;

        public GroupedDictionaryItemJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            ILocalizationService localizationService,
            IEnterspeedPropertyService enterspeedPropertyService)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _localizationService = localizationService;
            _enterspeedPropertyService = enterspeedPropertyService;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return GroupedDictionaryItemConstants.EntityId.Equals(job.EntityId, StringComparison.InvariantCultureIgnoreCase)
                && job.EntityType == EnterspeedJobEntityType.Dictionary;
        }

        public void Handle(EnterspeedJob job)
        {
            var allDictionaryItems = _localizationService.GetDictionaryItemDescendants(null).ToList();
            var umbracoData = CreateUmbracoDictionaryEntity(allDictionaryItems, job);
            Ingest(umbracoData, job);
        }

        protected virtual UmbracoGroupedDictionaryEntity CreateUmbracoDictionaryEntity(IList<IDictionaryItem> allDictionaryItems, EnterspeedJob job)
        {
            try
            {
                return new UmbracoGroupedDictionaryEntity(
                        _entityIdentityService,
                        _enterspeedPropertyService,
                        allDictionaryItems,
                        job.Culture);
            }
            catch (Exception e)
            {
                throw new JobHandlingException(
                    $"Failed creating entity ({job.EntityId}/{job.Culture}). Message: {e.Message}. StackTrace: {e.StackTrace}");
            }
        }

        protected virtual void Ingest(IEnterspeedEntity umbracoData, EnterspeedJob job)
        {
            var ingestResponse = _enterspeedIngestService.Save(umbracoData);
            if (!ingestResponse.Success)
            {
                var message = ingestResponse.Exception != null
                    ? ingestResponse.Exception.Message
                    : ingestResponse.Message;
                throw new JobHandlingException(
                    $"Failed ingesting entity ({job.EntityId}/{job.Culture}). Message: {message}");
            }
        }
    }
}
