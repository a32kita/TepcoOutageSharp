# TepcoOutageSharp
TepcoOutageSharp is a .NET library for retrieving power outage information published by Tokyo Electric Power Company (TEPCO).


## Example
```csharp
// 全域の停電情報 (県単位)
var outageInfo = await tepcoOISv.GetEntireServiceAreaOutageInfoAsync();

WriteOutageInfo(outageInfo);
Console.WriteLine();

// 県単位
Console.WriteLine("【千葉県詳細】");
var chibaArea = outageInfo.ChildAreaOutageInfos.Single(c => c.Title.Equals("千葉県"));
var chibaOutageInfo = await tepcoOISv.GetAreaOutageInfoAsync(chibaArea.AreaCode);

WriteOutageInfo(chibaOutageInfo);
```

[Please refer to this for the full version of the sample source code.](https://github.com/a32kita/TepcoOutageSharp/blob/master/src/DemoAndTests/TepcoOutageSharp.Example01/Program.cs)