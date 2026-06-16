using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TaxiPrevoz.Models;
using TaxiPrevoz.Services;

namespace TaxiPrevoz.Forms
{
    public class FormVoznje : Form
    {
        private DataGridView dgvVoznje;
        private ComboBox cbStatusFilter;
        private DateTimePicker dtpDatumFilter;
        private CheckBox chkEnableDatumFilter;
        private Button btnOtkaziVoznju;
        private Button btnZatvori;
        private Label lblNaslov;

        private List<Voznja> sveVoznje = new List<Voznja>();

        public FormVoznje()
        {
            InitializeComponent();
            UcitajVoznje();
        }

        private void InitializeComponent()
        {
            this.Text = "Taxi Prevoz - Pregled Voznji";
            this.Size = new Size(950, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(31, 41, 55);
            this.Font = new Font("Segoe UI", 9.5F);

            lblNaslov = new Label
            {
                Text = "Pregled voznji",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(40, 15),
                Size = new Size(280, 30)
            };
            this.Controls.Add(lblNaslov);

            Label lblStatus = new Label
            {
                Text = "Status:",
                ForeColor = Color.White,
                Location = new Point(300, 20),
                Size = new Size(50, 20)
            };
            this.Controls.Add(lblStatus);

            cbStatusFilter = new ComboBox
            {
                Location = new Point(355, 17),
                Size = new Size(130, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cbStatusFilter.Items.Add("Sve");
            cbStatusFilter.Items.Add("Narucena");
            cbStatusFilter.Items.Add("Prihvacena");
            cbStatusFilter.Items.Add("UToku");
            cbStatusFilter.Items.Add("Zavrsena");
            cbStatusFilter.Items.Add("Otkazana");
            cbStatusFilter.SelectedIndex = 0;
            cbStatusFilter.SelectedIndexChanged += Filter_Changed;
            this.Controls.Add(cbStatusFilter);

            chkEnableDatumFilter = new CheckBox
            {
                Text = "Datum:",
                ForeColor = Color.White,
                Location = new Point(505, 18),
                Size = new Size(70, 22),
                Cursor = Cursors.Hand
            };
            chkEnableDatumFilter.CheckedChanged += chkEnableDatumFilter_CheckedChanged;
            this.Controls.Add(chkEnableDatumFilter);

            dtpDatumFilter = new DateTimePicker
            {
                Location = new Point(580, 17),
                Size = new Size(130, 25),
                Format = DateTimePickerFormat.Short,
                Enabled = false
            };
            dtpDatumFilter.ValueChanged += Filter_Changed;
            this.Controls.Add(dtpDatumFilter);

            dgvVoznje = new DataGridView
            {
                Location = new Point(40, 60),
                Size = new Size(870, 370),
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.FromArgb(55, 65, 81),
                ForeColor = Color.Black,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            
            dgvVoznje.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(17, 24, 39);
            dgvVoznje.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvVoznje.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            dgvVoznje.EnableHeadersVisualStyles = false;

            dgvVoznje.SelectionChanged += DgvVoznje_SelectionChanged;
            this.Controls.Add(dgvVoznje);

            btnOtkaziVoznju = new Button
            {
                Text = "Otkazi voznju",
                BackColor = Color.FromArgb(239, 68, 68),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(20, 450),
                Size = new Size(160, 35),
                Enabled = false,
                Cursor = Cursors.Hand
            };
            btnOtkaziVoznju.FlatAppearance.BorderSize = 0;
            btnOtkaziVoznju.Click += BtnOtkaziVoznju_Click;
            this.Controls.Add(btnOtkaziVoznju);

            btnZatvori = new Button
            {
                Text = "Zatvori",
                BackColor = Color.FromArgb(75, 85, 99),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(780, 450),
                Size = new Size(140, 35),
                Cursor = Cursors.Hand
            };
            btnZatvori.FlatAppearance.BorderSize = 0;
            btnZatvori.Click += (s, e) => this.Close();
            this.Controls.Add(btnZatvori);
        }

        private void chkEnableDatumFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpDatumFilter.Enabled = chkEnableDatumFilter.Checked;
            UcitajVoznje();
        }

        private void Filter_Changed(object sender, EventArgs e)
        {
            UcitajVoznje();
        }

        private void UcitajVoznje()
        {
            sveVoznje = AppState.VoznjaService.GetVoznjeZaKorisnika(AppState.TrenutniKorisnik);
            
            var filtrirane = sveVoznje.AsEnumerable();

            if (cbStatusFilter.SelectedIndex > 0)
            {
                string statusString = cbStatusFilter.SelectedItem.ToString();
                var status = (StatusVoznje)Enum.Parse(typeof(StatusVoznje), statusString);
                filtrirane = filtrirane.Where(v => v.Status == status);
            }

            if (chkEnableDatumFilter.Checked)
            {
                DateTime izabranDatum = dtpDatumFilter.Value.Date;
                filtrirane = filtrirane.Where(v => v.VremeNarucivanja.Date == izabranDatum);
            }

            PrikaziVoznje(filtrirane.ToList());
        }

        private void PrikaziVoznje(List<Voznja> voznje)
        {
            dgvVoznje.DataSource = null;
            dgvVoznje.Columns.Clear();

            var korisnici = AppState.KorisnikRepo.GetAll();

            var displayList = voznje.Select(v => {
                var putnik = korisnici.OfType<Putnik>().FirstOrDefault(p => p.PutnikID == v.PutnikID);
                var vozac = korisnici.OfType<Vozac>().FirstOrDefault(d => d.VozacID == v.VozacID);

                return new
                {
                    ID = v.VoznjaID,
                    Putnik = putnik?.ImePrezime ?? v.PutnikID,
                    Vozac = vozac?.ImePrezime ?? v.VozacID ?? "Nije dodeljen",
                    Polaziste = v.Polaziste,
                    Odrediste = v.Odrediste,
                    Vreme = v.VremeNarucivanja.ToString("dd.MM.yyyy HH:mm"),
                    Km = v.Kilometraza.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + " km",
                    Cena = v.Cena.ToString("0") + " RSD",
                    Status = v.Status.ToString(),
                    Ocena = v.Ocena.HasValue ? v.Ocena.Value.ToString() + " ★" : "-",
                    Komentar = v.Komentar ?? ""
                };
            }).ToList();

            dgvVoznje.DataSource = displayList;

            for (int i = 0; i < dgvVoznje.Rows.Count; i++)
            {
                string status = dgvVoznje.Rows[i].Cells["Status"].Value.ToString();
                DataGridViewRow row = dgvVoznje.Rows[i];

                if (status == "Zavrsena")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(209, 250, 229);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(6, 78, 59);
                }
                else if (status == "UToku")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(254, 243, 199);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(120, 53, 4);
                }
                else if (status == "Prihvacena")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(219, 234, 254);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(30, 58, 138);
                }
                else if (status == "Otkazana")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(243, 244, 246);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(55, 65, 81);
                }
                else if (status == "Narucena")
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(243, 232, 255);
                    row.DefaultCellStyle.ForeColor = Color.FromArgb(88, 28, 135);
                }
            }
        }

        private void DgvVoznje_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvVoznje.SelectedRows.Count == 0)
            {
                btnOtkaziVoznju.Enabled = false;
                return;
            }

            string voznjaId = dgvVoznje.SelectedRows[0].Cells["ID"].Value.ToString();
            var voznja = sveVoznje.FirstOrDefault(v => v.VoznjaID == voznjaId);

            bool mozeOtkazatiDozvola = AppState.TrenutniKorisnik.MozeOtkazatiVoznju();
            btnOtkaziVoznju.Enabled = mozeOtkazatiDozvola && voznja != null && voznja.MozeSeOtkazati();
        }

        private void BtnOtkaziVoznju_Click(object sender, EventArgs e)
        {
            if (dgvVoznje.SelectedRows.Count == 0) return;

            string voznjaId = dgvVoznje.SelectedRows[0].Cells["ID"].Value.ToString();

            var rez = MessageBox.Show($"Da li ste sigurni da zelite da otkazete voznju {voznjaId}?", "Potvrda", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (rez == DialogResult.Yes)
            {
                try
                {
                    AppState.VoznjaService.OtkaziVoznju(voznjaId);
                    MessageBox.Show("Voznja je uspesno otkazana.", "Uspesno", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UcitajVoznje();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
