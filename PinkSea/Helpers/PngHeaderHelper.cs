namespace PinkSea.Helpers;

/// <summary>
/// Helper classes for the png header.
/// </summary>
public static class PngHeaderHelper
{
    /// <summary>
    /// Gets the dimension of the PNG file.
    /// </summary>
    /// <param name="pngData">The PNG data as a byte array.</param>
    /// <returns>The size of the PNG.</returns>
    /// <exception cref="ArgumentException">Thrown when we don't have enough data to read the header.</exception>
    /// <exception cref="InvalidDataException">Thrown when the data passed is invalid.</exception>
    public static (int width, int height) GetPngDimensions(byte[] pngData)
    {
        const int pngHeaderSize = 24;

        // Ensure the byte array is large enough to contain the PNG signature and IHDR chunk
        if (pngData == null || pngData.Length < pngHeaderSize)
        {
            throw new ArgumentException("The provided data is too small to be a valid PNG.");
        }

        // Verify PNG signature (first 8 bytes)
        if (pngData[0] != 0x89 || pngData[1] != 0x50 || pngData[2] != 0x4E || pngData[3] != 0x47 ||
            pngData[4] != 0x0D || pngData[5] != 0x0A || pngData[6] != 0x1A || pngData[7] != 0x0A)
        {
            throw new InvalidDataException("Not a valid PNG file.");
        }

        // Read width and height from IHDR chunk (starts at byte 16)
        var width = (pngData[16] << 24) | (pngData[17] << 16) | (pngData[18] << 8) | pngData[19];
        var height = (pngData[20] << 24) | (pngData[21] << 16) | (pngData[22] << 8) | pngData[23];

        return (width, height);
    }

    /// <summary>
    /// Validates the PNG dimensions for the oekaki.
    /// </summary>
    /// <param name="data">The data.</param>
    /// <returns>Whether they're valid.</returns>
    public static bool ValidateDimensionsForOekaki(byte[] data)
    {
        const int maxWidth = 800;
        const int maxHeight = 800;
        try
        {
            var (width, height) = GetPngDimensions(data);
            return width <= maxWidth
                   && height <= maxHeight;
        }
        catch
        {
            return false;
        }
    }
}