using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using LiteDB;

namespace izmirvapur
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://www.izdeniz.com.tr/tr/anasayfa";
            string htmlContent = GetContent(url);


            List<Line> LinesList = new List<Line>();

            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlContent);
            var nodes = doc.DocumentNode.SelectNodes("//option");

            for (int v = 0; v < 10; v++)
            {
                string name = nodes[v].NextSibling.InnerText;
                string no = nodes[v].Attributes["value"].Value;
                Line vapLine = new Line { lineNo = no, lineName = name };

                LinesList.Add(vapLine);
                add2linedb(vapLine);
            }
            for( int v = 0; v < LinesList.Count; v++)
            {
                Console.WriteLine(LinesList[v].lineNo);
                Console.WriteLine(LinesList[v].lineName);
                
            }
            Console.ReadLine();
            Console.WriteLine("Hat db hazır.");
            Console.ReadLine();
            

           
            List<Vapur> VapurlarList = new List<Vapur>();

            
                foreach (var line1 in LinesList)
                {
                    foreach (var line2 in LinesList)
                    {
                        Vapur nextVapur = new Vapur();
                        nextVapur.gunler = new List<Gunler>();
                    
                    nextVapur.vapurNo = line1.lineNo;
                    nextVapur.vapurName = line1.lineName + "-" + line2.lineName; 

                        int gunno = 0;
                        //HAFTAICI
                        gunno = 1;
                        if (gunno == 1)
                        {
                            string url2 = "http://www.izdeniz.com.tr/tr/HareketSaatleri/" + line1.lineNo + "/" + line2.lineNo + "/" + gunno;
                            string htmlContent2 = GetContent(url2);

                            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                            doc2.LoadHtml(htmlContent2);

                            if (gunno == 1)
                            {
                                Console.WriteLine("Haftaiçi");
                            }


                            Gunler HaftaIci = new Gunler();
                            HaftaIci.KalkisSaatler = new List<string>();
                            HaftaIci.VarisSaatler = new List<string>();

                            var hatNodes = doc2.DocumentNode.SelectNodes(@"//div[@class=""hat-tip""]");   //
                                                                                                          // Hat Bilgisi. Bak
                            string hatti = hatNodes[0].InnerText;                                         //

                            Console.WriteLine(hatti);



                            if (doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]") != null)
                            {
                                var durakNodes = doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]");
                                string kduragi = durakNodes.FirstOrDefault().InnerText; // [0].InnerText;
                                string vduragi = durakNodes.LastOrDefault().InnerText;
                                Console.WriteLine("------");
                                Console.WriteLine(kduragi);
                                HaftaIci.Gun = "Haftaiçi";
                                HaftaIci.Hat = hatti;
                                HaftaIci.KalkisDuragi = kduragi;
                                HaftaIci.VarisDuragi = vduragi;

                                var saatNodes = doc2.DocumentNode.SelectNodes(@"//tr[@class=""""]");
                                if (saatNodes != null)
                                {
                                    foreach (var node in saatNodes)
                                    {

                                        string nodeshtml = node.InnerHtml;
                                        var htmlDocnode = new HtmlDocument();
                                        htmlDocnode.LoadHtml(nodeshtml);
                                        var partsaatNodes = htmlDocnode.DocumentNode.SelectNodes(@"//td[@class=""sefertd""]");

                                        HaftaIci.KalkisSaatler.Add(partsaatNodes.FirstOrDefault().InnerText);
                                        HaftaIci.VarisSaatler.Add(partsaatNodes.LastOrDefault().InnerText);
                                    }

                                    nextVapur.gunler.Add(HaftaIci);
                                } // else 2/10a göre düzenlenecek



                                //Saatler ekrana yazdırılıyor.
                                for (int m = 0; m < HaftaIci.KalkisSaatler.Count; m++)
                                {
                                    Console.Write("Kalkış:");
                                    Console.WriteLine(HaftaIci.KalkisSaatler[m]);

                                }
                                Console.WriteLine("------");
                                Console.WriteLine(vduragi);

                                for (int n = 0; n < HaftaIci.VarisSaatler.Count; n++)
                                {
                                    Console.Write("Varış:");
                                    Console.WriteLine(HaftaIci.VarisSaatler[n]);

                                }
                                // Console.ReadLine();
                            }

                        } //HAFTAICI SONU

                        //CUMARTESI
                        gunno = 2;
                        if (gunno == 2)
                        {
                            string url2 = "http://www.izdeniz.com.tr/tr/HareketSaatleri/" + line1.lineNo + "/" + line2.lineNo + "/" + gunno;      //LinkSonu değer günü veriyor.      
                            string htmlContent2 = GetContent(url2);

                            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                            doc2.LoadHtml(htmlContent2);

                            if (gunno == 2)
                            {
                                Console.WriteLine("Cumartesi");
                            }


                            Gunler Cumartesi = new Gunler();
                            Cumartesi.KalkisSaatler = new List<string>();
                            Cumartesi.VarisSaatler = new List<string>();

                            var hatNodes = doc2.DocumentNode.SelectNodes(@"//div[@class=""hat-tip""]");   //
                                                                                                          // Hat Bilgisi. Bak
                            string hatti = hatNodes[0].InnerText;                                         //

                            Console.WriteLine(hatti);



                            if (doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]") != null)
                            {
                                var durakNodes = doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]");
                                string kduragi = durakNodes.FirstOrDefault().InnerText; // [0].InnerText;
                                string vduragi = durakNodes.LastOrDefault().InnerText;
                                Console.WriteLine("------");
                                Console.WriteLine(kduragi);
                                Cumartesi.Gun = "Cumartesi";
                                Cumartesi.Hat = hatti;
                                Cumartesi.KalkisDuragi = kduragi;
                                Cumartesi.VarisDuragi = vduragi;

                                var saatNodes = doc2.DocumentNode.SelectNodes(@"//tr[@class=""""]");

                                if (saatNodes != null)
                                {
                                    foreach (var node in saatNodes)
                                    {
                                        string nodeshtml = node.InnerHtml;
                                        var htmlDocnode = new HtmlDocument();
                                        htmlDocnode.LoadHtml(nodeshtml);
                                        var partsaatNodes = htmlDocnode.DocumentNode.SelectNodes(@"//td[@class=""sefertd""]");

                                        Cumartesi.KalkisSaatler.Add(partsaatNodes.FirstOrDefault().InnerText);
                                        Cumartesi.VarisSaatler.Add(partsaatNodes.LastOrDefault().InnerText);
                                    }

                                    nextVapur.gunler.Add(Cumartesi);
                                } // else 2/10a göre düzenlenecek


                                //Saatler ekrana yazdırılıyor.
                                for (int m = 0; m < Cumartesi.KalkisSaatler.Count; m++)
                                {
                                    Console.Write("Kalkış:");
                                    Console.WriteLine(Cumartesi.KalkisSaatler[m]);

                                }
                                Console.WriteLine("------");
                                Console.WriteLine(vduragi);

                                for (int n = 0; n < Cumartesi.VarisSaatler.Count; n++)
                                {
                                    Console.Write("Varış:");
                                    Console.WriteLine(Cumartesi.VarisSaatler[n]);

                                }
                                // Console.ReadLine();
                            }

                        }  //CUMARTESI SONU

                        //PAZAR
                        gunno = 3;
                        if (gunno == 3)
                        {
                            string url2 = "http://www.izdeniz.com.tr/tr/HareketSaatleri/" + line1.lineNo + "/" + line2.lineNo + "/" + gunno;      //LinkSonu değer günü veriyor.     
                            string htmlContent2 = GetContent(url2);

                            HtmlAgilityPack.HtmlDocument doc2 = new HtmlAgilityPack.HtmlDocument();
                            doc2.LoadHtml(htmlContent2);

                            if (gunno == 3)
                            {
                                Console.WriteLine("Pazar");
                            }


                            Gunler Pazar = new Gunler();
                            Pazar.KalkisSaatler = new List<string>();
                            Pazar.VarisSaatler = new List<string>();

                            var hatNodes = doc2.DocumentNode.SelectNodes(@"//div[@class=""hat-tip""]");   //
                                                                                                          // Hat Bilgisi. Bak
                            string hatti = hatNodes[0].InnerText;                                         //

                            Console.WriteLine(hatti);



                            if (doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]") != null)
                            {
                                var durakNodes = doc2.DocumentNode.SelectNodes(@"//td[@class=""sefertdbaslik""]");
                                string kduragi = durakNodes.FirstOrDefault().InnerText; // [0].InnerText;
                                string vduragi = durakNodes.LastOrDefault().InnerText;
                                Console.WriteLine("------");
                                Console.WriteLine(kduragi);
                                Pazar.Gun = "Pazar";
                                Pazar.Hat = hatti;
                                Pazar.KalkisDuragi = kduragi;
                                Pazar.VarisDuragi = vduragi;

                                var saatNodes = doc2.DocumentNode.SelectNodes(@"//tr[@class=""""]");

                                if (saatNodes != null)
                                {
                                    foreach (var node in saatNodes)
                                    {
                                        string nodeshtml = node.InnerHtml;
                                        var htmlDocnode = new HtmlDocument();
                                        htmlDocnode.LoadHtml(nodeshtml);
                                        var partsaatNodes = htmlDocnode.DocumentNode.SelectNodes(@"//td[@class=""sefertd""]");

                                        Pazar.KalkisSaatler.Add(partsaatNodes.FirstOrDefault().InnerText);
                                        Pazar.VarisSaatler.Add(partsaatNodes.LastOrDefault().InnerText);
                                    }

                                    nextVapur.gunler.Add(Pazar);
                                } // else 2/10a göre düzenlenecek


                                //Saatler ekrana yazdırılıyor.
                                for (int m = 0; m < Pazar.KalkisSaatler.Count; m++)
                                {
                                    Console.Write("Kalkış:");
                                    Console.WriteLine(Pazar.KalkisSaatler[m]);

                                }
                                Console.WriteLine("------");
                                Console.WriteLine(vduragi);

                                for (int n = 0; n < Pazar.VarisSaatler.Count; n++)
                                {
                                    Console.Write("Varış:");
                                    Console.WriteLine(Pazar.VarisSaatler[n]);

                                }
                                // Console.ReadLine();
                            }

                        } //PAZAR SONU


                        add2vdb(nextVapur);

                    }
                }
               


            Console.WriteLine("Vapur db hazır.");
            Console.ReadLine();
       
        }



        private static string GetContent(string urlAddress)
        {
            Uri url = new Uri(urlAddress);
            WebClient client = new WebClient();
            client.Encoding = System.Text.Encoding.UTF8;
            string html = client.DownloadString(url);

            return html;
        }


        private static void add2linedb(Line vh)
        {
            // Open database (or create if not exits)
            using (var db = new LiteDatabase(@"botart_ulasim_vapurhat.adbx"))
            {
                // Get hat collection
                var hatlardb = db.GetCollection<Line>("Line");

                var results = hatlardb.Find(x => x.lineNo == vh.lineNo).FirstOrDefault();
                if (results == null)
                {
                    vh._id = Guid.NewGuid().ToString();
                    hatlardb.Insert(vh);
                }
                else
                {
                    //Burada hattın bilgileri guncellenebilir
                    Console.WriteLine("UPDATING " + results._id);
                    hatlardb.Update(results._id, vh);
                }

                hatlardb.EnsureIndex(x => x.lineNo);
            }

        }




        private static void add2vdb(Vapur vapd)
        {
            if (vapd == null)
            {
                Console.WriteLine("NULL");
                return;

            }
            using (var db = new LiteDatabase(@"botart_ulasim_vapur.adbx"))
            {
                // Get hat collection
                var dbkvs = db.GetCollection<Vapur>("vapur");
                var results = dbkvs.Find(x => x.vapurNo == vapd.vapurNo).FirstOrDefault();
                if (results == null)
                {
                    vapd._id = Guid.NewGuid().ToString();
                    dbkvs.Insert(vapd);
                    
                }
                else
                {
                    //Burada hattın bilgileri guncellenebilir
                    Console.WriteLine("UPDATING " + results._id);
                    dbkvs.Update(results._id, vapd);


                }
            
                dbkvs.EnsureIndex(x => x.vapurNo);
                dbkvs.EnsureIndex(x => x.vapurName);
            }
        }

    }



    public class Line
    {
        public string _id { get; set; }
        public string lineNo { get; set; }
        public string lineName { get; set; }
    }


    public class Gunler
    {

        public string Gun { get; set; }
        public string Hat { get; set; }
        public string KalkisDuragi { get; set; }
        public string VarisDuragi { get; set; }
        public List<string> KalkisSaatler { get; set; }
        public List<string> VarisSaatler { get; set; }
    }

    public class Vapur
    {
        public string _id { get; set; }
        public string vapurNo { get; set; }
        public string vapurName { get; set; }
        public List<Gunler> gunler { get; set; }


    }

}
