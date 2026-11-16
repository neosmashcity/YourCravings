namespace YourCravings.Models;

public class CravingHistory
{
    // 送信ごとの一意なID
    public string RequestId { get; set; } = string.Empty;

    // 選択した日付（yyyy-MM-dd）
    public string Date { get; set; } = string.Empty;

    // 選択したサービス
    public List<string> SelectedServices { get; set; } = new();

    // 任意メッセージ
    public string? Message { get; set; }

    // メール送信成功日時
    public DateTimeOffset SentAt { get; set; }
}