using GhostscriptSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PDF_Compare_Tool.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;





namespace PDF_Compare_Tool
{
    
    class Program
    {

       public static List<HighlightViewModel> highlightPDF1 = new List<HighlightViewModel>();
      public  static List<HighlightViewModel> highlightPDF2 = new List<HighlightViewModel>();
       static int  imagePDFCount1 = 0 , imagePDFCount2 = 0;

        static TextInfo textInfo = new TextInfo();
        static void Main(string[] args)
        {
            

            Console.Write("\n START \n");
            CordsModel cords = new CordsModel();

            TextInfo info = new TextInfo();
           

            var inputFile1 = @"F:\Testfiles\bill1.pdf";
            var inputFile2 = @"F:\Testfiles\bill2.pdf";


          //  PDFToImage(@"F:\Testfiles\50page.pdf", @"F:\Testfiles\PDFImages", 100);

            var executableFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string PDF1Folder = executableFolderPath + @"\PDFData\PDF1";
            string PDF2Folder = executableFolderPath + @"\PDFData\PDF2";
            
            SplitPDF(inputFile1, PDF1Folder);
            SplitPDF(inputFile2, PDF2Folder);

            var PDF1FileCount = (from file in Directory.EnumerateFiles(PDF1Folder, "*.pdf", SearchOption.AllDirectories)
                             select file).Count();
            var PDF2FileCount = (from file in Directory.EnumerateFiles(PDF2Folder, "*.pdf", SearchOption.AllDirectories)
                                 select file).Count();
            int minFileCount=0;
            if (PDF1FileCount > PDF2FileCount)
                minFileCount = PDF2FileCount;
            else if (PDF1FileCount < PDF2FileCount)
                minFileCount = PDF1FileCount;
            else
                minFileCount = PDF1FileCount;

            for (int i = 1; i <= minFileCount; i++)
            {
                string newIF1 = PDF1Folder + "\\" + i + ".pdf";
                string newIF2 = PDF2Folder + "\\" + i + ".pdf";

                string newHL1 = executableFolderPath + @"\PDFData\HL1" + "\\" + i + ".pdf";
                string newHL2 = executableFolderPath + @"\PDFData\HL2" +"\\" + i + ".pdf";


                ReadTwoPDF(newIF1, newIF2);

                HighlightSpire(newIF1, newHL1, highlightPDF1);

                HighlightSpire(newIF2, newHL2, highlightPDF2);

                PDFToImage(newHL1, executableFolderPath + @"\PDFData\PDFImage1", imagePDFCount1,1);

                PDFToImage(newHL2, executableFolderPath + @"\PDFData\PDFImage2", imagePDFCount2,2);

                Deletefile(newHL1);
                Deletefile(newHL2);
                Deletefile(newIF1);
                Deletefile(newIF2);


                
            }

            ImageToPDF();





            Console.Write("\n \n END ");
            Console.ReadKey();

        }


        public static void ReadTwoPDF(string FirstPDF, string SecondPDF)
        {
            var FirstFile = "";
            var SecondFile = "";
            List<PDF1ViewModel> PageList1 = new List<PDF1ViewModel>();
            List<PDF2ViewModel> PageList2 = new List<PDF2ViewModel>();

            if (File.Exists(FirstPDF) && File.Exists(SecondPDF))
            {
                Spire.Pdf.PdfDocument reader = new Spire.Pdf.PdfDocument();
                reader.LoadFromFile(FirstPDF);
                for (int page = 0; page < reader.Pages.Count; page++)
                {

                    StringBuilder content = new StringBuilder();
                    content.Append(reader.Pages[page].ExtractText());
                    string newContent = content.ToString().Substring(70);
                    newContent = newContent.Replace("\r", "");
                   // Regex.Unescape(newContent);

                    // newContent = newContent.Replace("\u", "");
                    PageList1.Add(new PDF1ViewModel() { Lines = newContent, PageNo = page });

                }

                Spire.Pdf.PdfDocument reader2 = new Spire.Pdf.PdfDocument();
                reader2.LoadFromFile(SecondPDF);
                for (int page = 0; page < reader2.Pages.Count; page++)
                {

                    StringBuilder content = new StringBuilder();
                    content.Append(reader2.Pages[page].ExtractText());
                    string newContent = content.ToString().Substring(70);
                    
                    newContent = newContent.Replace("\r", "");
                  //  Regex.Unescape(newContent);
                    //   newContent = newContent.Replace("\u", "");
                    PageList2.Add(new PDF2ViewModel() { Lines = newContent, PageNo = page });

                }

                //PdfReader reader = new PdfReader(FirstPDF);
                //for (int page = 1; page <= reader.NumberOfPages; page++)
                //{
                //  //  ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                //    FirstFile  = PdfTextExtractor.GetTextFromPage(reader, page, new LocationTextExtractionStrategy());
                //    PageList1.Add(new PDF1ViewModel() { Lines = FirstFile, PageNo = page });
                //}



                //PdfReader reader1 = new PdfReader(SecondPDF);
                //for (int page = 1; page <= reader1.NumberOfPages; page++)
                //{
                //   // ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                //    SecondFile = PdfTextExtractor.GetTextFromPage(reader1, page, new LocationTextExtractionStrategy());
                //    PageList2.Add(new PDF2ViewModel() { Lines = SecondFile, PageNo = page });
                //}
            }
            else
            {
                Console.WriteLine("Files does not exist.");
            }
            PDFPage(PageList1, PageList2);
        }


