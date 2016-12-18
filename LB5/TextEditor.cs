using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Printing;

namespace LB5
{
    public partial class TextEditor : Form
    {
        private string fileName = string.Empty;  // ім'я файлу
        private bool docChanged = false; //чи був файл змінений
        int pagesPrinted = 0;              //номер поточної сторінки, що друкується
        string buferOut = "";                //буфер для виводу тексту

        OpenFileDialog openFileDialog1;
        SaveFileDialog saveFileDialog1;
        FontDialog fontDialog1;
        ColorDialog colorDialog1;

        /// КОНСТРУКТОР ФОРМИ
        public TextEditor()
        {
            InitializeComponent();

            openFileDialog1 = new OpenFileDialog();
            saveFileDialog1 = new SaveFileDialog();

            richTextBox1.Text = string.Empty;

            // вивести панель инструментов
            toolStrip1.Visible = true;

            // налаштування компонента openDialog1
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.Filter = "(*.txt)|*.txt|(*.rtf)|*.rtf|All Files(*.*)|*.* ";
            openFileDialog1.Title = "Відкрити документ";
            openFileDialog1.Multiselect = false;

            // налаштування компонента saveDialog1
            saveFileDialog1.Filter = "(*.txt)|*.txt|(*.rtf)|*.rtf|All Files(*.*)|*.* ";
            saveFileDialog1.Title = "Зберегти документ";

            fontDialog1 = new FontDialog();
            colorDialog1 = new ColorDialog();

        }
        
        /// ФАЙЛ
        /// НОВИЙ ФАЙЛ  
        private void NewDocument()
        {
            //меню Новий документ
            if (docChanged)
            {
                DialogResult dialogResult;
                dialogResult = MessageBox.Show(
                                      "Зберегти зміни?", "Yakubovych's TextEditor",
                                      MessageBoxButtons.YesNoCancel,
                                      MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        SaveDocument();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        return;
                };
            }
            docChanged = false;
            fileName = string.Empty;
            richTextBox1.Clear();
            this.Text = "Yakubovych's TextEditor";
        }

        private void toolStripMenuItem_New_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            NewDocument();
        }

        /// ВІДКРИТИ
        private void OpenDocument()
        {
            if (docChanged == true)
            {
                DialogResult dialogResult;
                dialogResult = MessageBox.Show(
                                      "Зберегти зміни?", "Yakubovych's TextEditor",
                                      MessageBoxButtons.YesNoCancel,
                                      MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        if (SaveDocument() == true)
                        {
                            docChanged = false;
                            fileName = string.Empty;
                        }
                        break;
                    case DialogResult.No:
                        docChanged = false;
                        fileName = string.Empty;
                        break;
                    case DialogResult.Cancel:
                        return;
                }
            }
            openFileDialog1.FileName = string.Empty;
            // Відобразити діалог відкриття файлу
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                // відобразити iм'я файла у заголовку вікна
                this.Text = fileName;
                try
                {
                    // читання даних з файлу
                    if (fileName.EndsWith(".rtf") == true)
                        richTextBox1.LoadFile(fileName);
                    else
                    {
                        System.IO.StreamReader sreader = new System.IO.StreamReader(fileName);
                        richTextBox1.Text = sreader.ReadToEnd();
                        richTextBox1.SelectionStart = richTextBox1.TextLength;
                        sreader.Close();
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Error reading file.\n" + exc.ToString(), "Yakubovych's TextEditor",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                }
            }
        }

        private void toolStripMenuItem_Open_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenDocument();
        }

        /// ЗБЕРЕГТИ
        private bool SaveDocument()
        {
            bool canSave = true;

            if (fileName == string.Empty)
            {
                // відобразити діалог Зберегти 
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    // відобразити ім'я файла у заголовку вікна
                    fileName = saveFileDialog1.FileName;
                    this.Text = fileName;
                }
                else canSave = false;
            }

            // зберегти файл
            if (fileName != string.Empty)
            {
                try
                {
                    if (fileName.EndsWith(".rtf") == true)
                        richTextBox1.SaveFile(fileName);
                    else
                    {
                        // отримуємо інформацiю про файл fileName
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileName);

                        // створюємо потік для запису (перезаписуємо файл)
                        System.IO.StreamWriter swriter = fileInfo.CreateText();
                        // записуємо дані
                        swriter.Write(richTextBox1.Text);

                        // закриваємо потік
                        swriter.Close();
                    }
                    docChanged = false;
                    canSave = true;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.ToString(), "Yakubovych's TextEditor", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
            return canSave;
        }

