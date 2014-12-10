﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;

namespace TagToTerminal
{
    public partial class Form1 : Form
    {
        Excel.Application xlApp;
        Excel.Workbook wb;
        Dictionary<string, List<string>> myDict;
        string[] files;
        string ss;
        Excel.Range range;

        public Form1()
        {
            InitializeComponent();
            xlApp = new Microsoft.Office.Interop.Excel.Application();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            folderBrowserDialog1.ShowDialog();
            files = Directory.GetFiles(folderBrowserDialog1.SelectedPath, "*.xlsx", SearchOption.AllDirectories);
            listBox1.Items.AddRange(files);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();      
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //xlApp.Visible = true
            List<string> templist;
            listBox1.Items.Clear();
            foreach (string s in files) 
            {
                if(s.Contains('~'))
                {
                    Console.WriteLine(s[0]);
                }

                else
                {
                    try
                    {
                        wb = xlApp.Workbooks.Open(Filename: s, IgnoreReadOnlyRecommended: true);
                    }
                    catch (System.Runtime.InteropServices.COMException ex)
                    {
                        Console.WriteLine(ex.Message.ToString());
                    }

                    foreach (Excel.Worksheet ws in wb.Worksheets)
                    {
                        int lastRow = ws.Cells.Find("*", System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;
                        listBox1.Items.Add(ws.Name + ": " + lastRow);
                        for (int i = 2; i <= lastRow; i++)
                        {
                            if (ws.Cells[i, 3].Value2 != null)
                            {
                                ss = ws.Cells[i, 3].Value2.ToString();
                                try
                                {
                                    if (ss[3].Equals('-') && ss[6].Equals('-'))
                                    {
                                        if(myDict.TryGetValue(ss, out templist))
                                        {
                                            listBox1.Items.Add("    Match: " + ss + " to " + templist[0] + templist[1]);

                                        }
                                    }
                                }
                                catch(Exception ex)
                                {
                                    Console.WriteLine(ex.Message.ToString());
                                }
                            }
                        }
                    }
                    wb.Close();
                }
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            myDict = new Dictionary<string,List<string>>();
            List<string> mylist; 
            Excel.Worksheet ws;
            Console.WriteLine(((OpenFileDialog)sender).FileName);
            wb = xlApp.Workbooks.Open(((OpenFileDialog)sender).FileName);
            ws = wb.Worksheets[1];
            
            int lastRow = ws.Cells.Find("*", System.Reflection.Missing.Value, System.Reflection.Missing.Value, System.Reflection.Missing.Value, Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlPrevious, false, System.Reflection.Missing.Value, System.Reflection.Missing.Value).Row;

            for(int i = 1 ; i <= lastRow ; i++)
            {
                mylist = new List<string>();
                mylist.Add(ws.Cells[i,2].Value2.ToString());
                mylist.Add(ws.Cells[i,3].Value2.ToString());
                myDict.Add(ws.Cells[i,1].Value2.ToString(), mylist);
            }
            wb.Close();
            button2.Visible = true;
        }
    }
}
