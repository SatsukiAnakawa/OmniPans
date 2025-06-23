# OmniPans

**A utility app that allows you to easily adjust the volume and pan (left/right balance) of all audio devices on Windows from a single location.**

![Image](https://github.com/user-attachments/assets/048eb8e1-6a1f-498a-ac20-95d065f40b48)

[![Latest Release 最新版)](https://img.shields.io/github/v/release/SatsukiAnakawa/OmniPans)](https://github.com/SatsukiAnakawa/OmniPans/releases/latest)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/SatsukiAnakawa/OmniPans)](https://github.com/SatsukiAnakawa/OmniPans/releases/latest)
[**最新版のダウンロードはこちらから (Download latest version here)**](https://github.com/SatsukiAnakawa/OmniPans/releases/latest)
<p align="center">
  <a href="https://github.com/SatsukiAnakawa/OmniPans/releases/latest">
    <img src="https://img.shields.io/badge/Download-Now-blue.svg?logo=github&style=for-the-badge" alt="Download">
  </a>
</p>


<hr>

<details>
<summary><strong>日本語</strong></summary>

## OmniPans

**Windowsの全オーディオデバイスの音量とパン（左右バランス）を、一つの場所から手軽に調整できるユーティリティアプリです。**

### 主な機能
* **集中コントロール**: 接続されている全ての音声出力デバイスの音量とパンを、一つのフライアウトウィンドウで管理できます。
* **スタートアップ登録**: PCの起動時に、アプリを自動で立ち上げるかどうかをタスクトレイメニューから簡単に設定できます。
* **デバイスの非表示**: 普段使わないデバイスを一時的に非表示にして、リストをスッキリさせることができます。
* **バックグラウンド動作**: フライアウトウィンドウを開いても、タスクマネージャー上では「バックグラウンドプロセス」として動作し続けます。

### 使い方

1.  アプリを起動すると、タスクトレイにアイコンが表示されます。
2.  アイコンを**左クリック**すると、デバイス一覧のフライアウトウィンドウが表示されます。
3.  アイコンを**右クリック**すると、設定メニュー（スタートアップ設定、非表示デバイスの再表示、終了）が開きます。

### 動作環境

* Windows 10 / 11
* .NET 8.0 Desktop Runtime

### 主な技術的特徴

このアプリケーションは、モダンな設計原則と最新の.NET技術に基づいて構築されています。

* **アーキテクチャ**: MVVM, DI, SRP, DIP, Factory, Messenger
* **UI (WPF)**: WPF Native Tray Icon, Popup Control, Declarative UI, Custom Control
* **C# / .NET**: .NET 8 / C# 12, Structured Logging, Externalized Configuration

### ビルド方法

* Visual Studio 2022 (Community Edition以上)
* .NET 8.0 SDK

</details>

<hr>

## Key Features
* **Centralized Control**: Manage the volume and pan of all connected audio output devices in a single flyout window.
* **Startup Registration**: Easily set whether the app starts automatically on PC boot from the task tray menu.
* **Hide Devices**: Temporarily hide devices you don't normally use to keep the list clean.
* **Background Operation**: Even when the flyout window is open, it continues to run as a "background process" in the Task Manager.

## How to Use

1.  When you launch the app, an icon will appear in the task tray.
2.  **Left-click** the icon to display the flyout window with the device list.
3.  **Right-click** the icon to open the settings menu (Startup settings, Re-show hidden devices, Exit).

## System Requirements

* Windows 10 / 11
* .NET 8.0 Desktop Runtime

## Key Technical Features

This application is built on modern design principles and the latest .NET technologies.

* **Architecture**: MVVM, DI, SRP, DIP, Factory, Messenger
* **UI (WPF)**: WPF Native Tray Icon, Popup Control, Declarative UI, Custom Control
* **C# / .NET**: .NET 8 / C# 12, Structured Logging, Externalized Configuration

## How to Build

To build this project from the source code, you will need the following environment:

* Visual Studio 2022 (Community Edition or higher)
* .NET 8.0 SDK

After cloning the repository, open `OmniPans.sln` in Visual Studio, restore the NuGet packages, and then build.
