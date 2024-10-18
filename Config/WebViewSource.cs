using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ChatGPTSidebar.Config;

public class WebViewSource
{
    private const float MouseOverLighten = 0.03f;
    private const float PressedLighten = 0.07f;
    private const float DisabledDarken = 0.2f;

    public string DisplayName { get; }
    public Uri Uri { get; }
    public Color BackgroundColor { get; }
    public Color TextColor { get; }

    public Color MouseOverColor => mouseOverColor ??= Lighten(BackgroundColor, MouseOverLighten);
    public Color PressedColor => pressedColor ??= Lighten(BackgroundColor, PressedLighten);
    public Color DisabledColor => disabledColor ??= Lighten(BackgroundColor, DisabledDarken);
    public bool IsCustom { get; }

    private Color? mouseOverColor;
    private Color? pressedColor;
    private Color? disabledColor;

    public WebViewSource(string uri, string name, Color backgroundColor, Color textColor, bool isCustom = false)
        : this(new Uri(uri), name, backgroundColor, textColor) { }

    public WebViewSource(Uri uri, string name, Color backgroundColor, Color textColor, bool isCustom = false)
    {
        Uri = uri;
        DisplayName = name;
        BackgroundColor = backgroundColor;
        TextColor = textColor;
        IsCustom = isCustom;
    }

    private static Color Lighten(Color color, float amount)
    {
        float r = MathF.Min(1, color.ScR + amount);
        float g = MathF.Min(1, color.ScG + amount);
        float b = MathF.Min(1, color.ScB + amount);

        return Color.FromScRgb(1f, r, g, b);
    }
}