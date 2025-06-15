using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileTool
{
    static class Program
    {
        static void Main(string[] args)
        {
            // --- 設定はここにまとめるよ ---

            string baseDirectory = @"D:\ソフト\開発\Visual Code 2022 data\OmniPans";
            string originalOutputFilePath = @"D:\ソフト\開発\omnipans\全コード.txt";

            // 対象にしたいファイルの拡張子
            var targetExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".xaml", ".cs", ".txt", ".xml", ".json", ".md", ".csproj"
            };

            // 除外したいディレクトリ名（この名前のフォルダは中を検索しないよ）
            var excludedDirectoryNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "bin", "obj", ".vs", "Properties" // Propertiesフォルダも除外してみる？
            };


            // --- ここから処理の始まり ---

            // 出力パスの生成ロジックはキミのアイデアをそのまま活かしたよ！
            string outputFilePath = GenerateOutputFilePath(originalOutputFilePath);

            Console.WriteLine("ファイル検索を始めるよ！");
            Console.WriteLine($"ソースディレクトリ: {baseDirectory}");
            Console.WriteLine($"出力ファイル: {outputFilePath}");
            Console.WriteLine("---");

            try
            {
                // 1. ファイルを再帰的に検索する
                var filePaths = FindFiles(baseDirectory, targetExtensions, excludedDirectoryNames);
                var skippedFiles = new List<string>();
                var combinedContent = new StringBuilder();

                // 2. 見つかったファイルを一つずつ処理する
                foreach (var filePath in filePaths)
                {
                    try
                    {
                        Console.WriteLine($"読み込み中: {filePath}");
                        string fileContent = File.ReadAllText(filePath, Encoding.UTF8);

                        // Uriを使う方法に戻すよ。baseDirectoryの最後にフォルダの区切り文字がないと変な結果になることがあるから、しっかり付けてあげるのがコツなんだ。
                        string relativePathForDisplay = new Uri(baseDirectory.TrimEnd('\\') + @"\").MakeRelativeUri(new Uri(filePath)).ToString().Replace("/", @"\");

                        combinedContent.AppendLine($"// ----- 内容ここから: {Path.GetFileName(filePath)} ({relativePathForDisplay}) -----");
                        combinedContent.AppendLine();
                        combinedContent.AppendLine(fileContent);
                        combinedContent.AppendLine();
                        combinedContent.AppendLine($"// ----- 内容ここまで: {Path.GetFileName(filePath)} -----");
                        combinedContent.AppendLine(new string('\n', 5));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"エラー発生 ({filePath}): {ex.Message}");
                        skippedFiles.Add($"{filePath} (エラー: {ex.Message})");
                    }
                }

                // 3. 結合した内容をファイルに書き出す
                WriteResultToFile(outputFilePath, combinedContent, skippedFiles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n処理の途中で大きなエラーが起きちゃった…: {ex.Message}");
            }

            Console.WriteLine("\n処理が終わったよ。何かキーを押して閉じてね。");
            Console.ReadKey();
        }

        /// <summary>
        /// 元の出力パスから、新しい出力先のフルパスを生成するメソッドだよ。
        /// </summary>
        private static string GenerateOutputFilePath(string originalPath)
        {
            string originalDirectory = Path.GetDirectoryName(originalPath);
            string fileName = Path.GetFileName(originalPath);
            string parentDirectory = Path.GetDirectoryName(originalDirectory);
            string targetDirectoryName = Path.GetFileName(originalDirectory);
            string newDirectoryName = targetDirectoryName + " メモ";
            return Path.Combine(parentDirectory, newDirectoryName, fileName);
        }

        /// <summary>
        /// 指定されたディレクトリから、条件に合うファイルを全部見つけてくるメソッドだよ。
        /// </summary>
        private static IEnumerable<string> FindFiles(string path, ISet<string> extensions, ISet<string> excludedDirs)
        {
            Console.WriteLine("対象ファイルを検索中...");
            // EnumerateFilesを使うと、大量のファイルがあってもメモリに優しいんだ
            return Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)
                .Where(filePath =>
                    // 除外ディレクトリに含まれていないかチェック
                    !filePath.Split(Path.DirectorySeparatorChar).Any(dir => excludedDirs.Contains(dir)) &&
                    // 対象の拡張子かチェック
                    extensions.Contains(Path.GetExtension(filePath)));
        }

        /// <summary>
        /// 結果をまとめてファイルに書き出すメソッドだよ。
        /// </summary>
        private static void WriteResultToFile(string path, StringBuilder content, List<string> skipped)
        {
            try
            {
                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(path, content.ToString(), Encoding.UTF8);
                Console.WriteLine($"\nぜんぶまとめたファイルを保存したよ！: {path}");

                if (skipped.Any())
                {
                    Console.WriteLine("\n--- スキップしたファイル ---");
                    foreach (var skippedFile in skipped)
                    {
                        Console.WriteLine(skippedFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nファイルの保存中にエラーが起きちゃった…: {ex.Message}");
            }
        }
    }
}