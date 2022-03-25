# Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries &middot; [![GitHub license](https://img.shields.io/badge/license-MIT-blue.svg)](./LICENSE) [![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/elglogins/Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries/pulls) [![NuGet version](https://img.shields.io/nuget/v/Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries)](https://www.nuget.org/packages/Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries//)

## What is this package about?

It is an extension on top of Enterspeeds source connector for Umbraco v9, instead of ingesting each dictionary item as a separate source entity per culture, it will ingest single grouped source entity per culture.

Ideas for usage:
- Can use `$expression` mapping instead of `$lookup`/`actions`

### Example of grouped dictionary item
```json
{
	"sourceId": "gid://Source/31a29cc5-5701-48aa-a495-af83ebb95dcf",
	"id": "gid://Source/31a29cc5-5701-48aa-a495-af83ebb95dcf/Entity/grouped-dictionaries-en-US",
	"type": "umbDictionaryGrouped", // always umbDictionaryGrouped
	"originId": "grouped-dictionaries-en-US", // grouped-dictionaries-{p.culture}
	"originParentId": null,
	"url": null,
	"properties": {
		"culture": "en-US",
		"items": [
			{
				"key": "Buttons",
				"translation": ""
			},
			{
				"key": "Buttons.Refresh",
				"translation": "Refresh!"
			},
			{
				"key": "Buttons.Cancel",
				"translation": "Cancel!"
			}
		]
	}
}
```

## Installation
```
dotnet add package Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries
```

Check other [installation options](https://www.nuget.org/packages/Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries/).

## Contributing

Pull requests are very welcome.  
Please fork this repository and make a PR when you are ready.

Otherwise you are welcome to open an Issue in our [issue tracker](https://github.com/elglogins/Enterspeed.Contrib.Source.UmbracoCms.V9.GroupedDictionaries/issues).

## License

This project is [MIT licensed](./LICENSE)
