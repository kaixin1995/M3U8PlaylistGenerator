using System;
using System.IO;
using System.Linq;

class M3U8PlaylistGenerator
{
    static void Main(string[] args)
    {
        // 获取用户输入
        Console.WriteLine("请输入需要检测的目录路径:");
        string directoryPath = Console.ReadLine();

        // 检查目录是否存在
        if (!Directory.Exists(directoryPath))
        {
            Console.WriteLine("目录不存在，请检查路径.");
            return;
        }

        // 获取目录中所有的 M3U8 文件
        var playlistFiles = Directory.GetFiles(directoryPath, "*.m3u8");

        string playlistPath;

        // 如果目录中存在 .m3u8 文件，默认使用第一个找到的文件
        if (playlistFiles.Length > 0)
        {
            playlistPath = playlistFiles.First();  // 默认使用第一个找到的歌单
            Console.WriteLine($"检测到现有的歌单文件: {playlistPath}，将继续使用该文件...");
        }
        else
        {
            // 如果目录中没有 M3U8 文件，提示用户输入新的歌单文件名
            Console.WriteLine("没有找到现有的歌单文件，请输入新建的歌单文件名（例如：playlist.m3u8）:");
            string playlistName = Console.ReadLine();

            // 检查用户输入的文件名是否已经包含 ".m3u8" 后缀
            if (!playlistName.EndsWith(".m3u8", StringComparison.OrdinalIgnoreCase))
            {
                // 如果没有 ".m3u8" 后缀，则自动加上
                playlistName += ".m3u8";
            }

            playlistPath = Path.Combine(directoryPath, playlistName);
        }

        // 检查歌单文件是否存在
        if (File.Exists(playlistPath))
        {
            Console.WriteLine("歌单文件已存在，正在更新歌单...");
            UpdatePlaylist(directoryPath, playlistPath);
        }
        else
        {
            Console.WriteLine("歌单文件不存在，创建新的歌单...");
            CreateNewPlaylist(directoryPath, playlistPath);
        }
    }

    // 更新歌单，删除不存在的文件并添加新文件
    static void UpdatePlaylist(string directoryPath, string playlistPath)
    {
        // 获取目录下所有音频文件（过滤掉 .lrc 和其他非音频文件）
        var audioExtensions = new[] { ".mp3", ".flac", ".wav", ".aac", ".ogg", ".m4a" };
        var allFiles = Directory.GetFiles(directoryPath)
                                .Where(f => audioExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                                .ToList();

        // 读取现有的歌单文件内容
        var existingPlaylist = File.ReadAllLines(playlistPath).ToList();

        // 获取已有的文件列表
        var existingFiles = existingPlaylist.Where(line => !line.StartsWith("#")).ToList();

        // 找出新增文件
        var newFiles = allFiles.Where(file => !existingFiles.Contains(file)).ToList();

        // 找出被删除的文件
        var deletedFiles = existingFiles.Where(file => !allFiles.Contains(file)).ToList();

        // 更新歌单内容
        var updatedPlaylist = existingPlaylist
            .Where(line => !deletedFiles.Contains(line)) // 删除不存在的文件
            .ToList();

        // 添加新文件
        foreach (var newFile in newFiles)
        {
            updatedPlaylist.Add(newFile);
        }

        // 写回歌单文件
        File.WriteAllLines(playlistPath, updatedPlaylist);
        Console.WriteLine("歌单更新完成!");
    }

    // 创建新的歌单文件
    static void CreateNewPlaylist(string directoryPath, string playlistPath)
    {
        // 获取目录下所有音频文件（过滤掉 .lrc 和其他非音频文件）
        var audioExtensions = new[] { ".mp3", ".flac", ".wav", ".aac", ".ogg", ".m4a" };
        var allFiles = Directory.GetFiles(directoryPath)
                                .Where(f => audioExtensions.Contains(Path.GetExtension(f), StringComparer.OrdinalIgnoreCase))
                                .ToList();

        // 创建新的歌单内容
        var newPlaylist = allFiles.Select(file => file).ToList();

        // 将歌单内容写入新文件
        File.WriteAllLines(playlistPath, newPlaylist);
        Console.WriteLine("新的歌单文件已创建!");
    }
}
