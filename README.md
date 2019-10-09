# Welcome to ![WorxBox](https://www.dropbox.com/s/p28vh2tdc6t7m37/worxbox.png?dl=0)

This is a workbox plug-in for Sitecore 7.5, Sitecore 8.0, 8.1 and 8.2.

The goal of Worxbox is to provide content authors and approvers with the ability to submit compositions of work based on Page templates and their references (through links from Page template fields and through Presentation references).

The updated release now provides users with the ability to create a custom rule set for filtering Workbox.  This rule set is based on the Sitecore Rules Engine and ships with 2 rules that allow users to show just their creations and/or
just their changes.  Using the rules engine for filtering provides a very high degree of extensibility through the addition of new rules.  Changing the filtering is as easy as using the default Sitecore Rules Editor.  

## Versions  

Sitecore 7.5 - http://bit.ly/2ucJoK5

Sitecore 8.0 - http://bit.ly/2jk3Eam

Sitecore 8.1 - http://bit.ly/2hrSyLN   

Sitecore 8.2 - http://bit.ly/2hrSyLN

## How It Worx

WorxBox composes pages based on data source references from renderings, their child items and references, and parents.  As long as the Links database is healthy WorxBox should capture references.  
Imagine a new page with a call to action button that references a call to action content item living in a new call to action content item folder.  In addition the call to action content item references an image in the media library also in a new media library folder.  WorxBox will return the page plus the new items under workflow.

Based on testing, WorxBox will omit circular references (Stack Overflows are not fun in recursive operations!) and references made to content deriving from templates specified as WorxBox templates.  

As a result of a recent request, we have altered the way Content Editor gutter, ribbon and Experience Editor workflow panel operate with WorxBox.  Now, content authors will get the original commands as well as WorxBox commands.  As an example, if a content item is compositable (its template is defined as composite), WorxBox will show 2 commands; one representing Sitecore's default command and one for composite submission.

### WorxBox Filtering  

WorxBox filtering in the Workbox is based on Sitecore's Rules Engine.  Based on tagging, a minimal subset of existing rules are included.  If you're new to the Rules Engine, the Rules Engine Cookbook found on sdn.sitecore.net is a good starter read.  

## Known Issues

Sitecore 8.0 and 8.1 have a known issue in which items referenced as Data Sources for renderings selected under personalization conditions do not get added as references.  This issue does not exist in 8.2 Update 1.  
To resolve this issue in your 8.0 or 8.1 version of Sitecore, please visit the Github project http://bit.ly/2hTTklJ
