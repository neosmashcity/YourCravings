using Microsoft.JSInterop;
using System.Text.Json;
using YourCravings.Models;

namespace YourCravings.Services;

public class HistoryService
{
    // JavaScriptのGAS通信処理を呼び出すためのランタイム
    private readonly IJSRuntime _jsRuntime;

    // GAS通信に使用する認証情報を管理するサービス
    private readonly AuthenticationService _authService;

    // GASから取得した履歴を保持するキャッシュ
    // 保存処理が行われると無効化される
    private List<CravingHistory>? _cachedHistories;

    // 履歴取得・保存の同時実行を防止するための排他制御
    private readonly SemaphoreSlim _historySemaphore = new(1, 1);

    // GASのウェブアプリURL
    private const string ApiUrl = "https://script.google.com/macros/s/AKfycbwFzuM1COf-lCD4327-PUhO5VgEfL1m0fkNxJrBxdlc5ELPjpz2ERze9lDSFgsdESih/exec";


    // JavaScriptランタイムと認証サービスを受け取り、フィールドに保持する
    public HistoryService(IJSRuntime jsRuntime, AuthenticationService authService)
    {
        _jsRuntime = jsRuntime;
        _authService = authService;
    }

    // GASに履歴を保存する
    public async Task<string> SaveHistoryAsync(CravingHistory history)
    {
        // 履歴取得・保存の他の処理が終わるまで待機
        await _historySemaphore.WaitAsync();

        try
        {
            // GASに送信する保存リクエストを作成
            var request = new
            {
                action = "save",
                password = _authService.GetApiPassword(),
                history
            };

            // GASへ履歴を送信し、処理結果を取得
            var result = await PostAsync(request);

            // 保存処理後、次回の取得に備えてキャッシュを無効化
            _cachedHistories = null;

            return result;
        }
        finally
        {
            // 成功・失敗にかかわらずロックを解放
            _historySemaphore.Release();
        }
    }

    // GASから履歴データをJSON文字列として取得する
    public async Task<string> GetHistoryAsync()
    {
        // GASに送信する履歴取得リクエストを作成
        var request = new
        {
            action = "get",
            password = _authService.GetApiPassword()
        };

        return await PostAsync(request);
    }

    // GASから取得したJSONをCravingHistoryのリストに変換して返す
    public async Task<List<CravingHistory>> GetHistoriesAsync()
    {
        // 他の履歴取得・保存処理が終わるまで待機
        await _historySemaphore.WaitAsync();

        try
        {
            // キャッシュがあればAPIを呼ばずに返す
            if (_cachedHistories != null)
            {
                return _cachedHistories;
            }

            // GASから履歴データをJSON文字列として取得
            var json = await GetHistoryAsync();

            // JSONを履歴取得レスポンスの形式に変換
            var response = JsonSerializer.Deserialize<HistoryApiResponse>(json, new JsonSerializerOptions
            {
                // JSONのプロパティ名とC#のプロパティ名の大文字・小文字を区別しない
                PropertyNameCaseInsensitive = true
            }
            );

            // レスポンスを正しく読み取れなかった場合はエラーにする
            if (response == null)
            {
                throw new InvalidOperationException("履歴データを読み込めなかったよ");
            }

            // GAS側で取得処理が失敗している場合はエラーメッセージを返す
            if (!response.Success)
            {
                throw new InvalidOperationException(response.Message ?? "履歴の取得に失敗したよ");
            }

            // 取得した履歴をキャッシュに保存
            // 履歴が存在しない場合は空のリストを保存する
            _cachedHistories = response.Histories ?? new List<CravingHistory>();

            return _cachedHistories;
        }
        finally
        {
            // 成功・失敗にかかわらずロックを解放
            _historySemaphore.Release();
        }
    }

    // GASに今日の送信済み状態を確認する
    public async Task<bool> CheckTodayAsync()
    {
        // 今日の送信状態を確認するリクエストを作成
        var request = new { action = "checkToday" };

        // GASから確認結果をJSON文字列として取得
        var json = await PostAsync(request);

        // JSONを今日の送信状態確認レスポンスに変換
        var response = JsonSerializer.Deserialize<TodayStatusApiResponse>(json, new JsonSerializerOptions
        {
            // JSONのプロパティ名とC#のプロパティ名の大文字・小文字を区別しない
            PropertyNameCaseInsensitive = true
        }
        );

        // レスポンスを正しく読み取れなかった場合はエラーにする
        if (response == null)
        {
            throw new InvalidOperationException("今日の送信状態を確認できなかったよ");
        }

        // GAS側で確認処理が失敗している場合はエラーメッセージを返す
        if (!response.Success)
        {
            throw new InvalidOperationException(response.Message ?? "送信状態の確認に失敗したよ");
        }

        // 今日すでに送信済みかどうかを返す
        return response.AlreadySelected;
    }

    // GASへPOSTする共通処理
    // リクエスト内容をJavaScript側に渡し、レスポンスを文字列で受け取る
    private async Task<string> PostAsync(object request)
    {
        return await _jsRuntime.InvokeAsync<string>(
            "historyApiHelper.post",
            ApiUrl,
            request
        );
    }

    // GASからの履歴取得レスポンスを受け取るためのクラス
    private class HistoryApiResponse
    {
        // GAS側の処理が成功したかどうか
        public bool Success { get; set; }

        // GASから取得した履歴一覧
        public List<CravingHistory>? Histories { get; set; }

        // GAS側から返されたメッセージやエラー内容
        public string? Message { get; set; }
    }

    // 今日の送信状態確認レスポンスを受け取るためのクラス
    private class TodayStatusApiResponse
    {
        // GAS側の確認処理が成功したかどうか
        public bool Success { get; set; }

        // 今日すでにわがままが送信されているか
        public bool AlreadySelected { get; set; }

        // GAS側から返されたメッセージやエラー内容
        public string? Message { get; set; }
    }
}