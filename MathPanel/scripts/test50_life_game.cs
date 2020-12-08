//test50_life_game.cs
using MathPanel;
using MathPanelExt;
using System.Net.Sockets;
using System;
/*
The game 'Life' was invented by the British mathematician John Conway in 1970. Successfully mimics the behavior of cell colonies.
It uses only 3 rules, but generates interesting ornaments.
Rule 1. A dead cell becomes alive if it has land borders with exactly three alive.
Rule 2. The living cell continues to live, if it borders with two or three live.
Rule 3. In all other cases, the cell is dead. The living die from lack of food (too many neighbors) or loss of heat (too few neighbors).
So people who die are alone or live in the crowd.
*/

///assemblies to be added
///[DLL]System.dll,System.Xaml.dll,WindowsBase.dll,PresentationFramework.dll,PresentationCore.dll,System.Drawing.dll,System.Net.dll,System.Net.Http.dll,System.Core.dll[/DLL]
///
namespace DynamoCode
{
    public class Script
    {
        //randomly init cells
        void Init(int [] arr, Random rnd, int nInit)
        {
            int sz = arr.Length;
            for (int i = 0; i < sz; i++) arr[i] = 0;//dead

            for (int i = 0; i < nInit; i++)
            {
                arr[rnd.Next(sz)] = 1;//alive
            }
        }

        //add one more alive cell
        void addLive(int[] arr, Random rnd)
        {
            int sz = arr.Length;
            int k = rnd.Next(sz);
            arr[k] = 1;
        }

        //update cells color
        void updateCells(int[] arr, System.Drawing.Color[] arrCol, System.Drawing.Color[] clrs)
        {
            int sz = arr.Length;
            for (int k = 0; k < sz; k++)
            {
                clrs[k] = arr[k] > 0 ? arrCol[1] : arrCol[0];
            }
        }

        //new iteration
        void Iterate(int[] arr, int m, int n, int [] arrNeighb)
        {
            int sz = arr.Length;
            int i, ind;
            //calculate neighboors
            for (i = 0; i < sz; i++) arrNeighb[i] = 0;

            for (i = 0; i < sz; i++)
            {   //check all 8 neighboors
                //Dynamo.Console("i=" + i);
                if (i % n == 0)
                {   //on left border
                }
                else
                {   //not most left
                    ind = i - 1;//left
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                    ind = i - 1 - n;//left and top
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                    ind = i - 1 + n;//left and bottom
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                }

                if (i % n == n - 1)
                {   //on right border
                }
                else
                {   //not most right
                    ind = i + 1;//right
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                    ind = i + 1 - n;//right, top
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                    ind = i + 1 + n;//right, bottom
                    if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                }

                //top
                ind = i - n;
                if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
                //bottom
                ind = i + n;
                if (ind >= 0 && ind < sz && arr[ind] == 1) arrNeighb[i]++;
            }

            //new generation
            for (i = 0; i < sz; i++)
            {
                if (arr[i] == 0 && arrNeighb[i] == 3)
                {   //new alive - Rule 1
                    arr[i] = 1;
                }
                else if (arr[i] == 1 && (arrNeighb[i] == 2 || arrNeighb[i] == 3))
                {   //keep alive - Rule 2
                }
                else
                {   //die - Rule 3
                    arr[i] = 0;
                }
            }
        }

        public void Execute()
        {
            Dynamo.Console("test50_life_game");

            //colors
            System.Drawing.Color[] arrCol = { System.Drawing.Color.Black, System.Drawing.Color.Green };//dead, alive
            int m = 40; //rows
            int n = 50; //columns
            int nInit = 500;    //number of initially alives
            int sz = m * n;     //table size
            int[] arr = new int[sz];        //cell state
            int[] arrNeighb = new int[sz];  //number of neighboors
                                            //cell colors
            System.Drawing.Color[] clrs = new System.Drawing.Color[sz];
            //number of iterations
            int NPOINTS = 1000;
            //the random number generator
            Random rnd = new Random();

            //define drawing optional parameters: use table (m by n) in canvas 800 by 600
            string sOpt = "{\"options\":{\"x0\": 0, \"x1\": " + n + ", \"y0\": 0, \"y1\": " + m + ", \"clr\": \"#00ff00\", \"sty\": \"dots\", \"size\":20, \"lnw\": 2, \"wid\": 800, \"hei\": 600 }";

            //initialize the colony
            Init(arr, rnd, nInit);

            //update table colors
            updateCells(arr, arrCol, clrs);
            //Dynamo.Console("init done");

            //loop for desired number
            for (int i = 0; i < NPOINTS; i++)
            {
                //create bitmap code
                var s1 = MathPanelExt.QuadroEqu.DrawBitmap(m, n, clrs);
                //Dynamo.Console("quadro done=" + i);

                //display
                var sJson = sOpt + ", \"data\":[" + s1 + "]}";
                Dynamo.SceneJson(sJson);
                //Dynamo.Console("json done=" + i);

                //sleep a while
                System.Threading.Thread.Sleep(500);
                //Dynamo.Console("sleep done=" + i);
                string resp = Dynamo.KeyConsole;
                if (resp == "Q")
                {
                    break;
                }
                if (resp == "A")
                {
                    addLive(arr, rnd);
                    Dynamo.Console("add 1 done=" + i);
                }

                //new iteration
                Iterate(arr, m, n, arrNeighb);
                //Dynamo.Console("iter done=" + i);

                //update table
                updateCells(arr, arrCol, clrs);
                //Dynamo.Console("upd done=" + i);
            }
        }
    }
}