        public static void PDFPage(List<PDF1ViewModel> FirstFilePages, List<PDF2ViewModel> SecondFilePages)
        {
            int a = FirstFilePages.Count;
            int b = SecondFilePages.Count;

            if (FirstFilePages.Count > SecondFilePages.Count)
            {
                for (int i = 0; i < FirstFilePages.Count; i++)
                {
                    if (i < b)
                    {
                       // for (int j = 0; j < FirstFilePages.Count; j++)
                      //  {
                            // Console.Write("fisrt page of 1 2 file");
                            CompareWords(FirstFilePages[i].Lines, SecondFilePages[i].Lines, (i + 1));
                           // a--;
                       // }
                    }
                    else
                    {
                        //Console.Write("second page of 1  file");
                        CompareWords(FirstFilePages[i].Lines, "", (i + 1));
                    }
                }
            }

            if (FirstFilePages.Count < SecondFilePages.Count)
            {
                for (int i = 0; i < SecondFilePages.Count; i++)
                {//
                    if (i < a)
                    {
                       // for (int j = 0; j <= i; j++)
                        //{

                            // Console.Write("fisrt page of 1 2 file");
                            CompareWords(FirstFilePages[i].Lines, SecondFilePages[i].Lines, (i + 1));
                           // b--;
                      //  }
                    }
                    else
                    {
                        //Console.Write("second page of 2  file");
                        CompareWords("", SecondFilePages[i].Lines, (i + 1));
                    }
                }
            }

            if (FirstFilePages.Count == SecondFilePages.Count)
            {
                for (int i = 0; i < FirstFilePages.Count; i++)
                {
                  //  for (int j = 0; j <= i; j++)
                  // {
                        CompareWords(FirstFilePages[i].Lines, SecondFilePages[i].Lines, (i + 1));
                    //}
                }
            }

        }

        public static void CompareWords(string FirstFile, string SecondFile, int page)
        {
            List<string> File1diff;
            List<string> File2diff;

            List<string> FindPositionList1 = new List<string>();
            List<string> FindPositionList2 = new List<string>();

            List<string> file1=new List<string>(); //= FirstFile.Trim().Split('\r', '\n');
            List<string> file2=new List<string>(); //= SecondFile.Trim().Split('\r', '\n');

            
          


            string[] words = FirstFile.Split('\n');
            string line;
            for (int j = 0, len = words.Length; j < len; j++)
            {
                line = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words[j]));
                file1.Add(line);
            }



