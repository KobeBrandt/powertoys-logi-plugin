namespace Loupedeck.WindowsPowerToysPlugin.Helpers;

public static class BitmapHelper
{
    private static readonly BitmapColor LabelColor = new(235, 235, 235);

    // Draws the icon centered on the button. On keypad buttons (Creative Console, Loupedeck) the label is drawn
    // under the icon. The Actions Ring has no predefined button size, so it never gets Width90 or Width116
    // and shows the icon only.
    public static BitmapImage MakeBitmapImage(String path, PluginImageSize imageSize, String label = null)
    {
        try
        {
            using var builder = new BitmapBuilder(imageSize);
            var cw = builder.Width;
            var ch = builder.Height;

            var showLabel = !String.IsNullOrEmpty(label) && imageSize is PluginImageSize.Width90 or PluginImageSize.Width116;
            if (!showLabel)
            {
                var size = (Int32)(Math.Min(cw, ch) * 0.66);
                builder.DrawImage(EmbeddedResources.ReadImage(path), (cw - size) / 2, (ch - size) / 2, size, size);
                return builder.ToImage();
            }

            var iconSize = (Int32)(Math.Min(cw, ch) * 0.48);
            var iconY = (Int32)(ch * 0.12);
            builder.DrawImage(EmbeddedResources.ReadImage(path), (cw - iconSize) / 2, iconY, iconSize, iconSize);

            var labelY = iconY + iconSize + (Int32)(ch * 0.04);
            var fontSize = Math.Max(9, ch / 8);
            builder.DrawText(label, 2, labelY, cw - 4, ch - labelY - 2, LabelColor, fontSize, fontSize + 1);
            return builder.ToImage();
        }
        catch (Exception e)
        {
            PluginLog.Error(e, "Failed to read image");
            return null;
        }
    }
}
