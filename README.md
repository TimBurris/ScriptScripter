[![Build Status](https://burristech.visualstudio.com/ScriptScripter/_apis/build/status/ScriptScripter.DesktopApp?branchName=master)](https://burristech.visualstudio.com/ScriptScripter/_build/latest?definitionId=1&branchName=master)
[[_TOC_]]

#What is ScriptScripter?
ScriptScripter (SS) is a utility built to assist developers with database versioning when using a database first approach.  The idea is simple, any change that you want to make to a database's structure/schema, you write T-Sql and commit it via SS. Using SS, scripts get applied to databases, in the order they were recorded.  The database keeps track of which scripts have been applied, allowing subsequent executions of SS to only apply new scripts.

#Simple Workflow Example
In its most simple form the workflow looks like this:
- Write the T-Sql to alter your database
- Record in Script File using SS
- Use SS to apply script(s) to dev DB
- Commit to Source Code Control 
- Pull/Merge scripts from other developers
- Use SS to apply scripts(s) to dev DB to stay up to date with the team
- Use SS to apply Scripts to Test/Production database during Deployment

#Who is it for?
Software Developers who want complete and total control over how their database changes throughout the development of a software product.  Developers must be interested and willing to write (or generate) the t-sql required to add or change the capabilities of the database.  

#What is it not?
It is not magic.  SS only changes the database in the ways you tell it to. SS doesn't _figure out_ how the database should be changed. 

#Screen Shots
##Home
Shows a list of databases/script files you use (can be as many as you want)
![image.png](/.attachments/image-19114b42-e491-4a5d-aa3d-480a85884557.png)

##Status (Up to date)
Shows whether or not a particular database is up to date
![image.png](/.attachments/image-d1b575d3-d059-4e2b-86d2-40cf8a9d68d6.png)

##New Script
Record a new script
![image.png](/.attachments/image-df52b8f0-d2c0-4402-943c-6c65d968c1b1.png)

##Status (Out of Date)
Shows that you have one or more scripts that need to be applied
![image.png](/.attachments/image-20f6f54e-bbdc-4fc3-9d43-c74022e3fe77.png)

##Prepare to apply scripts (aka 'Release the Ninja')
Shows the scripts that will be applied 
![image.png](/.attachments/image-2855cae0-ba32-4924-b67a-58fc929beb10.png)

##Progress
as scripts are applied, they show green
![image.png](/.attachments/image-1566e14b-ee0c-4e7b-93f2-6ca2a41ac8e6.png)

##All done
![image.png](/.attachments/image-f60c88b9-a4d2-4e00-b38b-7846d6fc9407.png)

#Sql Server 
SS uses Microsoft Sql Server SMO libraries to execute scripts, this means that you can use the exact same query syntax that you use in Sql Server Management Studio, including "GO" 

#Other Database Engines
Currently we've only developed support for Sql Server, however, adding support for other engines would be possible, since the concept is the same.  differences would only be in the Connection Info Area and in the engine used to actually apply the scripts.

#How does it work
SS will write the T-Sql script to an Xml file ([why not Json?](#why-xml-instead-of-json)).  
Along with the T-Sql, SS will log **_who_** created the script, record the Full Date and Time for **_when_**, and assign it a Guid.

When you (or a deployment pipeline) applies scripts, SS will query the target database to find all the ScriptIds that have already been applied.  It will compare that list to the list of Scripts in the script Xml file to determine which scripts have not yet been applied.  SS will then execute those outstanding scripts, in order of their Script Date, and write to a table in the database everything about the script, include when it was run, the user than ran it, etc.

#SCC Friendly
Since the Xml files have a Guid Script Id and the order is based on full date time (meaning the order they appear in the file in unimportant), the file is friendly for multi-developer branching and merging.  If developer A adds 3 scripts and developer B adds a script, any modern TFVC or Git merge engine will merge them nicely, keeping the revisions from both developers.

#Refrain from changing the Sql after the fact
Once a script has been applied a database, it will never be run again against that database.  As a result, developers should refrain from changing any T-Sql in a script file that has already been committed to SCC or possibly applied to any database.  If you script T-Sql locally, run it and it doesn't work, you can change/fix it.  Just know that if you change a script that has already been successfully applied to a database, those changes will not be reapplied to the same database.

#ScriptScripter versus Visual Studio Database Projects (DP)
SS is not meant to mirror the functionality of a Database Project in Visual Studio.  The good and bad of SS is that it's not a magic black box.  

DP will look at your database and do whatever it takes to make sure the resulting database matches the schema, but _how_ DP makes those changes is sometimes magical, so then you have try to reign it in with pre and post processing scripts.  

SS's approach is that it will only change what you tell it change, and change it exactly as you tell it.

DP does many things very well, but one aspect of DP that can be troubling is performing schema modifications along with the associated data migration.  For example, adding a non-null column to a table with existing data.  In DP, you solve this one of 2 ways, either add a default value to the column or using a pre/post processing script.  for the former, adding a default value _just_ so that DP can perform automatic migrations doesn't sit well.  If the column doesn't conceptually have a default, then it shouldn't have one in Sql Server. 

With SS, adding a non null column is pretty simple:
Write a single script:
```sql
-- add the column
alter table Test add MyCol bit;
GO
-- assign values to existing rows
update Test set MyCol=case when XYZ then 1 else 0;
GO
--make the column not null
alter table Test alter column MyCol bit NOT NULL
```

#Multiple Databases
The SS desktop app lets you manage multiple database script files, but the script files and databases are managed completely independent of one another.  You apply scripts to one database at a time, the fact that SS lets you manage multiple databases is just mean to be a convenience, so that you can run SS on your PC and see all the databases you work with.

#Why XML instead of Json
The issue with Json is that it makes a much less friendly file for Text Editors because Json doesn't allow line breaks in property values.  This means that a multi-line sql statement has to be written on a single line with \r\n literals, this makes it almost impossible to actually read the sql statements in a text editor.  As a result, we have continued to use Xml.
 
#Command Line Tool
A command line tool is available for automating the act of applying the scripts to a database; useful in an Azure Devops Deployment Pipeline or any other automated deployment script
[Using scriptscripter.command.exe](/CommandLine)

#Our Mascot
If you've used SS to apply scripts, you have probably run across our mascot, "The Dumpster Ninja".
This Ninja was discovered on a website in the early 2000's (I think around 2004) and immediately added to ScriptScripter.  Without him, we are pretty sure a black hole will swallow this planet, or worse your database scripts will absolutely fail to run.
The original web source of The Dumpster Ninja is not being revealed for fear he may unleash his real, ultimate power.
![image.png](/.attachments/image-f83df442-65c3-44f9-bca7-0d3e6da072c7.png)
#Contributers
Shout out to two other developers who played a huge role in ScriptScripter over the past 2 decades: 
Jeff Ciucci and Jay Collins!
