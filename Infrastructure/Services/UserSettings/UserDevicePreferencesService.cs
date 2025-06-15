// Infrastructure/Services/UserSettings/UserDevicePreferencesService.cs
// ユーザーによるデバイスごとの設定や表示設定を管理・永続化します。
namespace OmniPans.Infrastructure.Services.UserSettings;

using System.Collections.Concurrent;

/// <summary>
/// ユーザーによるデバイスごとの設定や表示設定を管理・永続化する実装クラスです。
/// </summary>
public class UserDevicePreferencesService : IUserDevicePreferencesService, IDisposable
{
    private readonly ConcurrentDictionary<string, DeviceSettings> _deviceSettingsCache;
    private readonly IDeviceSettingsManager _settingsManager;
    private readonly IDeviceSettingsFactory _deviceSettingsFactory;
    private readonly ILogger<UserDevicePreferencesService> _logger;
    private readonly IAudioDeviceStateReader _audioDeviceStateReader;

    private readonly Subject<bool> _saveTrigger = new();
    private readonly IDisposable _saveSubscription;
    private bool _isDisposed;

    /// <summary>
    /// <see cref="UserDevicePreferencesService"/> の新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="settingsManager">設定の永続化を管理するサービス。</param>
    /// <param name="deviceSettingsFactory">新しいデバイスのデフォルト設定を生成するファクトリ。</param>
    /// <param name="logger">ロギングサービス。</param>
    /// <param name="audioDeviceStateReader">OSからデバイス状態を読み取るサービス。</param>
    public UserDevicePreferencesService(
       IDeviceSettingsManager settingsManager,
       IDeviceSettingsFactory deviceSettingsFactory,
       ILogger<UserDevicePreferencesService> logger,
       IAudioDeviceStateReader audioDeviceStateReader,
       Core.Models.Configuration.BehaviorConfig config)
    {
        _settingsManager = settingsManager;
        _deviceSettingsFactory = deviceSettingsFactory;
        _logger = logger;
        _audioDeviceStateReader = audioDeviceStateReader;

        _deviceSettingsCache = new ConcurrentDictionary<string, DeviceSettings>(_settingsManager.LoadSettings() ?? []);

        // 設定変更の通知を受け取り、指定された時間後に保存処理を実行する
        _saveSubscription = _saveTrigger
            .Throttle(TimeSpan.FromSeconds(config.AutoSaveDelaySeconds))
            .Subscribe(async _ => await SaveAllSettingsAsync());
    }

    /// <summary>
    /// 指定されたデバイスIDの設定を現在のOSの状態と同期し、その最新の設定を取得します。
    /// </summary>
    /// <param name="deviceId">同期および取得するデバイスのID。</param>
    /// <returns>OSと同期された最新のデバイス設定。</returns>
    public DeviceSettings GetSyncedSettings(string deviceId)
    {
        var currentState = _audioDeviceStateReader.ReadCurrentState(deviceId);
        if (currentState.HasValue)
        {
            return _deviceSettingsCache.AddOrUpdate(deviceId,
                // Addの場合 (デバイスがキャッシュにない): 新規作成
                (id) => _deviceSettingsFactory.CreateDefaultSettings(id),
                // Updateの場合 (デバイスがキャッシュにある): IsUserHiddenを維持しつつ更新
                (id, savedSetting) => savedSetting with
                {
                    Volume = Math.Clamp(currentState.Value.Volume, DeviceSettings.MinVolume, DeviceSettings.MaxVolume),
                    Pan = Math.Clamp(currentState.Value.Pan, DeviceSettings.MinPan, DeviceSettings.MaxPan)
                });
        }

        // OSから状態を読み取れなかった場合は、既存のキャッシュ値（または新規作成）をそのまま返す
        return GetDeviceSettings(deviceId);
    }

    #region IUserDevicePreferencesService Implementation

    /// <summary>
    /// 指定されたデバイスIDの設定をキャッシュから取得します。設定が存在しない場合は、ファクトリを使用して新しい設定を生成して返します。
    /// </summary>
    /// <param name="deviceId">設定を取得するデバイスのID。</param>
    /// <returns>デバイスの設定。</returns>
    public DeviceSettings GetDeviceSettings(string deviceId)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        return _deviceSettingsCache.GetOrAdd(deviceId, id => _deviceSettingsFactory.CreateDefaultSettings(id));
    }

    /// <summary>
    /// 指定されたデバイスIDの設定を更新または追加し、自動保存をトリガーします。
    /// </summary>
    /// <param name="deviceId">設定を更新するデバイスのID。</param>
    /// <param name="newSettings">新しい設定。</param>
    public void UpdateDeviceSetting(string deviceId, DeviceSettings newSettings)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        ArgumentNullException.ThrowIfNull(newSettings);

        _deviceSettingsCache[deviceId] = newSettings;
        _saveTrigger.OnNext(true); // 保存をトリガー
    }

    /// <summary>
    /// デバイスのユーザーによる非表示設定を更新し、自動保存をトリガーします。
    /// </summary>
    /// <param name="deviceId">設定を更新するデバイスのID。</param>
    /// <param name="isHidden">非表示にする場合は <c>true</c>、再表示する場合は <c>false</c>。</param>
    public void SetUserHiddenPreference(string deviceId, bool isHidden)
    {
        ArgumentException.ThrowIfNullOrEmpty(deviceId);
        var currentSettings = GetDeviceSettings(deviceId);
        if (currentSettings.IsUserHidden == isHidden) return;

        var newSettings = currentSettings with { IsUserHidden = isHidden };
        UpdateDeviceSetting(deviceId, newSettings); // こちらもUpdateDeviceSetting経由でトリガー
    }

    /// <summary>
    /// 非表示に設定されているすべてのデバイスIDのリストを取得します。
    /// </summary>
    /// <returns>非表示に設定されたデバイスIDのリスト。</returns>
    public List<string> GetHiddenDeviceIds()
    {
        return _deviceSettingsCache
            .Where(pair => pair.Value.IsUserHidden)
            .Select(pair => pair.Key)
            .ToList();
    }

    /// <summary>
    /// 現在のすべての設定を永続的なストレージに非同期で保存します。
    /// </summary>
    /// <returns>処理の完了を示す <see cref="Task"/>。</returns>
    public Task SaveAllSettingsAsync()
    {
        _logger.LogInformation("ユーザー設定をファイルに保存しています...");
        var settingsToSave = new Dictionary<string, DeviceSettings>(_deviceSettingsCache);
        return _settingsManager.SaveSettingsAsync(settingsToSave);
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// このサービスが使用するリソース（Rxサブスクリプション）を解放します。
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// マネージドリソースを解放します。
    /// </summary>
    /// <param name="disposing">解放するかどうかを示す値。</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        if (disposing)
        {
            _saveSubscription?.Dispose();
            _saveTrigger?.Dispose();
        }
        _isDisposed = true;
    }

    #endregion
}