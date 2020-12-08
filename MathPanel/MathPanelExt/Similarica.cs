using System;
using System.Collections.Generic;
using System.Text;

namespace MathPanelExt
{
	//simple class to align strings and objects, estimate the similarity
	public class Similarica
	{
		//delegate template to calculate the difference between objects
		public delegate double CalcDifference(object x, object y);
		//members
		int m_width, m_height;	//dimensions
		double[] m_weight;		//weights or intermediate scores of cells
		int[] m_from;	//best parents
		int m_iType;	//0-compare strings, 1-arrays of strings, 2-arrays of objects
		string m_sWid, m_sHei; //strings to compare
		string[] m_sliWid;	//vectors to compare
		string[] m_sliHei;
		object[] m_objWid;	//objects to compare
		object[] m_objHei;
		double m_dMax;	//final score
		//parameters can be modified on fly
		public double m_dSkipFee = -1.0;    //fee for skip
		public double m_dMisFee = -1.0;     //fee for mismatch
		public double m_dMatchFee = 1.0;    //fee for match
		public bool bNoSpace = false;	//remove spaces from strings
		public CalcDifference pc_hand = null;	//handler to compare objects
		//simple costructor
		public Similarica()
		{
			m_dMax = 0;
			m_iType = -1;
		}

		//compare 2 strings
		public double Calc(string sWid, string sHei)
		{
			//copy data
			m_sWid = " " + sWid;    //' ' for init-column
			m_sHei = " " + sHei;
			m_iType = 0;

			// allocate the space
			m_height = m_sHei.Length;
			m_width = m_sWid.Length;
			m_weight = new double[m_height * m_width];
			m_from = new int[m_height * m_width];

			//for nice output
			m_sliWid = new string[m_sWid.Length];
			for (int i = 0; i < m_sWid.Length; i++)
				m_sliWid[i] = m_sWid.Substring(i, 1);

			m_sliHei = new string[m_sHei.Length];
			for (int i = 0; i < m_sHei.Length; i++)
				m_sliHei[i] = m_sHei.Substring(i, 1);

			//find weights and the best path
			return DoCalc();
		}

		//compare 2 string arrays
		public double Calc(string[] sliWid, string[] sliHei)
		{
			//copy data
			m_sliWid = new string[sliWid.Length + 1];
			m_sliWid[0] = "";
			for (int i = 0; i < sliWid.Length; i++)
				m_sliWid[i + 1] = sliWid[i];

			m_sliHei = new string[sliHei.Length + 1];
			m_sliHei[0] = "";
			for (int i = 0; i < sliHei.Length; i++)
				m_sliHei[i + 1] = sliHei[i];

			m_iType = 1;

			// allocate the space for weights and the best path
			m_height = m_sliHei.Length;
			m_width = m_sliWid.Length;
			m_weight = new double[m_height * m_width];
			m_from = new int[m_height * m_width];

			//find weights and the best path
			return DoCalc();
		}

		//compare 2 object arrays
		public double Calc(object[] objWid, object[] objHei, CalcDifference hand)
		{
			m_objWid = new object[objWid.Length + 1];
			m_objWid[0] = "";
			for (int i = 0; i < objWid.Length; i++)
				m_objWid[i + 1] = objWid[i];

			m_objHei = new object[objHei.Length + 1];
			m_objHei[0] = "";
			for (int i = 0; i < objHei.Length; i++)
				m_objHei[i + 1] = objHei[i];

			m_iType = 2;
			//set handler
			pc_hand = hand;

			// allocate the space
			m_height = m_objHei.Length;
			m_width = m_objWid.Length;
			m_weight = new double[m_height * m_width];
			m_from = new int[m_height * m_width];

			//for nice output
			m_sliWid = new string[m_objWid.Length];
			m_sliWid[0] = "";
			for (int i = 1; i < m_objWid.Length; i++)
				m_sliWid[i] = string.Format("{0}", m_objWid[i].ToString());

			m_sliHei = new string[m_objHei.Length];
			m_sliHei[0] = "";
			for (int i = 1; i < m_objHei.Length; i++)
				m_sliHei[i] = string.Format("{0}", m_objHei[i].ToString());

			//find weights and the best path
			return DoCalc();
		}

		// returns the maximum weight of the cell
		double GetWeight(int row0, int col0)
		{
			double dMax, weight;
			int i0 = row0 - 1;  //previous row
			int j0 = col0 - 1;  //previous column
			int ia = row0 * m_width + col0;

			//look in previous column, this row
			weight = m_weight[row0 * m_width + j0] + m_dSkipFee;    //fee for skip
			dMax = weight;
			m_from[ia] = -1;

			//look in previous row, this column
			weight = m_weight[i0 * m_width + col0] + m_dSkipFee;    //fee for skip
			if (weight > dMax)
			{
				dMax = weight;
				m_from[ia] = 1;
			}

			//diagonal, previous row and column
			weight = m_weight[i0 * m_width + j0] + GetMatch(row0, col0);
			if (weight > dMax)
			{
				dMax = weight;
				m_from[ia] = 0;
			}
			return dMax;
		}

		//get the corresponding score
		double GetMatch(int i, int j)
		{
			if (m_iType == 0)
			{	//compare strings
				string ch1 = m_sHei.Substring(i, 1);
				string ch2 = m_sWid.Substring(j, 1);
				return (ch1 == ch2) ? m_dMatchFee : m_dMisFee;
			}
			double d = 0;
			if (m_iType == 1)
			{   //compare arrays of strings
				string sl1 = m_sliHei[i];
				string sl2 = m_sliWid[j];
				if (bNoSpace)
				{
					sl1 = sl1.Replace(" ", "").Replace("\t", "");
					sl2 = sl2.Replace(" ", "").Replace("\t", "");
				}
				d = (sl1 == sl2) ? m_dMatchFee : m_dMisFee;
			}
			else if(pc_hand != null)
			{   //compare objects
				var diff = pc_hand((m_objHei[i]), (m_objWid[j]));
				d = (diff == 0) ? m_dMatchFee : m_dMisFee;
			}
			return d;
		}

