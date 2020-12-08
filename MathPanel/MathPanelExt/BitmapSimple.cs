using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.IO;

namespace MathPanel
{
    public class BitmapSimple
    {
        public int width, height;
        public int[] map;
        /// <summary>
        /// 2-dimensional image
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="colors">repeated colors</param>
        public BitmapSimple(int w, int h, System.Drawing.Color[] colors)
        {
            /* The Color Structure represents colors in terms of alpha, red, green, and blue (ARGB) channels. 
            The color of each pixel is represented as a 32-bit number: 8 bits each for the alpha, red, green, and blue channels (ARGB). 
            Each of the four components is a number from 0 to 255, where 0 means no intensity and 255 represents full intensity. 
            The alpha component sets the color transparency: 0 is fully transparent, and 255 is fully opaque. 
            To define the alpha, red, green, or blue component of a color, use the A, R, G, or B property, respectively. 
            You can create a custom color using one of the FromArgb() methods.
            The ToArgb() method returns the 32-bit ARGB value of this Color structure.
            */
            width = w;
            height = h;
            map = new int[width * height];
            for (int i = 0; i < width * height; i++)
            {
                map[i] = colors[i % colors.Length].ToArgb();
            }
        }

        /// <summary>
        /// 2-dimensional image into file
        /// </summary>
        /// <param name="fname">file name</param>
        public void Save(string fname)
        {
            /*
            Creating an object of the Bitmap type, which is used for working with images defined by pixel data. Encapsulates a GDI + bitmap consisting of pixel data from the graphic image and drawing attributes. Creating a Graphics object that encapsulates the GDI+ drawing surface.
            The GDI+ interface is a General-purpose drawing model for .NET applications. In the .NET environment the GDI+ interface is used in several places, including when sending documents to a printer, displaying graphics in Windows applications, and rendering graphic elements on a web page.
            */
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);

            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int offset = 0;
            int stride = bmData.Stride;
            /*
            image.LockBits locks the Bitmap object in system memory so that pixels can be changed.
            Getting a pointer to the allocated memory block
            */
            IntPtr Scan0 = bmData.Scan0;

