﻿using CodeGenergator.Model;
using System;
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
using UniNote.Web.Model;
using UniOrm;
using System.Reflection;
namespace CodeGenergator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
   
        public MainWindow( )
        {
            DataContext = new MainWindowsModel();
        }

      

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".dll";
            dlg.Filter = "dlls (.dll)|*.dll";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                FileNameTextBox.Text = filename;
            }
        }

        private void BtnGenerFiles_Click(object sender, RoutedEventArgs e)
        {
            var dllname = FileNameTextBox.Text.Trim();
            var ass = Assembly.LoadFile(dllname);
            var alltypes = ass.GetTypes();
            foreach(var t in alltypes)
            {
                var pis = t.GetProperties();
                foreach(var pi  in pis)
                {
                   // pi.Name
                }
            }
        }


    }
}
