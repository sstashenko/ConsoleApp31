using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace LR3
{
    public partial class fMain : Form
    {
        public fMain()
        {
            InitializeComponent();
        }

        private void btnAddBook_Click(object sender, EventArgs e)
        {
            Book book = new Book(); fBook fb = new fBook(book);
            if (fb.ShowDialog() == DialogResult.OK)
            {
                tbBooksInfo.Text += string.Format("Книга {0} написана {1} у {2} році. \n Опис: {3}. \n" +
                    "Видана: {4} у {5} обкладинці, обсягом {6} сторінок і коштує {7:0.00} грн." +
                    " Їй {8} років і публікація однієї сторінки коштує {9:0.00}\r\n\n\n",
                    book.Title, book.Author, book.YearPub, book.Description, book.Publisher,
                    book.TypeBinding ? "твердій" : "м'якій", book.Pages, book.Cost, book.HowOld(), book.CostOnePage());

                // Додавання об'єкту до ліста AllBooks
                Book.AllBooks.Add(book);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Припинити роботу застосунку?", "Припинити роботу", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                Application.Exit();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

            saveFileDialog.Filter = "Tekстовi þайли (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.Title = "Зберегти дані у текстовому форматі";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            StreamWriter sw;
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                sw = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);
                try
                {
                    sw.Write(tbBooksInfo.Text);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Сталась помилка: \n{0}", ex.Message,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sw.Close();
                }
            }
        }
        private void fMain_Resize(object sender, EventArgs e)
        {
            int buttonsSize = 9 * btnAddBook.Width + 3 * toolStripSeparator1.Width;
            btnExit.Margin = new Padding(Width - buttonsSize, 0, 0, 0);
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnSaveAsBinary_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = "Оайли даниx (*.towns)*.towns | All files (*.*)";
            saveFileDialog.Title = "Зберегти дані у бінарному форматі";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            BinaryWriter bw;

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                bw = new BinaryWriter(saveFileDialog.OpenFile());
                try
                {
                    foreach (var book in Book.AllBooks)
                    {
                        bw.Write(book.Title);
                        bw.Write(book.Author);
                        bw.Write(book.YearPub);
                        bw.Write(book.Description);
                        bw.Write(book.Publisher);
                        bw.Write(book.Pages);
                        bw.Write(book.Cost);
                        bw.Write(book.TypeBinding ? "твердій" : "м'якій");
                    }

                    MessageBox.Show("Дані успішно збережено у файл.", "Успіх", MessageBoxButtons.OK, MessageBoxIcon.Information);

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталась помилка: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnOpenFromText_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Текстові файли (*.txt)|*.txt|Усі файли (*.*)|*.*";
            openFileDialog.Title = "Прочитати дані у текстовому форматі";
            openFileDialog.InitialDirectory = Application.StartupPath;

            StreamReader sr = null;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    sr = new StreamReader(openFileDialog.FileName, Encoding.UTF8);
                    string s;
                    while ((s = sr.ReadLine()) != null)
                    {
                        // Передбачається, що кожен рядок містить дані однієї книги у потрібному форматі
                        string[] fields = s.Split(new[] { ',' }, StringSplitOptions.None);
                        if (fields.Length == 8)
                        {
                            Book book = new Book();
                            book.Title = fields[0];
                            book.Author = fields[1];
                            book.YearPub = int.Parse(fields[2]);
                            book.Description = fields[3];
                            book.Publisher = fields[4];
                            book.Pages = int.Parse(fields[5]);
                            book.Cost = int.Parse(fields[6]);
                            book.TypeBinding = (fields[7] == "твердій");
                            book.HowOld();
                            book.CostOnePage();

                            // Додавання об'єкту до ліста AllBooks
                            Book.AllBooks.Add(book);

                            tbBooksInfo.Text += string.Format("Книга {0} написана {1} у {2} році. \n Опис: {3}. \n" +
                    "Видана: {4} у {5} обкладинці, обсягом {6} сторінок і коштує {7:0.00} грн." +
                    " Їй {8} років і публікація однієї сторінки коштує {9:0.00}\r\n\n\n",
                    book.Title, book.Author, book.YearPub, book.Description, book.Publisher,
                    book.TypeBinding ? "твердій" : "м'якій", book.Pages, book.Cost, book.HowOld(), book.CostOnePage());
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Сталась помилка: \n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    sr?.Close();
                }
            }
        }

        private void btnOpenFromBinary_Click(object sender, EventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Оайли даниx (*.towns) | *.towns | All files (*.*) | *.*";
            openFileDialog.Title = "Прочитати дані у бінарному форматі";
            openFileDialog.InitialDirectory = Application.StartupPath;
            BinaryReader br;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                using (br = new BinaryReader(openFileDialog.OpenFile()))
                {
                    try
                    {
                        while (br.BaseStream.Position < br.BaseStream.Length)
                        {
                            Book book = new Book();
                            for (int i = 1; i <= 8; i++)
                            {
                                switch (i)
                                {
                                    case 1:
                                        book.Title = br.ReadString(); break;
                                    case 2:
                                        book.Author = br.ReadString(); break;
                                    case 3:
                                        book.YearPub = br.ReadInt32(); break;
                                    case 4:
                                        book.Description = br.ReadString(); break;
                                    case 5:
                                        book.Publisher = br.ReadString(); break;
                                    case 6:
                                        book.Pages = br.ReadInt32(); break;

                                }
                            }
                            tbBooksInfo.Text += string.Format("Книга {0} написана {1} у {2} році. \n Опис: {3}. \n" +
                   "Видана: {4} у {5} обкладинці, обсягом {6} сторінок і коштує {7:0.00} грн." +
                   " Їй {8} років і публікація однієї сторінки коштує {9:0.00}\r\n\n\n",
                   book.Title, book.Author, book.YearPub, book.Description, book.Publisher,
                   book.TypeBinding ? "твердій" : "м'якій", book.Pages, book.Cost, book.HowOld(), book.CostOnePage());
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Сталась помилка: \n{ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void fMain_Load(object sender, EventArgs e)
        {

        }
    }
}
