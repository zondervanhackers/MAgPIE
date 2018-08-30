# Development Configuration

## Visual Studio 2017 Professional Edition (15.8)

Install "Visual Studio Professional 2017 (15.8) (x86) - Web Installer (English)" from the MSDN subscription page (<https://msdn.microsoft.com/subscriptions/securedownloads/).>

The latest version of Sandcastle (<https://github.com/EWSoftware/SHFB/releases)> should be installed after Visual Studio 2017 to allow for documentation generation.

The T4 template generator extension ("T4 Toolbox for Visual Studio 2013") should also be added to visual studio via the extension manager.  This is used for several classes in the project and will be necessary if they are to be changed.

The latest edition of SQL Server Database tools should be installed from this link (<http://go.microsoft.com/fwlink/?LinkID=393521&clcid=0x409).>

> Warning! In order for Visual Studio to build the project correctly, it should be run in administrator mode (right click on the application and select "Run as Administrator").  This is so that it will have permission to create the Harvester Service on the local machine.

## SQL Server 2012

Download the SQL Server 2012 installer at <https://www.microsoft.com/en-us/download/confirmation.aspx?id=29062.>  Choose the "ENU\x64\SQLEXPRADV_x64_ENU.exe" option.  The default settings in the installer should work fine.