            string[] words2 = SecondFile.Split('\n');
            string line2;
            for (int j = 0, len = words2.Length; j < len; j++)
            {
                line2 = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(words2[j]));

                
                file2.Add(line2);
            }


            File1diff = file1.ToList();
            File2diff = file2.ToList();
           // List<List<int>> file1List= findLine(File1diff, File2diff);
            //List<List<int>> file2List = findLine(File2diff, File1diff);



            if (file1.Count() == file2.Count()  )
            {
                for (int i = 0; i < File2diff.Count; i++)
                {
                    List<string> File1WordList;
                    List<string> File2WordList;

                    IEnumerable<string> file1Word = File1diff[i].Trim().Split(' ', '\t');
                    IEnumerable<string> file2Word = File2diff[i].Trim().Split(' ', '\t');
                    File1WordList = file1Word.ToList();
                    File1WordList = File1WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    File2WordList = file2Word.ToList();
                    File2WordList = File2WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                    if (File1WordList.Count > File2WordList.Count)
                    {
                        int diff = (File1WordList.Count) - (File2WordList.Count);
                        for (int w = 1; w <= diff; w++)
                        {
                            File2WordList.Add("");
                        }
                        for (int k = 0; k < File1WordList.Count; k++)
                        {
                            

                                if (File1WordList.Contains(File2WordList[k],StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                                {
                                    Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                    FindPositionList1.Add(File1WordList[k]);
                                    FindPositionList2.Add(File2WordList[k]);
                                }
                                
                                    else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true && File2WordList[k] != "")
                                     {

                                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                                      FindPositionList1.Add(File1WordList[k]);
                                       FindPositionList2.Add(File2WordList[k]);

                                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                                     }

                                  else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                                     {
                                        FindPositionList1.Add(File1WordList[k]);
                                         FindPositionList2.Add(File2WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                     
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });
                                

                            }
                                  else  if(File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                                    {
                                        if (File2WordList[k] != "")

                                        {
                                            FindPositionList2.Add(File2WordList[k]);
                                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                            //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                            // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                            highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                                        }
                                        FindPositionList1.Add(File1WordList[k]);
                                        int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                        highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                        Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                                    }

                            
                         
                        }
                    }
                    else if (File1WordList.Count < File2WordList.Count)
                    {
                        int diff = (File2WordList.Count) - (File1WordList.Count);
                        for (int w=1;w<= diff;w++)
                        {
                            File1WordList.Add("");
                        }
                        for (int k = 0; k < File2WordList.Count; k++)
                        {
                            if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                            }

                            else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true )
                            {

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                                FindPositionList2.Add(File2WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            }

                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true && File1WordList[k] != "")
                            {
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                            }
                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                            {
                                if (File1WordList[k] != "")

                                {
                                    FindPositionList1.Add(File1WordList[k]);
                                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                }
                                

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                                FindPositionList2.Add(File2WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                            }

                        }
                    }
                    else if (File1WordList.Count == File2WordList.Count)
                    {
                        for (int k = 0; k < File1WordList.Count; k++)
                        {
                            if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                            }

                            else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);

                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            }

                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                            }
                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                            {
                  
                                    FindPositionList2.Add(File2WordList[k]);
                                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                    //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                    // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                                
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                            }

                            
                        }
                    }

                }
            }

            else
            {
                Diffreanciatefile1(File1diff, File2diff, page, FindPositionList1, FindPositionList2);
                Diffreanciatefile2(File2diff, File1diff, page, FindPositionList1, FindPositionList2);
            }

            if (file1.Count() == file2.Count() && 1 != 1)
            {
                //List<List<int>> file1List = findLine(File1diff, File2diff);
                var count = 0;
                for (int i = 0; i < File2diff.Count; i++)
                {
                    //int f1 = file1List[1][i];
                    //int f2 = file1List[0][i];

                    List<string> File1WordList;
                    List<string> File2WordList;

                    IEnumerable<string> file1Word = File1diff[i].Trim().Split(' ', '\t');
                    IEnumerable<string> file2Word = File2diff[i].Trim().Split(' ', '\t');
                    File1WordList = file1Word.ToList();
                    File2WordList = file2Word.ToList();

                    if (File1WordList.Count > File2WordList.Count)
                    {
                        int diff = (File1WordList.Count) - (File2WordList.Count);
                        for (int w = 1; w <= diff; w++)
                        {
                            File2WordList.Add("");
                        }
                        for (int k = 0; k < File1WordList.Count; k++)
                        {


                            if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                            }

                            else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true && File2WordList[k] != "")
                            {

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);

                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            }

                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                            }
                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                            {
                                if (File2WordList[k] != "")

                                {
                                    FindPositionList2.Add(File2WordList[k]);
                                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                    //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                    // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                                }
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                            }



                        }
                    }
                    else if (File1WordList.Count < File2WordList.Count)
                    {
                        int diff = (File2WordList.Count) - (File1WordList.Count);
                        for (int w = 1; w <= diff; w++)
                        {
                            File1WordList.Add("");
                        }
                        for (int k = 0; k < File2WordList.Count; k++)
                        {
                            if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                            }

                            else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                                FindPositionList2.Add(File2WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            }

                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true && File1WordList[k] != "")
                            {
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                            }
                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                            {
                                if (File1WordList[k] != "")

                                {
                                    FindPositionList1.Add(File1WordList[k]);
                                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                }


                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                                FindPositionList2.Add(File2WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                            }

                        }
                    }
                    else if (File1WordList.Count == File2WordList.Count)
                    {
                        for (int k = 0; k < File1WordList.Count; k++)
                        {
                            if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                            }

                            else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);

                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            }

                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                            {
                                FindPositionList1.Add(File1WordList[k]);
                                FindPositionList2.Add(File2WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                            }
                            else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                            {

                                FindPositionList2.Add(File2WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                            }


                        }
                    }
                    //if (File1WordList.Count > File2WordList.Count)
                    //{
                    //    for (int k = 0; k < File1WordList.Count; k++)
                    //    {
                    //        if (File2WordList.ElementAtOrDefault(k) != null)
                    //        {

                    //            if ( File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) )
                    //            {
                    //                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                    //                FindPositionList1.Add(File1WordList[k]);
                    //                FindPositionList2.Add(File2WordList[k]);
                    //            }
                    //            else
                    //            {
                    //                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                    //                FindPositionList1.Add(File1WordList[k]);
                    //                FindPositionList2.Add(File2WordList[k]);
                    //                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                    //                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                    //                Console.WriteLine("posi 1 =" + Fil1WordPosition);
                    //                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                    //                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });
                    //                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine("Page = " + page + " Extra  word in 1 file  in   " + i + " line = " + File1WordList[k]);
                    //            FindPositionList1.Add(File1WordList[k]);
                    //            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                    //            Console.WriteLine("posi 1 =" + Fil1WordPosition);
                    //            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });
                    //        }
                    //    }
                    //}
                    //else if (File1WordList.Count < File2WordList.Count)
                    //{
                    //    for (int k = 0; k < File2WordList.Count; k++)
                    //    {
                    //        if (File1WordList.ElementAtOrDefault(k) != null)
                    //        {
                    //            //File1WordList[k] == File2WordList[k]
                    //            if ( File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase))
                    //            {
                    //                Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                    //                FindPositionList1.Add(File1WordList[k]);
                    //                FindPositionList2.Add(File2WordList[k]);
                    //            }
                    //            else
                    //            {
                    //                Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                    //                FindPositionList1.Add(File1WordList[k]);
                    //                FindPositionList2.Add(File2WordList[k]);
                    //                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                    //                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                    //                Console.WriteLine("posi 1 =" + Fil1WordPosition);
                    //                Console.WriteLine("posi 2 =" + Fil2WordPosition);
                    //                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });
                    //                highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine("Page = " + page + " Extra  word in 2 file  in   " + i + " line = " + File2WordList[k]);
                    //            FindPositionList2.Add(File2WordList[k]);
                    //            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                    //            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                    //            highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                    //        }
                    //    }
                    //}
                    //else if (File1WordList.Count == File2WordList.Count)
                    //{
                    //    for (int k = 0; k < File1WordList.Count; k++)
                    //    {
                    //      //  File1WordList[k] == File2WordList[k]
                    //        if ( File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) )
                    //        {
                    //            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                    //            FindPositionList1.Add(File1WordList[k]);
                    //            FindPositionList2.Add(File2WordList[k]);
                    //        }
                    //        else
                    //        {
                    //            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                    //            FindPositionList1.Add(File1WordList[k]);
                    //            FindPositionList2.Add(File2WordList[k]);
                    //            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                    //            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                    //            Console.WriteLine("posi 1 =" + Fil1WordPosition);
                    //            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                    //            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });
                    //            highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                    //        }
                    //    }
                    //}
                    count++;
                }
                for (int i = count; i < File1diff.Count; i++)
                {
                    List<string> File1ExtraWord = File1diff[i].Trim().Split(' ', '\t').ToList();
                    for (int k = 0; k < File1ExtraWord.Count; k++)
                    {
                        Console.WriteLine("Page = " + page + " Extra content in File 1 " + File1ExtraWord[k]);
                        FindPositionList1.Add(File1ExtraWord[k]);
                        int Fil1WordPosition = wordPosition(File1ExtraWord[k], FindPositionList1.Count, FindPositionList1);
                        Console.WriteLine("posi 1 =" + Fil1WordPosition);
                        highlightPDF1.Add(new HighlightViewModel() { Word = File1ExtraWord[k], PageNo = page, PositionNo = Fil1WordPosition });
                    }
                }
            }

            //if (file1.Count() < file2.Count())
            //{


                #region comment


                //    List<List<int>> file1List= findLine(File1diff, File2diff);
                //  // List<List<int>> file2List = findLine(File2diff, File1diff);
                //    var count = 0;
                //    for (int i = 0; i < file1.Count; i++)
                //    {
                //        int f1 = file1List[0][i];
                //        int f2= file1List[1][i];


                //        List<string> File1WordList;
                //        List<string> File2WordList;
                //        IEnumerable<string> file1Word = File1diff[f1].Trim().Split(' ', '\t');
                //        IEnumerable<string> file2Word = File2diff[f2].Trim().Split(' ', '\t');
                //        File1WordList = file1Word.ToList();
                //        File1WordList = File1WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                //        File2WordList = file2Word.ToList();
                //        File2WordList = File2WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                //        if (File1WordList.Count > File2WordList.Count)
                //        {
                //            int diff = (File1WordList.Count) - (File2WordList.Count);
                //            for (int w = 1; w <= diff; w++)
                //            {
                //                File2WordList.Add("");
                //            }
                //            for (int k = 0; k < File1WordList.Count; k++)
                //            {


                //                if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                //                {
                //                    Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);
                //                }

                //                else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true && File2WordList[k] != "")
                //                {

                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);

                //                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                //                }

                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                //                {
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);
                //                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                //                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                //                }
                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                //                {
                //                    if (File2WordList[k] != "")

                //                    {
                //                        FindPositionList2.Add(File2WordList[k]);
                //                        int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                        //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                        // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                        highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                //                    }
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                //                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                //                }



                //            }
                //        }
                //        else if (File1WordList.Count < File2WordList.Count)
                //        {
                //            int diff = (File2WordList.Count) - (File1WordList.Count);
                //            for (int w = 1; w <= diff; w++)
                //            {
                //                File1WordList.Add("");
                //            }
                //            for (int k = 0; k < File2WordList.Count; k++)
                //            {
                //                if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                //                {
                //                    Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);
                //                }

                //                else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                //                {

                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                //                    FindPositionList2.Add(File2WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                //                }

                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true && File1WordList[k] != "")
                //                {
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                //                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                //                }
                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                //                {
                //                    if (File1WordList[k] != "")

                //                    {
                //                        FindPositionList1.Add(File1WordList[k]);
                //                        int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                //                        highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                //                    }


                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                //                    FindPositionList2.Add(File2WordList[k]);
                //                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                    //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                    // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                //                }

                //            }
                //        }
                //        else if (File1WordList.Count == File2WordList.Count)
                //        {
                //            for (int k = 0; k < File1WordList.Count; k++)
                //            {
                //                if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                //                {
                //                    Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);
                //                }

                //                else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                //                {

                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);

                //                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                //                }

                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                //                {
                //                    FindPositionList1.Add(File1WordList[k]);
                //                    FindPositionList2.Add(File2WordList[k]);
                //                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                //                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                //                }
                //                else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                //                {

                //                    FindPositionList2.Add(File2WordList[k]);
                //                    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                //                    //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                //                    // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //                    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                //                    FindPositionList1.Add(File1WordList[k]);
                //                    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                //                    highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                //                    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                //                }


                //            }
                //        }


                //        count++;
                //    }
                //    for (int i = count; i < File2diff.Count; i++)
                //    {
                //        List<string> File2ExtraWord = File2diff[i].Trim().Split(' ', '\t').ToList();
                //        File2ExtraWord = File2ExtraWord.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                //        for (int k = 0; k < File2ExtraWord.Count; k++)
                //        {
                //            Console.WriteLine("Page = " + page + " Extra content in File 2 " + File2ExtraWord[k]);
                //            FindPositionList2.Add(File2ExtraWord[k]);
                //            int Fil2WordPosition = wordPosition(File2ExtraWord[k], FindPositionList2.Count, FindPositionList2);
                //            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                //            highlightPDF2.Add(new HighlightViewModel() { Word = File2ExtraWord[k], PageNo = page, PositionNo = Fil2WordPosition });
                //        }
                //    }
                #endregion comment

              //  Diffreanciatefile1(File1diff, File2diff, page, FindPositionList1, FindPositionList2);
             //   Diffreanciatefile2(File2diff, File1diff, page, FindPositionList1, FindPositionList2);



            // }

        }


        public static void Diffreanciatefile1(List<string> File1diff, List<string> File2diff,int page,List<string> FindPositionList1, List<string> FindPositionList2)
        {
            FindPositionList1 = new List<string>();
            FindPositionList2 = new List<string>();
            List<List<int>> file1List = findLine(File1diff, File2diff);
            // List<List<int>> file2List = findLine(File2diff, File1diff);
            var count = 0;
            for (int i = 0; i < File1diff.Count; i++)
            {
                int f1 = file1List[0][i];
                int f2 = file1List[1][i];


                List<string> File1WordList;
                List<string> File2WordList;
                IEnumerable<string> file1Word = File1diff[f1].Trim().Split(' ', '\t');
                IEnumerable<string> file2Word = File2diff[f2].Trim().Split(' ', '\t');
                File1WordList = file1Word.ToList();
                File1WordList = File1WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                File2WordList = file2Word.ToList();
                File2WordList = File2WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                if (File1WordList.Count > File2WordList.Count)
                {
                    int diff = (File1WordList.Count) - (File2WordList.Count);
                    for (int w = 1; w <= diff; w++)
                    {
                        File2WordList.Add("");
                    }
                    for (int k = 0; k < File1WordList.Count; k++)
                    {


                        if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                        }

                        else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true && File2WordList[k] != "")
                        {

                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);

                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                          //  highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        }

                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        {
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        }
                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        {
                            if (File2WordList[k] != "")

                            {
                                FindPositionList2.Add(File2WordList[k]);
                                int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                                //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                                // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                            //    highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                            }
                            FindPositionList1.Add(File1WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                        }



                    }
                }
                else if (File1WordList.Count < File2WordList.Count)
                {
                    int diff = (File2WordList.Count) - (File1WordList.Count);
                    for (int w = 1; w <= diff; w++)
                    {
                        File1WordList.Add("");
                    }
                    for (int k = 0; k < File2WordList.Count; k++)
                    {
                        if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                        }

                        else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        {

                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                            FindPositionList2.Add(File2WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                          //  highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        }

                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true && File1WordList[k] != "")
                        {
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        }
                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        {
                            if (File1WordList[k] != "")

                            {
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                            }


                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                            FindPositionList2.Add(File2WordList[k]);
                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                          //  highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                        }

                    }
                }
                else if (File1WordList.Count == File2WordList.Count)
                {
                    for (int k = 0; k < File1WordList.Count; k++)
                    {
                        if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                        }

                        else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        {

                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);

                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            Console.WriteLine("posi 2 =" + Fil2WordPosition);
                         //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        }

                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        {
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        }
                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        {

                            FindPositionList2.Add(File2WordList[k]);
                            int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                       //     highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                            FindPositionList1.Add(File1WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                            highlightPDF1.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                            Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                        }


                    }
                }


                count++;
            }

           

            for (int i = count; i < File2diff.Count; i++)
            {
                List<string> File2ExtraWord = File2diff[i].Trim().Split(' ', '\t').ToList();
                File2ExtraWord = File2ExtraWord.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                for (int k = 0; k < File2ExtraWord.Count; k++)
                {
                    Console.WriteLine("Page = " + page + " Extra content in File 2 " + File2ExtraWord[k]);
                    FindPositionList2.Add(File2ExtraWord[k]);
                    int Fil2WordPosition = wordPosition(File2ExtraWord[k], FindPositionList2.Count, FindPositionList2);
                    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                  //  highlightPDF2.Add(new HighlightViewModel() { Word = File2ExtraWord[k], PageNo = page, PositionNo = Fil2WordPosition });
                }
            }


           


        }


        public static void Diffreanciatefile2(List<string> File1diff, List<string> File2diff, int page, List<string> FindPositionList1, List<string> FindPositionList2)
        {
            FindPositionList1 = new List<string>();
            FindPositionList2 = new List<string>();

            List<List<int>> file1List = findLine(File1diff, File2diff);
            // List<List<int>> file2List = findLine(File2diff, File1diff);
            var count = 0;
            for (int i = 0; i < File1diff.Count; i++)
            {
                int f1 = file1List[0][i];
                int f2 = file1List[1][i];


                List<string> File1WordList;
                List<string> File2WordList;
                IEnumerable<string> file1Word = File1diff[f1].Trim().Split(' ', '\t');
                IEnumerable<string> file2Word = File2diff[f2].Trim().Split(' ', '\t');
                File1WordList = file1Word.ToList();
                File1WordList = File1WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                File2WordList = file2Word.ToList();
                File2WordList = File2WordList.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                if (File1WordList.Count > File2WordList.Count)
                {
                    int diff = (File1WordList.Count) - (File2WordList.Count);
                    for (int w = 1; w <= diff; w++)
                    {
                        File2WordList.Add("");
                    }
                    for (int k = 0; k < File1WordList.Count; k++)
                    {


                        if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) )
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                           // FindPositionList2.Add(File2WordList[k]);
                        }

                       // else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true && File2WordList[k] != "")
                       // {

                       //     Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                       //     FindPositionList1.Add(File1WordList[k]);
                       //     FindPositionList2.Add(File2WordList[k]);

                       //     int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                       //     // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                       //     Console.WriteLine("posi 2 =" + Fil2WordPosition);
                       ////     highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                       // }

                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false )
                        {
                            FindPositionList1.Add(File1WordList[k]);
                            FindPositionList2.Add(File2WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                            highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        }
                        //else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        //{
                        //    if (File2WordList[k] != "")

                        //    {
                        //        FindPositionList2.Add(File2WordList[k]);
                        //        int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                        //        //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                        //        // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                        //     //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });

                        //    }
                        //    FindPositionList1.Add(File1WordList[k]);
                        //    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                        //    highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                        //    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                        //}



                    }
                }
                else if (File1WordList.Count < File2WordList.Count)
                {
                    int diff = (File2WordList.Count) - (File1WordList.Count);
                    for (int w = 1; w <= diff; w++)
                    {
                        File1WordList.Add("");
                    }
                    for (int k = 0; k < File2WordList.Count; k++)
                    {
                        if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                           // FindPositionList2.Add(File2WordList[k]);
                        }

                        //else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        //{

                        //    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                        //    FindPositionList2.Add(File2WordList[k]);
                        //    FindPositionList1.Add(File1WordList[k]);
                        //    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                        //    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                        //    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                        // //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        //}

                        //else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == true && File1WordList[k] != "")
                        //{
                        //    FindPositionList1.Add(File1WordList[k]);
                        //    FindPositionList1.Add(File1WordList[k]);
                        //    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                        //    highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        //}
                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        {
                            if (File1WordList[k] != "")

                            {
                                FindPositionList1.Add(File1WordList[k]);
                                int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                                highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                            }


                            //Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);

                            //FindPositionList2.Add(File2WordList[k]);
                            //int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                            //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                            // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                         //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });
                        }

                    }
                }
                else if (File1WordList.Count == File2WordList.Count)
                {
                    for (int k = 0; k < File1WordList.Count; k++)
                    {
                        if ( File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Page = " + page + " Match word in " + i + " line = " + File1WordList[k]);
                            FindPositionList1.Add(File1WordList[k]);
                           // FindPositionList2.Add(File2WordList[k]);
                        }

                        //else if (File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false && File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == true)
                        //{

                        //    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                        //    FindPositionList1.Add(File1WordList[k]);
                        //    FindPositionList2.Add(File2WordList[k]);

                        //    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                        //    // Console.WriteLine("posi 1 =" + Fil1WordPosition);
                        //    Console.WriteLine("posi 2 =" + Fil2WordPosition);
                        // //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        //}

                        else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false )
                        {
                            FindPositionList1.Add(File1WordList[k]);
                           // FindPositionList2.Add(File2WordList[k]);
                            int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);

                            highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });


                        }
                        //else if (File2WordList.Contains(File1WordList[k], StringComparer.OrdinalIgnoreCase) == false && File1WordList.Contains(File2WordList[k], StringComparer.OrdinalIgnoreCase) == false)
                        //{

                        //    FindPositionList2.Add(File2WordList[k]);
                        //    int Fil2WordPosition = wordPosition(File2WordList[k], FindPositionList2.Count, FindPositionList2);
                        //    //   Console.WriteLine("posi 1 =" + Fil1WordPosition);
                        //    // Console.WriteLine("posi 2 =" + Fil2WordPosition);
                        // //   highlightPDF2.Add(new HighlightViewModel() { Word = File2WordList[k], PageNo = page, PositionNo = Fil2WordPosition });


                        //    FindPositionList1.Add(File1WordList[k]);
                        //    int Fil1WordPosition = wordPosition(File1WordList[k], FindPositionList1.Count, FindPositionList1);
                        //    highlightPDF2.Add(new HighlightViewModel() { Word = File1WordList[k], PageNo = page, PositionNo = Fil1WordPosition });

                        //    Console.WriteLine("Page = " + page + " Diff word in " + i + " line = " + File1WordList[k] + " and " + File2WordList[k]);
                        //}


                    }
                }


                count++;
            }



            for (int i = count; i < File1diff.Count; i++)
            {
                List<string> File1ExtraWord = File1diff[i].Trim().Split(' ', '\t').ToList();
                File1ExtraWord = File1ExtraWord.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

                for (int k = 0; k < File1ExtraWord.Count; k++)
                {
                    Console.WriteLine("Page = " + page + " Extra content in File 2 " + File1ExtraWord[k]);
                    FindPositionList1.Add(File1ExtraWord[k]);
                    int Fil1WordPosition = wordPosition(File1ExtraWord[k], FindPositionList1.Count, FindPositionList1);
                    Console.WriteLine("posi 2 =" + Fil1WordPosition);
                       highlightPDF2.Add(new HighlightViewModel() { Word = File1ExtraWord[k], PageNo = page, PositionNo = Fil1WordPosition });
                }
            }





        }

        public static List<List<int>> findLine(List<string> oldfile1,List<String> oldfile2)
        {
            List<string> file1 = oldfile1;
            List<string> file2 = oldfile2;
            List<int> file1Index = new List<int>();
            List<int> file2Index = new List<int>();
            if(file1.Count>file2.Count)
            {
                int diff = (file1.Count) - (file2.Count);
                for (int w = 1; w <= diff; w++)
                {
                    file2.Add("");
                }
            }
            else if(file1.Count < file2.Count)
            {
                int diff = (file2.Count) - (file1.Count);
                for (int w = 1; w <= diff; w++)
                {
                    file1.Add("");
                }
            }


            for(int i=0;i<file1.Count;i++)
            {
               
                List<string> file1line = file1[i].Trim().Split(' ', '\t').ToList();
                file1line = file1line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                if (i == 0)
                {

                    List<string> file2line = file2[i].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count1 = occurrenceCount(file1line, file2line);

                    file2line = new List<string>();
                    file2line = file2[i +1].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count2 = occurrenceCount(file1line, file2line);

                    file2line = new List<string>();
                    file2line = file2[i + 2].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count3 = occurrenceCount(file1line, file2line);


                    if (count3 > count2)
                    {
                        if (count3 > count1)
                        {
                            Console.Write("Number three is the largest!\n");
                            file1Index.Add(i);
                            file2Index.Add(i+2);
                        }
                        else
                        {
                            Console.Write("Number one is the largest!\n");
                            file1Index.Add(i);
                            file2Index.Add(i);
                        }
                    }
                    else if (count2 > count1)
                    {
                        Console.Write("Number two is the largest!\n");
                        file1Index.Add(i);
                        file2Index.Add(i + 1);
                    }
                    else
                    {
                        Console.Write("Number one is the largest!\n");
                        file1Index.Add(i);
                        file2Index.Add(i);
                    }
                
                        


                }
                else if (i == file1.Count - 1)
                {
                   
                    List<string> file2line = file2[i].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count1 = occurrenceCount(file1line, file2line);

                    file2line = new List<string>();
                    
                    file2line = file2[i-1].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count2 = occurrenceCount(file1line, file2line);

                    file2line = new List<string>();
                    file2line = file2[i - 2].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count3 = occurrenceCount(file1line, file2line);



                    if (count1 > count2)
                    {
                        if (count1 > count3)
                        {
                            Console.Write("Number one is the largest!\n");
                            file1Index.Add(i);
                            file2Index.Add(i);
                        }
                        else
                        {
                            Console.Write("Number three is the largest!\n");
                            file1Index.Add(i);
                            file2Index.Add(i - 2);
                        }
                    }
                    else if (count2 > count3)
                    {
                        Console.Write("Number two is the largest!\n");
                        file1Index.Add(i);
                        file2Index.Add(i - 1);
                    }
                    else
                    {
                        Console.Write("Number three is the largest!\n");
                        file1Index.Add(i);
                        file2Index.Add(i - 2);
                    }



                }
                else
                {

                    List<string> file2line = file2[i].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count1 = occurrenceCount(file1line, file2line);

                    file2line = new List<string>();
                    file2line = file2[i - 1].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count2 = occurrenceCount(file1line, file2line);

                    int count3 = 0;
                    if (i >= 2)
                    {
                        file2line = new List<string>();
                        file2line = file2[i - 2].Trim().Split(' ', '\t').ToList();
                        file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        count3 = occurrenceCount(file1line, file2line);
                    }
                   
                        

                    file2line = new List<string>();
                    file2line = file2[i + 1].Trim().Split(' ', '\t').ToList();
                    file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                    int count4 = occurrenceCount(file1line, file2line);

                    int count5 = 0;
                    if (i <= file1.Count - 3)
                    {
                        file2line = new List<string>();
                        file2line = file2[i + 2].Trim().Split(' ', '\t').ToList();
                        file2line = file2line.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        count5 = occurrenceCount(file1line, file2line);
                    }



                    if (count5 > count2 && count5 > count3 && count5 > count4 && count5 > count1  )
                    {
                        file1Index.Add(i);
                        file2Index.Add(i+2);
                    }
                    else if (count2 > count1 && count2 > count3 && count2 > count4 && count2 > count5)
                    {
                        file1Index.Add(i);
                        file2Index.Add(i-1);
                    }
                    else if (count3 > count1 && count3 > count2 && count3 > count4 && count3 > count5)
                    {
                        file1Index.Add(i);
                        file2Index.Add(i-2);
                    }
                    else if (count4 > count1 && count4 > count2 && count4 > count3 && count4 > count5)
                    {
                        file1Index.Add(i);
                        file2Index.Add(i+1);
                    }
                    else
                    {
                        file1Index.Add(i);
                        file2Index.Add(i);
                    }

                }
            }


            return new List<List<int>> { file1Index, file2Index };
        }

        public static int occurrenceCount(List<string> line1,List<string> line2)
        {
            if (line1.Count > line2.Count)
            {
                int diff = (line1.Count) - (line2.Count);
                for (int w = 1; w <= diff; w++)
                {
                    line2.Add("");
                }
            }
            else if (line1.Count < line2.Count)
            {
                int diff = (line2.Count) - (line1.Count);
                for (int w = 1; w <= diff; w++)
                {
                    line1.Add("");
                }
            }
            int counter=0;
            for(int i=0;i < line1.Count; i++)
            {
                if (line1[i] != "")
                {
                    if (line2.Contains(line1[i], StringComparer.CurrentCultureIgnoreCase))
                    {
                        counter++;
                    }
                }
            }

            return counter;
        }

        public static int wordPosition(string word, int lineNumber, List<string> allLines)
        {

            
            int counter = 0;
            for (int i = 0;  i < lineNumber; i++)
            {
                string temp = allLines[i];
                    
             // temp = temp.ToLower();
                //word = word.ToLower();
                try
                {
                 // var a=  Regex.Match(temp, word, RegexOptions.Singleline);
                   var a  = string.Equals(temp,word);

                    if (a == true)
                    {
                        counter += 1;
                    }
                }
                catch (Exception ex)
                {
                    // Console.WriteLine("error "+ word);
                }

            }
            return counter;
        }

        #region OldCode
        //public static void TextHighlight(string newFile, string oldFile, List<HighlightViewModel> info, CordsModel cords)
        //{
        //    Console.WriteLine("\n Highliting All Marked Text...");


        //    PdfReader reader = new PdfReader(oldFile);



        //    using (FileStream fs = new FileStream(newFile, FileMode.Create, FileAccess.Write, FileShare.None))
        //    {
        //        using (PdfStamper stamper = new PdfStamper(reader, fs))
        //        {
        //            int minCount = 0;
        //            if (info.Count > cords.left.Count)
        //                minCount = cords.left.Count;
        //            else
        //                minCount = info.Count;

        //            for (int i = 0; i < minCount; i++)
        //            {


        //                //Create a rectangle for the highlight. NOTE: Technically this isn't used but it helps with the quadpoint calculation
        //                iTextSharp.text.Rectangle rect = new iTextSharp.text.Rectangle(cords.left[i], cords.top[i], cords.right[i], cords.bottom[i]);
        //                //Create an array of quad points based on that rectangle. NOTE: The order below doesn't appear to match the actual spec but is what Acrobat produces
        //                float[] quad = { rect.Left, rect.Bottom, rect.Right, rect.Bottom, rect.Left, rect.Top, rect.Right, rect.Top };

        //                //Create our hightlight
        //                PdfAnnotation highlight = PdfAnnotation.CreateMarkup(stamper.Writer, rect, null, PdfAnnotation.MARKUP_HIGHLIGHT, quad);

        //                //Set the color
        //                highlight.Color = BaseColor.RED;

        //                //Add the annotation
        //                stamper.AddAnnotation(highlight, info[i].PageNo);
        //            }
        //        }
        //    }
        //    Console.Write(" Complete");
        //}


        //public static CordsModel TextPosition(string inputFile, List<HighlightViewModel> info)
        //{
        //    Console.WriteLine("\n Fetching Text Position...");
        //    CordsModel cords = new CordsModel();

        //    List<float> top = new List<float>();
        //    List<float> left = new List<float>();
        //    List<float> right = new List<float>();
        //    List<float> bottom = new List<float>();


        //    using (var doc = Patagames.Pdf.Net.PdfDocument.Load(inputFile))
        //    {

        //        for (int i = 0; i < info.Count; i++)
        //        {
        //            int count = 0;
        //            var page = doc.Pages[info[i].PageNo - 1];

        //            var found = page.Text.Find(info[i].Word, FindFlags.MatchWholeWord, 0);

        //            if (found != null)
        //            {

        //                do
        //                {
        //                    count++;
        //                    if (count == info[i].PositionNo)
        //                    {
        //                        var textInfo = found.FoundText;
        //                        foreach (var rect in textInfo.Rects)
        //                        {
        //                            left.Add(rect.left);
        //                            top.Add(rect.top);
        //                            right.Add(rect.right);
        //                            bottom.Add(rect.bottom);

        //                        }
        //                    }
        //                } while (found.FindNext());
        //            }
        //            else
        //            {
        //                float x = 0.0F;
        //                left.Add(x);
        //                top.Add(x);
        //                right.Add(x);
        //                bottom.Add(x);
        //            }
        //            page.Dispose();
        //        }

        //        cords.top = top;
        //        cords.left = left;
        //        cords.right = right;
        //        cords.bottom = bottom;
        //    }

        //    Console.WriteLine(" Complete");
        //    return cords;

        //}
        #endregion

        private static void SplitAndSaveInterval(string pdfFilePath, string outputPath, int startPage, int interval, string pdfFileName)
        {
            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document();
                PdfCopy copy = new PdfCopy(document, new FileStream(outputPath + "\\" + pdfFileName + ".pdf", FileMode.Create));
                document.Open();

                for (int pagenumber = startPage; pagenumber < (startPage + interval); pagenumber++)
                {
                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                }

                document.Close();
            }
        }

        public static void SplitPDF(string pdfFilePath, string outputPath)
        {
            
            // string outputPath = @"F:\Testfiles\split";
            int interval = 10;
            int pageNameSuffix = 0;

            // Intialize a new PdfReader instance with the contents of the source Pdf file:  
            PdfReader reader = new PdfReader(pdfFilePath);

            FileInfo file = new FileInfo(pdfFilePath);
            string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";



            for (int pageNumber = 1; pageNumber <= reader.NumberOfPages; pageNumber += interval)
            {
                pageNameSuffix++;
                //   string newPdfFileName = string.Format(pdfFileName + "{0}", pageNameSuffix);
                string newPdfFileName = string.Format(pageNameSuffix.ToString());
                SplitAndSaveInterval(pdfFilePath, outputPath, pageNumber, interval, newPdfFileName);
            }
        }


        public static void HighlightSpire(string inputfile, string outputfile, List<HighlightViewModel> info)
        {
           // inputfile = @"F:\Testfiles\10page.pdf";
            Spire.Pdf.PdfDocument pdf = new Spire.Pdf.PdfDocument(inputfile);
            
            Spire.Pdf.General.Find.PdfTextFind[] result = null;

            int c = 0;
         for(int i=0;i<info.Count;i++)
            {
                Console.Write("\n"+c++ + " out of " + info.Count + "in" + outputfile);
                int count = 0;

                try
                {
                    String word = Regex.Unescape(info[i].Word);
                    result = pdf.Pages[info[i].PageNo-1].FindText(word, true).Finds;

                if(info[i].PositionNo==0)
                {
                    int abc = 0;
                }
                foreach (Spire.Pdf.General.Find.PdfTextFind find in result)
                {
                    count++;

                    if(count== info[i].PositionNo)
                    find.ApplyHighLight(Color.Red);

                }

                }
                catch
                {
                    int abc = 0;

                }

            }
            //foreach (Spire.Pdf.PdfPageBase page in pdf.Pages)
                
            //    {
                
            //         result = page.FindText("The",true).Finds;
                
            //         foreach (Spire.Pdf.General.Find.PdfTextFind find in result)
                    
            //          {
                    
            //             find.ApplyHighLight(Color.Red);
                  
            //          }
                
            //    }
            
                pdf.SaveToFile(outputfile);
            

        }


        public static void PDFToImage(string file, string outputPath, int imageCount,int k)
        {
           if(k==1)
            {
                 imagePDFCount1 = +1;
                imageCount = imagePDFCount1;
            }
           else
            {
                 imagePDFCount2 = +1;
                imageCount = imagePDFCount2;
            }
            PdfReader pdfReader = new PdfReader(file);
            int numberOfPages = pdfReader.NumberOfPages;
            pdfReader.Close();
            pdfReader.Dispose();

            for(int i=1;i<=numberOfPages;i++)
            {
                GhostscriptWrapper.GeneratePageThumbs(file, outputPath+@"\"+i+".jpeg", i, numberOfPages, 130, 130);
            }
            

        }

        public static void Deletefile(string file)
        {
            try
            {
                // Check if file exists with its full path    
                if (File.Exists(file))
                {
                    // If file found, delete it    
                    File.Delete(file);
                  //  Console.WriteLine("File deleted.");
                }
                //else Console.WriteLine("File not found");
            }
            catch (Exception ex)
            {
                
            }
        }

        public static void ImageToPDF()
        {
            var pgSize = new iTextSharp.text.Rectangle(800f, 500f);
            iTextSharp.text.Document doc = new iTextSharp.text.Document(pgSize, 0f, 0f, 0f, 0f);

            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;

            var directory = System.IO.Path.GetDirectoryName(path);

            PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(directory + @"\PDFData\FinalPDF\Result.pdf", FileMode.Create));

            doc.Open();

            try

            {
               
                string[] image1Array = Directory.GetFiles(directory + @"\PDFData\PDFImage1\", "*.jpeg");
                string[] image2Array = Directory.GetFiles(directory + @"\PDFData\PDFImage2\", "*.jpeg");

                if (image1Array.Count() > image2Array.Count())
                {

                    for (int i = 0; i < image2Array.Count(); i++)
                    {

                        string imageURL = image1Array[i];

                        string imageURL2 = image2Array[i];

                        Bitmap b = MergeTwoImages(imageURL, imageURL2);
                        System.Drawing.Image img = (System.Drawing.Image)b;

                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                        jpg.ScaleToFit(800f, 500f);
                        jpg.Alignment = Element.ALIGN_LEFT;


                        doc.Add(jpg);

                        Deletefile(imageURL);
                        Deletefile(imageURL2);


                    }
                    for (int i = image2Array.Count(); i < image1Array.Count(); i++)
                    {
                        string imageURL = image1Array[i];
                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                        jpg.ScaleToFit(400f, 500f);
                        jpg.Alignment = Element.ALIGN_LEFT;

                        doc.Add(jpg);
                        Deletefile(imageURL);
                    }

                }
                else if (image1Array.Count() < image2Array.Count())
                {
                    for (int i = 0; i < image1Array.Count(); i++)
                    {

                        string imageURL = image1Array[i];

                        string imageURL2 = image2Array[i];

                        Bitmap b = MergeTwoImages(imageURL, imageURL2);
                        System.Drawing.Image img = (System.Drawing.Image)b;

                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                        jpg.ScaleToFit(800f, 500f);
                        jpg.Alignment = Element.ALIGN_LEFT;


                        doc.Add(jpg);
                       
                        writer.Close();
                        writer.Dispose();
                        Deletefile(imageURL);
                        Deletefile(imageURL2);

                    }
                    for (int i = image1Array.Count(); i < image2Array.Count(); i++)
                    {
                        string imageURL = image2Array[i];
                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(imageURL);
                        jpg.ScaleToFit(400f, 500f);
                        jpg.Alignment = Element.ALIGN_RIGHT;

                        doc.Add(jpg);
                        Deletefile(imageURL);
                    }
                }
                else
                {
                    for (int i = 0; i < image1Array.Count(); i++)
                    {
                        string imageURL = image1Array[i];

                        string imageURL2 = image2Array[i];

                        Bitmap b = MergeTwoImages(imageURL, imageURL2);
                        System.Drawing.Image img = (System.Drawing.Image)b;

                        iTextSharp.text.Image jpg = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Jpeg);
                        jpg.ScaleToFit(800f, 500f);
                        jpg.Alignment = Element.ALIGN_LEFT;


                        doc.Add(jpg);
                        Deletefile(imageURL);
                        Deletefile(imageURL2);
                    }

                }



            }

            catch (Exception ex)

            { }

            finally

            {

                doc.Close();

            }
        }

        public static System.Drawing.Bitmap MergeTwoImages(string Image1, string Image2)
        {
            System.Drawing.Image firstImage = System.Drawing.Image.FromFile(Image1);
            System.Drawing.Image secondImage = System.Drawing.Image.FromFile(Image2);

            Bitmap bitmap = new Bitmap(firstImage.Width + secondImage.Width, Math.Max(firstImage.Height, secondImage.Height));
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(firstImage, new System.Drawing.Rectangle(new Point(), firstImage.Size),
                        new System.Drawing.Rectangle(new Point(), firstImage.Size), GraphicsUnit.Pixel);

                g.DrawImage(secondImage, new System.Drawing.Rectangle(new Point(firstImage.Width), secondImage.Size),
                       new System.Drawing.Rectangle(new Point(), secondImage.Size), GraphicsUnit.Pixel);
                //   g.DrawImage(secondImage, firstImage.Width, 0);

            }

            firstImage.Dispose();
            secondImage.Dispose();
            return bitmap;
        }

    }

    public class CordsModel
    {
        public List<float> left;
        public List<float> right;
        public List<float> top;
        public List<float> bottom;


    }

    public class TextInfo
    {
        public string Word { get; set; }
        public int PageNo { get; set; }
        public int PositionNo { get; set; }
    }
}
