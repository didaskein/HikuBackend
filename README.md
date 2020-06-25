# HikuBackend
Backend to manage Hiku device (Hiku device->Electricimp->HikuBackend)

- You need first to follow Rob Getting started : 
[Source of the Firmware of the Hiku Device](https://github.com/hikuinc/hiku_stm32_fw)

- Then you can update your Hiku firmware code ( agent & device .nut file)
  You will need to create a Electric Imp account (it's free)

- Then you can run the C# code below & run it on Azure.


# What it doing the Backend
The azure function receive all events coming from the Hiku (Izy) Device : 
- Battery
- RSSI
- Barcode request
- Audio request (transform with speech to text)

All data are stored on a SQL Database (using EF Core).


# Component on Azure to Deploy
- Azure Function
- Application Insight for Monitoring
- SQL Server and SQL Database
- Service Bus Queue
- Storage account
- SpeechToText component

