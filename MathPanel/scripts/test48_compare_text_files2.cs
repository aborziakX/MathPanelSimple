//test48_compare_text_files2
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
            Dynamo.Console("test48_compare_text_files2");
            string sDir = @"C:\Users\boraa\Documents\Visual Studio 2019\MyProjects\MathPanel2\MathPanel\MathExt\";
            string[] fnames = { "Similarica2020-11-10.cs", "Similarica.cs" };
            var solv = new Similarica();
            var dat0 = System.IO.File.ReadAllLines(sDir + fnames[0], System.Text.Encoding.UTF8);
            var dat1 = System.IO.File.ReadAllLines(sDir + fnames[1], System.Text.Encoding.UTF8);

            double dScore = solv.Calc(dat0, dat1);
            Dynamo.Console("Score=" + dScore);
            //var sWg = solv.Printweights("font-size:14pt;");
            //Dynamo.SetHtml(sWg);

            System.Threading.Thread.Sleep(200);
            var sRs = solv.PrintStrings("font-size:14pt;");
            Dynamo.SetHtml(sRs);
        }
    }
}
