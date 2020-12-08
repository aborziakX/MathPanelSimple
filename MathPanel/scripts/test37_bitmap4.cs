//test37_bitmap4
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
            Dynamo.Console("test37_bitmap4");
            //the path to the images folder 
            string sDir = @"c:\temp\";

            string [] fns = { "test37_bitmap4_a.png", "test37_bitmap4_b.jpg" };

            //sky with 3 light gray drops
            var bm = new BitmapSimple(800, 600, System.Drawing.Color.Blue, System.Drawing.Color.Red, true);
            var cc = System.Drawing.Color.LightGray;
            bm.Drop(cc, 100, 100, 80, 80, 0.9, false);
            bm.Drop(cc, 300, 100, 100, 50, 0.9, true);
            bm.Drop(cc, 500, 150, 50, 100, 0.9, false);
            bm.Save(sDir + fns[0]);

            //sky with 3 gray drops
            var bm2 = new BitmapSimple(800, 600, System.Drawing.Color.DarkBlue, System.Drawing.Color.Red, true);
            cc = System.Drawing.Color.Gray;
            bm2.Drop(cc, 100, 100, 80, 80, 0.9, false);
            bm2.Drop(cc, 300, 100, 100, 50, 0.9, true);
            bm2.Drop(cc, 500, 150, 50, 100, 0.9, false);
            bm2.Save(sDir + fns[1]);

            for ( int i = 0; i < 100; i++ )
            {
                Dynamo.SetBitmapImage(sDir + fns[i % 2]);
                System.Threading.Thread.Sleep(1000);
            }
            /*The first one is saved in png format, the second in jpeg format. 
            We see that the 2nd file turned out to be 3 times smaller 
            (jpeg uses compression with a slight loss of quality, invisible to the eye).
            */
        }
    }
}
