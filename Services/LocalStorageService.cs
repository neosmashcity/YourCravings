using Microsoft.JSInterop;
using System.Text.Json;
using YourCravings.Models;

namespace YourCravings.Services;

public class LocalStorageService
{
    // JavaScript経由でブラウザーのlocalStorageを操作するためのランタイム
    private readonly IJSRuntime _jsRuntime;

    // localStorageに送信処理の状態を保存する際に使用するキー
    private const string StorageKey = "YourCravings_SubmissionState";


    // JavaScriptランタイムを受け取り、フィールドに保持する
    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // JSONからSubmissionStateへ変換する際の設定
    // プロパティ名の大文字・小文字を区別しない
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    // localStorageから送信処理の状態を取得する
    public async Task<SubmissionState> GetStateAsync()
    {
        // 保存されているJSON文字列を取得
        var json = await _jsRuntime.InvokeAsync<string?>("localStorageHelper.getItem", StorageKey);

        // 保存データが存在しない場合は初期状態を返す
        if (string.IsNullOrWhiteSpace(json))
        {
            return new SubmissionState();
        }

        // JSON文字列をSubmissionStateに変換する
        // 変換結果がnullの場合も初期状態を返す
        return JsonSerializer.Deserialize<SubmissionState>(
            json,
            _jsonOptions
        ) ?? new SubmissionState();
    }

    // 送信処理の状態をJSON形式に変換してlocalStorageへ保存する
    public async Task SaveStateAsync(SubmissionState state)
    {
        // SubmissionStateをJSON文字列に変換
        var json = JsonSerializer.Serialize(state);

        // JavaScript経由でlocalStorageに保存
        await _jsRuntime.InvokeVoidAsync("localStorageHelper.setItem", StorageKey, json);
    }
}