		//find weights and the best path
		double DoCalc()
		{
			int i, j;
			// calculate the weights: first row
			for (j = 0; j < m_width; j++)
			{
				m_weight[0 * m_width + j] = j * m_dSkipFee;
			}
			// then inside	
			for (i = 1; i < m_height; i++)
			{   //row by row
				m_weight[i * m_width + 0] = i * m_dSkipFee;
				for (j = 1; j < m_width; j++)
				{
					m_weight[i * m_width + j] = GetWeight(i, j);
				}
			}
			// find maximum weight in last row, last column
			m_dMax = m_weight[((m_height - 1) * m_width) + m_width - 1];
			return m_dMax;
		}

		//debug function
		//generate html-table with weights and the best path
		public string Printweights(string style = "font-size:9pt;")
		{
			int i, j;
			//find best columns
			int[] iBestCol = new int[m_height];
			iBestCol[m_height - 1] = m_width - 1;
			for (i = m_height - 1; i > 0; i--)
			{
				j = iBestCol[i];
				while (j >= 0)
				{
					int from = m_from[i * m_width + j];
					if (from == 1)
					{   //up
						iBestCol[i - 1] = j;
						break;
					}
					else if (from == 0)
					{   //diagonal
						iBestCol[i - 1] = j > 0 ? j - 1 : 0;//2020-07-15
						break;
					}
					else
					{
						if (j == 0)
						{   //2020-07-15
							iBestCol[i - 1] = 0;
							break;
						}
						j--;
					}
				}
			}

			//generate table
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<table id='scoretable' border='1' cellpadding='0' cellspacing='0' style='{0}'>\n", style);
			sb.Append("<tr><td></td>\n");
			for (j = 0; j < m_sliWid.Length; j++)
			{
				var q = m_sliWid[j].Trim();
				sb.Append(string.Format("<td><b>{0}</b></td>\n", q.Length > 0 ? q.Substring(0, 1) : ""));
			}
			sb.Append("</tr>\n");

			for (i = 0; i < m_height; i++)
			{
				sb.Append("<tr>\n");
				var q2 = m_sliHei[i].Trim();
				sb.Append(string.Format("<td><b>{0}</b></td>\n", q2.Length > 0 ? q2.Substring(0, 1) : ""));

				for (j = 0; j < m_width; j++)
				{
					double ma = 0.0;
					if (i > 0 && j > 0) ma = GetMatch(i, j);
					sb.Append(string.Format("<td{0}>{1}<br>{2}</td>\n",
						(j == iBestCol[i]) ? " style='background:#cccccc'" : "",
						ma, m_weight[i * m_width + j]));
				}
				sb.Append("</tr>\n");
			}
			sb.Append("</table>\n");
			return sb.ToString();
		}

		//generate colorized html view
		//white - the same, red - data from the first array, green - data from the second array
		public string PrintStrings(string style = "font-size:9pt;")
		{
			int i, j;
			//find best columns
			int[] iBestCol = new int[m_height];
			iBestCol[m_height - 1] = m_width - 1;
			for (i = m_height - 1; i > 0; i--)
			{
				j = iBestCol[i];
				while (j >= 0)
				{
					int from = m_from[i * m_width + j];
					if (from == 1)
					{   //up
						iBestCol[i - 1] = j;
						break;
					}
					else if (from == 0)
					{   //diagonal
						iBestCol[i - 1] = j > 0 ? j - 1 : 0;//2020-07-15
						break;
					}
					else
					{
						if (j == 0)
						{   //2020-07-15
							iBestCol[i - 1] = 0;
							break;
						}
						j--;
					}
				}
			}

			//generate div's
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("<div style='{0}'>\n", style);
			int prevCol = 0;
			for (i = 1; i < m_height; i++)
			{
				j = iBestCol[i];
				if (prevCol + 1 == j)
				{   //diagonal
					double ma = GetMatch(i, j);
					if (ma > 0)
					{ //match
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "",
							m_sliHei[i].Replace("<", "&lt;").Replace(">", "&gt;")));
					}
					else
					{   //mis
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "red",
							m_sliWid[j].Replace("<", "&lt;").Replace(">", "&gt;")));
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "green",
							m_sliHei[i].Replace("<", "&lt;").Replace(">", "&gt;")));
					}
				}
				else if (prevCol == j)
				{   //down
					sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "green",
						m_sliHei[i].Replace("<", "&lt;").Replace(">", "&gt;")));
				}
				else
				{   //right
					prevCol++;
					double ma = GetMatch(i, prevCol);
					if (ma > 0)
					{   //match
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "",
							m_sliHei[i].Replace("<", "&lt;").Replace(">", "&gt;")));
					}
					else
					{   //mis
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "red",
							m_sliWid[prevCol].Replace("<", "&lt;").Replace(">", "&gt;")));
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "green",
							m_sliHei[i].Replace("<", "&lt;").Replace(">", "&gt;")));
					}
					prevCol++;
					while (prevCol <= j)
					{   //skip
						sb.Append(string.Format("<div class='{0}'>{1}</div>\n", "red",
							m_sliWid[prevCol].Replace("<", "&lt;").Replace(">", "&gt;")));
						prevCol++;
					}
				}

				prevCol = j;
			}

			sb.Append("</div>");
			return sb.ToString();
		}

	}
}
