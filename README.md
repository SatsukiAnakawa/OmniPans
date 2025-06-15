# OmniPans

**Windowsの全オーディオデバイスの音量とパン（左右バランス）を、一つの場所から手軽に調整できるユーティリティアプリです。**

![Image](https://github.com/user-attachments/assets/97aca3f7-cced-4467-8cc1-293dfe33fa8f)

! ## 主な機能

* **集中コントロール**: 接続されている全ての音声出力デバイスの音量とパンを、一つのフライアウトウィンドウで管理できます。
* **スタートアップ登録**: PCの起動時に、アプリを自動で立ち上げるかどうかをタスクトレイメニューから簡単に設定できます。
* **デバイスの非表示**: 普段使わないデバイスを一時的に非表示にして、リストをスッキリさせることができます。
* **バックグラウンド動作**: フライアウトウィンドウを開いても、タスクマネージャー上では「バックグラウンドプロセス」として動作し続けます。
* **UI仮想化によるパフォーマンス**: 多数のデバイスが接続されても、`VirtualizingStackPanel`によりUIの応答性が損なわれません。
* **高速操作への応答性**: スライダーを素早く連続操作しても、`CancellationToken`を用いたキャンセル処理により、最新の操作がスムーズに反映されます。

## 使い方

1.  アプリを起動すると、タスクトレイにアイコンが表示されます。
2.  アイコンを**左クリック**すると、デバイス一覧のフライアウトウィンドウが表示されます。
3.  アイコンを**右クリック**すると、設定メニュー（スタートアップ設定、非表示デバイスの再表示、終了）が開きます。

## 動作環境

* Windows 10 / 11
* .NET 8.0 Desktop Runtime

## 主な技術的特徴

このアプリケーションは、モダンな設計原則と最新の.NET技術に基づいて構築されています。

### アーキテクチャ
* **MVVM (Model-View-ViewModel)**: UIとロジックを明確に分離しています。
* **DI (Dependency Injection)**: `Microsoft.Extensions.DependencyInjection` を利用して、クラス間の依存関係を疎に保っています。
* **責務分離 (SRP)**: `DeviceFilter` (デバイスのフィルタリング)、`PanChangeNotifier` (パン変更通知)、`WindowPositioner` (ウィンドウ位置決め) のように、各クラスが単一の責任を持つように設計されています。
* **依存関係逆転の原則 (DIP)**: `IDisplayedDevice`インターフェースを導入し、UI層がNAudioのような特定のライブラリに直接依存しない構造になっています。
* **Factoryパターン**: `DeviceViewModelFactory`などを通じて、複雑なオブジェクトの生成ロジックをカプセル化しています。
* **Messengerパターン**: `CommunityToolkit.Mvvm`の`WeakReferenceMessenger`を利用し、ViewModel間の疎結合な通信を実現しています。

### UI (WPF)
* **WPFネイティブなトレイアイコン**: `Hardcodet.NotifyIcon.Wpf`ライブラリを利用して、Windows Formsへの依存を排除しました。
* **Popupコントロール**: OSにメインウィンドウと認識させないため、フライアウトは`Popup`コントロールで実装されています。
* **宣言的なUI**: `DataTrigger`や`Microsoft.Xaml.Behaviors.Wpf`を多用し、UIの振る舞いをXAML内で宣言的に記述しています。
* **カスタムコントロール**: 汎用的な`InteractiveSlider`をカスタムコントロールとして実装し、`VisualStateManager`によって見た目の状態を管理しています。

### C# / .NET
* **.NET 8** / **C# 12**: `プライマリコンストラクタ`や`コレクション式`など、最新の言語機能を積極的に採用し、コードの簡潔性と可読性を高めています。
* **構造化ロギング**: `Serilog`を利用して、構造化されたログを出力します。
* **設定の外部化**: `Microsoft.Extensions.Configuration`を利用し、`appsettings.json`からアプリケーションの振る舞いを設定できるようにしています。

## ビルド方法

このプロジェクトをソースコードからビルドするには、以下の環境が必要です。

* Visual Studio 2022 (Community Edition以上)
* .NET 8.0 SDK

リポジトリをクローンした後、Visual Studioで`OmniPans.sln`を開き、NuGetパッケージを復元してからビルドしてください。

## ライセンス

このプロジェクトは [MIT License](LICENSE.txt) の下で公開されています。
