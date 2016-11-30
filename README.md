# Welcome to ![WorxBox](https://dl.dropboxusercontent.com/u/67850614/worxbox.png)

This is a workbox replacement for Sitecore 8.x based on Sitecore's Simple Workflow. The goal of Worxbox is to provide content authors and approvers with the ability to submit compositions of work based on Page templates and their references (through links from Page template fields and through Presentation references).

In addition, Worxbox provides an extensible filtering mechanism that provides the ability to limit the number of items displayed in the view.  Out of the box, Worxbox provides filtering based on Author.

## Limitations with this version  

Worxbox composes pages based on references from specific page templates.  As an example, when a new page is created renderings are added and those renderings point to specific data sources.  This creates a reference in the links database from the Page template's Renderings field (Shared or Final) to the data source.  I bring this to your attention because Worxbox does not handle other practices such as child items or dynamically selected content (e.g., a rendering performs a search and displays items based on the search results). That said, you can rest assured that when an approver submits content through Workflow, the "Page" and its references will all be submitted as a single unit of work.  