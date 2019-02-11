using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SLNStokTakip.Hangar;

namespace SLNStokTakip.Fatura
{
    public partial class frmFaturaKes : Form
    {
        DbStokDataContext _db = new DbStokDataContext();
        Formlar _f = new Formlar(); 
        Mesajlar _m = new Mesajlar();
        Numaralar _n = new Numaralar();

        public bool _edit = false;
        int cksNo = -1;
        int fkayNo = -1;


        public frmFaturaKes()
        {
            InitializeComponent();
        }

        private void frmFaturaKes_Load(object sender, EventArgs e)
        {
            Temizle();
            SaatBul();
          
            if (AnaSayfa.Aktarma > 0 && AnaSayfa.Aktarma1 > 0)
            {
                ListeleIlk();
            }
                
            else
            {
                ListeleIki();
            }
        
        }

        void Temizle()
        {
            //foreach (Control ct in splitContainer1.Panel2.Controls)
            //    if (ct is TextBox || ct is ComboBox) ct.Text = "";
            //Liste.Rows.Clear();
            //_edit = false;
            txtEvrakNo.Text = _n.FaturaKayitNo();
            //AnaSayfa.Aktarma = -1;
        }


        void ListeleIlk()
        {
            cksNo = AnaSayfa.Aktarma;
            Liste.Rows.Clear();
            int i = 0;
            var srg = (from s in _db.stUrunCikis
                       where s.CikisNo == cksNo
                       select s);
            foreach(var k in srg)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = k.Id;
                Liste.Columns[1].Visible = true;
                Liste.Columns[2].Visible = false;
                Liste.Rows[i].Cells[1].Value = k.CikisNo;
                Liste.Rows[i].Cells[3].Value = k.stStokDurum.UrunKodu;
                Liste.Rows[i].Cells[4].Value = k.stStokDurum.LotSeriNo;
                Liste.Rows[i].Cells[5].Value = k.stStokDurum.Aciklama;
                Liste.Rows[i].Cells[6].Value = k.Adet;
                txtCariAdi.Text = k.bgFirma.Fadi;
                txtVd.Text = k.bgFirma.Fvd;
                txtTcv.Text = k.bgFirma.Fvno;
                txtAdres.Text = k.bgFirma.Fadres;
                i++;

            }
            
            Liste.AllowUserToAddRows = false;
            AnaSayfa.Aktarma = -1;
        }
        void ListeleIki()
        {
            fkayNo = AnaSayfa.Aktarma1;
            Liste.Rows.Clear();
            int i = 0;
            var srg = (from s in _db.vwFaturaKes
                       where s.FKayitNo == fkayNo
                       select s);
            foreach (var k in srg)
            {
             

                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = -1;
                Liste.Columns[1].Visible = false;
                Liste.Rows[i].Cells[1].Value = k.CikisNo;
                Liste.Columns[2].Visible = true;
                Liste.Rows[i].Cells[2].Value = k.FKayitNo;
                Liste.Rows[i].Cells[3].Value = k.UrunKodu;
                Liste.Rows[i].Cells[4].Value = k.LotSeriNo;
                Liste.Rows[i].Cells[5].Value = k.Aciklama;
                Liste.Rows[i].Cells[6].Value = k.Cadet;
                Liste.Rows[i].Cells[7].Value = k.Bfiyat;
                Liste.Rows[i].Cells[8].Value = k.Tutar;
                txtCariAdi.Text = k.Fadi;
                txtVd.Text = k.Fvd;
                txtTcv.Text = k.Fvno;
                txtAdres.Text = k.Fadres;
                dtpTarih.Text = k.Tarih.ToString();
                txtSaat.Text = k.Saat;
                txtEvrakNo.Text = k.FKayitNo.ToString().PadLeft(7,'0');
                txtAraT.Text = k.Atoplam.ToString();
                txtKdv.Text = k.KdvToplam.ToString();
                txtToplamT.Text = k.Ttutar.ToString();
                
                i++;

            }

            Liste.AllowUserToAddRows = false;
            AnaSayfa.Aktarma1 = -1;//formda herhangi bir şey olursa sıfırla demek..
        }



