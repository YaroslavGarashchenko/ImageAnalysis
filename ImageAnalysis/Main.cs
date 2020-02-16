using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Data.Linq;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Diagnostics;
using System.Threading;

namespace ImageAnalysis
{
    public partial class FormImageAnalysis : Form
    {
        public FormImageAnalysis()
        {
            InitializeComponent();
        }

        string fileName = "";
        static MyProcedures procedures = new MyProcedures();
        static List<Pixels> colors = new List<Pixels>();
        Color colorSelect = Color.White;
        static Bitmap bitmapTemp = null;
        List<Pixels> pixelsTemp = null;
        static int gistIntervals = 20;

        /// <summary>
        /// Выбор файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            //openFileDialog1.InitialDirectory = "d:\\";
            openFileDialog1.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Image img = Image.FromFile(openFileDialog1.FileName);
                    pictureBoxImage.Image = img;
                    fileName = openFileDialog1.FileName;
                    buttonAColor.Enabled = true;
                    toolStripStatusLabelInpho.Text = "Выбран файл изображения: " + fileName;
                    colors.Clear();
                    bitmapTemp = new Bitmap(fileName, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        /// <summary>
        /// Формирование базы данных пикселей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCreateData_Click(object sender, EventArgs e)
        {
            if (bitmapTemp == null) return;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            for (int xb = 0; xb < bitmapTemp.Width; xb++)
            {
                for (int yb = 0; yb < bitmapTemp.Height; yb++)
                {
                    Color pixelColor = bitmapTemp.GetPixel(xb, yb);

                    colors.Add(new Pixels() { X = xb, Y = yb, R = pixelColor.R, B = pixelColor.B, G = pixelColor.G,
                        H = pixelColor.GetHue(), S = pixelColor.GetSaturation(), V = pixelColor.GetBrightness() });
                }
                procedures.ProgressBarRefresh(toolStripProgressBarCalculate, xb, bitmapTemp.Width);
            }
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            toolStripStatusLabelInpho.Text = "Исходные данные загружены за " + ts.Minutes + " м: " +
                                             ts.Seconds + " с: " + ts.Milliseconds +
                                             " мс (общее количество миллисекунд - " + ts.TotalMilliseconds + ")";
        }

        /// <summary>
        /// Массив гистограмм R, G, B
        /// </summary>
        static List<Stat_analysis.ElementGist>[] gistRGB = new List<Stat_analysis.ElementGist>[6];
        /// <summary>
        /// Массив результатов стат.анализа
        /// </summary>
        static ColorParam listParRGB = new ColorParam() ;

        /// <summary>
        /// Статистический анализ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStatAnalysis_Click(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            if (colors.Count() == 0)
            {
                MessageBox.Show("Отсутствуют данные для анализа!", "Сообщение");
                return;
            }
            timer.Start();
            gistIntervals = (int)numericUpDownIntervals.Value;
            // Данные гистограмм
            new Thread(CreateGistR).Start();
            Thread.Sleep(0);
            new Thread(CreateGistG).Start();
            Thread.Sleep(0);
            new Thread(CreateGistB).Start();
            Thread.Sleep(0);
            new Thread(CreateGistH).Start();
            Thread.Sleep(0);
            new Thread(CreateGistS).Start();
            Thread.Sleep(0);
            new Thread(CreateGistV).Start();
            Thread.Sleep(0);
            // Результаты статистического анализа
            new Thread(CreateListParR).Start();
            Thread.Sleep(0);
            new Thread(CreateListParG).Start();
            Thread.Sleep(0);
            new Thread(CreateListParB).Start();
            Thread.Sleep(0);
            new Thread(CreateListParH).Start();
            Thread.Sleep(0);
            new Thread(CreateListParS).Start();
            Thread.Sleep(0);
            new Thread(CreateListParV).Start();
            Thread.Sleep(0);
            while (!listParRGB.VerifyData())
            {
                Thread.Sleep(200);
            }
            richTextBoxResult.Text = procedures.StatRezult(listParRGB);
            buttonGistR.Enabled = true;
            buttonGistG.Enabled = true;
            buttonGistB.Enabled = true;
            buttonGistH.Enabled = true;
            buttonGistS.Enabled = true;
            buttonGistV.Enabled = true;
            timer.Stop();
            TimeSpan ts = timer.Elapsed;
            toolStripStatusLabelInpho.Text = "Расчет выполнен за: " + ts.Minutes + " м: " +
                                             ts.Seconds + " с: " + ts.Milliseconds +
                                             " мс (общее количество миллисекунд - " + ts.TotalMilliseconds + ")";
        }
        static void CreateGistR() { gistRGB[0] = new Stat_analysis().Gist((colors.Select(x => (float)x.R)).ToArray(), gistIntervals); }
        static void CreateGistG() { gistRGB[1] = new Stat_analysis().Gist((colors.Select(y => (float)y.G)).ToArray(), gistIntervals); }
        static void CreateGistB() { gistRGB[2] = new Stat_analysis().Gist((colors.Select(z => (float)z.B)).ToArray(), gistIntervals); }
        static void CreateGistH() { gistRGB[3] = new Stat_analysis().Gist((colors.Select(x => (float)x.H)).ToArray(), gistIntervals); }
        static void CreateGistS() { gistRGB[4] = new Stat_analysis().Gist((colors.Select(y => (float)y.S)).ToArray(), gistIntervals); }
        static void CreateGistV() { gistRGB[5] = new Stat_analysis().Gist((colors.Select(z => (float)z.V)).ToArray(), gistIntervals); }
        static void CreateListParR() { listParRGB.R = new Stat_analysis().Stat((colors.Select(x => (float)x.R)).ToArray()); }
        static void CreateListParG() { listParRGB.G = new Stat_analysis().Stat((colors.Select(x => (float)x.G)).ToArray()); }
        static void CreateListParB() {listParRGB.B = new Stat_analysis().Stat((colors.Select(x => (float)x.B)).ToArray());  }
        static void CreateListParH() { listParRGB.H = new Stat_analysis().Stat((colors.Select(x => (float)x.H)).ToArray()); }
        static void CreateListParS() { listParRGB.S = new Stat_analysis().Stat((colors.Select(x => (float)x.S)).ToArray()); }
        static void CreateListParV() { listParRGB.V = new Stat_analysis().Stat((colors.Select(x => (float)x.V)).ToArray()); }
        /// <summary>
        /// Вывод гистограммы по компоненте R
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistR_Click(object sender, EventArgs e)
        {
            FormGist formGistogramR = procedures.CreateFormGist(gistRGB[0]);
            formGistogramR.Text += " Компонента цвета R";
            formGistogramR.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramR.chartGistogram.ChartAreas[0].AxisX.Maximum = 255;
            formGistogramR.Show();
        }
        /// <summary>
        /// Вывод гистограммы по компоненте G
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistG_Click(object sender, EventArgs e)
        {
            FormGist formGistogramG = procedures.CreateFormGist(gistRGB[1]);
            formGistogramG.Text += " Компонента цвета G";
            formGistogramG.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramG.chartGistogram.ChartAreas[0].AxisX.Maximum = 255;
            formGistogramG.Show();
        }
        /// <summary>
        /// Вывод гистограммы по компоненте B
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistB_Click(object sender, EventArgs e)
        {
            FormGist formGistogramB = procedures.CreateFormGist(gistRGB[2]);
            formGistogramB.Text += " Компонента цвета B";
            formGistogramB.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramB.chartGistogram.ChartAreas[0].AxisX.Maximum = 255;
            formGistogramB.Show();
        }
        /// <summary>
        /// Развертывание текстового поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RichTextBoxResult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (richTextBoxResult.Dock != DockStyle.Fill)
            {
                richTextBoxResult.Dock = DockStyle.Fill;
            }
            else
            {
                richTextBoxResult.Dock = DockStyle.None;
            }
        }
        /// <summary>
        /// Вывод гистограммы по компоненте H
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistH_Click(object sender, EventArgs e)
        {
            FormGist formGistogramH = procedures.CreateFormGist(gistRGB[3]);
            formGistogramH.Text += " Компонента цвета H";
            formGistogramH.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramH.chartGistogram.ChartAreas[0].AxisX.Maximum = 360;
            formGistogramH.Show();
        }
        /// <summary>
        /// Вывод гистограммы по компоненте S
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistS_Click(object sender, EventArgs e)
        {
            FormGist formGistogramS = procedures.CreateFormGist(gistRGB[4]);
            formGistogramS.Text += " Компонента цвета S";
            formGistogramS.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramS.chartGistogram.ChartAreas[0].AxisX.Maximum = 1;
            formGistogramS.Show();
        }
        /// <summary>
        /// Вывод гистограммы по компоненте V
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistV_Click(object sender, EventArgs e)
        {
            FormGist formGistogramV = procedures.CreateFormGist(gistRGB[5]);
            formGistogramV.Text += " Компонента цвета V";
            formGistogramV.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramV.chartGistogram.ChartAreas[0].AxisX.Maximum = 1;
            formGistogramV.Show();
        }
        /// <summary>
        /// Анализ изображения по площади выделенного цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAColor_Click(object sender, EventArgs e)
        {
            buttonShowGist.Enabled = false;
            Stopwatch timer = new Stopwatch();
            timer.Start();
            Size sizePixels = new Bitmap(fileName, true).Size;
            byte R1 = (byte)numericUpDownAR1.Value;
            byte R2 = (byte)numericUpDownAR2.Value;
            bool Rb = checkBoxAR.CheckState == CheckState.Checked;
            byte G1 = (byte)numericUpDownAG1.Value;
            byte G2 = (byte)numericUpDownAG2.Value;
            bool Gb = checkBoxAG.CheckState == CheckState.Checked;
            byte B1 = (byte)numericUpDownAB1.Value;
            byte B2 = (byte)numericUpDownAB2.Value;
            bool Bb = checkBoxAB.CheckState == CheckState.Checked;
            int H1 = (int)numericUpDownAH1.Value;
            int H2 = (int)numericUpDownAH2.Value;
            bool Hb = checkBoxAH.CheckState == CheckState.Checked;
            float S1 = (float)numericUpDownAS1.Value;
            float S2 = (float)numericUpDownAS2.Value;
            bool Sb = checkBoxAS.CheckState == CheckState.Checked;
            float V1 = (float)numericUpDownAV1.Value;
            float V2 = (float)numericUpDownAV2.Value;
            bool Vb = checkBoxAV.CheckState == CheckState.Checked;
            pixelsTemp = new List<Pixels>();
            foreach (Pixels item in colors) { pixelsTemp.Add(item); }
            if (Rb) pixelsTemp = pixelsTemp.Where(p => p.R >= R1 && p.R <= R2).ToList();
            if (Gb) pixelsTemp = pixelsTemp.Where(p => p.G >= G1 && p.G <= G2).ToList();
            if (Bb) pixelsTemp = pixelsTemp.Where(p => p.B >= B1 && p.B <= B2).ToList();
            if (Hb) pixelsTemp = pixelsTemp.Where(p => p.H >= H1 && p.H <= H2).ToList();
            if (Sb) pixelsTemp = pixelsTemp.Where(p => p.S >= S1 && p.S <= S2).ToList();
            if (Vb) pixelsTemp = pixelsTemp.Where(p => p.V >= V1 && p.V <= V2).ToList();
            float relation = (float)pixelsTemp.Count() / colors.Count();
            numericUpDownRelS.Value = (decimal)relation;
            string txt = "Относительное содержание выбранных пикселей: " + relation.ToString("E03") + "\n";
            richTextBoxAresult.Text += txt;

            if (checkBoxAVisual.CheckState == CheckState.Checked)
            {
                bitmapTemp = new Bitmap(fileName, true);
                foreach (var ip in pixelsTemp)
                {
                    bitmapTemp.SetPixel(ip.X, ip.Y, colorSelect);
                }
                pictureBoxAimage.Image = bitmapTemp;
                if (pictureBoxAimage.Size.Height / pictureBoxAimage.Size.Width < sizePixels.Height / sizePixels.Width)
                {
                    pictureBoxAimage.Size = new Size()
                    {
                        Width = (int)(sizePixels.Width * pictureBoxAimage.Size.Height / sizePixels.Height),
                        Height = pictureBoxAimage.Size.Height
                    };
                }
                else
                {
                    pictureBoxAimage.Size = new Size()
                    {
                        Width = pictureBoxAimage.Size.Width,
                        Height = (int)(sizePixels.Height * pictureBoxAimage.Size.Width / sizePixels.Width)
                    };
                }
            }
            timer.Stop();
            buttonFirstImage.Enabled = true;
            buttonFAnalysis.Enabled = true;
            TimeSpan ts = timer.Elapsed;
            toolStripStatusLabelInpho.Text = "Расчет выполнен за: " + ts.Minutes + " м. :" +
                                             ts.Seconds + " с." + ts.Milliseconds +
                                             " мс. (общее количество миллисекунд - " + ts.TotalMilliseconds;
        }

