using System;
using System.Collections.Generic;
using System.Text;

namespace ImageAnalysis
{
    /// <summary>
    /// Класс описания пикселя
    /// </summary>
    public class Pixels
    {
        /// <summary>
        /// Координата по оси X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Координата по оси Y
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Компонента цвета R
        /// </summary>
        public byte R { get; set; }
        /// <summary>
        /// Компонента цвета G
        /// </summary>
        public byte G { get; set; }
        /// <summary>
        /// Компонента цвета B
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// Компонента цвета R
        /// </summary>
        public float H { get; set; }
        /// <summary>
        /// Компонента цвета G
        /// </summary>
        public float S { get; set; }
        /// <summary>
        /// Компонента цвета B
        /// </summary>
        public float V { get; set; }
    }
}
