﻿using RetailPoC20.Models;
using RetailPoC20.ViewModels;
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
using System.Windows.Shapes;
using MahApps.Metro.Controls;

namespace RetailPoC20
{
    public delegate void GeneratingData(double percent, string text, bool isDone, Exception ex = null);

    /// <summary>
    /// Interaction logic for DataPanel.xaml
    /// </summary>
    public partial class DataPanel
    {
        public event GeneratingData GenerateDataEvent;
        private DataFactory dataFactory = new DataFactory(); // This is where items are queried from the database
        public List<ItemVM> Items { get; set; } // Storage for the items loaded on the datagrid
        public DataPanel()
        {
            InitializeComponent();
            DataProgressBar.Visibility = Visibility.Hidden;
        }

        private void textBox_KeyUp(object sender, KeyEventArgs e)
        {
            var searched = this.searchItemBox.Text.ToLower(); // The keyword for searching [SKU or Name]

            if (string.IsNullOrEmpty(searched)) // Check if keyword is empty
            {
                this.dataGrid.ItemsSource = this.Items; // Keyword is empty, so reset the datagrid to show all items
            }
            else
            {
                // Get the new datagrid items
                var newList = this.Items.Where(a => a.SKU.ToLower() == searched || a.Name.ToLower() == searched).ToList();

                this.dataGrid.ItemsSource = newList; // Assign the new items found to the datagrid
            }
        }

        public async void GenerateDataBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateDataEvent?.Invoke(0, "Preparing data generation...", false);

            try
            {
                using (PlanogramContext context = new PlanogramContext())
                {
                    int dataCount = context.Items.Count();
                    if (dataCount == this.NumItems)
                    {
                        GenerateDataEvent?.Invoke(100, "", true);
                    }
                    else
                    {
                        context.Database.Delete();
                        context.Database.CreateIfNotExists();

                        MockData data = new MockData(context);
                        data.NumItems = this.NumItems;

                        //generateDataBtn.IsEnabled = false;
                        //DataProgressBar.Visibility = Visibility.Visible;

                        data.GeneratingDataEvent += (p, t, d, ex) =>
                        {
                            GenerateDataEvent?.Invoke(p, t, d, ex);
                        };

                        //data.Refreshdata += refreshDataGrid;
                        await data.Generate();
                    }
                }
            }
            catch (Exception ex)
            {
                GenerateDataEvent?.Invoke(0, "An error occured. Please click here to view the full details.", true, ex);
            }
        }
        
        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            // Re-query from the database the list of items and store it to local variables
            refreshDataGrid(true);
        }
        

        void refreshDataGrid(bool refresh)
        {
            if (refresh)
            {
                this.dataGrid.ItemsSource = dataFactory.Items;
                this.Items = dataFactory.Items;
                generateDataBtn.IsEnabled = true;
                DataProgressBar.Value = 0;
                DataProgressBar.Visibility = Visibility.Hidden;
                //GenerateDataEvent?.Invoke("Done.");
            }
        }

        public int NumItems { get; set; }
        public int NumShelves { get; set; }
        public int NumSlots { get; set; }

        private void dataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //DataGridRow row = (DataGridRow)(sender as DataGrid).SelectedItem; // Get the selected row
            //ItemVM itemvm = row.Item as ItemVM; // Convert the selected row to the object bound to

            if (dataGrid.SelectedItem == null) return;

            ItemVM itemvm = (ItemVM)dataGrid.SelectedItem;
            var id = itemvm.ID; // Get the item id

            Item item;
            using (PlanogramContext ctx = new PlanogramContext())
            {
                item = ctx.Items.Include("Attributes").FirstOrDefault(a => a.ID == id); // Get the selected item with attributes
            }
            var attrs = item.Attributes; // Get the item attributes

            ItemAttributesPanel panel = new ItemAttributesPanel(); // Instantiate the dialog to show the attributes

            panel.itemAttributesGrid.AutoGenerateColumns = false;
            panel.itemAttributesGrid.ItemsSource = attrs; // Set the data source for the attributes grid

            // Binding and setting of columns for the datagrid
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "ID", Binding = new Binding("ID"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric1", Binding = new Binding("Metric1"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric2", Binding = new Binding("Metric2"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric3", Binding = new Binding("Metric3"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric4", Binding = new Binding("Metric4"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric5", Binding = new Binding("Metric5"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric6", Binding = new Binding("Metric6"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric7", Binding = new Binding("Metric7"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric8", Binding = new Binding("Metric8"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric9", Binding = new Binding("Metric9"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });
            panel.itemAttributesGrid.Columns.Add(new DataGridTextColumn() { Header = "Metric10", Binding = new Binding("Metric10"), Width = new DataGridLength(1, DataGridLengthUnitType.Star), IsReadOnly = true });

            panel.Width = 1500; // Set dialog width
            panel.ShowDialog();

            // Some operations with this row
        }

        private void btnDownloadRetailData_Click(object sender, RoutedEventArgs e)
        {
            using (PlanogramContext context = new PlanogramContext())
            {
                MockData data = new MockData(context);
                data.DownloadRetailData();
            }
        }
    }
}
