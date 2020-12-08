//test48_compare_text_files
using MathPanel;
using MathPanelExt;
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
            Dynamo.Console("test48_compare_text_files");
            //the path to the files folder 
            string sDir = @"c:\temp\";
            //the array of filenames
            string[] fnames = { "test.htm", "test2.htm" };

            //create an instance of Similarica class
            var solv = new Similarica();

            //read data from 1 file
            var dat0 = System.IO.File.ReadAllLines(sDir + fnames[0], System.Text.Encoding.UTF8);
            //read data from 2 file
            var dat1 = System.IO.File.ReadAllLines(sDir + fnames[1], System.Text.Encoding.UTF8);

            //get the score and weights
            double dScore = solv.Calc(dat0, dat1);
            Dynamo.Console("Score=" + dScore);

            //display alignments
            System.Threading.Thread.Sleep(200);
            var sRs = solv.PrintStrings("font-size:14pt;");
            Dynamo.SetHtml(sRs);
        }
    }
}
