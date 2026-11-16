namespace YourCravings.Services;

public class DateTimeService
{
    // 日本時間のUTCオフセット
    private static readonly TimeSpan JstOffset = TimeSpan.FromHours(9);

    // 現在の日本時間を取得
    public DateTimeOffset GetNow()
    {
        return DateTimeOffset.UtcNow.ToOffset(JstOffset);
    }

    // 今日の日付を取得
    public DateOnly GetToday()
    {
        return DateOnly.FromDateTime(GetNow().DateTime);
    }

    // 今日の日付を yyyy-MM-dd 形式で取得
    public string GetTodayString()
    {
        return GetToday().ToString("yyyy-MM-dd");
    }

    // 毎月5日かどうかを判定
    public bool IsSpecialDay()
    {
        return GetToday().Day == 5;
    }
}
