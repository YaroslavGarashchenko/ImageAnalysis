using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageAnalysis
{
    /// <summary>
    /// Класс для массива стат. параметров анализа цветов
    /// </summary>
    public class ColorParam
    {
        /// <summary>
        /// R компонента цвета RGB
        /// </summary>
        public Statistics R { get; set; }
        /// <summary>
        /// G компонента цвета RGB
        /// </summary>
        public Statistics G { get; set; }
        /// <summary>
        /// B компонента цвета RGB
        /// </summary>
        public Statistics B { get; set; }
        /// <summary>
        /// H компонента цвета HSV
        /// </summary>
        public Statistics H { get; set; }
        /// <summary>
        /// S компонента цвета HSV
        /// </summary>
        public Statistics S { get; set; }
        /// <summary>
        /// V компонента цвета HSV
        /// </summary>
        public Statistics V { get; set; }
        /// <summary>
        /// Горизонтальное распределение
        /// </summary>
        public Statistics Gor { get; set; }
        /// <summary>
        /// Вертикальное распределение
        /// </summary>
        public Statistics Vert { get; set; }
        /// <summary>
        /// Проверка наличия данных
        /// </summary>
        /// <returns></returns>
        public bool VerifyData()
        {
            return R != null && G != null && B != null && H != null && S != null && V != null;
        }
    }
}
