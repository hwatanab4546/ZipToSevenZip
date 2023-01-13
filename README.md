
# ZipToSevenZip

指定されたZipファイルの内容を含む7-zipファイルを、Zipファイルと同じフォルダに生成します

Zipファイルを解凍した結果をファイルシステムに書き出すことなく動作するので高速ですが、メモリを馬鹿食いするのでご注意ください

## 備考

* Zipファイル中の空フォルダは、生成された7-zipファイルに含まれません
* `Can not load 7-zip library or internal COM error! Message: DLL file does not exist.`
  * 実行時にこのエラーが発生する場合は`App.config`で定義されている`7zlocation`の設定値を変更してください

## 環境

* Windows 10 Home 21H2 (x64)
* Visual Studio Community 2022
* .NET 6.0 (C# 10.0)

## 使用ライブラリ

* [ReactiveProperty.Wpf](https://github.com/runceel/ReactiveProperty) 8.2.0
* [Squid-Box.SevenZipSharp](https://github.com/squid-box/SevenZipSharp) 1.5.0.366

## 関連リンク

* [圧縮・解凍ソフト 7-Zip](https://sevenzip.osdn.jp/)
