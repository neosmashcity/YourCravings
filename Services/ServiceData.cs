using YourCravings.Models;

namespace YourCravings.Services;

public static class ServiceData
{
    // サービス一覧
    public static readonly List<ServiceItem> Services = new()
    {
        new() { Id = 1, Name = "キスいっぱいしてほしい" },
        new() { Id = 2, Name = "ギューいっぱいしてほしい" },
        new() { Id = 3, Name = "バックハグいっぱいしてほしい" },
        new() { Id = 4, Name = "よしよしいっぱいしてほしい" },
        new() { Id = 5, Name = "とにかくいっぱい触ってほしい" },
        new() { Id = 6, Name = "いっぱいかわいいって言ってほしい" },
        new() { Id = 7, Name = "えっち...してほしいです" },
        new() { Id = 8, Name = "いっぱい甘えたい" },
        new() { Id = 9, Name = "いっぱい甘えてほしい" },
        new() { Id = 10, Name = "お腹を温めてほしい" },
        new() { Id = 11, Name = "背中を温めてほしい" },
        new() { Id = 12, Name = "とにかく会いたい" },
        new() { Id = 13, Name = "お家来て" },
        new() { Id = 14, Name = "お家におって" },
        new() { Id = 15, Name = "一緒に寝んねしたい" },
        new() { Id = 16, Name = "今日は早く寝て" },
        new() { Id = 17, Name = "今日は寝ないで" },
        new() { Id = 18, Name = "遠出をしたい" },
        new() { Id = 19, Name = "近くで遊びたい" },
        new() { Id = 20, Name = "お散歩したい" }
    };
}
