// Core/Utils/PanCalculator.cs
// パン値と左右チャンネルの音量スカラーを相互に変換する計算ロジックを提供します。
namespace OmniPans.Core.Utils;

/// <summary>
/// パン値と左右チャンネルの音量スカラーを相互に変換する計算ロジックを提供する静的クラスです。
/// </summary>
public static class PanCalculator
{
    private const float ScalarComparisonTolerance = 0.00001f;

    /// <summary>
    /// パン値 (-100 to 100) を左右チャンネルの音量スカラー (0.0-1.0) に変換します。
    /// </summary>
    /// <param name="panValue">変換するパン値。</param>
    /// <returns>左チャンネルと右チャンネルの音量スカラーのタプル。</returns>
    public static (float LeftScalar, float RightScalar) ConvertPanToScalars(float panValue)
    {
        panValue = Math.Clamp(panValue, -100f, 100f);
        float leftScalar = panValue <= 0f ? 1.0f : (100.0f - panValue) / 100.0f;
        float rightScalar = panValue <= 0f ? (panValue + 100.0f) / 100.0f : 1.0f;
        return (leftScalar, rightScalar);
    }

    /// <summary>
    /// 左右チャンネルの音量スカラー (0.0-1.0) からパン値 (-100 to 100) に変換します。
    /// </summary>
    /// <param name="leftScalar">左チャンネルの音量スカラー。</param>
    /// <param name="rightScalar">右チャンネルの音量スカラー。</param>
    /// <returns>計算されたパン値。</returns>
    public static double ConvertScalarsToPan(float leftScalar, float rightScalar)
    {
        static bool IsScalarEffectivelyZero(float scalar) => Math.Abs(scalar) < ScalarComparisonTolerance;
        return (IsScalarEffectivelyZero(leftScalar), IsScalarEffectivelyZero(rightScalar)) switch
        {
            (true, true) => 0.0,
            (true, false) => 100.0,
            (false, true) => -100.0,
            _ => CalculateDetailedPan(leftScalar, rightScalar)
        };
    }

    // 詳細なパン計算を行います。
    private static double CalculateDetailedPan(float leftScalar, float rightScalar)
    {
        double panValue = leftScalar >= rightScalar
            ? -100.0 * (1.0 - (double)rightScalar / leftScalar)
            : 100.0 * (1.0 - (double)leftScalar / rightScalar);
        return Math.Clamp(panValue, -100.0, 100.0);
    }
}