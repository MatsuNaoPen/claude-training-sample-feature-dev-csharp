namespace OrderMgmt.Infrastructure;

/// <summary>
/// 社内共通の HTTP クライアント。認証ヘッダー付与・リトライ・ログを一元化するため、
/// アプリケーションコードは HttpClient を直接使わずこのクラスを経由する。
/// （デモ用のため、実際には通信せず固定の応答を返す）
/// </summary>
public class HttpClientWrapper
{
    private readonly string _baseUrl;

    public HttpClientWrapper(string baseUrl)
    {
        _baseUrl = baseUrl.TrimEnd('/');
    }

    public string BaseUrl => _baseUrl;

    public virtual Task<string> GetStringAsync(string path, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var body = path switch
        {
            "/rates/JPY" => """{"currency":"JPY","rate":1.0}""",
            "/rates/USD" => """{"currency":"USD","rate":0.0068}""",
            "/rates/EUR" => """{"currency":"EUR","rate":0.0061}""",
            _ => throw new HttpRequestException($"404 Not Found: {_baseUrl}{path}"),
        };

        return Task.FromResult(body);
    }
}
