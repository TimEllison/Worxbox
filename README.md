# Welcome to ![WorxBox](https://dl.dropboxusercontent.com/u/67850614/worxbox.png)

This is a workbox plug-in for Sitecore 8.1 and 8.2. Future releases will include support for 8.0 although I highly recommend that you upgrade to 8.2 as soon as possible.

The goal of Worxbox is to provide content authors and approvers with the ability to submit compositions of work based on Page templates and their references (through links from Page template fields and through Presentation references).

The updated release now provides users with the ability to create a custom rule set for filtering Workbox.  This rule set is based on the Sitecore Rules Engine and ships with 2 rules that allow users to show just their creations and/or
just their changes.  Using the rules engine for filtering provides a very high degree of extensibility through the addition of new rules.  Changing the filtering is as easy as using the default Sitecore Rules Editor.  

## Versions  

Sitecore 8.1 - http://bit.ly/2hrSyLN

## How It Worx

WorxBox composes pages based on data source references from renderings, their child items and references, and parents.  As long as the Links database is healthy WorxBox should capture references.  
Imagine a new page with a call to action button that references a call to action content item living in a new call to action content item folder.  In addition the call to action content item references an image in the media library also in a new media library folder.  WorxBox will return the page plus the new items under workflow.

Based on testing, WorxBox will omit circular references (Stack Overflows are not fun in recursive operations!) and references made to content deriving from templates specified as WorxBox templates.  

Currently, the commands accessed via the gutter in content editor tie into WorxBox as do the commands in the Review ribbon and the Experience Editor commands.  

### WorxBox Filtering  

WorxBox filtering in the Workbox is based on Sitecore's Rules Engine.  Based on tagging, a minimal subset of existing rules are included.  If you're new to the Rules Engine, the Rules Engine Cookbook found on sdn.sitecore.net is a good starter read.  

## Known Issues

Sitecore 8.1 has a known issue in which items referenced as Data Sources for renderings selected under personalization conditions do not get added as references.  This issue does not exist in 8.2 Update 1.  
To resolve this issue in your 8.1 version of Sitecore, please visit the Github project http://bit.ly/2hTTklJ