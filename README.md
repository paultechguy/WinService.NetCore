# WinService.NetCore

This project was created to make spinning up new Windows Services fairly easy. The intention
was to keep things simple, but still provide some of the basics that a servicie would require.

The basic needs included in this Windows Service project are:

1. Normal Windows Service (long-running and support for the Windows *sc* commands)
2. Console support exists so things also run in a console window
3. Logging support exists using Serilog (default is console and file)
4. Email support exists, assuming you have access to an SMTP server (e.g. gmail)

Yea, I know...you are wondering why there is no database support. It was very intentional to leave this
out since there are several databases and database frameworks that exist.  I didn't want to complicate
this starter project and get into a *religious* discussion.

Things should work right out-of-the-box.  You can build the solution and run the app in a console window,
or install and run it as a service. See **Customize the Service** for making the service do your
own magic tricks.

## Requirements
- Windows 10 or higher
- Visual Studio 2022 with .NET Core 6

## Customize the Service
I suggest that before you begin customizing the code, you give things a test drive using the out-of-the-box
functionality.  Then when you are ready to customize it, modify the `ExecuteAsync` method in
`WinService.NetCore.Application:ApplicationService.cs`. Remember this is a *service* so if this method
exits, the Windows Service will stop.

**Note:** Be sure your code is sensitive to checking, somewhat frequently, the cancellation token to determine
if there is an incoming request for the service to stop...then you can safely exit the `ExecuteAsync` method and
allow the service to stop.

## Build Solution
Using Visual Studio 2022, open up the `src\WinService.NetCore.sln` and build the solution as you
normally would.

## Run as a Console App
After building, open up a console window and execute the following from your build directory:

        .\WinService.NetCore.exe

## Run as Windows Service
The following commands can be executed in an elevated (administor mode) console window.  For some of the
steps, you can also use the standard Windows Services user-interface (e.g. start, stop).

1) Create the service

        sc.exe create "My Service Name" binpath="C:\...\WinService.NetCore.exe"

2) Start the service

        sc.exe start "My Service Name"

3) Stop the service

        sc.exe stop "My Service Name"

4) Delete the service

        sc.exe delete "My Service Name"

**Note**: If you want to verify your service is running, you can check the log file for messages (see Logging).

## Logging

By default, log files will be created in the subdirectory where the `WinService.NetCore.exe` file is located.
The subdirectory for log files is called *logs*.  The most recent 31 days of log files are saved.  For more information
on the
[Serilog](https://serilog.net/)
configuration, see the `WinService.NetCore:appsettings.json` file.

Tim Corey's
[Serilog video](https://www.youtube.com/watch?v=_iryZxv8Rxw&t=2541s) video takes an in-depth
look at logging.

## Email

By default the Windows Service will not attempt to send email, but you can update the
`WinService.NetCore:appsettings.json` file to enable it.  If enabled, then a single email
will be sent when the starter project service starts (for demonstration purposes).

To enable email, you will need to do two things:

1) Update the to/from email addreses in `WinService.NetCore:appsettings.json`:

        "ApplicationSettings": {
          "MessageToEmailAddress": "email name <email address>",
          "MessageFromEmailAddress": "email name <email address>",
          "MessageReplyToEmailAddress": ""
        },

2) Configure an SMTP email server

    For local (i.e. localhost) testing you can use [Papercut-SMTP](https://www.papercut-smtp.com/);
this is an excellent tool for verifying email is working before your deploy to production environments.
You can also use Gmail by creating an *App Password* in your Gmail account.

    The following two sections show the changes in `WinService.NetCore:appsettings.json` for
configurating an SMTP server.

    ### Papercut-SMTP Configuration

        "EmailSettings": {
          "ServerHost": "localhost",
          "ServerPort": "25",
          "ServerEnableSsl": "false",
          "ServerUsername": "",
          "ServerPassword": ""
        }

    ### Gmail SMTP Configuration

        "EmailSettings": {
          "ServerHost": "smtp.gmail.com",
          "ServerPort": "587",
          "ServerEnableSsl": "true",
          "ServerUsername": "{your Gmail app username (i.e. your Gmail email address)}",
          "ServerPassword": "{your Gmail app password}"
        }

## .editorConfig
The code has its own set of formatting and language rules.  If you don't like these, feel free
to modify the .editorConfig file, or remove it entirely from the project. If you remove the
.editorConfig, then you can also remove the StyleCop.Anayzers nuget from all projects in the
solution.

## License
[MIT](https://github.com/paultechguy/WinService.NetCore/blob/develop/LICENSE.txt)

## Credits
This project as inspired by the Microsoft documentation
on [Windows Services](https://docs.microsoft.com/en-us/dotnet/core/extensions/windows-service).
