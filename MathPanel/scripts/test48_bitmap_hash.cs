//test48_bitmap_hash
using MathPanel;
using MathPanelExt;
using System.Net.Sockets;
using System;
using System.Drawing;

///assemblies to use
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        public void Execute()
        {
            Dynamo.Console("test48_bitmap_hash");
            //the folder with files
            string sDir = @"c:\temp\";
            //palette colors
            Color[] palette = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.DarkBlue,
                Color.Magenta, Color.White, Color.Black, Color.Gray };

            //build the first hash
            var fn = sDir + "test37_bitmap4_a.png";
            var bm = new BitmapSimple(fn);
            var s = bm.Hash(20, 20, palette, "ROYGBDMWKA", true);
            Dynamo.Console("s=" + s);
            var fn_hash = (sDir + "test37_bitmap4_a_hash.png");
            bm.Save(fn_hash);

            //build the second hash
            var fn_2 = sDir + "test37_bitmap5_с.png";
            var bm_2 = new BitmapSimple(fn_2);
            var s_2 = bm_2.Hash(20, 20, palette, "ROYGBDMWKA", true);
            Dynamo.Console(" s_2=" + s_2);
            var fn_2_hash = (sDir + "test37_bitmap5_с_hash.png");
            bm_2.Save(fn_2_hash);

            //build the third hash
            var fn_3 = sDir + "test37_bitmap3_c.png";
            var bm_3 = new BitmapSimple(fn_3);
            var s_3 = bm_3.Hash(20, 20, palette, "ROYGBDMWKA", true);
            Dynamo.Console(" s_3=" + s_3);
            var fn_3_hash = (sDir + "test37_bitmap3_c_hash.png");
            bm_3.Save(fn_3_hash);

            //create a Similarica-object
            var solv = new Similarica();
            //compare hashs 1 and 2
            double dScore = solv.Calc(s, s_2);
            Dynamo.Console(" s, s_2 score=" + dScore);

            //compare hashs 1 and 3
            dScore = solv.Calc(s, s_3);
            Dynamo.Console(" s, s_3 score=" + dScore);

            //compare hashs 3 and 2
            dScore = solv.Calc(s_3, s_2);
            Dynamo.Console(" s_3, s_2 score=" + dScore);
        }
    }
}