            int nOffset = stride - image.Width * 4;
            int k = 0;
            /*
            Next, in a double loop, we move through our map array, extracting from each element of the structure Color
            */
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int argb = map[k];
                    var cc = System.Drawing.Color.FromArgb(argb);
                    //Writing colors to memory, increasing the offset
                    Marshal.WriteByte(Scan0, offset, (byte)cc.B);
                    Marshal.WriteByte(Scan0, offset + 1, (byte)cc.G);
                    Marshal.WriteByte(Scan0, offset + 2, (byte)cc.R);
                    Marshal.WriteByte(Scan0, offset + 3, (byte)cc.A);
                    k++;
                    offset += 4;
                }
                offset += nOffset;
            }

            //then we unlock the memory, save it to a PNG file, and free up resources
            image.UnlockBits(bmData);

            g.Flush();
            if (File.Exists(fname)) File.Delete(fname);
            image.Save(fname, System.Drawing.Imaging.ImageFormat.Png);
            image.Dispose();
        }

        /// <summary>
        /// constructor: load 2-dimensional image from file
        /// </summary>
        /// <param name="fname">file name</param>
        //from file
        public BitmapSimple(string fname)
        {
            FromFile(fname);
        }

        /// <summary>
        /// 2-dimensional image from file
        /// </summary>
        /// <param name="fname">file name</param>
        //from file
        public void FromFile(string fname)
        {
            /* Creating an imageOrig object of the Image type (an abstract class that outputs Bitmap and Metafile) 
            directly from the file name (there is a necessary method in the class).
            Create an image object of the Bitmap type from this object and release the resource
            */
            Image imageOrig = new Bitmap(fname);
            Bitmap image = new Bitmap(imageOrig);
            imageOrig.Dispose();

            //Find the image size and allocate the required memory
            width = image.Width;
            height = image.Height;
            map = new int[width * height];

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
            BitmapData bmData = image.LockBits(new Rectangle(0, 0, image.Width, image.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            int offset = 0;
            int stride = bmData.Stride;
            //image.LockBits locks the Bitmap object in system memory so that pixels can be read/modified.
            //Getting a refference to the allocated memory block
            IntPtr Scan0 = bmData.Scan0;

            int bBl, bGr, bRd, bAl;
            int nOffset = stride - image.Width * 4;
            int k = 0;
            //Then in a double loop we read the pixel data from the image and copy it to our array map
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    bBl = Marshal.ReadByte(Scan0, offset);
                    bGr = Marshal.ReadByte(Scan0, offset + 1);
                    bRd = Marshal.ReadByte(Scan0, offset + 2);
                    bAl = Marshal.ReadByte(Scan0, offset + 3);
                    map[k++] = System.Drawing.Color.FromArgb(bAl, bRd, bGr, bBl).ToArgb();
                    offset += 4;
                }
                offset += nOffset;
            }
            //unlock the memory block to free up resources
            image.UnlockBits(bmData);
        }

        /// <summary>
        /// change pixel(s)
        /// </summary>
        public void Pixel(int x0, int y0, int alpha, int red, int green, int blue, int size_x = 1, int size_y = 1)
        {
            /*Changing the pixel at position x0, y0. To do this, use the parameters alpha, red, green, blue to form the color cc. 
            Find an element of the map array and assign it. 
            Not only one pixel can be changed, but also its neighbors in a rectangle with the size size_x, size_y (by default, they are equal to 1).
            */
            var cc = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
            int k;
            for (int y = y0; y < height && y < y0 + size_y; y++)
            {
                for (int x = x0; x < width && x < x0 + size_x; x++)
                {
                    k = y * width + x;
                    map[k] = cc;
                }
            }
        }

        /// <summary>
        /// 2-dimensional image with gradient (vertical or horizontal)
        /// </summary>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <param name="col1">color on top (left)</param>
        /// <param name="col2">color on bottom (right)</param>
        /// <param name="bVert">if true col1 on top, col2 on bottom, else col1 on left, col2 on right side</param>
        public BitmapSimple(int w, int h, System.Drawing.Color col1, System.Drawing.Color col2, bool bVert = true)
        {
            width = w;
            height = h;
            map = new int[width * height];
            int k = 0, alpha, red, green, blue;
            byte a1 = col1.A;
            byte r1 = col1.R;
            byte g1 = col1.G;
            byte b1 = col1.B;
            byte a2 = col2.A;
            byte r2 = col2.R;
            byte g2 = col2.G;
            byte b2 = col2.B;
            if (bVert)
            {
                //If the gradient is vertical, then everything is simple. For each line of the image, we calculate the ARGB pixel values as a linear relationship
                for (int j = 0; j < height; j++)
                {
                    alpha = a1 + ((a2 - a1) * j) / height;
                    red = r1 + ((r2 - r1) * j) / height;
                    green = g1 + ((g2 - g1) * j) / height;
                    blue = b1 + ((b2 - b1) * j) / height;
                    int argb = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                    for (int i = 0; i < width; i++)
                    {
                        //Write to the map array
                        map[k++] = argb;
                    }
                }
            }
            else
            {
                //If the gradient is horizontal, it is slightly more difficult. 
                //For each column of the image, we calculate the ARGB pixel values. 
                for (int j = 0; j < width; j++)
                {
                    alpha = a1 + ((a2 - a1) * j) / width;
                    red = r1 + ((r2 - r1) * j) / width;
                    green = g1 + ((g2 - g1) * j) / width;
                    blue = b1 + ((b2 - b1) * j) / width;
                    int argb = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                    //Forming a loop over the column elements, calculating the index of the element
                    for (int i = 0; i < height; i++)
                    {
                        map[i * width + j] = argb;
                    }
                }
            }
        }

        /// <summary>
        /// drop effect
        /// </summary>
        /// <param name="clr">drop color</param>
        /// <param name="x_0">horizontal position</param>
        /// <param name="y_0">vertical position</param>
        /// <param name="wi">drop width</param>
        /// <param name="he">drop height</param>
        /// <param name="dInitForce">drop initial force</param>
        public void Drop(System.Drawing.Color clr, int x_0, int y_0, int wi, int he, double dInitForce, bool bLinear = true)
        {
            //it simulates a drop of paint
            if (wi <= 0 || he <= 0 || x_0 < 0 || x_0 >= width || y_0 < 0 || y_0 >= height) return;
            if (dInitForce <= 0 || dInitForce > 1) dInitForce = 1;
            int k, i, j, x, y;
            double dEllipse = (wi * 1.0) / he;
            double dRadiusMax = Math.Sqrt(wi * wi + he * he * dEllipse * dEllipse) / 2;
            for (i = 0; i < wi; i++)
            {
                for (j = 0; j < he; j++)
                {
                    x = x_0 - wi / 2 + i;
                    y = y_0 - he / 2 + j;
                    if (x < 0 || x >= width || y < 0 || y >= height) { }
                    else
                    {
                        double dRadius = Math.Sqrt((x - x_0) * (x - x_0) + (y - y_0) * (y - y_0) * dEllipse * dEllipse);
                        if (dRadiusMax <= dRadius) continue;
                        k = y * width + x;
                        int argb = map[k];
                        var clr_orig = System.Drawing.Color.FromArgb(argb);
                        int argb_new = Merge(clr_orig, clr, dInitForce * Math.Pow((dRadiusMax - dRadius) / dRadiusMax, bLinear ? 1 : 2));
                        map[k] = argb_new;
                    }
                }
            }
        }

        /// <summary>
        /// set channel value in range 0-255
        /// </summary>
        /// <param name="cc">channel value</param>
        public static int SafeColor(int cc)
        {
            //Don't allow the value to go out of range 0-255
            if (cc < 0) return 0;
            if (cc > 255) return 255;
            return cc;
        }

        /// <summary>
        /// merge 2 colors
        /// </summary>
        /// <param name="clr_orig">main color</param>
        /// <param name="clr">drop color</param>
        /// <param name="dInitForce">drop initial force</param>
        public static int Merge(System.Drawing.Color clr_orig, System.Drawing.Color clr, double dInitForce)
        {
            //We sum up two values – the new color with the dInitForce weight and the original color 
            //with the weight(1 - dInitForce).And so on all channels

            int alpha, red, green, blue;
            alpha = clr_orig.A;
            red = SafeColor((int)(dInitForce * clr.R + clr_orig.R * (1 - dInitForce)));
            green = SafeColor((int)(dInitForce * clr.G + clr_orig.G * (1 - dInitForce)));
            blue = SafeColor((int)(dInitForce * clr.B + clr_orig.B * (1 - dInitForce)));
            return System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
        }

        /// <summary>
        /// add noise
        /// </summary>
        /// <param name="nNoise">number of variations</param>
        /// <param name="iNoiceStrenth">strength of variation</param>
        public void Randomize(int nNoise, int iNoiceStrenth = 2)
        {
            if (nNoise <= 0 || iNoiceStrenth < 1) return;
            int k, alpha, red, green, blue, i;
            //generate noise
            //Using the random number generator
            Random rnd = new Random();
            for (i = 0; i < nNoise; i++)
            {
                //in the loop, we find a random element in the array by the number of variations 
                k = rnd.Next(0, width * height);
                int argb = map[k];
                var clr = System.Drawing.Color.FromArgb(argb);
                //Then we slightly change the values in the channels
                alpha = clr.A;
                red = SafeColor(clr.R + rnd.Next(0, iNoiceStrenth) - iNoiceStrenth / 2);
                green = SafeColor(clr.G + rnd.Next(0, iNoiceStrenth) - iNoiceStrenth / 2);
                blue = SafeColor(clr.B + rnd.Next(0, iNoiceStrenth) - iNoiceStrenth / 2);
                //and save pixels
                map[k] = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
            }
        }

        //transformations
        /// <summary>
        /// to gray
        /// </summary>
        public void Gray()
        {
            int alpha, red, green, blue;
            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int argb = map[k];
                    var cc = System.Drawing.Color.FromArgb(argb);
                    alpha = cc.A;
                    //Converting the image to grayscale. Find the average value for the channels 
                    red = (cc.R + cc.G + cc.B) / 3;
                    green = red;
                    blue = red;
                    map[k++] = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                }
            }
        }

        /// <summary>
        /// to black and white
        /// <param name="threshold">threshold</param>
        /// </summary>
        public void BlackWhite(int threshold = 127)
        {
            int alpha, red, green, blue;
            int k = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int argb = map[k];
                    var cc = System.Drawing.Color.FromArgb(argb);
                    alpha = cc.A;
                    //Converting the image to black and white. Find the average value for the channels and compare it with the threshold
                    red = (cc.R + cc.G + cc.B) / 3;
                    red = red > threshold ? 255 : 0;
                    green = red;
                    blue = red;
                    map[k++] = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                }
            }
        }

        /// <summary>
        /// Smoothing
        /// <param name="num">number of repeats</param>
        /// <param name="step">step into neighborhood</param>
        /// </summary>
        public void Smooth(int num = 1, int step = 1)
        {
            //Reducing the image contrast. Allocating memory for a temporary array
            int[] map_2 = new int[width * height];
            int alpha, red, green, blue;
            int k, m, n, x_2, y_2;
            for (int l = 0; l < num; l++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        k = y * width + x;
                        int argb = map[k];
                        map_2[k] = argb;
                        alpha = 0;
                        red = 0;
                        green = 0;
                        blue = 0;
                        n = 0;
                        //Find the neighbors of the current point , accumulate values by channels
                        for (int i = -step; i <= step; i++)
                        {
                            y_2 = y + i;
                            if (y_2 < 0 || y_2 >= height) continue;
                            for (int j = -step; j <= step; j++)
                            {
                                x_2 = x + j;
                                if (x_2 < 0 || x_2 >= width) continue;
                                n++;
                                m = y_2 * width + x_2;
                                argb = map[m];
                                var cc = System.Drawing.Color.FromArgb(argb);
                                alpha += cc.A;
                                red += cc.R;
                                green += cc.G;
                                blue += cc.B;
                            }
                        }
                        //n – number of neighbors found
                        if (n > 0)
                        {
                            map_2[k] = System.Drawing.Color.FromArgb(alpha / n, red / n, green / n, blue / n).ToArgb();
                        }
                    }
                }
                //Finally, we copy from the temporary array to the main one
                for (int y = 0; y < height * width; y++) map[y] = map_2[y];
            }
        }

        /// <summary>
        /// filter
        /// <param name="fil">list of rules (x, y, value)</param>
        /// <param name="num">number of repeats</param>
        /// </summary>
        public void Filter(List<Tuple<int, int, double>> fil, int num = 1)
        {
            //The first parameter contains a list of filter rules, the list elements are tuples of 3 values: 
            //1st - x-axis offset, 2nd - y-axis offset, 3rd - weight

            //Allocating memory for a temporary array
            int[] map_2 = new int[width * height];
            int alpha, red, green, blue;
            int k, m, n, x_2, y_2;
            for (int l = 0; l < num; l++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        k = y * width + x;
                        int argb = map[k];
                        map_2[k] = argb;
                        alpha = 255;
                        red = 0;
                        green = 0;
                        blue = 0;
                        n = 0;
                        //For each point in the source image, we find neighbors and accumulate values in channels
                        foreach (var tup in fil)
                        {
                            y_2 = y + tup.Item2;
                            if (y_2 < 0 || y_2 >= height) continue;
                            x_2 = x + tup.Item1;
                            if (x_2 < 0 || x_2 >= width) continue;
                            n++;
                            m = y_2 * width + x_2;
                            argb = map[m];
                            var cc = System.Drawing.Color.FromArgb(argb);
                            //alpha += (int)(cc.A * tup.Item3);
                            red += (int)(cc.R * tup.Item3);
                            green += (int)(cc.G * tup.Item3);
                            blue += (int)(cc.B * tup.Item3);
                        }
                        if (n > 0)
                        {   //??negative
                            map_2[k] = System.Drawing.Color.FromArgb(SafeColor(alpha),
                                SafeColor(red), SafeColor(green), SafeColor(blue)).ToArgb();
                        }

                    }
                }
                //Finally, we copy from the temporary array to the main one
                for (int y = 0; y < height * width; y++) map[y] = map_2[y];
            }
        }

        /// <summary>
        /// Sobel filter to detect edges
        /// if background is white , then early detection, else delayed.
        /// <param name="bHoriz">if true horizontal, else vertical</param>
        /// <param name="num">number of repeats</param>
        /// </summary>
        public void Sobel(bool bHoriz = true, int num = 1)
        {
            List<Tuple<int, int, double>> fil = new List<Tuple<int, int, double>>();
            Tuple<int, int, double> m00, m01, m02, m10, m11, m12, m20, m21, m22;
            if (bHoriz)
            {
                m00 = new Tuple<int, int, double>(-1, -1, 1);
                m01 = new Tuple<int, int, double>(-1, 0, 2);
                m02 = new Tuple<int, int, double>(-1, 1, 1);

                m10 = new Tuple<int, int, double>(0, -1, 0);
                m11 = new Tuple<int, int, double>(0, 0, 0);
                m12 = new Tuple<int, int, double>(0, 1, 0);

                m20 = new Tuple<int, int, double>(1, -1, -1);
                m21 = new Tuple<int, int, double>(1, 0, -2);
                m22 = new Tuple<int, int, double>(1, 1, -1);
            }
            else
            {
                m00 = new Tuple<int, int, double>(-1, -1, 1);
                m01 = new Tuple<int, int, double>(-1, 0, 0);
                m02 = new Tuple<int, int, double>(-1, 1, -1);

                m10 = new Tuple<int, int, double>(0, -1, 2);
                m11 = new Tuple<int, int, double>(0, 0, 0);
                m12 = new Tuple<int, int, double>(0, 1, -2);

                m20 = new Tuple<int, int, double>(1, -1, 1);
                m21 = new Tuple<int, int, double>(1, 0, 0);
                m22 = new Tuple<int, int, double>(1, 1, -1);
            }

            fil.Add(m00);
            fil.Add(m01);
            fil.Add(m02);

            fil.Add(m10);
            fil.Add(m11);
            fil.Add(m12);

            fil.Add(m20);
            fil.Add(m21);
            fil.Add(m22);

            Filter(fil, num);
        }

        /// <summary>
        /// change alpha value
        /// <param name="x0">horizontal position</param>
        /// <param name="y0">vertical position</param>
        /// <param name="alpha">new alpha value</param>
        /// <param name="size_x">horizontal size</param>
        /// <param name="size_y">vertical size</param>
        /// </summary>
        public void Alpha(int x0, int y0, int alpha, int size_x = 1, int size_y = 1)
        {
            int k, red, green, blue;
            for (int y = y0; y < height && y < y0 + size_y; y++)
            {
                for (int x = x0; x < width && x < x0 + size_x; x++)
                {
                    k = y * width + x;
                    var cc = System.Drawing.Color.FromArgb(map[k]);
                    red = cc.R;
                    green = cc.G;
                    blue = cc.B;
                    var cc2 = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                    map[k] = cc2;
                }
            }
        }

        /// <summary>
        /// put bitmap based on alpha
        /// <param name="bm">bitmap is being copied</param>
        /// </summary>
        public void Put(BitmapSimple bm)
        {
            //bitmap is being copied should be the same size as current one
            if (width != bm.width || height != bm.height) return;
            int alpha, red, green, blue, k;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    k = y * width + x;
                    var clr = System.Drawing.Color.FromArgb(bm.map[k]);
                    if (clr.A == 255) map[k] = bm.map[k]; //замещение
                    else
                    {   //add
                        var clr_orig = System.Drawing.Color.FromArgb(map[k]);
                        alpha = clr_orig.A;
                        double dInitForce = (1.0 * clr.A) / 255;
                        red = SafeColor((int)(dInitForce * clr.R + clr_orig.R * (1 - dInitForce)));
                        green = SafeColor((int)(dInitForce * clr.G + clr_orig.G * (1 - dInitForce)));
                        blue = SafeColor((int)(dInitForce * clr.B + clr_orig.B * (1 - dInitForce)));
                        var cc2 = System.Drawing.Color.FromArgb(alpha, red, green, blue).ToArgb();
                        map[k] = cc2;
                    }
                }
            }
        }

        /// <summary>
        /// generate the hash of BitmapSimple - A hash function is any function 
        /// that can be used to map data of arbitrary size to fixed-size values. 
        /// The values returned by a hash function are called hash values, hash codes, digests, or simply hashes.
        /// </summary>
        /// <param name="nx">divide BitmapSimple by 'nx' horizontally</param>
        /// <param name="ny">divide BitmapSimple by 'ny' vertically</param>
        /// <param name="palette">the array of colors to match</param>
        /// <param name="paletteCode">the string to codify our palette colors</param>
        /// <param name="bUpdate">if true, modify itself</param>
        public string Hash(int nx, int ny, Color[] palette, string paletteCode, bool bUpdate = false)
        {
            StringBuilder s = new StringBuilder();//to accelrate
            int k, n, alpha, red, green, blue, iMin, index;
            //division parameters
            int d_x = width / nx;   //rectangle width
            int d_y = height / ny;  //rectangle height
            //divide all area into small rectangles
            //loop through our map
            for (int i = 0; i < nx; i++)
            {
                for (int j = 0; j < ny; j++)
                {
                    //find an average color in the small rectangle
                    //init counters
                    alpha = 0;
                    red = 0;
                    green = 0;
                    blue = 0;
                    n = 0;
                    //go through rectangle pixels, accumulate in channels
                    for (int y = j * d_y; y < (j + 1) * d_y; y++)
                    {
                        for (int x = i * d_x; x < (i + 1) * d_x; x++)
                        {
                            k = y * width + x;
                            var cc = System.Drawing.Color.FromArgb(map[k]);
                            alpha += cc.A;
                            red += cc.R;
                            green += cc.G;
                            blue += cc.B;
                            n++;
                        }
                    }
                    //get average
                    alpha /= n;
                    red /= n;
                    green /= n;
                    blue /= n;

                    //find a nearest color in the palette
                    iMin = int.MaxValue;
                    index = 0;
                    for (int m = 0; m < palette.Length; m++)
                    {
                        var clr = palette[m];
                        int diff = Math.Abs(clr.A - alpha) + Math.Abs(clr.R - red) +
                            Math.Abs(clr.G - green) + Math.Abs(clr.B - blue);
                        if (diff < iMin)
                        {
                            iMin = diff;
                            index = m;
                        }
                    }

                    //add to hash a letter with found index
                    s.Append(paletteCode.Substring(index, 1));

                    if (bUpdate)
                    {   //set pixels inside the small rectangle to a new selected color
                        var best = palette[index].ToArgb();
                        for (int y = j * d_y; y < (j + 1) * d_y; y++)
                        {
                            for (int x = i * d_x; x < (i + 1) * d_x; x++)
                            {
                                k = y * width + x;
                                map[k] = best;
                            }
                        }
                    }
                }
            }

            return s.ToString();
        }
    }
}
