//test51_labyrinth.cs
using MathPanel;
using MathPanelExt;
using System.Net.Sockets;
using System;
//new
using System.Collections.Generic;

/*
The “Wave” algorithm. It was invented for optimal wiring of printed circuit boards with microchips. 
Here we use it to exit the labyrinth. You can get out of the labyrinth by holding one of the walls with your hand. 
But it is long and painful. We use all the power of the computer and use the wave algorithm.
*/

///assemblies to be added
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        //update cells color
        void updateCells(int[] arrState, System.Drawing.Color[] arrCol, System.Drawing.Color[] clrs)
        {
            int sz = arrState.Length;
            for (int k = 0; k < sz; k++)
            {   //set color according to state of cell
                clrs[k] = arrCol[arrState[k]];
            }
        }

        //new iteration
        bool Iterate(int[] arrState, int m, int n, HashSet<int> lstFront, HashSet<int> lstFrontOld)
        {
            bool rc = false;
            int sz = arrState.Length;
            int ind;

            foreach (var j in lstFrontOld)
            {
                arrState[j] = 4;//filled

                //check all 4 neighboors
                if (j % n == 0)
                {   //on left border, skip
                }
                else
                {   //not most left
                    ind = j - 1;//left               
                    if (ind >= 0 && ind < sz && arrState[ind] == 1)
                    {
                        arrState[ind] = 5; //front
                        lstFront.Add(ind);
                    }
                    if (ind >= 0 && ind < sz && arrState[ind] == 3) rc = true;
                }

                if (j % n == n - 1)
                {   //on right border
                }
                else
                {   //not most right
                    ind = j + 1;//right
                    if (ind >= 0 && ind < sz && arrState[ind] == 1)
                    {
                        arrState[ind] = 5; //front
                        lstFront.Add(ind);
                    }
                    if (ind >= 0 && ind < sz && arrState[ind] == 3) rc = true;
                }

                //top
                ind = j - n;
                if (ind >= 0 && ind < sz && arrState[ind] == 1)
                {
                    arrState[ind] = 5; //front
                    lstFront.Add(ind);
                }
                if (ind >= 0 && ind < sz && arrState[ind] == 3) rc = true;

                //bottom
                ind = j + n;
                if (ind >= 0 && ind < sz && arrState[ind] == 1)
                {
                    arrState[ind] = 5; //front
                    lstFront.Add(ind);
                }
                if (ind >= 0 && ind < sz && arrState[ind] == 3) rc = true;
            }

            return rc;
        }

        //check for correctness
        int Check(BitmapSimple bm, out int iStart, out int iFinish, int [] arrState)
        {
            iStart = -1;
            iFinish = -1;
            bool bBlack = false;
            bool bRed = false;
            bool bGreen = false;
            int iWhite = System.Drawing.Color.White.ToArgb();
            int iBlack = System.Drawing.Color.Black.ToArgb();
            int iRed = System.Drawing.Color.Red.ToArgb();
            int iGreen = System.Drawing.Color.FromArgb(255, 0, 255, 0).ToArgb();
            for (int k = 0; k < bm.width * bm.height; k++)
            {
                int argb = bm.map[k];
                if (argb == iBlack)
                {   //borders
                    bBlack = true;
                    arrState[k] = 0;
                }
                else if (argb == iRed)
                {   //start
                    bRed = true;
                    if (iStart == -1)
                    {
                        iStart = k;
                        arrState[k] = 2;
                    }
                    else
                    {   //space
                        bm.map[k] = iWhite;
                        arrState[k] = 1;
                    }
                }
                else if (argb == iGreen)
                {   //finish
                    bGreen = true;
                    if (iFinish == -1)
                    {
                        iFinish = k;
                        arrState[k] = 3;
                    }
                    else
                    {   //space
                        bm.map[k] = iWhite;
                        arrState[k] = 1;
                    }
                }
                else
                {   //space
                    bm.map[k] = iWhite;
                    arrState[k] = 1;
                }
            }
            return (bBlack ? 0 : 1) + (bRed ? 0 : 2) + (bGreen ? 0 : 4);
        }

        public void Execute()
        {
            Dynamo.Console("test51_labyrinth");
            //the path to the images folder 
            string sDir = @"c:\temp\";
            string fname = "labyrinth.png";
            var bm = new BitmapSimple(sDir + fname);
            Dynamo.Console("bm size " + bm.width + "," + bm.height);

            int m = bm.height; //rows
            int n = bm.width; //columns
            int sz = m * n;     //table size
            int[] arrState = new int[sz];        //cell state: 0-border, 1-space, 2-start, 3-finish, 4-filled, 5-front
            //cell colors
            System.Drawing.Color[] clrs = new System.Drawing.Color[sz];
            //number of iterations
            int NPOINTS = 1000;
            //the random number generator
            Random rnd = new Random();
            //state colors
            System.Drawing.Color[] arrCol = { 
                System.Drawing.Color.Black, //border
                System.Drawing.Color.White, //space
                System.Drawing.Color.Red,   //start
                System.Drawing.Color.Green, //goal
                System.Drawing.Color.Blue, //filled
                System.Drawing.Color.Orange, //front
            };
            //define drawing optional parameters: use table (m by n) in canvas 800 by 600
            string sOpt = "{\"options\":{\"x0\": -0.5, \"x1\": " + (n) + ", \"y0\": -0.5, \"y1\": " + (m) + ", \"clr\": \"#00ff00\", \"sty\": \"dots\", \"size\":20, \"lnw\": 2, \"wid\": 800, \"hei\": 600 }";
            bool bManual = false;    //manual mode

            int iStart; //start position in map
            int iFinish; //finish position in map
            int rc = Check(bm, out iStart, out iFinish, arrState);
            Dynamo.Console("rc=" + rc + ", iStart=" + iStart +", iFinish=" + iFinish);
            if (rc != 0) return;

            HashSet<int> lstFront = new HashSet<int>();   //the new wave front
            HashSet<int> lstFrontOld = new HashSet<int>();//the previous wave front
            lstFront.Add(iStart);

            //update table
            updateCells(arrState, arrCol, clrs);

            //loop for desired number
            for (int i = 0; i < NPOINTS; i++)
            {
                //create bitmap code
                var s1 = MathPanelExt.QuadroEqu.DrawBitmap(m, n, clrs, 0, 0, false);
                //Dynamo.Console("quadro done=" + i);

                //display
                var sJson = sOpt + ", \"data\":[" + s1 + "]}";
                Dynamo.SceneJson(sJson);
                //Dynamo.Console("json done=" + i);

                //sleep a while
                System.Threading.Thread.Sleep(50);
                //Dynamo.Console("sleep done=" + i);
                string resp = Dynamo.KeyConsole;
                if (resp == "Q")
                {
                    break;
                }
                if (bManual)
                {
                    if (resp != "N")
                        continue;
                }

                //new iteration
                lstFrontOld.Clear();
                foreach(var hz in lstFront) lstFrontOld.Add(hz);
                lstFront.Clear();
                bool rc2 = Iterate(arrState, m, n, lstFront, lstFrontOld);
                //Dynamo.Console("iter done=" + i + ",lstFront.cnt=" + lstFront.Count + ",lstFrontOld.cnt=" + lstFrontOld.Count);

                //update table
                updateCells(arrState, arrCol, clrs);
                //Dynamo.Console("upd done=" + i);
                if ( rc2 ) break;
            }
        }
    }
}
