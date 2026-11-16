using Microsoft.JSInterop;
using YourCravings.Models;

namespace YourCravings.Services;

public class EmailJsService
{
    // JavaScriptのEmailJS処理を呼び出すためのランタイム
    private readonly IJSRuntime _jsRuntime;

    // EmailJSの接続情報
    private const string ServiceId = "YourCravings";
    private const string TemplateId = "template_9a8bejk";
    private const string PublicKey = "VXCb9UViTqcqSrF6e";


    // JavaScriptランタイムを受け取り、フィールドに保持する
    public EmailJsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // 送信内容をEmailJSのテンプレートに渡し、メールを送信する
    public async Task<bool> SendEmailAsync(
        PendingSubmission submission)
    {
        // EmailJSのテンプレートで使用するパラメーターを作成
        var templateParams = new
        {
            // わがままを送信した日付
            request_date = submission.Date,

            // 選択されたサービスを改行区切りでまとめる
            selected_services = string.Join(
                "\n",
                submission.SelectedServices.Select(
                    service => "・" + service
                )
            ),

            // 入力されたメッセージ（未入力の場合は空文字）
            message = submission.Message ?? string.Empty
        };

        // JavaScript側のEmailJS送信処理を呼び出し、送信結果を返す
        return await _jsRuntime.InvokeAsync<bool>(
            "emailJsHelper.sendEmail",
            ServiceId,
            TemplateId,
            PublicKey,
            templateParams
        );
    }
}