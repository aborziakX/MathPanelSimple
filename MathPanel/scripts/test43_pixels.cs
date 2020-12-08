//test43_pixels
/* Create a green bitmap, draw a red square on it, and save it. 
 * Creating a black bitmap, setting alpha=200 (almost opaque) in the first half, and 100 (semi – transparent) in the second half. 
 * We put the 2nd bitmap on the first one and save it. 
*/
using MathPanel;
//using MathPanelExt;
using System.Net.Sockets;
using System;
using System.Collections.Generic;

///assemblies to use
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        public void Execute()
        {
            Dynamo.Console("test43_pixels");
            //the path to the images folder  
            string sDir = @"c:\temp\";

            string[] fnames = { "red" };
            System.Drawing.Color[] col_green = { System.Drawing.Color.Green };
            System.Drawing.Color[] col_black = { System.Drawing.Color.Black };

            for (int i = 0; i < fnames.Length; i++)
            {
                var fn = fnames[i];
                //Create a green bitmap
                var bm = new BitmapSimple(800, 600, col_green);
                //draw a red square on it
                DateTime dt1 = DateTime.Now;
                bm.Pixel(300, 200, 255, 255, 0, 0, 200, 200);
                var fn_2 = sDir + fn + "_pix.png";
                //and save it
                bm.Save(fn_2);
                Dynamo.SetBitmapImage(fn_2);
                DateTime dt2 = DateTime.Now;
                TimeSpan diff = dt2 - dt1;
                int ms = (int)diff.TotalMilliseconds;
                //display time span
                Dynamo.Console("ms=" + ms);

                //Creating a black bitmap
                var bm2 = new BitmapSimple(800, 600, col_black);
                //setting alpha=200 (almost opaque) in the first half
                bm2.Alpha(0, 0, 200, 400, 600);
                //and 100 (semi – transparent) in the second half
                bm2.Alpha(400, 0, 100, 400, 600);
                //We put the 2nd bitmap on the first one
                bm.Put(bm2);
                //and save it
                fn_2 = sDir + fn + "_pix_alfa.png";
                bm.Save(fn_2);

                System.Threading.Thread.Sleep(2000);
                Dynamo.SetBitmapImage(fn_2);
            }
        }
    }
}