        /// <summary>
        /// Цвет выделения выделенного диапазона компонент цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LabelColor_Click(object sender, EventArgs e)
        {
            if (colorDialogSelect.ShowDialog() == DialogResult.OK)
            {
                labelColor.BackColor = colorSelect = colorDialogSelect.Color;
            }
        }

        private void CheckBoxAR_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAR.CheckState == CheckState.Checked)
            { numericUpDownAR1.Enabled = numericUpDownAR2.Enabled = true; }
            else
            { numericUpDownAR1.Enabled = numericUpDownAR2.Enabled = false; }
        }

        private void CheckBoxAG_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAG.CheckState == CheckState.Checked)
            { numericUpDownAG1.Enabled = numericUpDownAG2.Enabled = true; }
            else
            { numericUpDownAG1.Enabled = numericUpDownAG2.Enabled = false; }
        }

        private void CheckBoxAB_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAB.CheckState == CheckState.Checked)
            { numericUpDownAB1.Enabled = numericUpDownAB2.Enabled = true; }
            else
            { numericUpDownAB1.Enabled = numericUpDownAB2.Enabled = false; }
        }

        private void CheckBoxAH_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAH.CheckState == CheckState.Checked)
            { numericUpDownAH1.Enabled = numericUpDownAH2.Enabled = true; }
            else
            { numericUpDownAH1.Enabled = numericUpDownAH2.Enabled = false; }
        }

        private void CheckBoxAS_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAS.CheckState == CheckState.Checked)
            { numericUpDownAS1.Enabled = numericUpDownAS2.Enabled = true; }
            else
            { numericUpDownAS1.Enabled = numericUpDownAS2.Enabled = false; }
        }

        private void CheckBoxAV_CheckStateChanged(object sender, EventArgs e)
        {
            if (checkBoxAV.CheckState == CheckState.Checked)
            { numericUpDownAV1.Enabled = numericUpDownAV2.Enabled = true; }
            else
            { numericUpDownAV1.Enabled = numericUpDownAV2.Enabled = false; }
        }
        /// <summary>
        /// Визуализация выделения цветом пикселей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxAVisual_CheckStateChanged(object sender, EventArgs e)
        {
            if (!(checkBoxAVisual.Font.Bold))
            {
                checkBoxAVisual.Font = new Font(checkBoxAVisual.Font, FontStyle.Bold);
            }
            else
            {
                checkBoxAVisual.Font = new Font(checkBoxAVisual.Font, FontStyle.Regular);
            }
        }
        /// <summary>
        /// Определение цвета по нажатию курсором мышки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxAimage_MouseClick(object sender, MouseEventArgs e)
        {
            Point pointCursor = e.Location;
            int deltaX = 0, deltaY = 0, pixCurrentX = 0, pixCurrentY = 0;

            if (Math.Abs(pictureBoxAimage.Size.Width / pictureBoxAimage.Size.Height -
                pictureBoxAimage.Image.Size.Width / pictureBoxAimage.Image.Size.Height) < 0.02f)
            {
                pixCurrentX = (int)(pictureBoxAimage.Image.Width * pointCursor.X / pictureBoxAimage.Width);
                pixCurrentY = (int)(pictureBoxAimage.Image.Height * pointCursor.Y / pictureBoxAimage.Height);
            }
            else if (pictureBoxAimage.Size.Width / pictureBoxAimage.Size.Height > pictureBoxAimage.Image.Size.Width / pictureBoxAimage.Image.Size.Height)
            {
                deltaX = (int)((pictureBoxAimage.Image.Width - pictureBoxAimage.Image.Size.Width / pictureBoxAimage.Image.Size.Height *
                         pictureBoxAimage.Height) / 2);
                pixCurrentX = (int)(pictureBoxAimage.Image.Width * (pointCursor.X - deltaX) / (pictureBoxAimage.Width - 2 * deltaX));
                pixCurrentY = (int)(pictureBoxAimage.Image.Height * pointCursor.Y / pictureBoxAimage.Height);
            }
            else if (pictureBoxAimage.Size.Width / pictureBoxAimage.Size.Height < pictureBoxAimage.Image.Size.Width / pictureBoxAimage.Image.Size.Height)
            {
                deltaY = (int)(pictureBoxAimage.Image.Height - pictureBoxAimage.Width / pictureBoxAimage.Image.Size.Width /
                         pictureBoxAimage.Image.Size.Height) / 2; ;
                pixCurrentX = (int)(pictureBoxAimage.Image.Width * pointCursor.X / pictureBoxAimage.Width);
                pixCurrentY = (int)(pictureBoxAimage.Image.Height * (pointCursor.Y - deltaY) / (pictureBoxAimage.Height - 2 * deltaY)); ;
            }
            //MessageBox.Show("Курсор: " + pointCursor.ToString());

            if (e.Button == MouseButtons.Right)
            {
                panel2.BackColor = bitmapTemp.GetPixel(pixCurrentX, pixCurrentY);
            }
            else
            {
                panel1.BackColor = bitmapTemp.GetPixel(pixCurrentX, pixCurrentY);
            }
        }

        private void NumericUpDownAR1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAR1.Value >= numericUpDownAR2.Value)
            {
                numericUpDownAR1.Value = numericUpDownAR2.Value - 1;
            }
            panel1.BackColor = Color.FromArgb((int)numericUpDownAR1.Value, panel1.BackColor.G, panel1.BackColor.B);
        }

        private void NumericUpDownAR2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAR1.Value >= numericUpDownAR2.Value)
            {
                numericUpDownAR2.Value = numericUpDownAR1.Value + 1;
            }
            panel2.BackColor = Color.FromArgb((int)numericUpDownAR2.Value, panel2.BackColor.G, panel2.BackColor.B);
        }

        private void NumericUpDownAG1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAG1.Value >= numericUpDownAG2.Value)
            {
                numericUpDownAG1.Value = numericUpDownAG2.Value - 1;
            }
            panel1.BackColor = Color.FromArgb(panel1.BackColor.R, (int)numericUpDownAG1.Value, panel1.BackColor.B);
        }

        private void NumericUpDownAG2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAG1.Value >= numericUpDownAG2.Value)
            {
                numericUpDownAG2.Value = numericUpDownAG1.Value + 1;
            }
            panel2.BackColor = Color.FromArgb(panel2.BackColor.R, (int)numericUpDownAG2.Value, panel2.BackColor.B);
        }

        private void NumericUpDownAB1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAB1.Value >= numericUpDownAB2.Value)
            {
                numericUpDownAB1.Value = numericUpDownAB2.Value - 1;
            }
            panel1.BackColor = Color.FromArgb(panel1.BackColor.R, panel1.BackColor.G, (int)numericUpDownAB1.Value);
        }

        private void NumericUpDownAB2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAB1.Value >= numericUpDownAB2.Value)
            {
                numericUpDownAB2.Value = numericUpDownAB1.Value + 1;
            }
            panel2.BackColor = Color.FromArgb(panel2.BackColor.R, panel2.BackColor.G, (int)numericUpDownAB2.Value);
        }

        private void NumericUpDownAH1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAH1.Value >= numericUpDownAH2.Value)
            {
                numericUpDownAH1.Value = numericUpDownAH2.Value - 1;
            }
            //panel1.BackColor = procedures.ConvertHSVRGB((int)numericUpDownAH1.Value, (int)numericUpDownAS1.Value, (int)numericUpDownAV1.Value);
        }

        private void NumericUpDownAH2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAH1.Value >= numericUpDownAH2.Value)
            {
                numericUpDownAH2.Value = numericUpDownAH1.Value + 1;
            }
        }

        private void NumericUpDownAS1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAS1.Value >= numericUpDownAS2.Value)
            {
                numericUpDownAS1.Value = numericUpDownAS2.Value - numericUpDownAS1.Increment;
            }
        }

        private void NumericUpDownAS2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAS1.Value >= numericUpDownAS2.Value)
            {
                numericUpDownAS2.Value = numericUpDownAS1.Value + numericUpDownAS2.Increment;
            }
        }

        private void NumericUpDownAV1_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAV1.Value >= numericUpDownAV2.Value)
            {
                numericUpDownAV1.Value = numericUpDownAV2.Value - numericUpDownAV1.Increment;
            }
        }

        private void NumericUpDownAV2_ValueChanged(object sender, EventArgs e)
        {
            if (numericUpDownAV1.Value >= numericUpDownAV2.Value)
            {
                numericUpDownAV2.Value = numericUpDownAV1.Value + numericUpDownAV2.Increment;
            }
        }
        /// <summary>
        /// Изменение цвета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Panel1_BackColorChanged(object sender, EventArgs e)
        {
            numericUpDownAR1.Value = panel1.BackColor.R;
            numericUpDownAG1.Value = panel1.BackColor.G;
            numericUpDownAB1.Value = panel1.BackColor.B;
            numericUpDownAH1.Value = (decimal)panel1.BackColor.GetHue();
            numericUpDownAS1.Value = (decimal)panel1.BackColor.GetSaturation();
            numericUpDownAV1.Value = (decimal)panel1.BackColor.GetBrightness();
        }

        private void Panel2_BackColorChanged(object sender, EventArgs e)
        {
            numericUpDownAR2.Value = panel2.BackColor.R;
            numericUpDownAG2.Value = panel2.BackColor.G;
            numericUpDownAB2.Value = panel2.BackColor.B;
            numericUpDownAH2.Value = (decimal)panel2.BackColor.GetHue();
            numericUpDownAS2.Value = (decimal)panel2.BackColor.GetSaturation();
            numericUpDownAV2.Value = (decimal)panel2.BackColor.GetBrightness();
        }
        /// <summary>
        /// Показывать исходное изображение
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button1_Click(object sender, EventArgs e)
        {
            pictureBoxAimage.Image = bitmapTemp = new Bitmap(fileName, true);
        }

        private void RichTextBoxAresult_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (richTextBoxAresult.Dock != DockStyle.Fill)
            {
                richTextBoxAresult.Dock = DockStyle.Fill;
            }
            else
            {
                richTextBoxAresult.Dock = DockStyle.None;
            }
        }

        /// <summary>
        /// Выполнение расчета по одной мере для фрактального анализа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FAnalysis(object sender, EventArgs e)
        {

        }

        private void ButtonFAnalysis_Click(object sender, EventArgs e)
        {
            if (pixelsTemp != null)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Base_fract_anal fractAnal = procedures.FractalDimension(pixelsTemp, (int)numericUpDownCountMeasure.Value,
                    (int)numericUpDownMeasureOne.Value, checkBoxRelativeOrAbsolute.CheckState == CheckState.Checked, 
                    toolStripProgressBarCalculate);
                richTextBoxAresult.Text += "\n" + fractAnal.ToString(fractAnal.FractalDimensionSquare);
                timer.Stop();
                TimeSpan ts = timer.Elapsed;
                toolStripStatusLabelInpho.Text = "Фрактальный анализ выполнен за: " + ts.Minutes + " м. :" +
                                                 ts.Seconds + " с." + ts.Milliseconds +
                                                 " мс. (общее количество миллисекунд - " + ts.TotalMilliseconds;
            }
            else
            {
                MessageBox.Show("Отсутствуют исходные данные.","Ошибка!");
            }
        }

        /// <summary>
        /// Массив гистограмм для вертикального распределения
        /// </summary>
        private List<Stat_analysis.ElementGist> gistVert = new List<Stat_analysis.ElementGist>();
        /// <summary>
        /// Массив гистограмм для горизонтального распределения
        /// </summary>
        private List<Stat_analysis.ElementGist> gistGor = new List<Stat_analysis.ElementGist>();
        /// <summary>
        /// Массив результатов стат.анализа вертикального и горизонтального распределения 
        /// </summary>
        private ColorParam parVG = new ColorParam();
        
        /// <summary>
        /// Формирование гистограмм вертикального и горизонтального распределения показателей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonGistGVertical_Click(object sender, EventArgs e)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            if (bitmapTemp == null || pixelsTemp == null || colors.Count() == 0)
            {
                MessageBox.Show("Отсутствуют данные для анализа!", "Сообщение");
                return;
            }
            float[] dataX = pixelsTemp.Select(p => (float)p.X).ToArray();
            float[] dataY = pixelsTemp.Select(p => (float)p.Y).ToArray();
            // Данные гистограмм
            gistGor = new Stat_analysis().Gist(dataX, gistIntervals);
            gistVert = new Stat_analysis().Gist(dataY, gistIntervals);
            // Результаты статистического анализа
            parVG.Gor = new Stat_analysis().Stat(dataX);
            parVG.Vert = new Stat_analysis().Stat(dataY);
            richTextBoxAresult.Text += procedures.StatRezult(parVG);
            timer.Stop();
            buttonShowGist.Enabled = true;
            TimeSpan ts = timer.Elapsed;
            toolStripStatusLabelInpho.Text = "Расчет выполнен за: " + ts.Minutes + " м: " +
                                             ts.Seconds + " с: " + ts.Milliseconds +
                                             " мс (общее количество миллисекунд - " + ts.TotalMilliseconds + ")";
        }
        /// <summary>
        /// Показать гистограммы распределения пикселей по X/Y
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonShowGist_Click(object sender, EventArgs e)
        {
            FormGist formGistogramX = procedures.CreateFormGist(gistGor);
            FormGist formGistogramY = procedures.CreateFormGist(gistVert);
            formGistogramX.Text += " Распределение выделенных пикселей по оси X";
            formGistogramY.Text += " Распределение выделенных пикселей по оси Y";
            formGistogramX.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramY.chartGistogram.ChartAreas[0].AxisX.Minimum = 0;
            formGistogramX.chartGistogram.ChartAreas[0].AxisX.Maximum = bitmapTemp.Width;
            formGistogramY.chartGistogram.ChartAreas[0].AxisX.Maximum = bitmapTemp.Height;
            formGistogramX.Show();
            formGistogramY.Show();
        }

        private void CheckBoxRelativeOrAbsolute_CheckStateChanged(object sender, EventArgs e)
        {
            if(((CheckBox)sender).CheckState == CheckState.Checked)
            {
                ((CheckBox)sender).Text = "Абсол.величина меры";
            }
            else
            {
                ((CheckBox)sender).Text = "Относит.величина меры";
            }
        }
        /// <summary>
        /// Удаление текста отчета
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonErase_Click(object sender, EventArgs e)
        {
            richTextBoxAresult.Clear();
        }

        private void buttonCalculate_Click(object sender, EventArgs e)
        {
            MessageBox.Show("CountProcessor: " + Environment.ProcessorCount.ToString());
        }
    }
}
