using System.Net;
using System.Text;

namespace ReclaimCS.Shared.Menus;

public static class CenterMenuHtml
{
    public const int DefaultPageSize = 5;

    public static void AppendHeader(StringBuilder builder, string title, string meta, string color)
    {
        builder.Append("<font class='fontSize-m' color='").Append(color).Append("'>[ ")
            .Append(Encode(title))
            .Append(" ]</font> <font color='#ffd166'>")
            .Append(Encode(meta))
            .Append("</font><br>");
    }

    public static void AppendStatus(StringBuilder builder, string status, int maxLength = 58)
    {
        if (string.IsNullOrWhiteSpace(status))
            return;

        builder.Append("<font class='fontSize-s' color='#ffd166'>")
            .Append(Encode(Compact(status, maxLength)))
            .Append("</font><br>");
    }

    public static void AppendPagedList(StringBuilder builder, int count, int selectedIndex, Func<int, string> render, int pageSize = DefaultPageSize)
    {
        var start = Math.Clamp(selectedIndex - 2, 0, Math.Max(0, count - pageSize));
        var end = Math.Min(count, start + pageSize);

        for (var i = start; i < end; i++)
            AppendLine(builder, i == selectedIndex, render(i), "#f3f6ff");
    }

    public static void AppendLine(StringBuilder builder, bool selected, string text, string color)
    {
        var lineColor = selected ? "#6dff8f" : color;
        var pointer = selected ? "<font color='#ff5f66'>&#9658;</font> " : "";
        var suffix = selected ? " <font color='#ff5f66'>&#9668;</font>" : "";

        builder.Append(pointer)
            .Append("<font class='fontSize-m' color='").Append(lineColor).Append("'>")
            .Append(Encode(text))
            .Append("</font>")
            .Append(suffix)
            .Append("<br>");
    }

    public static void AppendControls(StringBuilder builder, string useText, string tabText)
    {
        builder.Append("<br><font class='fontSize-s' color='#ff5f66'>W/S</font> ")
            .Append("<font class='fontSize-s' color='#ffffff'>move</font> ")
            .Append("<font class='fontSize-s' color='#ff5f66'>")
            .Append(Encode(useText))
            .Append("</font> ")
            .Append("<font class='fontSize-s' color='#ffd166'>")
            .Append(Encode(tabText))
            .Append("</font>");
    }

    public static string Notice(string title, string message, string color)
    {
        return $"<font color='{color}'><b>{Encode(title)}</b></font><br><font color='#ffffff'>{Encode(message)}</font>";
    }

    public static string Compact(string value, int maxLength)
    {
        value = string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
        if (value.Length <= maxLength)
            return value;

        return maxLength <= 3 ? value[..maxLength] : value[..(maxLength - 3)] + "...";
    }

    public static string Encode(string value)
    {
        return WebUtility.HtmlEncode(value);
    }
}
