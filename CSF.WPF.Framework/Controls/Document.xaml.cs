﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CSF.WPF.Framework.Controls
{
    /// <summary>
    /// Document.xaml 的交互逻辑
    /// </summary>
    public partial class Document : UserControl
    {
        public Document()
        {
            InitializeComponent();
            DataContext = new ViewModel.DocumentVM();
        }
        public Document(ViewModel.DocumentVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}