//2020, Andrei Borziak
using System;
using MathPanel;

namespace MathPanelExt
{
	/// <summary>
	/// class for solving the quadratic equation
	/// </summary>

	public class QuadroEqu
	{
		public QuadroEqu()
		{
		}
		/// <summary>
		/// calculate roots of quadratic equation
		/// </summary>
		public static void Solve(double a, double b, double c, out double x1, out double x2)
		{
			double discr = Math.Sqrt(b * b - 4 * a * c);
			x1 = (-b - discr) / (2 * a);
			x2 = (-b + discr) / (2 * a);
		}

		/// <summary>
		/// preparing data for a quadratic equation
		/// </summary>
		public static string DrawRange(double a, double b, double c, double x0, double x1, int n)
		{
			string s = "{{\"x\":{0},\"y\":{1}}}";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			double step = (x1 - x0) / n;
			double x = x0;
			for (int i = 0; i <= n; i++, x += step)
			{
				var y = x * (a * x + b) + c;
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y));
			}
			return sb.ToString();
		}
		/// <summary>
		/// preparing data for an ellipse
		/// </summary>
		public static string DrawEllipse(double a, double b, double x0, double y0, double fi0, double fi1, int n)
		{
			string s = "{{\"x\":{0},\"y\":{1}}}";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			double step = (fi1 - fi0) / n;
			double fi = fi0;
			for (int i = 0; i <= n; i++, fi += step)
			{
				var x = x0 + a * Math.Cos(fi);
				var y = y0 + b * Math.Sin(fi);
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y));
			}
			return sb.ToString();
		}
		/// <summary>
		/// preparing data for the line
		/// </summary>
		public static string DrawLine(double x0, double y0, double x1, double y1)
		{
			string s = "{{\"x\":{0},\"y\":{1}}}";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i <= 1; i++)
			{
				var x = i == 0 ? x0 : x1;
				var y = i == 0 ? y0 : y1;
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y));
			}
			return sb.ToString();
		}

		/// <summary>
		/// calculate a level 1 Bezier point for a given step
		/// </summary>
		static double Bezier1(double x0, double x1, double step)
		{
			return x0 + (x1 - x0) * step;
		}
		/// <summary>
		/// calculate a level 2 Bezier point for a given step
		/// </summary>
		static double Bezier2(double x0, double x1, double x2, double step)
		{
			double p0 = Bezier1(x0, x1, step);
			double p1 = Bezier1(x1, x2, step);
			return Bezier1(p0, p1, step);
		}
		/// <summary>
		/// calculate a level 3 Bezier point for a given step
		/// </summary>
		static double Bezier3(double x0, double x1, double x2, double x3, double step)
		{
			double p0 = Bezier2(x0, x1, x1, step);
			double p1 = Bezier2(x1, x2, x3, step);
			return Bezier1(p0, p1, step);
		}

		/// <summary>
		/// level 2 Bezier curve data preparation
		/// </summary>
		public static string DrawBezier2(double x0, double y0, double x1, double y1, double x2, double y2, int n)
		{
			string s = "{{\"x\":{0},\"y\":{1}}}";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i <= n; i++)
			{
				double step = ((double)i) / n;
				var x = Bezier2(x0, x1, x2, step);
				var y = Bezier2(y0, y1, y2, step);
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y));
			}
			return sb.ToString();
		}

		/// <summary>
		/// level 3 Bezier curve data preparation
		/// </summary>
		public static string DrawBezier3(double x0, double y0, double x1, double y1, double x2, double y2, double x3, double y3, int n)
		{
			string s = "{{\"x\":{0},\"y\":{1}}}";

			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for (int i = 0; i <= n; i++)
			{
				double step = ((double)i) / n;
				var x = Bezier3(x0, x1, x2, x3, step);
				var y = Bezier3(y0, y1, y2, y3, step);
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y));
			}
			return sb.ToString();
		}
		static readonly char[] cc = new char[2];
		static string Byte2Hex(byte b)
		{
			byte b1 = ((byte)(b >> 4));
			cc[0] = (char)(b1 > 9 ? b1 + 0x37 + 0x20 : b1 + 0x30);

			byte b2 = ((byte)(b & 0x0F));
			cc[1] = (char)(b2 > 9 ? b2 + 0x37 + 0x20 : b2 + 0x30);

			return new string(cc);
		}
		public static string ColorHtml(System.Drawing.Color color)
		{
			return "#" + string.Format("{0}{1}{2}", Byte2Hex(color.R), Byte2Hex(color.G), Byte2Hex(color.B));
		}

		/// <summary>
		/// fills in the colors of the table cell of type bitmap
		/// </summary>
		public static string DrawBitmap(int rows, int cols, System.Drawing.Color[] clrs,
			int xShift = 0, int yShift = 0, bool bFromBottom = true)
		{
			string s = "{{\"x\":{0},\"y\":{1},\"clr\":\"{2}\"}}";
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			for (int i = 0; i < clrs.Length && i < rows * cols; i++)
			{
				var x = i % cols + xShift;
				var y = bFromBottom ? (i / cols + yShift) : (rows - 1 - i / cols + yShift);
				if (i != 0) sb.Append(",");
				sb.AppendFormat(s, Dynamo.D2S(x), Dynamo.D2S(y), ColorHtml(clrs[i]));
			}
			return sb.ToString();
		}

	}
}