        void YeniKaydet()//veritabanındaki bilgileri yazdık..
        {
            try
            {
                ftFaturaKesUst ust = new ftFaturaKesUst();
                ust.FKayitNo = int.Parse(txtEvrakNo.Text);
                ust.CariId = _db.bgFirmas.First(x => x.Fadi == txtCariAdi.Text).Fno;
                ust.Tarih = DateTime.Parse(dtpTarih.Text);
                ust.Saat = txtSaat.Text;
                ust.Yazi = txtYazi.Text;
                ust.Atoplam = decimal.Parse(txtAraT.Text);
                ust.KdvToplam = decimal.Parse(txtKdv.Text);
                ust.Ttutar = decimal.Parse(txtToplamT.Text);
                ust.CikisNo = cksNo;


                

                _db.ftFaturaKesUsts.InsertOnSubmit(ust);
                _db.SubmitChanges();


                var alt = new ftFaturaKesAlt[Liste.RowCount];
                for (int i = 0; i < Liste.RowCount; i++)
                {
                    alt[i] = new ftFaturaKesAlt();
                    alt[i].Bfiyat = decimal.Parse(Liste.Rows[i].Cells[7].Value.ToString());
                    alt[i].Cadet = int.Parse(Liste.Rows[i].Cells[6].Value.ToString());
                    alt[i].FKayitNo = int.Parse(txtEvrakNo.Text);
                    alt[i].Tutar = decimal.Parse(Liste.Rows[i].Cells[8].Value.ToString());
                    alt[i].UrunId = _db.stStokDurums.First(x => x.UrunKodu == Liste.Rows[i].Cells[3].Value.ToString() && x.LotSeriNo == Liste.Rows[i].Cells[4].Value.ToString()).Id;
                    _db.ftFaturaKesAlts.InsertOnSubmit(alt[i]);
                    _db.SubmitChanges();

                    //Faturalarda kesilmemiş olanları listelemek için bu güncelleme kodunu yazdık..

                    int aa = alt[i].UrunId.Value;
                    int bb = int.Parse(Liste.Rows[i].Cells[1].Value.ToString());
                    var gncl = (from s in _db.stUrunCikis where s.UrunId == aa where s.CikisNo == bb select s).ToList(); //gncl=güncellenin kısaltması.. Ürün çıkış tablosunu güncelliyoruz.. 
                    if(gncl.Count!=0)
                    {
                        var srg = _db.stUrunCikis.First(x => x.UrunId == aa && x.CikisNo == bb);
                        srg.FaturaDurum =true;
                        _db.SubmitChanges();
                    }
                }
                _m.YeniKayit("Kayıt oluşturuldu.");
                Temizle();
               
                
            }
            catch (Exception e)
            {

                _m.Hata(e);
            }

        }

        void Sil()
        {
            for(int i=0;i<Liste.RowCount;i++)
            {
                int bb =int.Parse(Liste.Rows[i].Cells[1].Value.ToString());


                var UrnId = _db.stStokDurums.First(x => x.UrunKodu == Liste.Rows[i].Cells[3].Value.ToString() && x.LotSeriNo == Liste.Rows[i].Cells[4].Value.ToString()).Id;
            
                var srg = _db.stUrunCikis.First(x => x.UrunId == UrnId && x.CikisNo == bb);
                srg.FaturaDurum = false;

               // _db.stUrunCikis.First(x => x.UrunId == UrnId && x.CikisNo == bb).FaturaDurum = false;
                _db.SubmitChanges();
            }
            _db.ftFaturaKesUsts.DeleteOnSubmit(_db.ftFaturaKesUsts.First(x => x.FKayitNo == fkayNo));

             var alt = (from c in _db.ftFaturaKesAlts where c.FKayitNo == fkayNo
                                                            select c).ToList();

            _db.ftFaturaKesAlts.DeleteAllOnSubmit(alt);//Deleteallonsubmit hepsini sil komutudur..
            _db.SubmitChanges();

            MessageBox.Show("Silme işlemi başarılı. Başın göğe erdi mi?");
            Close();
            _f.FaturaCikis();

           
        }


