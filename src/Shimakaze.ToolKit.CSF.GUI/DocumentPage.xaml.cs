﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace Shimakaze.ToolKit.CSF.GUI
{
    /// <summary>
    /// DocumentPage.xaml 的交互逻辑
    /// </summary>
    public partial class DocumentPage : Page
    {
        public DocumentPage()
        {
            InitializeComponent();
            StatusBlock.Show(StatusBlock.Control.TextBlock, "请拖放或打开一个文件以继续");
        }
        public string FilePath { get; set; } = string.Empty;
        public bool IsOpenedFile { get; set; } = false;



        private void OpenFile_Drop(object sender, DragEventArgs e) => OpenFile(((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString());
        public void OpenFile(string path)
        {
            var CsfClassFileBW = new Data.CsfBackgroundWorker<Kernel.CsfClassFile>();
            FilePath = path;
            var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            IsOpenedFile = false;

            void finished(Data.CsfBackgroundWorker<Kernel.CsfClassFile> sender, Kernel.CsfClassFile result)
            {
                CsfClassFileBW.Finished -= finished;
                CsfClassFileBW.PropertyChanged -= propChanged;
                StatusBlock.Button.Click -= btnAct;
                DataContext = new ViewModel.CsfDocument(result);
                IsOpenedFile = true;
                if (result != null) StatusBlock.Hide();
            };
            void btnAct(object o, RoutedEventArgs e)
            {
                CsfClassFileBW.Finished -= finished;
                CsfClassFileBW.PropertyChanged -= propChanged;
                StatusBlock.Button.Click -= btnAct;
                CsfClassFileBW.Cancel();
                StatusBlock.TextBlock.Text = "用户已取消";
                StatusBlock.HideProgressBar();
                StatusBlock.HideButton();
                fs.Close();
            }
            void propChanged(object o, System.ComponentModel.PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case nameof(CsfClassFileBW.StatusString):// 字符串变更
                        lock (StatusBlock.TextBlock)
                            StatusBlock.TextBlock.Text = CsfClassFileBW.StatusString;
                        break;
                    case nameof(CsfClassFileBW.UnknowProgress):// 进度未知
                        if (CsfClassFileBW.UnknowProgress)
                        {
                            StatusBlock.ProgressBar.IsIndeterminate = true;
                        }
                        else// 已知
                        {
                            StatusBlock.ProgressBar.IsIndeterminate = false;
                            StatusBlock.ShowProgressBar();
                            StatusBlock.ProgressBar.Maximum = CsfClassFileBW.MaxProgress;
                        }
                        break;
                    case nameof(CsfClassFileBW.Progress):// 进度变更
                        lock (StatusBlock.ProgressBar)
                            StatusBlock.ProgressBar.Value = CsfClassFileBW.Progress;
                        break;
                    case nameof(CsfClassFileBW.Error):// 发生异常
                        if (CsfClassFileBW.Error is Exception exception)
                        {
                            CsfClassFileBW.Finished -= finished;
                            CsfClassFileBW.PropertyChanged -= propChanged;
                            StatusBlock.Button.Click -= btnAct;
                            lock (StatusBlock.TextBlock)
                                StatusBlock.TextBlock.Text = exception.Message;
                            StatusBlock.HideProgressBar();
                            StatusBlock.HideButton();
                            fs.Close();
                        }
                        break;
                }
            }


            // 显示状态块
            StatusBlock.Show(StatusBlock.Control.ProgressBar | StatusBlock.Control.TextBlock);
            StatusBlock.ProgressBar.IsIndeterminate = true;
            StatusBlock.TextBlock.Text = "正在读取中, 请稍候...";
            StatusBlock.Button.Content = "取消";
            StatusBlock.ShowButton();
            StatusBlock.Button.Click += btnAct;
            // 属性变更通知
            CsfClassFileBW.PropertyChanged += propChanged;
            // 任务完成
            CsfClassFileBW.Finished += finished;

            // 加载文件
            CsfClassFileBW.BeginParse(fs);
        }

        public void SaveFile(string path)
        {
            var CsfClassFileBW = new Data.CsfBackgroundWorker<Kernel.CsfClassFile>();
            FilePath = path;
            var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read);

            void finished(Data.CsfBackgroundWorker<Kernel.CsfClassFile> sender, Kernel.CsfClassFile _)
            {
                CsfClassFileBW.Finished -= finished;
                CsfClassFileBW.PropertyChanged -= propChanged;
                StatusBlock.Button.Click -= btnAct;
                StatusBlock.Hide();
            }
            void btnAct(object o, RoutedEventArgs e)
            {
                CsfClassFileBW.Finished -= finished;
                CsfClassFileBW.PropertyChanged -= propChanged;
                StatusBlock.Button.Click -= btnAct;
                CsfClassFileBW.Cancel();
                StatusBlock.TextBlock.Text = "用户已取消";
                StatusBlock.HideProgressBar();
                StatusBlock.HideButton();
                fs.Close();
                if (File.Exists(path)) File.Delete(path);
            }
            void propChanged(object o, System.ComponentModel.PropertyChangedEventArgs e)
            {
                switch (e.PropertyName)
                {
                    case nameof(CsfClassFileBW.StatusString):// 字符串变更
                        lock (StatusBlock.TextBlock)
                            StatusBlock.TextBlock.Text = CsfClassFileBW.StatusString;
                        break;
                    case nameof(CsfClassFileBW.UnknowProgress):// 进度未知
                        if (CsfClassFileBW.UnknowProgress)
                        {
                            StatusBlock.ProgressBar.IsIndeterminate = true;
                        }
                        else// 已知
                        {
                            StatusBlock.ProgressBar.IsIndeterminate = false;
                            StatusBlock.ShowProgressBar();
                            StatusBlock.ProgressBar.Maximum = CsfClassFileBW.MaxProgress;
                        }
                        break;
                    case nameof(CsfClassFileBW.Progress):// 进度变更
                        lock (StatusBlock.ProgressBar)
                            StatusBlock.ProgressBar.Value = CsfClassFileBW.Progress;
                        break;
                    case nameof(CsfClassFileBW.Error):// 发生异常
                        if (CsfClassFileBW.Error is Exception exception)
                        {
                            lock (StatusBlock.TextBlock)
                                StatusBlock.TextBlock.Text = exception.Message;
                            StatusBlock.HideProgressBar();
                            StatusBlock.HideButton();
                            CsfClassFileBW.Finished -= finished;
                            CsfClassFileBW.PropertyChanged -= propChanged;
                            StatusBlock.Button.Click -= btnAct;
                            fs.Close();
                            if (File.Exists(path)) File.Delete(path);
                        }
                        break;
                }
            };

            // 显示状态块
            StatusBlock.Show(StatusBlock.Control.ProgressBar | StatusBlock.Control.TextBlock);
            StatusBlock.ProgressBar.IsIndeterminate = true;
            StatusBlock.TextBlock.Text = "正在写入中, 请稍候...";
            StatusBlock.Button.Content = "取消";
            StatusBlock.ShowButton();

            StatusBlock.Button.Click += btnAct;
            // 属性变更通知
            CsfClassFileBW.PropertyChanged += propChanged;
            // 任务完成
            CsfClassFileBW.Finished += finished;

            // 加载文件
            CsfClassFileBW.BeginDeparse(fs, (DataContext as ViewModel.CsfDocument)?.ClassList);
        }

    
}
}
