//test48_compare_float
using MathPanel;
using MathPanelExt;
using System.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

///assemblies to use
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        //our function to calculate the difference between doubles (objects)
        double my_compare(object x, object y)
        {
            //Dynamo.Console("x " + x.ToString().Replace(",", "."));
            //Dynamo.Console("y " + y.ToString().Replace(",", "."));
            //create a double from a first object
            double x1 = double.Parse(x.ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            //create a double from a second object
            double y1 = double.Parse(y.ToString().Replace(",", "."), System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            //find the absolute difference
            double dMax = (Math.Abs(x1) > Math.Abs(y1)) ? Math.Abs(x1) : Math.Abs(y1);
            if (dMax == 0) return 0.0;    //equal
            //find the relative difference
            var q = Math.Abs(x1 - y1) / dMax;
            //if the difference is pretty small, return zero, otherwise -1
            return q <= 0.01 ? 0.0 : -1.0;
        }
        public void Execute()
        {
            Dynamo.Console("test48_compare_float");
            //the first array of doubles
            object[] f0 = { 1, 2, 3, 4, 7 };
            //the second array of doubles
            object[] f1 = { 1.1, 2.01, 3.1, 4.01, 5.1, 6.1, 7 };

            //create an instance of Similarica class
            var solv = new MathPanelExt.Similarica();
            //get the score and weights, third parameter is our delegate
            double dScore = solv.Calc(f0, f1, my_compare);
            //display alignments
            var sRs = solv.PrintStrings("font-size:14pt;");
            Dynamo.SetHtml(sRs);
        }
    }
}
