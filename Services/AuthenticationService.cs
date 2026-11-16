using Microsoft.JSInterop;
using System.Security.Cryptography;
using System.Text;

namespace YourCravings.Services;

public class AuthenticationService
{
    // localStorageへのアクセスに使用するJavaScriptランタイム
    private readonly IJSRuntime _jsRuntime;

    // localStorageに保存するログイン状態のキー
    private const string AuthStorageKey = "YourCravings_IsAuthenticated";

    // 合言葉のSHA-256ハッシュ値
    private const string PasswordHash1 = "108821b3ae13d0bf1ca94d1a736b54350cf3f41fb2321ff0a1181d54779c6de6";
    private const string PasswordHash2 = "634e7f395a9fda05901b35489a9a52b488ef3bab80fa465a3c3dcd0b7b1cf7dc";

    // GAS通信に使用する合言葉（ログイン成功後、メモリ上のみで保持）
    private string? _apiPassword;


    // JavaScriptランタイムを受け取り、フィールドに保持する
    public AuthenticationService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // 合言葉をSHA-256でハッシュ化して検証し、正しければログイン状態を保存する
    public async Task<bool> LoginAsync(string passphrase)
    {
        // 未入力または空白だけの場合はログインを拒否する
        if (string.IsNullOrWhiteSpace(passphrase))
        {
            return false;
        }

        try
        {
            // 入力された合言葉をSHA-256でハッシュ化する
            using var sha256 = SHA256.Create();

            // 合言葉をUTF-8のバイト配列に変換
            var inputBytes = Encoding.UTF8.GetBytes(passphrase);

            // 入力された合言葉のSHA-256ハッシュ値
            var inputHash = sha256.ComputeHash(inputBytes);

            // 比較対象のハッシュ値を16進数文字列からバイト配列に変換
            var expectedHash1 = Convert.FromHexString(PasswordHash1);
            var expectedHash2 = Convert.FromHexString(PasswordHash2);

            // 入力されたハッシュ値を2つの登録済みハッシュ値と比較する
            // FixedTimeEqualsを使用して比較時間の差による情報漏えいを抑える
            var isValid =
                CryptographicOperations.FixedTimeEquals(inputHash, expectedHash1)
                || CryptographicOperations.FixedTimeEquals(inputHash, expectedHash2);

            // どちらのハッシュ値とも一致しなければログインを拒否する
            if (!isValid)
            {
                return false;
            }

            // GAS通信に使用する合言葉をメモリ上に保持
            _apiPassword = passphrase;

            // localStorageにログイン状態を保存
            await _jsRuntime.InvokeVoidAsync(
                "localStorageHelper.setItem",
                AuthStorageKey,
                "true"
            );

            return true;
        }
        catch (FormatException)
        {
            // ハッシュ値の形式が不正な場合はログインを拒否する
            return false;
        }
    }

    // GAS通信に使用する合言葉を取得する
    public string GetApiPassword()
    {
        // メモリ上に合言葉が存在しない場合は、再ログインを要求する
        if (string.IsNullOrEmpty(_apiPassword))
        {
            throw new InvalidOperationException("GAS通信に必要な認証情報がありません。再ログインしてね");
        }

        return _apiPassword;
    }

    // localStorageのログイン状態と、GAS通信に必要な認証情報の有無を確認する
    public async Task<bool> IsAuthenticatedAsync()
    {
        // localStorageから保存済みのログイン状態を取得
        var result = await _jsRuntime.InvokeAsync<string?>(
            "localStorageHelper.getItem",
            AuthStorageKey
        );

        // ログイン状態がtrueで、かつメモリ上に合言葉がある場合のみ認証済みとする
        return result == "true" && !string.IsNullOrEmpty(_apiPassword);
    }
}