using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;

namespace ImageAnalysis
{
    /// <summary>
    /// Класс результатов анализа фрактальной размерности
    /// </summary>
    public class Base_fract_anal
    {
        /// <summary>
        /// listE.Count - Количество пикселей изображения
        /// </summary>
        public int CountElements { get; set; }

        /// <summary>
        /// Первоначальный размер клетки
        /// </summary>
        public float Mstart { get; set; }

        /// <summary>
        /// Список фрактальных размерностей для клеточного метода
        /// </summary>
        public List<float> FractalDimensionSquare { get; set; }

        /// <summary>
        /// Список мер для клеточного метода
        /// </summary>
        public List<float> SizeSquare { get; set; }

        /// <summary>
        /// Список размеров (количества клеток содержащих пиксели)
        /// </summary>
        public List<float> Length { get; set; }

        /// <summary>
        /// Список количества клеток покрывающих контур
        /// </summary>
        public List<int> CountSquare { get; set; }

        /// <summary>
        /// Список координат клеток
        /// </summary>
        public List<FractalMeraS> PointS { get; set; }

        /// <summary>
        /// Средняя величина фрактальной размерности
        /// </summary>
        /// <returns></returns>
        public float Mean(List<float> FDimension)
        {
            if (FDimension == null || FDimension.Count == 0)
            { return float.NaN; }

            return FDimension.Average();
        }

        /// <summary>
        /// Фрактальная размерность для всей области скейлинга
        /// </summary>
        /// <returns></returns>
        public float FractalSizeGeneral()
        {
            if (SizeSquare == null || SizeSquare.Count == 0 || Length == null || Length.Count == 0)
            {
                return float.NaN;
            }
                return (float)((Math.Log(Length[0]) - Math.Log(Length[Length.Count - 1])) /
                               (Math.Log(SizeSquare[SizeSquare.Count - 1] / SizeSquare[0])));
        }

        string text;

        /// <summary>
        /// Вывод данных по анализу фрактальной размерности
        /// </summary>
        /// <returns></returns>
        public string ToString(List<float> FDimension)
        {
            if (FDimension == null || FDimension.Count == 0)
            { return "Нет данных!"; }
            text = "Результаты анализа фрактальной размерности:" + "\n";
            text += "Количество выделенных пикселей: " + CountElements + ";\n";
            text += "Первоначальная величина меры: " + Mstart + " пикселей;\n";
            text += "Список фрактальных размерностей: " + "\n";
            for (int i = 0; i < FDimension.Count; i++) { text += "\t" + FDimension[i] + "\n"; }
            text += "Среднеарифметическая величина фрактальной размерности: " + Mean(FDimension) + " ;\n";
            text += "Минимальная величина фрактальной размерности: " + FDimension.Min() + " ;\n";
            text += "Максимальная величина фрактальной размерности: " + FDimension.Max() + " ;\n";
            text += "Список мер: " + "\n";
            for (int i = 0; i < SizeSquare.Count; i++) { text += "\t" + SizeSquare[i] + "\n"; }
            text += "Список количества клеток покрывающих выделенные пиксели: " + "\n";
            for (int i = 0; i < CountSquare.Count; i++){ text += "\t" + CountSquare[i] + "\n"; }
            return text;
        }
    }
    /// <summary>
    /// Класс задания клетки для визуализации
    /// </summary>
    public class FractalMeraS
    {
        /// <summary>
        /// Размер клетки
        /// </summary>
        public float R;

        /// <summary>
        /// Координата точки положения квадрата
        /// </summary>
        public PointF[] S;

        /// <summary>
        /// Порядковый номер итерации
        /// </summary>
        public int nomIteration;

        /// <summary>
        /// Порядковый номер меры
        /// </summary>
        public int nomMeasure;
    }
}
