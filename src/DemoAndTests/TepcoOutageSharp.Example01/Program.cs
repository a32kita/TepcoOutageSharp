using System.Runtime.CompilerServices;

namespace TepcoOutageSharp.Example01
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var tepcoOISv = new TepcoOutageInfoService())
                Task.Run(async () => await Main2(tepcoOISv)).Wait();
        }

        static async Task Main2(TepcoOutageInfoService tepcoOISv)
        {
            // 全域の停電情報
            var outageInfo = await tepcoOISv.GetEntireServiceAreaOutageInfoAsync();

            WriteOutageInfo(outageInfo);
            Console.WriteLine();

            Console.WriteLine("千葉県詳細");
            var chibaArea = outageInfo.ChildAreaOutageInfos.Single(c => c.Title.Equals("千葉県"));
            var chibaOutageInfo = await tepcoOISv.GetAreaOutageInfoAsync(chibaArea.AreaCode);

            WriteOutageInfo(chibaOutageInfo);
        }

        static void WriteOutageInfo(TepcoAreaOutageInfo outageInfo)
        {
            Console.WriteLine("名前     = {0}", outageInfo.Title);
            Console.WriteLine("ｴﾘｱｺｰﾄﾞ  = {0}", outageInfo.AreaCode);
            Console.WriteLine("停電件数 = {0}", outageInfo.OutageCount);
            Console.WriteLine();
            Console.WriteLine("サブエリア情報;");
            foreach(var childAreaOutageInfo in outageInfo.ChildAreaOutageInfos)
            {
                Console.WriteLine("名前     = {0}", childAreaOutageInfo.Title);
                Console.WriteLine("ｴﾘｱｺｰﾄﾞ  = {0}", childAreaOutageInfo.AreaCode);
                Console.WriteLine("停電件数 = {0}", childAreaOutageInfo.OutageCount);
            }
        }
    }
}
