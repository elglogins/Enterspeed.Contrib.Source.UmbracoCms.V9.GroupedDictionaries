using System.Collections.Generic;
using System.Linq;
using Enterspeed.Source.Sdk.Api.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;
using Enterspeed.Source.UmbracoCms.V9.Services;
using Umbraco.Cms.Core.Models;
using UmbracoCms.V9.GroupedDictionaries;

namespace UmbracoCms.V9.GroupedDictionaries.Models
{
    public class UmbracoGroupedDictionaryEntity : IEnterspeedEntity
    {
        private readonly IEntityIdentityService _entityIdentityService;
        private readonly string _culture;

        public UmbracoGroupedDictionaryEntity(
            IEntityIdentityService entityIdentityService,
            IEnterspeedPropertyService propertyService,
            IList<IDictionaryItem> items,
            string culture)
        {
            _entityIdentityService = entityIdentityService;
            _culture = culture;
            Properties = new Dictionary<string, IEnterspeedProperty>();
            Properties.Add("culture", new StringEnterspeedProperty(culture));

            var dictionaryItemObjects = (items ?? new List<IDictionaryItem>())
                .Select(item =>
                {
                    var objectProperty = new ObjectEnterspeedProperty(propertyService.GetProperties(item, culture));
                    objectProperty.Properties.Remove("culture");
                    return objectProperty;
                })
                .ToArray();
            Properties.Add("items", new ArrayEnterspeedProperty("items", dictionaryItemObjects));
        }

        public string Id => _entityIdentityService.GetId(GroupedDictionaryItemConstants.EntityId, _culture);
        public string Type => "umbDictionaryGrouped";
        public string Url => null;
        public string[] Redirects => null;
        public string ParentId => null;
        public IDictionary<string, IEnterspeedProperty> Properties { get; }
    }
}
