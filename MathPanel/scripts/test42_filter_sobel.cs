//test42_filter_sobel
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
            Dynamo.Console("test42_filter_sobel");
            //the path to the images folder 
            string sDir = @"c:\temp\";
            //file names
            string[] fnames = { "black_rects", "white_rects", "cubes", "white_lines" };

            //it applies the filter to 4 images
            for (int i = 0; i < fnames.Length; i++)
            {
                var fn = fnames[i];
                var bm = new BitmapSimple(sDir + fn + ".png");
                DateTime dt1 = DateTime.Now;
                bm.Sobel(true);// false, 1);
                var fn_2 = fn + "_horz.png";// +"_vert.png";
                bm.Save(sDir + fn_2);
            }

            //do a slide show
            for (int i = 0; i < 100; i++)
            {
                Dynamo.SetBitmapImage(sDir + fnames[i % fnames.Length] + "_horz.png");
                System.Threading.Thread.Sleep(1000);
            }

        }

    }
}
