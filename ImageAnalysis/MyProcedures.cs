using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Text;
using System.Threading;

namespace ImageAnalysis
{
    public class MyProcedures
    {
        /// <summary>
        /// Процедура перехода между аддитивными цветовыми моделями (RGB->HSV)
        /// </summary>
        public Pixels ConvertRGBHSV(Pixels pixelColor)
        {
            pixelColor.H = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B).GetHue();
            pixelColor.S = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B).GetSaturation();
            pixelColor.V = Color.FromArgb(pixelColor.R, pixelColor.G, pixelColor.B).GetBrightness();
            return pixelColor;
        }
        /// <summary>
        /// Процедура перехода между аддитивными цветовыми моделями (HSV(L)->RGB)
        /// </summary>
        public Color ConvertHSVRGB(float H, float S, float L)
        {
            Color pixelColor = new Color();
            float C = (1 - Math.Abs(2L - 1)) * S;
            float X = C * (1 - Math.Abs((H / 60)%2 - 1));
            float m = L - C / 2;
            //Диапазон значений H
            if (H >= 0 && H < 60) pixelColor = Color.FromArgb((int)((C + m)*255), (int)((X + m) * 255), (int)((0 + m) * 255));
            else if (H >= 60 && H < 120) pixelColor = Color.FromArgb((int)((X + m) * 255), (int)((C + m) * 255), (int)((0 + m) * 255));
            else if (H >= 120 && H < 180) pixelColor = Color.FromArgb((int)((0 + m) * 255), (int)((C + m) * 255), (int)((X + m) * 255));
            else if (H >= 180 && H < 240) pixelColor = Color.FromArgb((int)((0 + m) * 255), (int)((X + m) * 255), (int)((C + m) * 255));
            else if (H >= 240 && H < 300) pixelColor = Color.FromArgb((int)((X + m) * 255), (int)((0 + m) * 255), (int)((C + m) * 255));
            else if (H >= 300 && H < 360) pixelColor = Color.FromArgb((int)((C + m) * 255), (int)((0 + m) * 255), (int)((X + m) * 255));
            return pixelColor;
        }
        /// <summary>
        /// Обновление ProgressBar
        /// </summary>
        /// <param name="ProgressBar">Объект класса ToolStripProgressBar</param>
        /// <param name="i">Текущее значение</param>
        /// <param name="count">Общее количество объема данных</param>
        public void ProgressBarRefresh(ToolStripProgressBar ProgressBar, int i, int count)
        {
            if (count != 0)
                ProgressBar.Value = (int)(ProgressBar.Minimum + (ProgressBar.Maximum - ProgressBar.Minimum) * i / count);
            Application.DoEvents();
        }

        //Углы поворота модели вокруг осей X и Y
        double angleXrad;
        double angleYrad;
        double angleZrad;

        /// <summary>
        /// Поворот системы координат вокруг осей X и Y
        /// </summary>
        /// <param name="X">координата точки по оси X</param>
        /// <param name="Y">координата точки по оси Y</param>
        /// <param name="Z">координата точки по оси Z</param>
        /// <returns>Массив XYZ точки в новой системе координат</returns>
        public float[] TurnXY(float X, float Y, float Z, float angleX, float angleY)
        {
            //угол в радианах
            angleXrad = Math.PI * angleX / 180;
            angleYrad = Math.PI * angleY / 180;

            return new float[]{ (float)(X * Math.Cos(angleYrad) + Z * Math.Sin(angleYrad)),
                                (float)((X * Math.Sin(angleYrad) - Z * Math.Cos(angleYrad)) * Math.Sin(angleXrad) +
                                Y * Math.Cos(angleXrad)),
                                (float)((-1*X * Math.Sin(angleYrad) + Z * Math.Cos(angleYrad)) * Math.Cos(angleXrad) +
                                Y * Math.Sin(angleXrad))};
        }

        /// <summary>
        /// Поворот вокруг оси Z
        /// </summary>
        /// <param name="X">координата точки по оси X</param>
        /// <param name="Y">координата точки по оси Y</param>
        /// <param name="angleZ">угол поворота</param>
        /// <returns></returns>
        public float[] TurnZ(float X, float Y, float angleZ)
        {
            //угол в радианах
            angleZrad = Math.PI * angleZ / 180;
            float X1 =  X * (float)Math.Cos(angleZrad) - Y * (float)Math.Sin(angleZrad);
            float Y1 = X * (float)Math.Sin(angleZrad) + Y * (float)Math.Cos(angleZrad);
            return new float[2] { X1, Y1 };
        }

        /// <summary>
        /// Окраска элементов по линейной зависимости
        /// </summary>
        /// <param name="X">величина</param>
        /// <param name="Xmin">минимальное значение интервала</param>
        /// <param name="Xmax">максимальное значение интервала</param>
        /// <param name="R1">компонента R для мин.значения</param>
        /// <param name="G1">компонента G для мин.значения</param>
        /// <param name="B1">компонента B для мин.значения</param>
        /// <param name="R2">компонента R для макс.значения</param>
        /// <param name="G2">компонента G для макс.значения</param>
        /// <param name="B2">компонента B для макс.значения</param>
        /// <returns></returns>
        public int[] ColorElementLine(int X, int Xmin, int Xmax,
                                  int R1, int G1, int B1,
                                  int R2, int G2, int B2)
        {
            int[] RGB = new int[3];
            //R
            RGB[0] = (int)(R1 + (R2 - R1) * (X - Xmin) / (Xmax - Xmin));
            //G
            RGB[1] = (int)(G1 + (G2 - G1) * (X - Xmin) / (Xmax - Xmin));
            //B
            RGB[2] = (int)(B1 + (B2 - B1) * (X - Xmin) / (Xmax - Xmin));
            //Исключения
            if (RGB[0] > 255) { RGB[0] = 255; }
            if (RGB[1] > 255) { RGB[1] = 255; }
            if (RGB[2] > 255) { RGB[2] = 255; }
            return RGB;
        }
        /// <summary>
        /// Окраска элементов по вероятностной зависимости
        /// </summary>
        /// <param name="X">величина</param>
        /// <param name="Xmin">минимальное значение интервала</param>
        /// <param name="Xmax">максимальное значение интервала</param>
        /// <param name="R1">компонента R для мин.значения</param>
        /// <param name="G1">компонента G для мин.значения</param>
        /// <param name="B1">компонента B для мин.значения</param>
        /// <param name="R2">компонента R для макс.значения</param>
        /// <param name="G2">компонента G для макс.значения</param>
        /// <param name="B2">компонента B для макс.значения</param>
        /// <returns></returns>
        public int[] ColorElementRandom(int X, int Xmin, int Xmax,
                                  int R1, int G1, int B1,
                                  int R2, int G2, int B2)
        {
            int[] RGB = new int[3];
            //Random r = new Random();
            //double value = r.NextDouble();
            //R
            RGB[0] = (int)((R2 + R1) / 2 + (int)Math.Floor((decimal)((R2 - R1) / 2)
                        * ((decimal)Math.Cos(Math.PI * (X - Xmin) / (Xmax - Xmin)))));
            //G
            RGB[1] = (int)((G2 + G1) / 2 + (int)Math.Floor((decimal)((G2 - G1) / 2)
                        * ((decimal)Math.Cos(Math.PI / 4 + Math.PI * (X - Xmin) / (Xmax - Xmin)))));
            //B
            //RGB[2] = (int)((B2 + B1) / (3/2 + value) + (int)Math.Floor((decimal)(value * (B2 - B1) / 2)
            //    * ((decimal)Math.Cos(Math.PI / 2 + Math.PI * (X - Xmin) / (Xmax - Xmin)))));
            RGB[2] = (int)((B2 + B1) / 2 + (int)Math.Floor((decimal)((B2 - B1) / 2)
                        * ((decimal)Math.Cos(Math.PI / 2 + Math.PI * (X - Xmin) / (Xmax - Xmin)))));
            //Исключения
            if (RGB[0] > 240) { RGB[0] = 255; }
            if (RGB[1] > 240) { RGB[1] = 255; }
            if (RGB[2] > 240) { RGB[2] = 255; }
            if (RGB[0] <= 10)   { RGB[0] = 0; }
            if (RGB[1] <= 10)   { RGB[1] = 0; }
            if (RGB[2] <= 10)   { RGB[2] = 0; }
            return RGB;
        }

        /// <summary>
        /// Площадь многоугольника (сумма площадей по двум вершинам ребер)
        /// </summary>
        /// <param name="P1">Первая вершина</param>
        /// <param name="P2">Вторая вершина</param>
        /// <returns></returns>
        public float SquareSection(PointF P1, PointF P2)
        {
            //return Math.Abs((P1.X + P2.X) * (P1.Y - P2.Y) / 2);
            return (P1.X + P2.X) * (P1.Y - P2.Y) / 2;
        }

        /// <summary>
        /// Сохранение данных DataGridView в буфер обмена
        /// </summary>
        /// <param name="dataGridView"></param>
        public void SaveDataGridInXlc(DataGridView dataGridView, ToolStripProgressBar tempProgressBar, int columnSave)
        {
                Clipboard.Clear();
                string clipboardTable = "";
                for (int j = 0; j < dataGridView.ColumnCount; j++)
                {
                    clipboardTable += dataGridView.Columns[j].HeaderText;
                    clipboardTable += "\t";
                }
                clipboardTable += "\n";
                //
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    ProgressBarRefresh(tempProgressBar, i, dataGridView.Rows.Count);

                    for (int j = 0; j < columnSave; j++)
                    {
                        if (dataGridView.Rows[i].Cells[j].Value != null)
                        {
                            clipboardTable += dataGridView.Rows[i].Cells[j].Value.ToString();
                        }
                        else
                        {
                            clipboardTable += "---";
                        }
                        clipboardTable += "\t";
                    }
                    clipboardTable += "\n";
                }
                Clipboard.SetText(clipboardTable);
                MessageBox.Show("Данные таблицы помещены в буфер обмена");
        }

        /// <summary>
        /// Сохранение данных DataGridView в CSV файл
        /// </summary>
        /// <param name="dataGridView"></param>
        public void SaveDataGridInCsv(DataGridView dataGridView, int columnSave)
        {
            if (dataGridView.ColumnCount == 0 || dataGridView.RowCount == 0)
            { return; }
            string fileName = Application.StartupPath + @"\Data\statdata_" + 
                              DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "_" +
                              DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + ".csv";
            
            //string fileName = Application.StartupPath + @"\Data\statdata.csv";
            StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default);
            sw.WriteLine(dataGridView.Columns[0].HeaderText + ",");
            for (int j = 1; j < dataGridView.ColumnCount - 1; j++)
            {
                sw.Write(dataGridView.Columns[j].HeaderText + ",");
            }
            sw.Write(dataGridView.Columns[dataGridView.ColumnCount - 1].HeaderText);
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                sw.WriteLine((dataGridView.Rows[i].Cells[0].Value.ToString()).Replace(",", ".") + ",");

                for (int j = 1; j < columnSave - 1; j++)
                {
                    sw.Write((dataGridView.Rows[i].Cells[j].Value.ToString()).Replace(",", ".") + ",");
                }
                sw.Write((dataGridView.Rows[i].Cells[columnSave - 1].Value.ToString()).Replace(",", "."));
            }
            sw.Close();
        }
        /// <summary>
        /// Сохранение данных DataGridView в TSV файл
        /// </summary>
        /// <param name="dataGridView"></param>
        public void SaveDataGridInTsv(DataGridView dataGridView, int columnSave)
        {
            if (dataGridView.ColumnCount == 0 || dataGridView.RowCount == 0)
            { return; }
            string fileName = Application.StartupPath + @"\Data\statdata_" +
                              DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString("00") + DateTime.Now.Day.ToString("00") + "_" +
                              DateTime.Now.Hour.ToString("00") + DateTime.Now.Minute.ToString("00") + DateTime.Now.Second.ToString("00") + ".tsv";

            StreamWriter sw = new StreamWriter(fileName, false, Encoding.Default);
            sw.WriteLine(dataGridView.Columns[0].HeaderText + "\t");
            for (int j = 1; j < dataGridView.ColumnCount - 1; j++)
            {
                sw.Write(dataGridView.Columns[j].HeaderText + "\t");
            }
            sw.Write(dataGridView.Columns[dataGridView.ColumnCount - 1].HeaderText);
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                sw.WriteLine(dataGridView.Rows[i].Cells[0].Value + "\t");

                for (int j = 1; j < columnSave - 1; j++)
                {
                    sw.Write(dataGridView.Rows[i].Cells[j].Value + "\t");
                }
                sw.Write(dataGridView.Rows[i].Cells[columnSave - 1].Value);
            }
            sw.Close();
        }
        /// <summary>
        /// Сохранение данных DataGridView в CSV файл
        /// </summary>
        /// <param name="dataGridView"></param>
        public void SaveDataGridInCsv(DataGridView dataGridView)
        {
            SaveDataGridInCsv(dataGridView, dataGridView.ColumnCount);
        }
        /// <summary>
        /// Сохранение данных DataGridView в TSV файл
        /// </summary>
        /// <param name="dataGridView"></param>
        public void SaveDataGridInTsv(DataGridView dataGridView)
        {
            SaveDataGridInTsv(dataGridView, dataGridView.ColumnCount);
        }

        static int countProc = Environment.ProcessorCount; // Количество процессоров
        static List<int> iScale = new List<int>(); //Количество мер

        /// <summary>
        /// Определение фрактальной размерности выделенных пикселей
        /// </summary>
        /// <param name="listE">Список пикселей изображения</param>
        /// <param name="CountFractalAnalysis">Количество фрактальных размерностей</param>
        /// <param name="m">Размер меры (масштаба)</param>
        /// <param name="absOrRelR">Абсолютное или относительное значение меры</param>
        /// <returns></returns>
        public Base_fract_anal FractalDimension(List<Pixels> listE, int CountFractalAnalysis,
                                                int m, bool absOrRelR, ToolStripProgressBar progressCalculate)
        {
            Base_fract_anal ResultFractalAnalysis = new Base_fract_anal();
            if (listE.Count < 3) { return ResultFractalAnalysis; }

            ResultFractalAnalysis.FractalDimensionSquare = new List<float>();
            ResultFractalAnalysis.CountSquare = new List<int>(); ;
            ResultFractalAnalysis.SizeSquare = new List<float>();
            ResultFractalAnalysis.PointS = new List<FractalMeraS>();
            ResultFractalAnalysis.CountElements = listE.Count();
            List<PointF> listPointF = new List<PointF>();
            int mera;//Масштаб измерения
            int[] LS = new int[CountFractalAnalysis]; // Размерность контура (количество клеток-мер)
            float[] MS = new float[CountFractalAnalysis]; // Мера для метода клеток

            //Клеточный метод определения фрактальной размерности 
            //Мин. и макс. координат изображения
            float xmin = 0, xmax = listE.Select(s => s.X).Max();
            float ymin = 0, ymax = listE.Select(s => s.Y).Max();
            // Первоначальный размер клетки  
            if (absOrRelR)
            { mera = m; } //абсолютный размер
            else
            { mera = (int)(((xmax - xmin) + (ymax - ymin)) / (2 * m)); } //клетка относительно среднеарифметического для размеров изображения 
            ResultFractalAnalysis.Mstart = mera;
            //Изменение меры
            for (int varMera = 0; varMera < CountFractalAnalysis; varMera++)
            {
                //int iSquare = 0; // Количество клеток содержащих выделенные пиксели
                List<PointF> listSquare = new List<PointF>(); //Список клеток (клетка задана двумя точками - t1(xmin,ymin), t2(xmax,ymax))
                float x = 0;
                while (x < xmax)
                {
                    float y = 0;
                    while (y < ymax)
                    {
                        listSquare.Add(new PointF() { X = x, Y = y });
                        y += mera;
                    }
                    x += mera;
                }
                List<PointF>[] listSquareForPrCount = new List<PointF>[countProc];
                int range = listSquare.Count / countProc + 1;
                for (int i = 0; i < countProc; i++)
                {
                    listSquareForPrCount[i] = listSquare.GetRange(i * range,
                        (i + 1) * range > listSquare.Count ? listSquare.Count - i * range : range);
                    DataCalculate dataTemp = new DataCalculate() { iRange = i, 
                                                                   dmera = mera,
                                                                   listPixels = listE,
                                                                   listPoint = listSquareForPrCount[i]
                    };
                    new Thread(ScaleRange).Start(dataTemp);
                    Thread.Sleep(0);
                }
                while (iScale.Count() != countProc)
                {
                    Thread.Sleep(500);
                }
                LS[varMera] = iScale.Sum();
                iScale.Clear();
                /*
                // Определяем количество клеток
                foreach (var itemS in listSquare)
                {
                    int pointInSquare = listE.Count(p => p.X >= itemS.X && p.Y >= itemS.Y && p.X < itemS.X + mera && p.Y < itemS.Y + mera);
                    if (pointInSquare >= 1 && pointInSquare < mera * mera)
                    {
                        LS[varMera]++;
                    }
                    
                    //ProgressBarRefresh(progressCalculate, varMera + i++/listSquare.Count(), CountFractalAnalysis);
                    //
                    foreach (var itemE in listE)
                    {
                        if (itemE.X >= itemS.X && itemE.Y >= itemS.Y && itemE.X < itemS.X + mera && itemE.Y < itemS.Y + mera)
                        {
                           LS[varMera]++;
                           break;
                        }
                    }
                    //
                }
                */
                MS[varMera] = mera;
                if (varMera > 0)
                {
                    ResultFractalAnalysis.FractalDimensionSquare.Add((float)((Math.Log(LS[varMera - 1]) - Math.Log(LS[varMera])) /
                                                 (Math.Log(MS[varMera] / MS[varMera - 1]))));
                }
                ResultFractalAnalysis.SizeSquare.Add(MS[varMera]);
                ResultFractalAnalysis.CountSquare.Add(LS[varMera]);
                mera /= 2;
                ProgressBarRefresh(progressCalculate, varMera + 1, CountFractalAnalysis);
            }
            return ResultFractalAnalysis;
        }

        /// <summary>
        /// Расчет количества мер по отдельному массиву List<PointF>
        /// </summary>
        /// <param name="data"></param>
        static void ScaleRange(object data)
        {
            List<PointF> listP = ((DataCalculate)data).listPoint;
            int iR = ((DataCalculate)data).iRange;
            List<Pixels> listPix = ((DataCalculate)data).listPixels;
            int mera = ((DataCalculate)data).dmera;
            int scale = 0;
            foreach (var itemS in listP)
            {
                int pointInSquare = listPix.Count(p => p.X >= itemS.X && p.Y >= itemS.Y && p.X < itemS.X + mera && p.Y < itemS.Y + mera);
                if (pointInSquare >= 1 && pointInSquare < mera * mera)
                {
                    scale++;
                }
            }
            iScale.Add(scale);
        }

        /// <summary>
        /// Создание формы гистограммы
        /// </summary>
        /// <param name="gistParTemp"></param>
        /// <returns></returns>
        public FormGist CreateFormGist(List<Stat_analysis.ElementGist> gistParTemp)
        {
            FormGist formGistogram = new FormGist();
            try
            {
                formGistogram.Activate();
                //Вывод данных на гистограмму распределения
                Series seriesStatisticalPar = new Series();
                Series seriesStatisticalPar2 = new Series();
                float SumInt = 0, SumPar = 0;
                //Относительные величины
                for (int i = 0; i < gistParTemp.Count; i++)
                { SumPar += gistParTemp[i].Y; }
                //
                for (int i = 0; i < gistParTemp.Count; i++)
                {
                    seriesStatisticalPar.Points.Add(
                        new DataPoint(Math.Round((gistParTemp[i].Xmin + gistParTemp[i].Xmax) / 2, 2), gistParTemp[i].Y / SumPar));
                    SumInt += gistParTemp[i].Y;
                    seriesStatisticalPar2.Points.Add(
                        new DataPoint(Math.Round((gistParTemp[i].Xmin + gistParTemp[i].Xmax) / 2, 2), SumInt / SumPar));
                }
                seriesStatisticalPar2.ChartArea =
                seriesStatisticalPar.ChartArea = "ChartArea1";
                seriesStatisticalPar2.ChartType =
                seriesStatisticalPar.ChartType = SeriesChartType.Column;
                formGistogram.chartIntegral.Palette =
                formGistogram.chartGistogram.Palette = ChartColorPalette.BrightPastel;
                formGistogram.chartGistogram.Series.Add(seriesStatisticalPar);
                formGistogram.chartIntegral.Series.Add(seriesStatisticalPar2);
                formGistogram.seriesDensity = seriesStatisticalPar;
            }
            catch (Exception e1)
            { MessageBox.Show(e1.Message); }
            return formGistogram;
        }
        /// <summary>
        /// Формирование строки с результатами стат.анализа
        /// </summary>
        /// <returns></returns>
        public string StatRezult(ColorParam parameters)
        {
            string text = "";
            //RGB,HSV
            if (parameters.R != null)
            {
                text += "Параметр: R \n";
                text += "Минимальное значение: " + parameters.R.Min + "\n" +
                        "Максимальное значение: " + parameters.R.Max + "\n" +
                        "Интервал: " + parameters.R.Delta + "\n" +
                        "Дисперсия: " + parameters.R.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.R.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.R.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.R.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.R.excess + "\n" +
                        "Коэффициент вариации: " + parameters.R.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.R.SampleSize + "\n";
            }
            if (parameters.G != null)
            {
                text += "Параметр: G \n";
                text += "Минимальное значение: " + parameters.G.Min + "\n" +
                        "Максимальное значение: " + parameters.G.Max + "\n" +
                        "Интервал: " + parameters.G.Delta + "\n" +
                        "Дисперсия: " + parameters.G.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.G.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.G.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.G.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.G.excess + "\n" +
                        "Коэффициент вариации: " + parameters.G.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.G.SampleSize + "\n";
            }
            if (parameters.B != null)
            {
                text += "Параметр: B \n";
                text += "Минимальное значение: " + parameters.B.Min + "\n" +
                        "Максимальное значение: " + parameters.B.Max + "\n" +
                        "Интервал: " + parameters.B.Delta + "\n" +
                        "Дисперсия: " + parameters.B.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.B.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.B.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.B.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.B.excess + "\n" +
                        "Коэффициент вариации: " + parameters.B.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.B.SampleSize + "\n";
            }
            if (parameters.H != null)
            {
                text += "Параметр: H \n";
                text += "Минимальное значение: " + parameters.H.Min + "\n" +
                        "Максимальное значение: " + parameters.H.Max + "\n" +
                        "Интервал: " + parameters.H.Delta + "\n" +
                        "Дисперсия: " + parameters.H.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.H.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.H.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.H.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.H.excess + "\n" +
                        "Коэффициент вариации: " + parameters.H.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.H.SampleSize + "\n";
            }
            if (parameters.S != null)
            {
                text += "Параметр: S \n";
                text += "Минимальное значение: " + parameters.S.Min + "\n" +
                        "Максимальное значение: " + parameters.S.Max + "\n" +
                        "Интервал: " + parameters.S.Delta + "\n" +
                        "Дисперсия: " + parameters.S.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.S.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.S.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.S.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.S.excess + "\n" +
                        "Коэффициент вариации: " + parameters.S.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.S.SampleSize + "\n";
            }
            if (parameters.V != null)
            {
                text += "Параметр: V \n";
                text += "Минимальное значение: " + parameters.V.Min + "\n" +
                        "Максимальное значение: " + parameters.V.Max + "\n" +
                        "Интервал: " + parameters.V.Delta + "\n" +
                        "Дисперсия: " + parameters.V.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.V.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.V.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.V.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.V.excess + "\n" +
                        "Коэффициент вариации: " + parameters.V.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.V.SampleSize + "\n";
            }
            if (parameters.Gor != null)
            {
                text += "Горизонтальное распределение выделенных пикселей: \n";
                text += "Минимальное значение: " + parameters.Gor.Min + "\n" +
                        "Максимальное значение: " + parameters.Gor.Max + "\n" +
                        "Интервал: " + parameters.Gor.Delta + "\n" +
                        "Дисперсия: " + parameters.Gor.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.Gor.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.Gor.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.Gor.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.Gor.excess + "\n" +
                        "Коэффициент вариации: " + parameters.Gor.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.Gor.SampleSize + "\n";
            }
            if (parameters.Vert != null)
            {
                text += "Вертикальное распределение выделенных пикселей: \n";
                text += "Минимальное значение: " + parameters.Vert.Min + "\n" +
                        "Максимальное значение: " + parameters.Vert.Max + "\n" +
                        "Интервал: " + parameters.Vert.Delta + "\n" +
                        "Дисперсия: " + parameters.Vert.D + "\n" +
                        "Среднеквадратическое отклонение: " + parameters.Vert.Sigma + "\n" +
                        "Среднеарифметическое значение: " + parameters.Vert.MeanAr + "\n" +
                        "Коэффициент асимметрии: " + parameters.Vert.Asymmetry + "\n" +
                        "Коэффициент эксцесса: " + parameters.Vert.excess + "\n" +
                        "Коэффициент вариации: " + parameters.Vert.KoefVariations + "\n" +
                        "Объем выборки: " + parameters.Vert.SampleSize + "\n";
            }
            return text;
        }

    /// <summary>
        /// Копирование данных гистограммы в буфер обмена
        /// </summary>
        /// <param name="sender"></param>
        public void CopyData(Chart sender)
        {
            if (sender.Series.Count == 0)
                return;
            try
            {
                Clipboard.Clear();
                string clipboardTable = "";
                for (int i = 0; i < sender.Series[0].Points.Count; i++)
                {
                    clipboardTable += sender.Series[0].Points[i].XValue.ToString() + "\t";
                    for (int j = 0; j < sender.Series[0].Points[i].YValues.Length; j++)
                    {
                        clipboardTable += sender.Series[0].Points[i].YValues[j].ToString() + "\t";
                    }
                    clipboardTable += "\n";
            }
                Clipboard.SetText(clipboardTable);
                MessageBox.Show("Данные графика помещены в буфер обмена. ",
                                "Отчет о выполнении действия ...");
            }
            catch (Exception e)
            {
                MessageBox.Show("Данные не внесены в буфер обмена... \n" + e.Message, "Ошибка");
            }
            
        }
    }
}
