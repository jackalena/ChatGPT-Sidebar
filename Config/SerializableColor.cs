using System.Windows.Media;

namespace ChatGPTSidebar.Config;

public class SerializableColor
{
    public byte A;
    public byte R;
    public byte G;
    public byte B;

    public SerializableColor()
    {
        A = 255;
        R = 0;
        G = 0;
        B = 0;
    }

    public SerializableColor(byte r, byte g, byte b)
    {
        A = 255;
        R = r;
        G = g;
        B = b;
    }

    public SerializableColor(byte a, byte r, byte g, byte b)
    {
        A = a;
        R = r;
        G = g;
        B = b;
    }

    public SerializableColor(double a, double r, double g, double b)
    {
        A = (byte) Math.Min(Math.Max(a * 255, 0), 255);
        R = (byte) Math.Min(Math.Max(r * 255, 0), 255);
        G = (byte) Math.Min(Math.Max(g * 255, 0), 255);
        B = (byte) Math.Min(Math.Max(b * 255, 0), 255);
    }

    public static Color GetColor(SerializableColor c)
    {
        return Color.FromArgb(c.A, c.R, c.G, c.B);
    }

    public static SerializableColor FromColor(Color c)
    {
        return new(c.A, c.R, c.G, c.B);
    }

    public static implicit operator Color(SerializableColor c)
    {
        return GetColor(c);
    }

    public static explicit operator SerializableColor(Color c)
    {
        return FromColor(c);
    }
}