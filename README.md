# Welcome to ![WorxBox](https://dl.dropboxusercontent.com/u/67850614/worxbox.png)

This is a workbox plug-in for Sitecore 8.1 and 8.2. Future releases will include support for 8.0 although I highly recommend that you upgrade to 8.2 as soon as possible.

The goal of Worxbox is to provide content authors and approvers with the ability to submit compositions of work based on Page templates and their references (through links from Page template fields and through Presentation references).

The updated release now provides users with the ability to create a custom rule set for filtering Workbox.  This rule set is based on the Sitecore Rules Engine and ships with 2 rules that allow users to show just their creations and/or
just their changes.  Using the rules engine for filtering provides a very high degree of extensibility through the addition of new rules.  Changing the filtering is as easy as using the default Sitecore Rules Editor.  

## Versions  

Sitecore 8.1 - http://bit.ly/2hrSyLN
Sitecore 8.2 - http://bit.ly/2hrSyLN

## Limitations

Worxbox composes pages based on references from specific page templates.  As an example, when a new page is created renderings are added and those renderings point to specific data sources.  This creates a reference in the links database from the Page template's Renderings field (Shared or Final) to the data source.  I bring this to your attention because Worxbox does not handle other practices such as child items or dynamically selected content (e.g., a rendering performs a search and displays items based on the search results). That said, you can rest assured that when an approver submits content through Workflow, the "Page" and its references will all be submitted as a single unit of work.  

## Known Issues

Sitecore 8.1 has a known issue in which items referenced as Data Sources for renderings selected under personalization conditions do not get added as references.  This issue does not exist in 8.2 Update 1.  
To resolve this issue in your 8.1 version of Sitecore, please visit the Github project http://bit.ly/2hTTklJ
