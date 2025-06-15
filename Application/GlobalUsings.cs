// Application/GlobalUsings.cs
// プロジェクト全体で使用する名前空間をグローバルに定義します。

#region NuGet
global using CommunityToolkit.Mvvm.ComponentModel;
global using CommunityToolkit.Mvvm.Input;
global using CommunityToolkit.Mvvm.Messaging;
global using Hardcodet.Wpf.TaskbarNotification;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Win32;
global using NAudio.CoreAudioApi;
global using NAudio.CoreAudioApi.Interfaces;
#endregion

#region OmniPans
global using OmniPans.Application;
global using OmniPans.Application.ServiceExtensions;
global using OmniPans.Core;
global using OmniPans.Core.Models;
global using OmniPans.Core.Services.Application;
global using OmniPans.Core.Services.Audio;
global using OmniPans.Core.Services.Factories;
global using OmniPans.Core.Services.UserInterface;
global using OmniPans.Core.Services.UserSettings;
global using OmniPans.Core.Utils;
global using OmniPans.Infrastructure.Services.Application;
global using OmniPans.Infrastructure.Services.Audio;
global using OmniPans.Infrastructure.Services.Factories;
global using OmniPans.Infrastructure.Services.UserInterface;
global using OmniPans.Infrastructure.Services.UserSettings;
global using OmniPans.Presentation.InteractionHandlers;
global using OmniPans.Presentation.Messages;
global using OmniPans.Presentation.ViewModels;
global using OmniPans.Presentation.Views;
#endregion

#region System
global using Serilog;
global using System;
global using System.Collections.Generic;
global using System.Collections.ObjectModel;
global using System.Collections.Specialized;
global using System.Globalization;
global using System.IO;
global using System.Linq;
global using System.Reactive.Concurrency;
global using System.Reactive.Linq;
global using System.Reactive.Subjects;
global using System.Runtime.Versioning;
global using System.Text.Json;
global using System.Threading.Tasks;
#endregion

#region WPF
global using System.Windows;
global using System.Windows.Input;
global using System.Windows.Interop;
global using System.Windows.Media;
global using System.Windows.Media.Imaging;
global using System.Windows.Threading;
global using Brush = System.Windows.Media.Brush;
global using Brushes = System.Windows.Media.Brushes;
#endregion

#region Tuple Aliases
global using DefaultDeviceChangedArgs = (NAudio.CoreAudioApi.DataFlow Flow, NAudio.CoreAudioApi.Role Role, string DefaultDeviceId);
global using DevicePropertyChangedArgs = (string DeviceId, NAudio.CoreAudioApi.PropertyKey PropertyKey);
global using DeviceStateChangedArgs = (string DeviceId, NAudio.CoreAudioApi.DeviceState NewState);
#endregion

#region WPF Type Aliases
global using HorizontalAlignment = System.Windows.HorizontalAlignment;
global using KeyEventArgs = System.Windows.Input.KeyEventArgs;
global using MessageBox = System.Windows.MessageBox;
global using MessageBoxButton = System.Windows.MessageBoxButton;
global using MessageBoxImage = System.Windows.MessageBoxImage;
global using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
global using MouseEventArgs = System.Windows.Input.MouseEventArgs;
global using MouseWheelEventArgs = System.Windows.Input.MouseWheelEventArgs;
#endregion