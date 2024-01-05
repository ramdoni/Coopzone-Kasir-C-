using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Font printFont;
        String content = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                PrintDialog printDlg = new PrintDialog();
                PrintDocument printDoc = new PrintDocument();
                Margins margins = new Margins(1, 1, 1, 1);
                
                printDoc.DefaultPageSettings.Margins = margins;
                printDoc.DefaultPageSettings.PaperSize = new PaperSize("MyPaper",190 , 800);
                printDoc.DocumentName = "fileName";

                printDlg.Document = printDoc;
                printDlg.AllowSelection = true;
                printDlg.AllowSomePages = true;

                printFont = new Font("Serif", 7);

                //Call ShowDialog
                //if (printDlg.ShowDialog() == DialogResult.OK)
                //{
                printDoc.PrintPage += new PrintPageEventHandler(pd_PrintPage);
                    printDoc.Print();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while printing", ex.ToString());
            }
        }


        private async Task getApiProductAsync()
        {
             try
             {
                string url = "http://localhost:8000/api/get-product";

                string data = "{\"token\":\"b9cbbeb6661a760fef54e696b954ed17\"}";
                HttpClient client = new HttpClient();

                HttpContent queryString = new StringContent(data,Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await client.PostAsync(url, queryString);
                response.EnsureSuccessStatusCode();

                //data.data?.ForEach(Console.WriteLine);

                string responseBody = await response.Content.ReadAsStringAsync();

                string items = JsonSerializer.Deserialize(responseBody);


                textRespon.Text = responseBody;

                //textBoxProductId.AutoCompleteMode = AutoCompleteMode.Suggest;
                //textBoxProductId.AutoCompleteSource = AutoCompleteSource.CustomSource;
                //AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection()
                //textBoxProductId.AutoCompleteCustomSource = DataCollection;
            }
             catch (Exception ex)
             {
                Console.WriteLine("An error occurred while printing", ex.ToString());
                MessageBox.Show(ex.ToString());
             }
        }

        private void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            content = "Koperasi WIPO\n";
            float linesPerPage = 0;

            linesPerPage = ev.MarginBounds.Height / printFont.GetHeight(ev.Graphics);

            float leftMargin = 0 ;
            float yPos = 0;
            float topMargin = ev.MarginBounds.Top;
            ev.Graphics.DrawString(content, printFont, Brushes.Black,
                            leftMargin, topMargin, new StringFormat());

            yPos = topMargin + (1 * printFont.GetHeight(ev.Graphics));
            ev.Graphics.DrawString("Jln. Pulogadung No. 32 RT 01/RW 12 Jatinegara Kec Cakung", printFont, Brushes.Black,
                            leftMargin, yPos , new StringFormat());
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /**
            * NO, PRODUCT, HARGA, QTY, TOTAL
            */
            string[] data = {
            "1","Kopi","12000","1","12000"
            };
            dataGridProduct.Rows.Add(data);

            getApiProductAsync();
        }

        private void dataGridProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void button3_Click(object sender, EventArgs e)
        {
            pageSetting form = new pageSetting();
            form.ShowDialog();
        }
    }
}


public class ListProducts
{
    public int id { get; set; }
    public string keterangan { get; set; }
    public string text { get; set; }
    public string harga { get; set; }
    public int harga_number { get; set; }
    public string barcode { get; set; }
    public int qty { get; set; }
}