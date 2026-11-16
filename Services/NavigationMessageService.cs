namespace YourCravings.Services;

public class NavigationMessageService
{
    // 送信完了メッセージを表示するかどうか
    // 外部からは読み取りのみ可能
    public bool ShowSubmissionSuccess { get; private set; }

    // 記念日ポップアップを表示するかどうか
    // ログイン画面からの遷移時だけ有効にする
    private bool ShowAnniversaryPopup { get; set; }

    // 送信成功後、完了メッセージを表示する状態にする
    public void SetSubmissionSuccess()
    {
        ShowSubmissionSuccess = true;
    }

    // 送信完了メッセージの表示状態を取得し、一度取得したらリセットする
    // ログイン画面でメッセージを一度だけ表示するために使用
    public bool ConsumeSubmissionSuccess()
    {
        // 現在の表示状態を取得
        var result = ShowSubmissionSuccess;

        // 次回以降に再表示されないようにリセット
        ShowSubmissionSuccess = false;

        return result;
    }

    // ログイン画面からの遷移時に記念日ポップアップを表示する状態にする
    public void SetAnniversaryPopup()
    {
        ShowAnniversaryPopup = true;
    }

    // 記念日ポップアップの表示状態を取得し、一度取得したらリセットする
    public bool ConsumeAnniversaryPopup()
    {
        // 現在の表示状態を取得
        var result = ShowAnniversaryPopup;

        // 次回以降に再表示されないようにリセット
        ShowAnniversaryPopup = false;

        return result;
    }
}
