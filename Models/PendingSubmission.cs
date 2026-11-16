namespace YourCravings.Models;

public class PendingSubmission
{
    // 送信ごとの一意なID
    public string RequestId { get; set; } = string.Empty;

    // 選択した日付（yyyy-MM-dd）
    public string Date { get; set; } = string.Empty;

    // 選択したサービス
    public List<string> SelectedServices { get; set; } = new();

    // 任意メッセージ
    public string? Message { get; set; }

    // メール送信が成功したか
    public bool EmailSent { get; set; }

    // 履歴保存が成功したか
    public bool HistorySaved { get; set; }

    // メール送信成功日時
    public DateTimeOffset? SentAt { get; set; }
}