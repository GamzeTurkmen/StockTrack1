using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SLNStokTakip.Stok;
using SLNStokTakip.Hangar;
using SLNStokTakip.Bilgi;

namespace SLNStokTakip.Stok
{
    public partial class frmStokDurum : Form
    {
        DbStokDataContext _db = new DbStokDataContext();
        Formlar _f = new Formlar();
        Numaralar _n = new Numaralar();
        Mesajlar _m = new Mesajlar();

        public bool Secim = false;
        int _secimId = -1;


        public frmStokDurum()
        {
            InitializeComponent();
        }

        private void frmStokDurum_Load(object sender, EventArgs e)
        {
            Listele();
        }

        void Listele()
        {
            Liste.Rows.Clear();
            int i = 0;
            var lst = (from s in _db.stStokDurums
                       where s.UrunKodu.Contains(txtStokBul.Text)
                       select new
                       {
                           id = s.Id,
                           uk = s.UrunKodu,
                           lsn = s.LotSeriNo,
                           adt = s.Adet,
                           ack = s.Aciklama,
                       }).OrderBy(x => x.adt);

            foreach (var k in lst)
            {
                Liste.Rows.Add();
                Liste.Rows[i].Cells[0].Value = k.id;
                Liste.Rows[i].Cells[1].Value = k.uk;
                Liste.Rows[i].Cells[2].Value = k.lsn;
                Liste.Rows[i].Cells[3].Value = k.adt;
                Liste.Rows[i].Cells[4].Value = k.ack;

                i++;
            }
            Liste.AllowUserToAddRows = false;
        }


        void Sec()
        {
            try
            {
                if (Liste.CurrentRow != null) _secimId = int.Parse(Liste.CurrentRow.Cells[0].Value.ToString());
            }
            catch (Exception)
            {
                _secimId = -1;
            }
        }

        private void btnKapat_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBul_Click(object sender, EventArgs e)
        {
            Listele();
        }

        private void Liste_DoubleClick(object sender, EventArgs e)
        {
            Sec();
            if (Secim && _secimId > 0)
            {
                AnaSayfa.Aktarma = _secimId;
                Close();
            }

        }
        void Yaz()
        {
            Print.frmPrint prn = new Print.frmPrint();
            prn.HangiListe = "StokListe";
            prn.Show();
        }

        private void txtStokBul_TextChanged(object sender, EventArgs e)
        {
            Listele();
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

        private void btnPrint_Click(object sender, EventArgs e)
        {
            Yaz();
        }

    private void label3_Click(object sender, EventArgs e)
    {

    }
  }
}
