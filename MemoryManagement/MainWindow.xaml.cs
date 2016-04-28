using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MemoryManagement.Data;

namespace MemoryManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Hole> holes;
        private readonly List<Process> processes;
        public MainWindow()
        {
            InitializeComponent();
             holes = new List<Hole>();
            processes = new List<Process>();
            HoleDGrid.ItemsSource = holes;
            ProcessesDGrid.ItemsSource = processes;
        }

        #region Algorithims
        //todo : Change the function to run the bestFit Algorthim on the set of holes to find and return the target         for process P

        private Hole BestFitAlgorithm(Process p)
        {
            return null;
        }

        //todo : Change the function to run the worstFit Algorthim on the set of holes to find and return the target         for process P

        private Hole WorstFitAlgorithm(Process p)
        {
            return null;
        }

        //todo : Change the function to run the firstFit Algorthim on the set of holes to find and return the target         for process P

        private Hole FirstFitAlgorithm(Process p)
        {
            return null;
        }
        #endregion
        #region creating new elements
        private void Create_New_Hole_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //This must not be 
                uint size = uint.Parse(HoleSize.Text);
                uint baseReg = uint.Parse(HoleBaseReg.Text);
                if(size == 0)
                    throw new FormatException();


                //This new hole must not collide with another holes
                foreach (Hole hole in holes)
                {
                    uint min = hole.BaseReg;
                    uint max = hole.BaseReg + hole.Size - 1;
                    
                    if ((baseReg >= min && baseReg <= max) || (baseReg + size - 1 >= min && baseReg + size - 1 <= max))
                    {
                        //Collision happened
                        MessageBox.Show(this, "This hole collides with other holes", "Universe Holes Problem", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                    if (baseReg <= min && baseReg + size - 1 >= max )
                    {
                        //Collision happened
                        MessageBox.Show(this, "This hole collides with other holes", "Universe Holes Problem", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }


                }
                holes.Add(new Hole(baseReg,size));
                HoleDGrid.Items.Refresh();
                HoleDGrid.UpdateLayout();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "You must enter valid values for the base register and the size of a new hole","New Hole Creation Error",MessageBoxButton.OK,MessageBoxImage.Error);

            }
            
            

        }


        private void Create_New_Process_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //This must not be 
                uint size = uint.Parse(ProcessMemorySize.Text);
                if (size == 0)
                    throw new FormatException();


                processes.Add(new Process( size));
                ProcessesDGrid.Items.Refresh();
                ProcessesDGrid.UpdateLayout();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "The process can't have memory size of zero unit", "New Process Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }
        #endregion

        #region Textbox validation functions
        private void NumericInputOnly_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        #endregion
    }
}
