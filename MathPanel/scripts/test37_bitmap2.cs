//test37_bitmap2
using MathPanel;
using MathPanelExt;
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
            Dynamo.Console("test37_bitmap2");
            //the path to the images folder 
            string sDir = @"c:\temp\";

            var fn = sDir + "test37_bitmap1.png";
            //create an object of BitmapSimple from file 
            var bm = new BitmapSimple(fn);
            //put black rectangle on it
            bm.Pixel(15, 15, 255, 0, 0, 0, 10, 10);
            var fn_2 = sDir + "test37_bitmap2.png";
            //save it
            bm.Save(fn_2);
            Dynamo.SetBitmapImage(fn_2);
        }
    }
}
