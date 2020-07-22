
## Overview

Azure Function to take an HTTP request and save to a file in Azure Data Lake Store Gen1
General purpose is to accept REST POST requests from a Solace broker and copy the data to Azure Data Lake Store Gen1

Fill out the Azure details

```
private static string adlsAccountName = "<datalakestore-domain-name>";
private static string applicationId = "<application-id>";     // Also called client id
private static string clientSecret = "<client-secret>";
private static string tenantId = "<tenant-id>";
```

Publish your Azure Function
https://tutorials.visualstudio.com/first-azure-function/publish



## Resources

These were both very useful:
https://docs.microsoft.com/en-us/azure/data-lake-store/data-lake-store-data-operations-net-sdk#prerequisites
https://github.com/Azure-Samples/data-lake-store-adls-dot-net-get-started/blob/master/AdlsSDKGettingStarted/Program.cs
