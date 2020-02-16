﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace ImageAnalysis
{
    /// <summary>
    /// Класс статистического анализа
    /// </summary>
    public class Stat_analysis
    {
        /// <summary>
        /// Массив статистических характеристик
        /// </summary>
        private Statistics stat = new Statistics();
        /// <summary>
        /// Определение статистических характеристик для массива данных
        /// </summary>
        /// <returns>0 - мин., 1 - макс., 2 - интервал, 3 - дисперсия, 4 - ср.кв.откл., 5 - ср.арифм., 
        /// 6 - коэф.асимметрии, 7 - эксцесса, 8 - вариации, 9- меана, 10 - мода (0), 11 - медиана, 12 - объем выборки</returns>
        public Statistics Stat(float[] DataArray)
        {
            stat.Min = DataArray.Min(); //минимальное значение
            stat.Max = DataArray.Max(); //максимальное значение
            stat.Delta = stat.Max - stat.Min; // интервал значений
            stat.SampleSize = DataArray.Length; // объем выборки
            //
            float sumx = 0;
            float sumx2 = 0;
            float sumx3 = 0;
            float sumx4 = 0;
            for (int i = 0; i < stat.SampleSize; i++)
            {
                sumx += DataArray[i];
                sumx2 += DataArray[i] * DataArray[i];
                sumx3 += DataArray[i] * DataArray[i] * DataArray[i];
                sumx4 += DataArray[i] * DataArray[i] * DataArray[i] * DataArray[i];
            }
            //
            stat.MeanAr = sumx / DataArray.Length; // среднеарифметическое значение (начальный момент первого порядка)
            // начальные моменты
            float nm2 = sumx2 / DataArray.Length;
            float nm3 = sumx3 / DataArray.Length;
            float nm4 = sumx4 / DataArray.Length;
            // центральные моменты
            float cm3 = nm3 - 3 * stat.MeanAr * nm2 + 2 * stat.MeanAr * stat.MeanAr * stat.MeanAr;
            float cm4 = nm4 - 4 * stat.MeanAr * nm3 + 6 * stat.MeanAr * stat.MeanAr * nm2 - 3 * stat.MeanAr * stat.MeanAr * stat.MeanAr * stat.MeanAr;
            //
            stat.D = nm2 - stat.MeanAr * stat.MeanAr; //дисперсия
            stat.Sigma = (float)Math.Sqrt(stat.D); // среднеквадратическое отклонение
            stat.Asymmetry = cm3 / (stat.D * stat.MeanAr); // коэффициент ассиметрии
            stat.excess = cm4 / (stat.D * stat.D) - 3; // коэффициент эксцесса
            stat.KoefVariations = stat.Sigma / stat.MeanAr; // коэффициент вариации
            stat.Mean = (stat.Max - stat.Min) / 2; // меана
            stat.Moda = 0; // мода
            try
            {
                if (DataArray.Length % 2 == 0)
                    stat.Median = (DataArray[DataArray.Length / 2 - 1] + DataArray[DataArray.Length / 2]) / 2; // медиана
                else
                    stat.Median = DataArray[(int)Math.Truncate((decimal)DataArray.Length / 2)];
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return stat;
        }
        /// <summary>
        /// Определение статистических характеристик для массива данных (Перегруженный) с модой
        /// </summary>
        /// <returns>0 - мин., 1 - макс., 2 - интервал, 3 - дисперсия, 4 - ср.кв.откл., 5 - ср.арифм., 
        /// 6 - коэф.асимметрии, 7 - эксцесса, 8 - вариации, 9- меана, 10 - мода (высчитывается), 11 - медиана, 12 - объем выборки</returns>
        public Statistics Stat(float[] DataArray, List<ElementGist> gist)
        {
            stat = Stat(DataArray);
            //
            float Maxgistmas = float.MinValue;
            stat.Moda = 0; // мода
            for (int i = 0; i < gist.Count; i++)
            {
                if (Maxgistmas < gist[i].Y)
                {
                    Maxgistmas = gist[i].Y;
                    stat.Moda = (gist[i].Xmin + gist[i].Xmax) / 2;
                }
            }
            return stat;
        }

        /// <summary>
        /// Данные элемента гистограммы
        /// </summary>
        public class ElementGist
        {
            private float xmin;
            /// <summary>
            /// Минимальное значение интервала
            /// </summary>
            public float Xmin
            {
                get { return xmin; }
                set { xmin = value; }
            }
            private float xmax;
            /// <summary>
            /// Максимальное значение интервала
            /// </summary>
            public float Xmax
            {
                get { return xmax; }
                set { xmax = value; }
            }
            private float y;
            /// <summary>
            /// Величина плотности интервала (относительная высота столбика)
            /// </summary>
            public float Y
            {
                get { return y; }
                set { y = value; }
            }
        }

        /// <summary>
        /// Список данных гистограммы
        /// </summary>
        private List<ElementGist> gist = new List<ElementGist>();

        /// <summary>
        /// Формирование данных для гистограмм
        /// </summary>
        /// <returns></returns>
        public List<ElementGist> Gist(float[] DataArray, int NumInt)
        {
            //Объем выборки
            int Nvib = DataArray.Length;
            //создаем список интервалов
            gist.Clear();
            //
            if (Nvib == 0)
            {
                MessageBox.Show("Нет данных для анализа", "Проблема!");
                return gist;
            }
            float Xmin = DataArray.Min();
            float Xmax = DataArray.Max();
            float Xint = Xmax - Xmin; // интервал значений

            for (int i = 0; i < NumInt; i++)
            {
                ElementGist tempElementGist = new ElementGist();
                tempElementGist.Xmin = Xmin + i * (Xint / NumInt);
                tempElementGist.Xmax = Xmin + (i + 1) * (Xint / NumInt);
                tempElementGist.Y = 0;
                gist.Add(tempElementGist);
            }
            if (!float.IsNaN(Xint) && !float.IsNaN(Xmin) && Xint != 0)
            {
                int k = 0; //индекс текущего интервала
                           //
                for (int j = 0; j < Nvib; j++)
                {
                    k = (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) != gist.Count ?
                        (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) : gist.Count - 1;
                    gist[k].Y += 1;
                }
            }
            return gist;
        }

        /// <summary>
        /// Формирование данных для гистограмм (перегружен с учетом весового коэффициента)
        /// </summary>
        /// <returns></returns>
        public List<ElementGist> Gist(float[] DataArray, int NumInt, float[] KoefVesArray)
        {
            //Объем выборки
            int Nvib = DataArray.Length;
            if (Nvib != KoefVesArray.Length)
            {
                MessageBox.Show("Несоответствие размеров исходных массивов для статистического анализа");
            }
            //
            float Xmin = DataArray.Min();
            float Xmax = DataArray.Max();
            float Xint = Xmax - Xmin; // интервал значений
            //создаем список интервалов
            gist.Clear();
            for (int i = 0; i < NumInt; i++)
            {
                ElementGist tempElementGist = new ElementGist();
                tempElementGist.Xmin = Xmin + i * (Xint / NumInt);
                tempElementGist.Xmax = Xmin + (i + 1) * (Xint / NumInt);
                tempElementGist.Y = 0;
                gist.Add(tempElementGist);
            }
            if (Xint != 0 && !float.IsNaN(Xmin) && !float.IsNaN(Xint))
            {
                int k = 0; //индекс текущего интервала
                           //
                for (int j = 0; j < Nvib; j++)
                {
                    k = (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) != gist.Count ?
                        (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) : gist.Count - 1;
                    gist[k].Y += KoefVesArray[j];
                }
            }
            return gist;
        }

        /// <summary>
        /// Формирование данных для гистограмм (перегружен с учетом весового коэффициента) и заданных мин. и макс. значения
        /// </summary>
        /// <returns></returns>
        public List<ElementGist> Gist(float[] DataArray, int NumInt, float[] KoefVesArray, float Xmin, float Xmax)
        {
            //Объем выборки
            int Nvib = DataArray.Length;
            if (Nvib != KoefVesArray.Length)
            {
                MessageBox.Show("Несоответствие размеров исходных массивов для статистического анализа");
            }
            //
            float Xint = Xmax - Xmin; // интервал значений
            //создаем список интервалов
            gist.Clear();
            for (int i = 0; i < NumInt; i++)
            {
                ElementGist tempElementGist = new ElementGist();
                tempElementGist.Xmin = Xmin + i * (Xint / NumInt);
                tempElementGist.Xmax = Xmin + (i + 1) * (Xint / NumInt);
                tempElementGist.Y = 0;
                gist.Add(tempElementGist);
            }
            if (Xint != 0 && !float.IsNaN(Xmin) && !float.IsNaN(Xint))
            {
                int k = 0; //индекс текущего интервала
                           //
                for (int j = 0; j < Nvib; j++)
                {
                    k = (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) != gist.Count ?
                        (int)Math.Floor((DataArray[j] - Xmin) / (Xint / NumInt)) : gist.Count - 1;
                    gist[k].Y += KoefVesArray[j];
                }
            }
            return gist;
        }

        /// <summary>
        /// Комплексный статистический анализ
        /// </summary>
        /// <param name="DataArray">Массив исходных данных</param>
        /// <param name="NumInt">Количество интервалов гистограммы</param>
        /// <returns> 0 - resultStatParLayer, 1 - seriesDensity, 2 - seriesIntegralFunction, 3 - resultQuartile</returns>
        public object[] ComplexAnalysis(float[] DataArray, int NumInt)
        {
            List<ElementGist> gistPar = Gist(DataArray, NumInt);
            Statistics resultStatParLayer = Stat(DataArray, gistPar);
            float[] resultQuartile = Quartile(DataArray);
            //Вывод данных на гистограмму распределения
            Series seriesDensity = new Series();
            Series seriesIntegralFunction = new Series();
            float SumInt = 0;
            float SumPar = 0;
            //Относительные величины
            for (int i = 0; i < gistPar.Count; i++)
                SumPar += gistPar[i].Y;

            for (int i = 0; i < gistPar.Count; i++)
            {
                seriesDensity.Points.Add(
                    new DataPoint(Math.Round((gistPar[i].Xmin + gistPar[i].Xmax) / 2, 2), gistPar[i].Y / SumPar));
                SumInt += gistPar[i].Y;
                seriesIntegralFunction.Points.Add(
                    new DataPoint(Math.Round((gistPar[i].Xmin + gistPar[i].Xmax) / 2, 2), SumInt / SumPar));
            }
            return new object[4] { resultStatParLayer, seriesDensity, seriesIntegralFunction, resultQuartile };
        }

        /// <summary>
        /// Определение квартилей для массива данных
        /// </summary>
        /// <returns>0 - мин., 1 - 1 квартиль., 2 - медиана, 3 - 3 квартиль, 4 - макс.</returns>
        public float[] Quartile(float[] DataArray)
        {
            if (DataArray == null || DataArray.Length < 5)
                return new float[5] { 0, 0, 0, 0, 0 };

            float[] result = new float[5];
            float[] dArray = DataArray.OrderBy(d => d).ToArray();
            //Array.Sort(dArray); ссылочный тип
            result[0] = dArray[0];
            result[1] = dArray[(int)Math.Floor((double)dArray.Length / 4)] +
                       (dArray[(int)Math.Floor((double)dArray.Length / 4)] -
                        dArray[(int)Math.Ceiling((double)dArray.Length / 4)]) *
                       ((float)Math.Floor((double)dArray.Length / 4) - (dArray.Length / 4));
            result[2] = (dArray[(int)Math.Ceiling((double)dArray.Length / 2)] +
                         dArray[(int)Math.Floor((double)dArray.Length / 2)]) / 2;
            result[3] = dArray[(int)Math.Floor((double)dArray.Length * 3 / 4)] +
                       (dArray[(int)Math.Floor((double)dArray.Length * 3 / 4)] -
                        dArray[(int)Math.Ceiling((double)dArray.Length * 3 / 4)]) *
                       ((float)Math.Floor((double)dArray.Length * 3 / 4) - (dArray.Length * 3 / 4));
            result[4] = dArray[dArray.Length - 1];
            return result;
        }

        /// <summary>
        /// Статистический сравнительный анализ двух выборок
        /// </summary>
        /// <param name="data1">Первый ряд данных</param>
        /// <param name="data2">Второй ряд данных</param>
        /// <returns> Массив значений критериев { [0] Kruskal–Wallis, [1] Mann–Whitney, [2] Манна — Уитни, 
        /// [3] Вальда — Вольфовица, [4] Колмогорова — Смирнова, [5] Пирсона } </returns>
        public static float[] ComparisonStatData( float[] data1, float[] data2 )
        {
            //U-критерий Манна
            //Составить единый ранжированный ряд из обеих сопоставляемых выборок, 
            //расставив их элементы по степени нарастания признака и приписав меньшему 
            //значению меньший ранг. Общее количество рангов получится равным:
            int GeneralLength = data1.Length + data2.Length;
            List<PointMassive> GeneralMassive = new List<PointMassive>();
            for (int i = 0; i < GeneralLength; i++)
            {
                if (i < data1.Length)
                {
                    GeneralMassive.Add(new PointMassive()
                    {
                        Rank = 0,
                        NumGroup = 1,
                        Point = new Point3D() { X = data1[i], Y = 0, Z = 0 }
                    });
                }
                else
                {
                    GeneralMassive.Add(new PointMassive()
                    {
                        Rank = 0,
                        NumGroup = 2,
                        Point = new Point3D() { X = data2[i - data1.Length], Y = 0, Z = 0 }
                    });
                }
            }
            List<PointMassive> GeneralMassiveOrdered = 
                                      GeneralMassive.OrderBy(p => p.Point.X).ToList<PointMassive>();
            //Определение рангов с учетом повторов значений
            int tempRank = 0;
            for (int j = 0; j < GeneralMassiveOrdered.Count(); j++)
            {
                tempRank = j + 1;
                int tCount = GeneralMassiveOrdered.Where(p => p.Point.X == GeneralMassiveOrdered[j].Point.X).Count();
                for (int i = 0; i < tCount; i++)
                {
                    GeneralMassiveOrdered[j + i].Rank = tempRank + (tCount - 1) / 2;
                }
                j += (tCount - 1);
            }

            int Ri1 = 0, Ri2 = 0;
            for (int i = 0; i < GeneralMassiveOrdered.Count(); i++)
            {
                if (GeneralMassiveOrdered[i].NumGroup == 1)
                {
                    Ri1 += GeneralMassiveOrdered[i].Rank;
                }
                else
                {
                    Ri2 += GeneralMassiveOrdered[i].Rank;
                }
            }
            float Ri1Mean = Ri1 / data1.Length;
            float Ri2Mean = Ri2 / data2.Length;

            // Kruskal–Wallis one-way analysis of variance
            //https://en.wikipedia.org/wiki/Kruskal–Wallis_one-way_analysis_of_variance
            float KruskalWallis = (12 / (GeneralLength * (GeneralLength + 1))) *
                                  (Ri1Mean * Ri1Mean * data1.Length +
                                   Ri2Mean * Ri2Mean * data2.Length) -
                                  3 * (GeneralLength + 1);
            // Mann–Whitney U test 
            //https://en.wikipedia.org/wiki/Mann–Whitney_U_test
            float U1 = Ri1 - (data1.Length * (data1.Length + 1) / 2);
            float U2 = Ri2 - (data2.Length * (data2.Length + 1) / 2);
            float Ueng = (new float[] { U1, U2 }).Min();
            // U-критерий Манна — Уитни
            //https://ru.wikipedia.org/wiki/U-критерий_Манна_—_Уитни
            float Rix = (new float[] { Ri1, Ri2 }).Max();
            float nx = Rix == Ri1 ? data1.Length : data2.Length;
            float Urus = data1.Length * data2.Length + nx * (nx + 1) / 2 - Rix;
            // Серийный критерий Вальда — Вольфовица
            //http://meteoinfo12.ru/files/lab_rab_2_aoed.pdf
            int[] GroupCount = new int[2] {0, 0 };
            for (int i = 0; i < GeneralMassiveOrdered.Count; i++)
            {
                int rank = GeneralMassiveOrdered.Where(n => n.Rank == GeneralMassiveOrdered[i].Rank).Count();
                if (rank > 1)
                {
                    if (i == 0)
                    { ++GroupCount[GeneralMassiveOrdered[i].NumGroup - 1]; }
                    else 
                    { ++GroupCount[0]; ++GroupCount[1]; }
                    i = i + rank - 1;
                }
                else
                {
                    if (i == 0)
                    {
                        ++GroupCount[ GeneralMassiveOrdered[i].NumGroup - 1];
                    }
                    else if (GeneralMassiveOrdered[i-1].NumGroup != GeneralMassiveOrdered[i].NumGroup)
                    {
                            ++GroupCount[GeneralMassiveOrdered[i].NumGroup - 1];
                    }
                }
            }
            float RValda = GroupCount.Sum();
            // Критерий Колмогорова — Смирнова
            float[] unique = data1.Union(data2).ToArray(); //Ряд с уникальными значениями
            int ALength = unique.Length;
            float maxDifference = 0;
            for (int i = 0; i < ALength; i++)
            {
                float value1 = GeneralMassiveOrdered.Where(n => n.Value <= unique[i] && n.NumGroup == 1).
                               Sum(n => n.Value);
                float value1Rel = value1 / GeneralMassiveOrdered.Where(n => n.Value <= unique[i] && n.NumGroup == 1).
                                  Count();
                float value2 = GeneralMassiveOrdered.Where(n => n.Value <= unique[i] && n.NumGroup == 2).
                               Sum(n => n.Value);
                float value2Rel = value2 / GeneralMassiveOrdered.Where(n => n.Value <= unique[i] && n.NumGroup == 2).
                                  Count();
                maxDifference = Math.Abs(value1Rel - value2Rel) > maxDifference ? 
                                Math.Abs(value1Rel - value2Rel) : maxDifference;
            }
            float lamda = maxDifference * maxDifference * (data1.Length * data2.Length / GeneralLength);
            // Критерий Пирсона Хи квадрат
            float pirson = float.NaN;
                
            return new float[6]{ KruskalWallis, Ueng, Urus, RValda, lamda, pirson };
        }

        /// <summary>
        /// Сравнение гистограмм (> 7 количество интервалов)
        /// </summary>
        /// <param name="data1">Список элементов 1-й гистограммы</param>
        /// <param name="data2">Список элементов 2-й гистограммы</param>
        /// <param name="criterion">Вычисляемый критерий</param>
        /// <param name="A">Уровень значимости</param>
        /// <returns></returns>
        public Validity ComparisonGistData(List<ElementGist> data1, List<ElementGist> data2, 
                                           StatisticalCriterion criterion = StatisticalCriterion.Wilcoxon,
                                           float A = 0.05F)
        {
            if (data1.Count() != data2.Count() || data1.Count() < 7 || 
                (data1.Count() > 50 && criterion == StatisticalCriterion.Wilcoxon))
                return Validity.excluded;
            //Список рангов
            List<PointMassive> listRank = new List<PointMassive>();  
            // Определение различий
            for (int i = 0; i < data1.Count(); i++)
            {
                listRank.Add(new PointMassive() {  Value = data1[i].Y - data2[i].Y });
            }
            switch (criterion)
            {
                case StatisticalCriterion.Wilcoxon:
                    listRank.RemoveAll(p => p.Value == 0);
                    if (listRank.Count() == 0)
                        return Validity.excluded;
                    List<PointMassive> listRankOrdered =
                                              listRank.OrderBy(p => Math.Abs(p.Value)).ToList<PointMassive>();
                    //Определение рангов с учетом повторов значений
                    int tempRank = 0;
                    for (int j = 0; j < listRankOrdered.Count(); j++)
                    {
                        tempRank = j + 1;
                        int tCount = listRankOrdered.Where(p => p.Value == listRankOrdered[j].Value).Count();
                        for (int i = 0; i < tCount; i++)
                        {
                            listRankOrdered[j + i].Rank = tempRank + (tCount - 1) / 2;
                        }
                        j += (tCount - 1);
                    }
                    int Ri1 = 0, Ri2 = 0;
                    for (int i = 0; i < listRankOrdered.Count(); i++)
                    {
                        if (listRankOrdered[i].Value < 0)
                        {
                            Ri1 += listRankOrdered[i].Rank;
                        }
                        else if (listRankOrdered[i].Value > 0)
                        {
                            Ri2 += listRankOrdered[i].Rank;
                        }
                    }
                    float Ri = Ri1 < Ri2 ? Ri1 : Ri2;
                    float limit = 0;
                    try
                    {
                        limit = Stat_criterion.listLimitWilcoxon.Where(c => c.N == data1.Count() && c.A == A).
                                                                       First().Value;
                    }
                    catch (Exception)
                    {
                        return Validity.excluded;
                    }
                    return Ri < limit ? Validity.no : Validity.yes;

                case StatisticalCriterion.Chesnokov:
                    int valueZero = listRank.Where(v => v.Value == 0).Count();
                    int valueLess = listRank.Where(v => v.Value < 0).Count();
                    int valueMore = listRank.Where(v => v.Value > 0).Count();
                    //Объем связи: C = K0 / (K0 + K1)
                    float C = valueZero / (valueZero + valueLess);
                    // Дефект связи: Де = 1 — K0 / (K0 + K2)
                    float De = 1 - valueZero / (valueZero + valueMore);
                    //{С  ≤ 0,5; Де ≥ 0,5}
                    return C <= 0.5F && De >= 0.5F ? Validity.no : Validity.yes;

                case StatisticalCriterion.PearsonsChiSquare:
                    if (data1[0].Xmin != data2[0].Xmin)
                    {
                        return Validity.excluded;
                    }
                    float chi = 0;
                    for (int i = 0; i < data1.Count(); i++)
                    {
                        chi += (data1[i].Y - data2[i].Y) * (data1[i].Y - data2[i].Y) / data1[i].Y;
                    }
                    chi *= data1.Count();
                    return VerifyCriterionPirson(chi, data1.Count(), A);

                default:
                    break;
            }
            return Validity.excluded;
        }

        /// <summary>
        /// Проверка справедливости нулевой гипотезы по критерию Вальда — Вольфовица
        /// </summary>
        /// <param name="R">Расчетная величина критерия</param>
        /// <param name="n1">Размер первого ряда значений</param>
        /// <param name="n2">Размер второго ряда значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public static Validity VerifyCriterionR(float R, int n1, int n2, float al)
        {
            int a = 2 * n1 * n2;
            int b = n1 + n2;
            float MR = a / b + 1; // Мат.ожидание R-критерия
            float SR = (float)Math.Sqrt(a * (a - b) / (b * b * (b - 1))); // Среднее квадр-е отклонение R-критерия
            float Rverify = (MR - R - 0.5F) / SR;
            float Rlimit = 1;
            if (al == 0.05F) Rlimit = 1.96F; 
            else if (al == 0.01F) Rlimit = 2.58F; 
            else return Validity.excluded; 

            return R <= Rlimit ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка справедливости нулевой гипотезы по критерию Колмогорова — Смирнова
        /// </summary>
        /// <param name="Lambda">Расчетная величина критерия</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public static Validity VerifyCriterionK(float Lambda, float al)
        {
            float Rlimit = 1;
            if (al == 0.05F) Rlimit = 1.84F; 
            else if (al == 0.01F) Rlimit = 2.65F;
            else return Validity.excluded; 

            return Lambda <= Rlimit ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка справедливости нулевой гипотезы по U-критерию Манна — Уитни
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="n1">Размер первого ряда значений</param>
        /// <param name="n2">Размер второго ряда значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public static Validity VerifyCriterionU(float U, int n1, int n2, float al)
        {
            float MU = n1 * n2 / 2; //Мат.ожидание U-критерия
            float DU = n1 * n2 * (n1 + n2 + 1) / 12; //Дисперсия U-критерия
            float alRel = Stat_criterion.NormFunction.Where(n => n.X <= (al / 2)).Min(n => n.Y);
            float Umin = MU - alRel * (float)Math.Sqrt(DU);
            return U > Umin ? Validity.yes : Validity.no;
        }
        /// <summary>
        /// Проверка гипотезы о принадлежности выборки равномерному закону
        /// </summary>
        /// <param name="data">Ряд данных</param>
        /// <param name="criterion">Критерий проверки</param>
        /// <returns> Величины критериев [0] Шермана, [1] Кимбала, [2] Андерсона-Дарлинга, [3] Крамера-Мизеса-Смирнова, [4-7] Ньюмана-Бартона,
        ///          [8] Дудевича, [9] Пирсона </returns>
        public float[] VerifyUniformLaw(float[] data, StatisticalCriterion criterion)
        {
            float[] result = new float[10];
            int N = data.Length; //Объем выборки
            Array.Sort(data);
            data = NormalizedData(data);
            float sum = 0;
            switch (criterion)
            {
                case StatisticalCriterion.Sherman:
                    sum = Math.Abs(data[0] - 1/(N + 1));
                    for (int i = 1; i < N; i++)
                    {
                        sum += Math.Abs(data[i] - data[i-1] - 1 / (N + 1));
                    }
                    result[0] = 0.5F * sum;
                    break;
                case StatisticalCriterion.Kimball:
                    sum = (data[0] - 1 / (N + 1)) * (data[0] - 1 / (N + 1));
                    for (int i = 1; i < N; i++)
                    {
                        sum += (float)Math.Pow(data[i] - data[i - 1] - 1 / (N + 1), 2);
                    }
                    result[1] = 0.5F * sum;
                    break;
                case StatisticalCriterion.AndersonDarling:
                    sum = -N;
                    for (int i = 1; i <= N; i++)
                    {
                        sum += (2*i-1) * (float)Math.Log(data[i - 1])/(2*N) + 
                               (1 - (2*i-1)/(2*N)) * (float)Math.Log(1 - data[i - 1]) ;
                    }
                    result[2] = -2F * sum;
                    break;
                case StatisticalCriterion.KramerMisesSmirnov:
                    sum = 1/(12*N);
                    for (int i = 1; i <= N; i++)
                    {
                        sum += (float)Math.Pow(data[i - 1] - (2 * i - 1) / (2 * N), 2);
                    }
                    result[3] = sum;
                    break;
                case StatisticalCriterion.NeymanBarton1polinom:
                    for (int i = 0; i < N; i++)
                    {
                        result[4] += 3.4641F * (data[i] - 0.5F);
                    }
                    break;
                case StatisticalCriterion.NeymanBarton2polinom:
                    for (int i = 0; i < N; i++)
                    {
                        result[5] += 2.2361F * (6 * (data[i] - 0.5F) * (data[i] - 0.5F) - 0.5F);
                    }
                    break;
                case StatisticalCriterion.NeymanBarton3polinom:
                    for (int i = 0; i < N; i++)
                    {
                        result[6] += 2.6458F * (20 * (float)Math.Pow(data[i] - 0.5F, 3) - 3 * (data[i] - 0.5F));
                    }
                    break;
                case StatisticalCriterion.NeymanBarton4polinom:
                    for (int i = 0; i < N; i++)
                    {
                        result[7] += 3 * (70 * (float)Math.Pow(data[i] - 0.5F, 4)
                                       - 15 * (float)Math.Pow(data[i] - 0.5F, 2) + 0.375F);
                    }
                    break;
                case StatisticalCriterion.DudevichVanDerMuelen:
                    int M = Stat_criterion.DudevichaValueM(N);
                    for (int i = 0; i < N; i++)
                    {
                        if (data[i + M >= N ? N - 1 : i + M] - data[i - M < 0 ? 0 : i - M] != 0)
                        sum += (float)Math.Log((N/(2*M))*(
                                data[i + M >= N ? N - 1 : i + M] 
                                - 
                                data[i - M < 0 ? 0 : i - M])); 
                    }
                    result[8] = sum / (-N);
                    break;
                case StatisticalCriterion.PearsonsChiSquare:
                    int n = СhoiceNumberIntervals(N);
                    float frequency = N / n; // частоты теоретические 
                    List<ElementGist> researchGist = Gist(data, n);
                    for (int j = 0; j < n; j++)
                    {
                        result[9] += (frequency - researchGist[j].Y) * (frequency - researchGist[j].Y) / researchGist[j].Y;
                    }
                    break;
                default:
                    MessageBox.Show("Неправильный выбор критерия!","Ошибка...");
                    break;
            }
            return result;
        }
        /// <summary>
        /// Нармализация массива чисел
        /// </summary>
        /// <param name="data">Исходный массив данных</param>
        /// <returns></returns>
        public float[] NormalizedData(float[] data)
        {
            float[] result = new float[data.Length];
            float min = data.Min();
            float max = data.Max();

            for (int i = 0; i < data.Length; i++)
            {
                result[i] = (data[i] - min) / (max - min);
            }
            return result;
        }

        /// <summary>
        /// Проверка гипотезы о принадлежности выборки равномерному закону (Критерий Шермана)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionSherman(float value, int N, float al)
        {
            float valueMin = float.NaN;
            if (Stat_criterion.listLimitSherman.Where(v => v.A == al && v.N <= N).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitSherman.Where(v => v.A == al && v.N <= N).OrderByDescending(v => v.N).
                                                Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return value < valueMin ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка на проверку гипотезы о принадлежности выборки равномерному закону (Критерий Кимбела)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionKimbela(float value, int N, float al)
        {
            float valueMin = float.NaN;
            if (Stat_criterion.listLimitKimbela.Where(v => v.A == al && v.N <= N).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitKimbela.Where(v => v.A == al && v.N <= N).OrderByDescending(v => v.N).
                                                Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return value < valueMin ? Validity.yes : Validity.no;
        }
        //Кобзарь Л.И. - Прикладная математическая статистика, стр. 220
        //Лучше, если использовать модифицированную форму [226]
        /// <summary>
        /// Проверка на проверку гипотезы о принадлежности выборки равномерному закону (Критерий Андерсона–Дарлинга)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionAndersonDarling(float value, int N, float al)
        {
            //Модифицированная форма
            float valueMod = (N*N*value + N + 1) / (N*N + N + 1);
            float valueMin = float.NaN;
            if (Stat_criterion.listLimitAndersonDarling.Where(v => v.A == al).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitAndersonDarling.Where(v => v.A == al).
                                                Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return valueMod < valueMin ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка на проверку гипотезы о принадлежности выборки равномерному закону (Критерий Крамера–Мизеса–Смирнова)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionKramerMisesSmirnov(float value, int N, float al)
        {
            float valueMod = N < 40 ? (value - 0.4F / N + 0.6F / (N * N)) * (1 + 1 / N) : value;

            float valueMin = float.NaN;
            if (Stat_criterion.listLimitKramerMisesSmirnov.Where(v => v.A == al).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitKramerMisesSmirnov.Where(v => v.A == al).
                                                Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return valueMod < valueMin ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка на проверку гипотезы о принадлежности выборки равномерному закону (Критерий Неймана–Бартона)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionNeymanBarton(float[] value, int N, float al)
        {
            if (value.Length != 10) return Validity.excluded;

            float valueMin2 = float.NaN, valueMin3 = float.NaN, valueMin4 = float.NaN;
            if (Stat_criterion.listLimitNeymanBarton2.Where(v => v.A >= al && v.N >= N).Count() != 0)
            {
                valueMin2 = Stat_criterion.listLimitNeymanBarton2.Where(v => v.A >= al && v.N >= N).
                                                Select(v => v.Value).First();
                valueMin3 = Stat_criterion.listLimitNeymanBarton3.Where(v => v.A >= al && v.N >= N).
                                                Select(v => v.Value).First();
                valueMin4 = Stat_criterion.listLimitNeymanBarton4.Where(v => v.A >= al && v.N >= N).
                                                Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return value[5] < valueMin2 && value[6] < valueMin3 && value[7] < valueMin4 ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка на проверку гипотезы о принадлежности выборки равномерному закону (Критерий Дудевича-ван дер Мюлена)
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="N">Размер выборки значений</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionDudevichVanDerMuelen(float value, int N, float al)
        {
            float valueMin = float.NaN;
            if (Stat_criterion.listLimitDudevichVanDerMuelen.Where( v => v.A >= al && v.N >= N ).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitDudevichVanDerMuelen.Where(v => v.A >= al && v.N >= N).
                                                Select(v => v.Value).First();
            }
            else if (Stat_criterion.listLimitDudevichVanDerMuelen.Where(v => v.A >= al).Count() == 0)
            {
                valueMin = Stat_criterion.listLimitDudevichVanDerMuelen.Where(v => v.A >= al && v.N <= N).
                                                Select(v => v.Value).Min();
            }
            else
            {
                return Validity.excluded;
            }
            return value < valueMin ? Validity.yes : Validity.no;
        }

        /// <summary>
        /// Проверка по критерию согласия хи-квадрат Пирсона
        /// </summary>
        /// <param name="U">Расчетная величина критерия</param>
        /// <param name="K">Количество интервалов гистограммы</param>
        /// <param name="al">Уровень значимости</param>
        /// <returns></returns>
        public Validity VerifyCriterionPirson(float value, int K, float al)
        {
            float valueMin = float.NaN;
            int n = 2; //число параметров теоретического закона распределения (по умолчанию)
            int r = K - 1 - n; // число степеней свободы
            if (Stat_criterion.listLimitPirson.Where(v => v.A >= al && v.K <= r).Count() != 0)
            {
                valueMin = Stat_criterion.listLimitPirson.Where(v => v.A >= al && v.K <= r).Select(v => v.Value).First();
            }
            else
            {
                return Validity.excluded;
            }
            return value < valueMin ? Validity.yes : Validity.no;
        }
        /// <summary>
        /// Выбор числа интервалов гистограммы от объема выборки (Формула Старджесса)
        /// </summary>
        /// <param name="N">Объем выборки</param>
        /// <returns></returns>
        public int СhoiceNumberIntervals(int N)
        {
            // AN006266. С.А. Бардасов. Гистограммы. Критерии оптимальности
            // Среднеарифметическое значение (округленное) от трех методов расчета (Стерджесс, Скотт, информационный критерий)
            List<NumIntervalsGistogram> recomendation = new List<NumIntervalsGistogram>()
                                               { new NumIntervalsGistogram() {Min =    0, Max =   30, Num = 4 },
                                                 new NumIntervalsGistogram() {Min =   31, Max =   60, Num = 5 },
                                                 new NumIntervalsGistogram() {Min =   61, Max =  100, Num = 6 },
                                                 new NumIntervalsGistogram() {Min =  101, Max =  200, Num = 7 },
                                                 new NumIntervalsGistogram() {Min =  201, Max =  500, Num = 8 },
                                                 new NumIntervalsGistogram() {Min =  501, Max = 1000, Num = 10 },
                                                 new NumIntervalsGistogram() {Min = 1001, Max = 5000, Num = 14 },
                                                 new NumIntervalsGistogram() {Min = 5001, Max = 10000, Num = 18 },
                                                 new NumIntervalsGistogram() {Min = 10001, Max = 50000, Num = 25 },
                                                 new NumIntervalsGistogram() {Min = 50001, Max = 100000, Num = 35 },
                                                 new NumIntervalsGistogram() {Min = 100001, Max = 200000, Num = 40 },
                                                 new NumIntervalsGistogram() {Min = 200001, Max = 500000, Num = 55 },
                                                 new NumIntervalsGistogram() {Min = 500001, Max = 1000000, Num = 70 },
                                                 new NumIntervalsGistogram() {Min = 1000001, Max = 5000000, Num = 85 },
                                                 new NumIntervalsGistogram() {Min = 5000001, Max = 10000000, Num = 100 },
                                                 new NumIntervalsGistogram() {Min = 10000001, Max = 50000000, Num = 170 },
                                                 new NumIntervalsGistogram() {Min = 50000001, Max = int.MaxValue, Num = 200 }
                                               };
            return recomendation.Where(n => n.Min <= N && n.Max >= N).Select(n => n.Num).First();
        }

        /// <summary>
        /// Количество вариантов сравнения набора данных
        /// </summary>
        /// <param name="numData"></param>
        /// <returns></returns>
        public static int NumVariantsComparison(int numData)
        {
            int numVar = 0;
            if (numData >= 2)
            {
                numVar = 1;
                for (int i = 2; i < numData; i++)
                {
                    numVar += i;
                }
            }
            return numVar;
        }

        /// <summary>
        /// Генерирование отчета по проверке гипотезы о законе распределения исследуемого признака
        /// </summary>
        /// <param name="resultStatParLayer">Массив статистических характеристик</param>
        /// <param name="researchMassive">Массив исследуемых признаков</param>
        /// /// <returns></returns>
        public string GenerateReportTestDistribution(Statistics resultStatParLayer, float[] researchMassive)
        {
            string result;
            if (resultStatParLayer == null)
            { result = "Нет результатов статистического анализа распределения исследуемого признака! \n"; }
            else
            {
                result = "Статистические характеристики распределения исследуемого признака:" + "\n";
                result += "Минимальная величина: " + resultStatParLayer.Min + " ;\n";
                result += "Максимальная величина: " + resultStatParLayer.Max + " ;\n";
                result += "Интервал величин: " + resultStatParLayer.Delta + " ;\n";
                result += "Дисперсия: " + resultStatParLayer.D + " ;\n";
                result += "Среднеквадратическое отклонение: " + resultStatParLayer.Sigma + " ;\n";
                result += "Среднеарифметическое значение: " + resultStatParLayer.MeanAr + " ;\n";
                result += "Коэффициент асимметрии: " + resultStatParLayer.Asymmetry + " ;\n";
                result += "Коэффициент эксцесса: " + resultStatParLayer.excess + " ;\n";
                result += "Коэффициент вариации: " + resultStatParLayer.KoefVariations + " ;\n";
                result += "Меана: " + resultStatParLayer.Mean + " ;\n";
                result += "Мода: " + resultStatParLayer.Moda + " ;\n";
                result += "Медиана: " + resultStatParLayer.Median + " ;\n";
                result += "Объем выборки: " + resultStatParLayer.SampleSize + " ;\n\n";

                result += "Проверка гипотезы о принадлежности выборки равномерному закону:";
                //Критерии: 0 - Шермана
                try
                {
                    float valueSherman = VerifyUniformLaw(researchMassive, StatisticalCriterion.Sherman)[0];
                    result += "\nКритерий Шермана - " + valueSherman + " ; \n";
                    //Проверка критерия Шермана по уровню значимости и объему выборки
                    foreach (var item in Stat_criterion.listLimitSherman.GroupBy(Gr => Gr.A).Select(alfa => alfa.First()).ToList())
                    {
                        Validity validitySherman = VerifyCriterionSherman(valueSherman, researchMassive.Count(), item.A);
                        result += validitySherman == Validity.yes ?
                        "При уровне значимости " + item.A + " гипотеза равномерности не отклоняется; \n" :
                        validitySherman != Validity.excluded ?
                        "При уровне значимости " + item.A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Шермана!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по Критерию Шермана \n\n" + ex.Message, "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 1 - Кимбала
                try
                {
                    float valueKimball = VerifyUniformLaw(researchMassive, StatisticalCriterion.Kimball)[1];
                    result += "\nКритерий Кимбала - " + valueKimball + " ; \n";
                    //Проверка критерия Кимбала по уровню значимости и объему выборки
                    foreach (var item in Stat_criterion.listLimitKimbela.GroupBy(Gr => Gr.A).Select(alfa => alfa.First()).ToList())
                    {
                        Validity validityKimball = VerifyCriterionKimbela(valueKimball, researchMassive.Count(), item.A);
                        result += validityKimball == Validity.yes ?
                        "При уровне значимости " + item.A + " гипотеза равномерности не отклоняется; \n" :
                        validityKimball != Validity.excluded ?
                        "При уровне значимости " + item.A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Шермана!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по Критерию Кимбала \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 2 - Андерсона-Дарлинга
                try
                {
                    float valueAndersonDarling = VerifyUniformLaw(researchMassive, StatisticalCriterion.AndersonDarling)[2];
                    result += "\nКритерий Андерсона-Дарлинга - " + valueAndersonDarling + " ; \n";
                    //Проверка критерия Андерсона-Дарлинга по уровню значимости и объему выборки
                    foreach (var A in Stat_criterion.listLimitAndersonDarling.Select(alfa => alfa.A).ToList())
                    {
                        Validity validityAndersonDarling = VerifyCriterionAndersonDarling(valueAndersonDarling, researchMassive.Count(), A);
                        result += validityAndersonDarling == Validity.yes ?
                        "При уровне значимости " + A + " гипотеза равномерности не отклоняется; \n" :
                        validityAndersonDarling != Validity.excluded ?
                        "При уровне значимости " + A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Андерсона-Дарлинга!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по Критерию Андерсона-Дарлинга \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 3 - Крамера-Мизеса-Смирнова
                try
                {
                    float valueKramerMisesSmirnov = VerifyUniformLaw(researchMassive, StatisticalCriterion.KramerMisesSmirnov)[3];
                    result += "\nКритерий Крамера-Мизеса-Смирнова - " + valueKramerMisesSmirnov + " ; \n";
                    //Проверка критерия Крамера-Мизеса-Смирнова по уровню значимости и объему выборки
                    foreach (var A in Stat_criterion.listLimitKramerMisesSmirnov.Select(alfa => alfa.A).ToList())
                    {
                        Validity validityKramerMisesSmirnov = VerifyCriterionKramerMisesSmirnov(valueKramerMisesSmirnov, researchMassive.Count(), A);
                        result += validityKramerMisesSmirnov == Validity.yes ?
                        "При уровне значимости " + A + " гипотеза равномерности не отклоняется; \n" :
                        validityKramerMisesSmirnov != Validity.excluded ?
                        "При уровне значимости " + A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Крамера-Мизеса-Смирнова!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по Критерию Крамера-Мизеса-Смирнова \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 4-7 Ньюмана-Бартона
                try
                {
                    float[] valueNeymanBarton = VerifyUniformLaw(researchMassive, StatisticalCriterion.NeymanBarton1polinom);
                    valueNeymanBarton[5] = VerifyUniformLaw(researchMassive, StatisticalCriterion.NeymanBarton2polinom)[5];
                    valueNeymanBarton[6] = VerifyUniformLaw(researchMassive, StatisticalCriterion.NeymanBarton3polinom)[6];
                    valueNeymanBarton[7] = VerifyUniformLaw(researchMassive, StatisticalCriterion.NeymanBarton4polinom)[7];
                    result += "\nКритерий Ньюмана-Бартона (1 полином) - " + valueNeymanBarton[4] + " ; \n";
                    result += "Критерий Ньюмана-Бартона (2 полином) - " + valueNeymanBarton[5] + " ; \n";
                    result += "Критерий Ньюмана-Бартона (3 полином) - " + valueNeymanBarton[6] + " ; \n";
                    result += "Критерий Ньюмана-Бартона (4 полином) - " + valueNeymanBarton[7] + " ; \n";
                    //Проверка критерия Ньюмана-Бартона по уровню значимости и объему выборки
                    foreach (var item in Stat_criterion.listLimitNeymanBarton2.GroupBy(Gr => Gr.A).Select(alfa => alfa.First()).ToList())
                    {
                        Validity validityNeymanBarton = VerifyCriterionNeymanBarton(valueNeymanBarton, researchMassive.Count(), item.A);
                        result += validityNeymanBarton == Validity.yes ?
                        "При уровне значимости " + item.A + " гипотеза равномерности не отклоняется; \n" :
                        validityNeymanBarton != Validity.excluded ?
                        "При уровне значимости " + item.A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Ньюмана-Бартона!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по критерию Ньюмана-Бартона \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 8 Дудевича
                try
                {
                    float valueDudevichVanDerMuelen = VerifyUniformLaw(researchMassive, StatisticalCriterion.DudevichVanDerMuelen)[8];
                    result += "\nКритерий Дудевича - " + valueDudevichVanDerMuelen + " ; \n";
                    //Проверка критерия Дудевича по уровню значимости и объему выборки
                    foreach (var item in Stat_criterion.listLimitDudevichVanDerMuelen.GroupBy(Gr => Gr.A).Select(alfa => alfa.First()).ToList())
                    {
                        Validity validityDudevichVanDerMuelen = VerifyCriterionDudevichVanDerMuelen(valueDudevichVanDerMuelen, researchMassive.Count(), item.A);
                        result += validityDudevichVanDerMuelen == Validity.yes ?
                        "При уровне значимости " + item.A + " гипотеза равномерности не отклоняется; \n" :
                        validityDudevichVanDerMuelen != Validity.excluded ?
                        "При уровне значимости " + item.A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Дудевича!\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по критерию Дудевича \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
                //Критерии: 9 Пирсона
                try
                {
                    float valuePearsonsChiSquare = VerifyUniformLaw(researchMassive, StatisticalCriterion.PearsonsChiSquare)[9];
                    result += "\nКритерий Пирсона - " + valuePearsonsChiSquare + " ; \n";
                    //Проверка критерия Пирсона по уровню значимости и объему выборки
                    foreach (var item in Stat_criterion.listLimitPirson.GroupBy(Gr => Gr.A).Select(alfa => alfa.First()).ToList())
                    {
                        Validity validityPearsonsChiSquare = VerifyCriterionPirson(valuePearsonsChiSquare, researchMassive.Count(), item.A);
                        result += validityPearsonsChiSquare == Validity.yes ?
                        "При уровне значимости " + item.A + " гипотеза равномерности не отклоняется; \n" :
                        validityPearsonsChiSquare != Validity.excluded ?
                        "При уровне значимости " + item.A + " не подтверждена гипотеза; \n" :
                        "Исключение при проверке по критерию Пирсона!\n\n";
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете по критерию Пирсона \n\n" + ex.Message,
                                    "Проверка гипотезы о принадлежности выборки равномерному закону");
                }
            }
            //
            return result;
        }
    }

    /// <summary>
    /// Класс статистических характеристик
    /// </summary>
    public class Statistics
    {
        /// <summary>
        /// Минимальное значение
        /// </summary>
        public float Min { get; set; }
        /// <summary>
        /// Максимальное значение
        /// </summary>
        public float Max { get; set; }
        /// <summary>
        /// Размах значений
        /// </summary>
        public float Delta { get; set; }
        /// <summary>
        /// Дисперсия
        /// </summary>
        public float D { get; set; }
        /// <summary>
        /// Среднеквадратическое отклонение
        /// </summary>
        public float Sigma { get; set; }
        /// <summary>
        /// Среднеарифметическое значение
        /// </summary>
        public float MeanAr { get; set; }
        /// <summary>
        /// Коэффициент асимметрии
        /// </summary>
        public float Asymmetry { get; set; }
        /// <summary>
        /// Коэффициент эксцесса
        /// </summary>
        public float excess { get; set; }
        /// <summary>
        /// Коэффициент вариации
        /// </summary>
        public float KoefVariations { get; set; }
        /// <summary>
        /// Меана
        /// </summary>
        public float Mean { get; set; }
        /// <summary>
        /// Мода
        /// </summary>
        public float Moda { get; set; }
        /// <summary>
        /// Медиана
        /// </summary>
        public float Median { get; set; }
        /// <summary>
        /// Объем выборки
        /// </summary>
        public float SampleSize { get; set; }
    }
}