        protected override void OnLoad(EventArgs e)//textbox taki butonu çalıştıran kod. Resimli butonu..
        {
            var btnfn = new Button();
           btnfn.Size = new Size(25, txtEvrakNo.ClientSize.Height + 2);
           btnfn.Location = new Point(txtEvrakNo.ClientSize.Width - btnfn.Width, -1);
           btnfn.Cursor = Cursors.Default;
            btnfn.Image = SLNStokTakip.Properties.Resources.arrow_1176;
            txtEvrakNo.Controls.Add(btnfn);
            SendMessage(txtEvrakNo.Handle, 0xd3, (IntPtr)2, (IntPtr)(btnfn.Width << 16));
            btnfn.Anchor = (AnchorStyles.Top | AnchorStyles.Right);


            base.OnLoad(e);
            btnfn.Click += btnfn_Click;
            
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void btnfn_Click(object sender, EventArgs e)
        {
            int id = _f.FaturaKesListe(true);
            if (id > 0)
            {
                Ac(id);
            }
            AnaSayfa.Aktarma1 = -1;
        }
        void Ac(int id)
        {
            try
            {
                Liste.Rows.Clear();
                int i = 0;
                _edit = true;
                ftFaturaKesUst ust = _db.ftFaturaKesUsts.First(s => s.CikisNo == id);
                txtAdres.Text = ust.bgFirma.Fadres;
                txtAraT.Text = ust.Atoplam.ToString();
                txtCariAdi.Text = ust.bgFirma.Fadi;
                txtEvrakNo.Text = ust.CikisNo.ToString().PadLeft(7, '0');
                txtKdv.Text = ust.KdvToplam.ToString();
                txtSaat.Text = ust.Saat;
                txtTcv.Text = ust.bgFirma.Fvno;
                txtToplamT.Text = ust.Ttutar.ToString();
                txtVd.Text = ust.bgFirma.Fvd;
                txtYazi.Text = ust.Yazi;
                dtpTarih.Text = ust.Tarih.ToString();


                var srg = (from s in _db.ftFaturaKesAlts
                           where s.FKayitNo == ust.FKayitNo
                           select s).ToList();
                foreach (var k in srg)
                {
                    Liste.Rows.Add();
                    Liste.Rows[i].Cells[0].Value = k.Id;
                    Liste.Rows[i].Cells[1].Value = ust.CikisNo;
                    Liste.Rows[i].Cells[2].Value = k.FKayitNo;
                    Liste.Rows[i].Cells[3].Value = k.stStokDurum.UrunKodu;
                    Liste.Rows[i].Cells[4].Value = k.stStokDurum.LotSeriNo;
                    Liste.Rows[i].Cells[5].Value = k.stStokDurum.Aciklama;
                    Liste.Rows[i].Cells[6].Value = k.Cadet;
                    Liste.Rows[i].Cells[7].Value = k.Bfiyat;
                    Liste.Rows[i].Cells[0].Value = k.Tutar;


                }
                Liste.AllowUserToAddRows = false;
                Liste.ReadOnly = true;
            }
            catch (Exception e)
            {

                _m.Hata(e);
            }


        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnKaydet_Click(object sender, EventArgs e)
        {
            YeniKaydet();
        }

        private void Liste_CellClick(object sender, DataGridViewCellEventArgs e)//Sütunlar arası işlem yaptırıyor. DataGridView de..
        {
            if(e.ColumnIndex==8)//Tutarın dgv deki sırası.. Adet ve Bfiyat çarpımını tutara yazdırıyoruz..
            {
                decimal a, b,t ;
                a = decimal.Parse(Liste.CurrentRow.Cells[6].Value.ToString());
                b = decimal.Parse(Liste.CurrentRow.Cells[7].Value.ToString());
                t = a * b;
                Liste.CurrentRow.Cells[8].Value = t;

                Toplam();
                
            }
        }

        void Toplam()
        {
            decimal b=0,c=0,t=0;
            try
            {
                for(int i=0;i<Liste.Rows.Count;i++)
                {
                    b = b + Convert.ToDecimal(Liste.Rows[i].Cells[7].Value);
                    c = (b * 18) / 100;
                    t = b + c;
                }
                txtAraT.Text = b.ToString();
                txtKdv.Text = c.ToString();
                txtToplamT.Text = t.ToString();
                
            }
            catch (Exception e)
            {
                _m.Hata(e);
            }
        }

        private void btnCollaps_Click(object sender, EventArgs e)
        {
            switch (splitContainer1.Panel2Collapsed)
            {
                case true:
                    splitContainer1.Panel2Collapsed = false;
                    btnCollaps.Text = "GİZLE";
                    break;
                case false:
                    splitContainer1.Panel2Collapsed = true;
                    btnCollaps.Text = "GÖSTER";
                    break;
            }
        }
        public static string yaziyacevir(decimal tutar)//faturaya yazı yazdırdığımız kod..
        {
            bool tutarNegatifMi = false;
            if(tutar<0)
            {
                tutarNegatifMi = true;
                tutar = tutar * (-1);
            }
            string sTutar = tutar.ToString("F2").Replace(',', ',');//Replace(',',',') ondalık ayracının . olma durumu için..
            string lira = sTutar.Substring(0, sTutar.IndexOf(','));//Tutarın tam kısmı..
            string kurus = sTutar.Substring(sTutar.IndexOf(',') + 1, 2);//buradaki +1 virgülden sonraki ilk karakteri al demek..
            string yazi = "";

            string[] birler = { "", "BİR", "İKİ", "ÜÇ", "DÖRT", "BEŞ", "ALTI", "YEDİ", "SEKİZ", "DOKUZ" };// BİRLER BASAMAĞINDA ALABİLECEĞİ TÜM DEĞERLERİ YAZDIK..BOŞ OLABİLİR 1 DEN 9 A KADAR DEĞER ALABİLİR..
            string[] onlar = { "", "ON", "YİRMİ", "OTUZ", "KIRK", "ELLİ", "ALTMIŞ", "YETMİŞ", "SEKSEN", "DOKSAN" };
            string[] binler = { "KATRİLYON", "TRİLYON", "MİLYAR", "MİLYON", "BİN", "" };
            int grupSayisi = 6;//sayıdaki 3'lü grup sayısı. katrilyon için 6.(1.234,00 daki grup sayısı 2 dir.)
            //Katrilyonun başına ekleyeceğiniz her değer için grup sayısını arttırınız.
            lira = lira.PadLeft(grupSayisi * 3, '0');//sayının soluna sıfır eklenerek (grup sayısı *3) basamaklı yapılıyor.
            string grupDegeri;
            for(int i=0;i<grupSayisi*3;i+=3)//i+=3 3 er 3 er arttır demek..
            {
                grupDegeri = "";
                if (lira.Substring(i, 1) != "0") grupDegeri += birler[Convert.ToInt32(lira.Substring(i, 1))] + "YÜZ";//Yüzler..
                if (grupDegeri == "BİRYÜZ")//biryüz ifadesi düzeltiliyor..
                    grupDegeri = "YÜZ";
                grupDegeri += onlar[Convert.ToInt32(lira.Substring(i + 1, 1))];//onlar..
                grupDegeri += birler[Convert.ToInt32(lira.Substring(i + 2, 1))];//birler..
                if (grupDegeri != "")//binler değiştiriliyor..
                    grupDegeri += binler[i / 3];
                if (grupDegeri == "BİRBİN")//birbin düzeltiliyor..
                    grupDegeri = "BİN";
                yazi += grupDegeri;

            }
            if (yazi != "")
                yazi += "TL";
            int yaziUzunlugu = yazi.Length;
            if (kurus.Substring(0, 1) != "0")//kuruş olanlar..
                yazi += onlar[Convert.ToInt32(kurus.Substring(0, 1))];
            if (kurus.Substring(1, 1) != "0")//kuruş birler..
                yazi += birler[Convert.ToInt32(kurus.Substring(1, 1))];
            if (yazi.Length > yaziUzunlugu)
                yazi += "Kr.";
            else
                yazi += "SIFIR Kr.";

            if (tutarNegatifMi)
                return "Eksi" + yazi;
            return yazi;
        }

        private void txtToplamT_TextChanged(object sender, EventArgs e)
        {
            txtYazi.Text = yaziyacevir(Convert.ToDecimal(txtToplamT.Text));
        }

        void SaatBul()
        {
            DateTime zmn = DateTime.Now;//Şimdiki zamana git..
            txtSaat.Text = zmn.ToShortTimeString();
        }

        private void chbManuel_CheckedChanged(object sender, EventArgs e)
        {
            if(!chbManuel.Checked)//ünlem ==false un kısaltması..
            {
                SaatBul();
            }
            else
            {
                txtSaat.Text = "00:00";
            }
        }

        private void btnSil_Click(object sender, EventArgs e)
        {
            Sil();
        }

       
    }
}
