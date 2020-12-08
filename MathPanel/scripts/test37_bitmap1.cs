//test37_bitmap1
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
            Dynamo.Console("test37_bitmap1");
            //the path to the images folder 
            string sDir = @"c:\temp\";

            //Here we define an array of several colors, 
            System.Drawing.Color[] colors = { 
                System.Drawing.Color.Red,
                System.Drawing.Color.Orange,
                System.Drawing.Color.Yellow,
                System.Drawing.Color.Green,
                System.Drawing.Color.Blue,
                System.Drawing.Color.Magenta,
                System.Drawing.Color.Cyan,
                System.Drawing.Color.White,
                System.Drawing.Color.Green,
            };
            
            //create our BitmapSimple object
            var bm = new BitmapSimple(40, 40, colors);
            var fn = sDir +"test37_bitmap1.png";
            //save it to a file
            bm.Save(fn);
            //and pass the file to our Image component
            Dynamo.SetBitmapImage(fn);
        }
    }
}
