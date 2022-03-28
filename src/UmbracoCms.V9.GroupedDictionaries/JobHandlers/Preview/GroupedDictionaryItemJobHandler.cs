using System;
using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Services;
using Enterspeed.Source.UmbracoCms.V9.Data.Models;
using Enterspeed.Source.UmbracoCms.V9.Exceptions;
using Enterspeed.Source.UmbracoCms.V9.Handlers;
using Enterspeed.Source.UmbracoCms.V9.Models;
using Enterspeed.Source.UmbracoCms.V9.Providers;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UmbracoCms.V9.GroupedDictionaries.Models;

namespace UmbracoCms.V9.GroupedDictionaries.JobHandlers.Preview
{
    public class GroupedDictionaryItemJobHandler : IEnterspeedJobHandler
    {
        private readonly IEnterspeedIngestService _enterspeedIngestService;
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly ILocalizationService _localizationService;
        private readonly IEnterspeedPropertyService _enterspeedPropertyService;
        private readonly IEnterspeedConnectionProvider _enterspeedConnectionProvider;

        public GroupedDictionaryItemJobHandler(
            IEnterspeedIngestService enterspeedIngestService,
            IEntityIdentityService entityIdentityService,
            ILocalizationService localizationService,
            IEnterspeedPropertyService enterspeedPropertyService,
            IEnterspeedConnectionProvider enterspeedConnectionProvider)
        {
            _enterspeedIngestService = enterspeedIngestService;
            _entityIdentityService = entityIdentityService;
            _localizationService = localizationService;
            _enterspeedPropertyService = enterspeedPropertyService;
            _enterspeedConnectionProvider = enterspeedConnectionProvider;
        }

        public bool CanHandle(EnterspeedJob job)
        {
            return GroupedDictionaryItemConstants.EntityId.Equals(job.EntityId, StringComparison.InvariantCultureIgnoreCase)
               &&  _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview) != null
               && job.ContentState == EnterspeedContentState.Preview
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
            var ingestResponse = _enterspeedIngestService.Save(umbracoData, _enterspeedConnectionProvider.GetConnection(ConnectionType.Preview));
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
