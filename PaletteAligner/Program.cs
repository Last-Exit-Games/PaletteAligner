using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Cryptography.X509Certificates;

internal class Program
{
	private static void Main(string[] args)
	{
		new Program();

	}

	private Dictionary<Color, Color> SwapColors = new Dictionary<Color, Color>();
	private List<Color> palette = new List<Color>();
	public Color GetClosestColor(Color c)
	{
		if(palette.Contains(c))
			return c;
		if (SwapColors.ContainsKey(c))
			return SwapColors[c];
		double minDistance = double.MaxValue;
		Color closestColor = Color.Black;
		foreach (var color in palette)
		{
			double distance = Math.Sqrt(Math.Pow(c.R - color.R, 2) + Math.Pow(c.G - color.G, 2) + Math.Pow(c.B - color.B, 2));
			if (distance < minDistance)
			{
				minDistance = distance;
				closestColor = color;
			}
		}
		SwapColors.Add(c, closestColor);
		return closestColor;
	}
	public Program()
	{
		string PaletteDirectory = @"E:\Source\PaletteAligner\Art\Palettes";
		string ArtDirectory = @"E:\Source\PaletteAligner\Art\Original";
		string FinalDirectory = @"E:\Source\PaletteAligner\Art\Final";
		foreach (var file in Directory.GetFiles(PaletteDirectory, "*.png", SearchOption.AllDirectories))
		{
			var myBitmap = new Bitmap(file);
			int width = myBitmap.Width;
			int height = myBitmap.Height;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var color = myBitmap.GetPixel(x, y);
					if (color.A == 0)
						continue;
					if (palette.Contains(color))
						continue;
					palette.Add(color);
				}
			}
		}
		foreach (var file in Directory.GetFiles(FinalDirectory, "*.png", SearchOption.AllDirectories))
		{
			File.Delete(file);
		}
		foreach (var file in Directory.GetFiles(ArtDirectory, "*.png", SearchOption.AllDirectories))
		{
			string finalFilename = Path.Combine(FinalDirectory, Path.GetFileName(file));
			if (File.Exists(finalFilename))
			{
				finalFilename = Path.Combine(FinalDirectory, Path.GetFileNameWithoutExtension(file) + Guid.NewGuid().ToString().Substring(0, 4) + Path.GetExtension(file));
			}
			var myBitmap = new Bitmap(file);
			var newBitmap = myBitmap.Clone(new Rectangle(0, 0, myBitmap.Width, myBitmap.Height), PixelFormat.Format32bppArgb);
			int width = myBitmap.Width;
			int height = myBitmap.Height;
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					var color = myBitmap.GetPixel(x, y);
					if (color.A == 0)
						continue;
					if (palette.Contains(color))
						continue;
					newBitmap.SetPixel(x, y, GetClosestColor(color));
				}
			}
			newBitmap.Save(finalFilename);
		}
	}
}