        private void toolStripMenuItem_Save_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveDocument();
        }

        /// ДРУКУВАТИ
        private void pd_PrintPages(object sender, PrintPageEventArgs e)
        {
            float yPos = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            string line = null;
            //розбиваємо текст на рядки і обчислюємо їх кількість
            string[] split = buferOut.Split(new Char[] { '\n' });
            int nLines = split.Length; //число строк
            //розраховуємо кількість рядків на сторінці
            int linesPerPage = (int)(e.MarginBounds.Height / this.Font.GetHeight(e.Graphics));
            int lineNo = this.pagesPrinted * linesPerPage;
            //друкуємо кожен рядок з масиву
            int count = 0;
            while (count < linesPerPage && lineNo < nLines)
            {
                line = split[count];
                yPos = topMargin + (count * this.Font.GetHeight(e.Graphics));
                e.Graphics.DrawString(line, this.Font, Brushes.Black, leftMargin, yPos, new StringFormat());
                lineNo++;
                count++;
            }
            if (nLines > lineNo)
                e.HasMorePages = true;
            else
                e.HasMorePages = false;
            pagesPrinted++;  //подсчет напечатанных страниц
        }
        private void PrintDocument()
        {
            //Організація діалогу друкування
            buferOut = richTextBox1.Text;
            this.pagesPrinted = 0;
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(this.pd_PrintPages);
            printDialog1.ShowDialog();
        }
        private void toolStripMenuItem_Print_Click(object sender, EventArgs e)
        {
            PrintDocument();
        }
        private void printToolStripButton_Click(object sender, EventArgs e)
        {
            PrintDocument();
        }

        /// ВИХІД
        private void toolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Завершення роботи - перевірити чи збережені зміни у файлі
            if (docChanged)
            {
                DialogResult dialogResult;
                dialogResult = MessageBox.Show("Зберегти зміни?", "Yakubovych's TextEditor",
                                      MessageBoxButtons.YesNoCancel,
                                      MessageBoxIcon.Warning);
                switch (dialogResult)
                {
                    case DialogResult.Yes:
                        SaveDocument();
                        break;
                    case DialogResult.No:
                        break;
                    case DialogResult.Cancel:
                        // відмінити закриття вікна програми
                        e.Cancel = true;
                        return;
                };
            }
        }

        /// ПАРАМЕТРИ
        /// ВКЛ./ВИКЛ. ПАНЕЛЬ ІНСТРУМЕНТІВ
        private void toolStripMenuItem_Toolbar_Click(object sender, EventArgs e)
        {
            toolStrip1.Visible = !toolStrip1.Visible;
            toolStripMenuItem_Toolbar.Checked = !toolStripMenuItem_Toolbar.Checked;
        }

        /// ШРИФТ
        private void toolStripMenuItem_Font_Click(object sender, EventArgs e)
        {
            fontDialog1.Font = richTextBox1.Font;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionFont = fontDialog1.Font;
            }

        }

        /// КОЛІР ТЕКСТУ
        private void toolStripMenuItem_Color_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.OK)
                richTextBox1.SelectionColor = colorDialog1.Color;
        }

        /// ДОПОМОГА
        /// МОЯ ДОВІДКА
        private void toolStripMenuItem_MyHelp_Click(object sender, EventArgs e)
        {
            MyHelp help = new MyHelp();
            help.ShowDialog();
        }

        private void helpToolStripButton_Click(object sender, EventArgs e)
        {
            toolStripMenuItem_MyHelp_Click(this, new EventArgs());
        }

        /// КНОПКИ
        /// ТЕКСТОВЕ ПОЛЕ
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            docChanged = true;
        }

        /// ВИРІЗАТИ
        private void cutToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        /// КОПІЮВАТИ
        private void copyToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        /// ПОМІСТИТИ
        private void pasteToolStripButton_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        /// ВИРІВНЮВАННЯ ТЕКСТУ
        /// ЗЛІВА
        private void toolStripButton_AlignmentLeft_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;
        }

        /// ПО ЦЕНТРУ
        private void toolStripButton_AlignmentCenter_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
        }

        /// СПРАВА
        private void toolStripButton_AlignmentRight_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

    }
}