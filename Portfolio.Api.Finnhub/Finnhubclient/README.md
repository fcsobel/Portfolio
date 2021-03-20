## FinnhubClient Client

#### Code Generation:

[Finnhubclient/FinnhubClient.cs](FinnhubClient.cs) is generate by
 [Nswag](https://github.com/NSwag/NSwag) package from [Finnhubclient/FinnhubClient.json](FinnhubClient.json) schema using script in ```PreBuild``` Event:  

```
if $(ConfigurationName) == Debug $(NSwagExe_Core30) swagger2csclient  
/parameterDateTimeFormat:"yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz" 
/classname:FinnhubClient 
/namespace:Portfolio.Api.Finnhub 
/classstyle:Poco 
/exceptionClass:FinnhubApiException 
/arrayType:System.Collections.Generic.List 
/arrayInstanceType:System.Collections.Generic.List 
/arrayBaseType:System.Collections.Generic.List	
/dateType:System.DateTime 
/dateTimeType:System.DateTime 
/timeType:System.TimeSpan 
/timeSpanType:System.TimeSpan 
/injectHttpClient:true 
/generateExceptionClasses:false  
/OperationGenerationMode:SingleClientFromOperationId 
/input:"$(ProjectDir)FinnhubClient\FinnhubClient3.json" 
/output:"$(ProjectDir)FinnhubClient\FinnhubClient.cs"
```

- [NSwag Command Line Reference](https://github.com/NSwag/NSwag/wiki/CommandLine)

#### Dependencies:

- [NSwag.MSBuild](https://github.com/NSwag/NSwag/wiki/MSBuild)
- Newtonsoft.Json



