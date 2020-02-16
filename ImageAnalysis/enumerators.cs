﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ImageAnalysis
{
    /// <summary>
    /// Список методов определения фрактальной размерности {0 - клеточный, 1 - масштабов }
    /// </summary>
    public enum FractalMethod
    {
        cell,
        scale
    };

    /// <summary>
    /// Усечение гистограммы плотности распределения
    /// </summary>
    public enum TrimHistogram
    {
        no,
        leftTrim,
        rightTrim,
        allTrim
    };

    /// <summary>
    /// Справедлива или не справедлива нулевая гипотеза
    /// </summary>
    public enum Validity
    {
        no,
        yes,
        excluded
    };

    /// <summary>
    /// Статистические критерии проверки нулевой гипотезы о принадлежности выборки равномерному закону
    /// </summary>
    public enum StatisticalCriterion
    {
        Wilcoxon,
        Chesnokov,
        KruskallWallis,
        MannWhitney,
        WaldWolfowitz,
        Sherman,
        Kimball,
        Moran1,
        Moran2,
        ChengSpiring,
        HegaziGreen,
        Youngs,
        Frosini,
        Greenwoods,
        GreenwoodQuesenberryMiller,
        NeymanBarton1polinom,
        NeymanBarton2polinom,
        NeymanBarton3polinom,
        NeymanBarton4polinom,
        DudevichVanDerMuelen,
        Etropy2,
        Cressi1,
        Cressi2,
        Pardo,
        Schwartzs,
        SarkadiKosika,
        Kolmogorov,
        Coopers,
        KramerMisesSmirnov,
        Watsons,
        AndersonDarling,
        Zhang,
        PearsonsChiSquare
    };

    /// <summary>
    /// Сортировака данных
    /// </summary>
    public enum Sort
    {
        no,
        descending,
        ascending
    };

    /// <summary>
    /// Структура для параллельного расчета количество клеток при определении фрактальной размерности
    /// </summary>
    public struct DataCalculate
    {
        /// <summary>
        /// Список первоначальных точек клеток
        /// </summary>
        public List<PointF> listPoint;
        /// <summary>
        /// Номер диапазона
        /// </summary>
        public int iRange;
        /// <summary>
        /// Список пикселей
        /// </summary>
        public List<Pixels> listPixels;
        /// <summary>
        /// Текущая мера измерения
        /// </summary>
        public int dmera;
    }
}
