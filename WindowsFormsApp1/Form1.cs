using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Font printFont;
        String content = "";
        private BindingSource source = null;
        List<ListProducts> ListProducts = null; 

        public Form1()
        {
            InitializeComponent();

            _ = getApiProductAsync();
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
                textBoxProductId.Enabled = false;
                string url = "http://localhost:8000/api/get-product";

                string data = "{\"token\":\"b9cbbeb6661a760fef54e696b954ed17\",\"all_data\":1}";
                HttpClient client = new HttpClient();

                HttpContent queryString = new StringContent(data,Encoding.UTF8, "application/json");
                
                HttpResponseMessage response = await client.PostAsync(url, queryString);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                JsonNode results = JsonNode.Parse(responseBody);
                JsonNode items = JsonNode.Parse(results["items"].ToJsonString());

                ListProducts = new List<ListProducts>();

                foreach (JsonNode row in items.AsArray())
                {
                    JsonNode item = JsonNode.Parse(row.ToJsonString());
                    Console.WriteLine($"JSON={item["id"]}");

                    ListProducts.AddRange(new[] {
                        new ListProducts() {id= (int)item["id"],keterangan=(string)item["keterangan"],harga=(string)item["harga"],
                        text = (string)item["text"],harga_number = (int)item["harga_number"],barcode = (string)item["barcode"],
                        qty = (int)item["qty"]
                        }
                    });

                }

                source = new BindingSource();
                source.DataSource = ListProducts;

                textRespon.Text = results.ToString();

                textBoxProductId.AutoCompleteMode = AutoCompleteMode.Suggest;
                textBoxProductId.AutoCompleteSource = AutoCompleteSource.CustomSource;
                AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();
                textBoxProductId.AutoCompleteCustomSource = DataCollection;
                textBoxProductId.AutoCompleteCustomSource.AddRange(ListProducts.Select(n => n.text).ToArray());
                textBoxProductId.Focus();
                textBoxProductId.Enabled = true;

                //Binding textBind = new Binding("Text", source, "text", true, DataSourceUpdateMode.OnPropertyChanged);
                //textBind.Parse += (s, evt) => {
                //    source.CurrencyManager.Position = ListProducts.FindIndex(1, r => r.id.Equals(evt.Value));
                //};

                //textBoxProductId.DataBindings.Add(textBind);
             }
             catch (Exception ex)
             {
                Console.WriteLine("An error occurred while printing", ex.ToString());
                MessageBox.Show(ex.ToString());
             }
        }

        private void textBoxProductId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;

                var product =  ListProducts.Find(r => r.text == textBoxProductId.Text);

             
                Console.WriteLine(textBoxProductId.Text);
                Console.WriteLine(product.harga.ToString());
                
                int total = 0;int totalQty = 0;

                var totalRow = product.harga_number * 1;
                string[] data = {"1",textBoxProductId.Text, product.harga.ToString(), "1", totalRow.ToString(),product.harga_number.ToString()};

                dataGridProduct.Rows.Add(data);

                for (int rows = 0; rows < dataGridProduct.Rows.Count; rows++)
                {
                    if (dataGridProduct.Rows[rows].Cells[1].Value != null)
                    {
                        try {
                            var qty = Convert.ToInt16(dataGridProduct.Rows[rows].Cells[3].Value.ToString());
                            var harga = Convert.ToInt16(dataGridProduct.Rows[rows].Cells[5].Value.ToString());
                            Console.WriteLine("Harga {0}", harga);

                            totalQty += qty;
                            total += qty * harga;
                            //Console.WriteLine("Total {0}", total);
                        }catch (Exception err)
                        {
                            Console.WriteLine("Error : ", err.ToString());
                        }
                    }
                }

                labelQty.Text = totalQty.ToString();
                labelTotal.Text = total.ToString();

                textBoxProductId.Text = "";
                textBoxProductId.Focus();
            }
        }

        private void calculate()
        {

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
            //string[] data = {
            //"1","Kopi","12000","1","12000"
            //};
            //dataGridProduct.Rows.Add(data);
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