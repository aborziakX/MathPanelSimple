//test37_bitmap3
using MathPanel;
//using MathPanelExt;
using System.Net.Sockets;
using System;

///assemblies to use
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        public void Execute()
        {
            Dynamo.Console("test37_bitmap3");
            //the path to the images folder 
            string sDir = @"c:\temp\";

            //Create 4 bitmaps with a gradient from white to blue and load them in turn into the Image1 component.
            string[] fns = { "test37_bitmap3_a.png", "test37_bitmap3_b.png", "test37_bitmap3_c.png", "test37_bitmap3_d.png" };
            var bm = new BitmapSimple(200, 200, System.Drawing.Color.White, System.Drawing.Color.Blue, false);
            bm.Save(sDir + fns[0]);

            var bm2 = new BitmapSimple(200, 200, System.Drawing.Color.White, System.Drawing.Color.Blue, true);
            bm2.Save(sDir + fns[1]);

            var bm3 = new BitmapSimple(200, 200, System.Drawing.Color.Blue, System.Drawing.Color.White, false);
            bm3.Save(sDir + fns[2]);

            var bm4 = new BitmapSimple(200, 200, System.Drawing.Color.Blue, System.Drawing.Color.White, true);
            bm4.Save(sDir + fns[3]);

            for ( int i = 0; i < 100; i++ )
            {
                Dynamo.SetBitmapImage(sDir + fns[i % 4]);
                System.Threading.Thread.Sleep(1000);
            }
        }
    }
}
