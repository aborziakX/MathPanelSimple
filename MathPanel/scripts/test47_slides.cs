//test47_slides
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
            Dynamo.Console("test47_slides");
            //the path to the files folder 
            string sDir = @"c:\temp\";
            //the array of filenames
            string[] fnames = { "pat1_rot.png", "pat2_rot.png", "pat3_rot.png", "pat4_rot.png", "pat5_rot.png",
                "pat6_rot.png", "pat7_rot.png", "pat8_rot.png", "pat9_rot.png", "pat10_rot.png",
                "pat11_rot.png", "pat12_rot.png"
            };
            //loop through all files
            for (int i = 0; i < fnames.Length; i++)
            {
                var fn = fnames[i];
                //load a file into Image component
                Dynamo.SetBitmapImage(sDir + fn);
                //sleep for 500 ms
                System.Threading.Thread.Sleep(500);
            }
        }
    }
}
