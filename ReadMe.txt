### Sitemap Updater
### Developed by: Trevor Thompson
### 5/4/2020

#Description
My first attempt at a Windows Service
Creates and updates a sitemap.xml file for your website. Keep the search engines up to date on changes to your site.
This service calls a stored procedure on your database and builds a sitemap file using the resultset. 
The service polls every 30 minutes by default and checks for changes to the file by comparing a temp version of the file
to the sitemap generated.

Note: The installer file did not work for me! use the method in the install instructions below.

# Testing Notes
   > Tested on Windows Server 2012 with MSSQL Server 2012
   > Tested on Windows 10

#Requirments
   > Website with a A Sql Server backend
   > A properly configured appsettings.json file
   > A Stored Procedure that returns a row for each endpoint on the website (sample provided).

#Install Instructions:
1. Build the application in Release on your dev machine.
2. Copy the Release folder to a .zip and copy the .zip to your hosting machine
>>>>IF YOU COPIED THE FOLDER TO YOUR SERVER FROM ANOTHER MACHINE (LIKE YOUR DEV MACHINE) THIS IS AN IMPORTANT STEP<<<<
>>>>3. Right-click the .zip file and select properties. Then click Unblock. Apply.<<<<
4. Extract the entire Release folder into a sub-folder on the hosting Server c:\<sub_folder_name>
5. Launch an elevated command prompt
   > cd Windows\Microsoft.NET\Framework\v4.0.30319 (FRAMEWORK Version may vary. cd to the Framework directory and use TAB to see all versions)
   > InstallUtil c:\<sub_folder_name>\Release\sitemap_updater.exe
6. Configure your appsettings.json and start the service. 

Note: Check Event Viewer to ensure polling time and paths are correct
   > Event Viewer Applications and Services Log > SiteMapUpdLog

Uninstall Instructions:
If you make changes to the source you'll want to uninstall the service before reinstalling it.
> cd Windows\Microsoft.NET\Framework\v4.0.30319 (FRAMEWORK Version may vary. cd to framework and use TAB to see all versions)
> InstallUtil /u c:\<sub_folder_name>\Release\sitemap_updater.exe


The MIT License (MIT)

Copyright (c) 2020 Trevor Thompson

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
(the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
to do so, subject to the following conditions:
The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.