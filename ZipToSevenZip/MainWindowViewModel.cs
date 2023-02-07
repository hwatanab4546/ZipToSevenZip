using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SevenZip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;

namespace ZipToSevenZip
{
    internal class MainWindowViewModel : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ReactivePropertySlim<string> ZipFilePath { get; }
        public ReactiveCommand RunCommand { get; }

        public MainWindowViewModel()
        {
            ZipFilePath = new ReactivePropertySlim<string>();

            RunCommand = ZipFilePath
                .Select(x => File.Exists(x))
                .ToReactiveCommand()
                .WithSubscribe(() =>
                {
                    try
                    {
                        var zipFilePath = ZipFilePath.Value;
                        var sevenZipFilePath = Path.ChangeExtension(zipFilePath, ".7z");

                        ZipToSevenZip(zipFilePath, sevenZipFilePath);

                        MessageBox.Show("完了");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "例外発生", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                })
                .AddTo(disposables);
        }

        private readonly CompositeDisposable disposables = new();

        private static void ZipToSevenZip(string zipFilePath, string sevenZipFilePath)
        {
            // Shift-JISエンコーディングのためのおまじない
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var dic = new Dictionary<string, Stream>();

#if true
            // UpdateモードでオープンしてMemoryStream使わないようにするとメモリ使用量が何割か減るけど
            // 元ファイルの最終更新日時が更新されるのが気持ち悪い

            // エントリ情報の日本語がShift-JISのようなのでエンコードの指定が必要
            using var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Read, Encoding.GetEncoding("Shift_JIS"));
            foreach (var entry in zip.Entries)
            {
                // ディレクトリはスキップ
                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }

                // SevenZipCompressor.CompressStreamDictionary()に渡すStreamはseek可能じゃないとダメらしい
                // ZipファイルをReadモードでオープンしているためにentry.Open()が返すStreamがseek不可になっているので
                // 一旦MemoryStreamにコピーしたものをCompressStreamDictionary()に渡す
                var ms = new MemoryStream();
                using var fs = entry.Open();
                fs.CopyTo(ms);
                ms.Position = 0;

                dic.Add(entry.FullName, ms);
            }
#else
            // エントリ情報の日本語がShift-JISのようなのでエンコードの指定が必要
            using var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Update, Encoding.GetEncoding("Shift_JIS"));
            foreach (var entry in zip.Entries)
            {
                // ディレクトリはスキップ
                if (string.IsNullOrEmpty(entry.Name))
                {
                    continue;
                }

                dic.Add(entry.FullName, entry.Open());
            }
#endif

            var compressor = new SevenZipCompressor();
            compressor.CompressStreamDictionary(dic, sevenZipFilePath);
        }

        public void Dispose() => disposables.Dispose();
    }
}
