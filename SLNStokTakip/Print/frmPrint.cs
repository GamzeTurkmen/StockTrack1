using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SLNStokTakip.Hangar;
using SLNStokTakip.Bilgi;
using SLNStokTakip.Print;
using SLNStokTakip.Stok;

namespace SLNStokTakip.Print
{
    public partial class frmPrint : Form
    {
        DbStokDataContext _db = new DbStokDataContext();
        public string HangiListe;

        public frmPrint()
        {
            InitializeComponent();
        }

        private void frmPrint_Load(object sender, EventArgs e)
        {
            switch (HangiListe)
            {
                case "KulListe":
                    kulliste();
                    break;
                case "UC":
                    UrunCikis();
                    break;
                case "UrunKayit":
                    urunkayitt();
                    break;
                case "StokListe":
                    StokListesi();
                    break;

                default:
                    break;
            }
        }
        private void kulliste()
        {
            frmKulListe kl = Application.OpenForms["frmKulListe"] as frmKulListe;//Açık olan form için yazıyoruz..
            KulListesi cr = new KulListesi();//cr crystal report kısaltması..
            var lst = (from s in _db.bgKullanicilars //bu bilgiyi bgku
                       select s).ToList();
            if(lst!=null)
            {
                PrintYardım ch = new PrintYardım();//ch crystal help kısası..
                DataTable dt = ch.ConvertTo(lst);//listeyi crystal report a göre çevirsin..
                cr.SetDataSource(dt);
                crvPrint.ReportSource = cr;
            }
        }

        private void StokListesi()
        {
            frmStokDurum kl = Application.OpenForms["frmStokDurum"] as frmStokDurum;//Açık olan form için yazıyoruz..
            StokDurumListesi cr = new StokDurumListesi();//cr crystal report kısaltması..
            var lst = (from s in _db.stStokDurums //bu bilgiyi bgku
                       select s).ToList();
            if (lst != null)
            {
                PrintYardım ch = new PrintYardım();//ch crystal help kısası..
                DataTable dt = ch.ConvertTo(lst);//listeyi crystal report a göre çevirsin..
                cr.SetDataSource(dt);
                crvPrint.ReportSource = cr;
            }
        }

        private void urunkayitt()
        {
            frmUrunKayitListe kl = Application.OpenForms["UrunKayitListe"] as frmUrunKayitListe;//Açık olan form için yazıyoruz..
            UrunKayitListesi cr = new UrunKayitListesi();//cr crystal report kısaltması..
            var lst = (from s in _db.vwUKLs //bu bilgiyi bgku
                       select s).ToList();
            if (lst != null)
            {
                PrintYardım ch = new PrintYardım();//ch crystal help kısası..
                DataTable dt = ch.ConvertTo(lst);//listeyi crystal report a göre çevirsin..
                cr.SetDataSource(dt);
                crvPrint.ReportSource = cr;
            }
        }

        private void UrunCikis()
        {
            int id = AnaSayfa.Aktarma;
            Stok.frmUrunCikis uc = Application.OpenForms["frmUrunCikis"] as Stok.frmUrunCikis;

            Print.pUrunCikis cr = new pUrunCikis();

            var srg = (from s in _db.vwUCs
                       where s.CikisNo==id
                       select s ).ToList();
            if(srg!=null)
            {
                PrintYardım ch = new PrintYardım();//printyardımı yeni baştan oluşturuyoruz her geldiğinde sıfırlansın..
                DataTable dt = ch.ConvertTo(srg);
                cr.SetDataSource(dt);
                crvPrint.ReportSource = cr;

            }
            AnaSayfa.Aktarma = -1;
        }

    private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
    {

    }
  }
}
