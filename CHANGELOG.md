# 変更履歴

このプロジェクトの全ての変更点は、このファイルに記録されます。

## [1.2.0] - 2025-06-19

### 追加
- スライダーのラベル（「音量」「パン」）を、状態に応じて動的に変化するアイコンに変更。
- スピーカーアイコンのクリックによるミュート切り替え機能。
- パンアイコンのクリックによるパンのリセット（中央揃え）/復元機能。
- パンのリセット状態をアプリ終了後も記憶し、永続化する機能。
- デバイス数が画面の高さを超えた場合に、フライアウトウィンドウにスクロールバーを表示する機能。
- 非表示デバイスのメニューで、デバイスを再表示してもメニューが閉じないよう変更。

### 修正
- コード全体の品質と保守性を向上させるため、大規模なリファクタリングを実施。
- パンのリセット機能が、OSの実際のパン設定に即時反映されない問題を修正。
- スライダーの手動操作でパンを0にした際に、意図せずリセット前の値に復元されてしまう問題を修正。
- アプリ実行中のオーディオデバイスの有効化/無効化時に、タスクトレイアイコンが応答しなくなることがある安定性の問題を修正。
- コンテキストメニューの項目で、マウスカーソルを余白に合わせた際に文字だけが取り残されたように見える表示の不具合を修正。


## [1.1.0] - 2025-06-16

### 追加
- アプリケーションの多重起動を防止する機能を追加。

### 修正
- アプリが起動に失敗する重大な不具合を修正。


## [1.0.0] - 2025-06-15

#### 主な機能と特徴

- **集中コントロール**: 接続されている全ての音声出力デバイスの音量とパン（左右バランス）を、一つのフライアウトウィンドウで管理。
- **スタートアップ登録**: PC起動時のアプリ自動起動設定を、タスクトレイメニューから切り替え可能。
- **デバイスの非表示/再表示**: 不要なデバイスを非表示にし、メニューから簡単に再表示可能。
- **バックグラウンド動作**: フライアウト表示中もタスクマネージャー上では「バックグラウンドプロセス」として動作。
