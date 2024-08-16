using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TepcoOutageSharp
{
    public class TepcoOutageInfoService : IDisposable
    {
        private HttpClient _httpClient;
        private CookieContainer _cookieContainer;
        private bool _isDisposed;


        public string BaseUrl
        {
            get;
            private set;
        }

        public TepcoOutageInfoService(string? baseUrl = null)
        {
            if (baseUrl == null)
                baseUrl = "https://teideninfo.tepco.co.jp";
            if (baseUrl[baseUrl.Length - 1] == '/')
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);

            this.BaseUrl = baseUrl;

            this._cookieContainer = new CookieContainer();

            var httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = this._cookieContainer;
            httpClientHandler.UseCookies = true;

            this._httpClient = new HttpClient(httpClientHandler);
            this._isDisposed = false;
        }

        private void _checkDisposed()
        {
            if (this._isDisposed)
                throw new ObjectDisposedException(this.GetType().FullName);
        }

        public async Task<TepcoAreaOutageInfo> GetAreaOutageInfoAsync(TepcoAreaCode areaCode)
        {
            this._checkDisposed();

            this._httpClient.DefaultRequestHeaders.Referrer = new Uri(this.BaseUrl);
            this._httpClient.DefaultRequestHeaders.UserAgent.Clear();
            this._httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:128.0) Gecko/20100101 Firefox/128.0");

            using (var hRes = await _httpClient.GetAsync(this.BaseUrl + "/js/custom/blackout/home.js"))
            {
#if true
                var jsContent = await hRes.Content.ReadAsStringAsync();
                var keyMatchingPattern = @"MapController\('teideninfo-auth', '([^']*)',";
                var keyMatch = Regex.Match(jsContent, keyMatchingPattern);
                var authKey = "sk3PT518";

                if (keyMatch.Success)
                    authKey = keyMatch.Groups[1].Value;
                else
                    throw new Exception("teideninfo-auth の取得に失敗しました");

                this._cookieContainer.Add(new Uri(this.BaseUrl), new Cookie("teideninfo-auth", authKey));
#else
                this._cookieContainer.Add(new Uri(this.BaseUrl), new Cookie("teideninfo-auth", "sk3PT518"));
#endif
            }

            var uri = new Uri(this.BaseUrl + "/flash/xml/" + areaCode + ".xml");
            try
            {
#if true
                using (var hRes = await this._httpClient.GetAsync(uri))
                using (var content = await hRes.Content.ReadAsStreamAsync())
                {
                    var xmlDoc = XDocument.Load(content);
#else
                using (var hReq = new HttpRequestMessage(HttpMethod.Get, uri))
                using (var hRes = await this._httpClient.SendAsync(hReq))
                {
                    // debug
                    var cookies = this._cookieContainer.GetCookies(uri);
                    foreach (Cookie cookie in cookies)
                    {
                        Console.WriteLine($"Name: {cookie.Name}, Value: {cookie.Value}, Domain: {cookie.Domain}");
                    }

                    var contentStr = await hRes.Content.ReadAsStringAsync();
                    var xmlDoc = XDocument.Parse(contentStr);

#endif

                    var xmlRoot = xmlDoc.Element("東京電力停電情報");

                    var title = xmlRoot.Element("タイトル")?.Value;

                    var outageCount = -1L;
                    var outageCountStr = xmlRoot.Element("停電軒数")?.Value;
                    Int64.TryParse(outageCountStr, out outageCount);

                    var childAreasDoc = xmlRoot.Elements("エリア");
                    var childAreas = new List<TepcoAreaOutageInfo>();
                    foreach (var childAreaDoc in childAreasDoc)
                    {
                        var childAreaCode = new TepcoAreaCode(childAreaDoc.Attribute("コード")?.Value ?? "_unknown_");
                        var childAreaTitle = childAreaDoc.Element("名前")?.Value;

                        var childAreaOutageCount = -1L;
                        var childAreaOutageCountStr = childAreaDoc.Element("停電軒数")?.Value;
                        Int64.TryParse(childAreaOutageCountStr, out childAreaOutageCount);

                        childAreas.Add(new TepcoAreaOutageInfo(childAreaTitle ?? "_unknown_", childAreaCode, childAreaOutageCount, new List<TepcoAreaOutageInfo>()));
                    }

                    return new TepcoAreaOutageInfo(title ?? "_unknown_", areaCode, outageCount, childAreas);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<TepcoAreaOutageInfo> GetEntireServiceAreaOutageInfoAsync()
        {
            return await this.GetAreaOutageInfoAsync(new TepcoAreaCode("00000000000"));
        }


        public void Dispose()
        {
            this._httpClient.Dispose();
            this._isDisposed = true;
        }
    }
}
