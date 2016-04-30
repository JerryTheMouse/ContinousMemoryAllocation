using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using MemoryManagement.Data;

namespace MemoryManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    //todo : add DataGrid for AssignedHoles 
    //todo : add a way to deallocate a hole from assigned holes datagrid
    //todo : When an assigned hole is deallocated, recombine it with other available holes if possible

    public partial class MainWindow : Window
    {
        private readonly List<Hole> holes;
        private readonly List<Process> processes;
        private readonly Dictionary<Hole,Process> assignedHoles;

        public MainWindow()
        {
            InitializeComponent();
            holes = new List<Hole>();
            processes = new List<Process>();
            assignedHoles = new Dictionary<Hole, Process>();
            HoleDGrid.ItemsSource = holes;
            ProcessesDGrid.ItemsSource = processes;
        }

        #region Algorithms Related functions

        private void RunAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            AlgorthimCb.IsEnabled = false;
            ResetButton.IsEnabled = true;
            processes.Reverse();

            switch (AlgorthimCb.SelectedIndex)
            {
                case 0:
                    for (int i = processes.Count - 1; i >= 0; i--)
                    {
                        Hole h = FirstFitAlgorithm(processes[i]);
                        if (h != null)
                            AssignHoleToProcess(h, processes[i]);
                    }


                    break;
                case 1:
                    break;
                case 2:
                    break;

            }
            processes.Reverse();
            RefreshLayout();
        }


        private void ResetAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            //Enable Buttons
            StartButton.IsEnabled = true;
            AlgorthimCb.IsEnabled = true;
            ResetButton.IsEnabled = false;
            //Clear Processes, Holes and Assigned
            holes.Clear();
            processes.Clear();
            assignedHoles.Clear();
            //Refresh Layout
            RefreshLayout();

        }
        private void AssignHoleToProcess(Hole h, Process p)
        {
            if (h.Size < p.Size)
            {
                throw new ArgumentException("Can't assign smaller hole than the memory required by the process");
            }
            if (h.Size > p.Size)
            {
                Hole newHole = new Hole(h.BaseReg + p.Size, h.Size - p.Size);
                holes.Add(newHole);
                h.Size = p.Size;
            }
            //Remove the hole from available holes
            holes.Remove(h);
            //Remove process from its queue
            processes.Remove(p);

            //Add hole and its corresping process
            assignedHoles.Add(h, p);

        }

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
            foreach (Hole h in holes)
            {
                if (h.Size >= p.Size)
                    return h;
            }
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
                if (size == 0)
                    throw new FormatException();


                //This new hole must not collide with another holes
                foreach (Hole hole in holes)
                {
                    uint min = hole.BaseReg;
                    uint max = hole.BaseReg + hole.Size - 1;

                    if ((baseReg >= min && baseReg <= max) || (baseReg + size - 1 >= min && baseReg + size - 1 <= max))
                    {
                        //Collision happened
                        MessageBox.Show(this, "This hole collides with other holes", "Universe Holes Problem",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                    if (baseReg <= min && baseReg + size - 1 >= max)
                    {
                        //Collision happened
                        MessageBox.Show(this, "This hole collides with other holes", "Universe Holes Problem",
                            MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }
                }
                holes.Add(new Hole(baseReg, size));
                HoleDGrid.Items.Refresh();
                HoleDGrid.UpdateLayout();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "You must enter valid values for the base register and the size of a new hole",
                    "New Hole Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
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


                processes.Add(new Process(size));
                ProcessesDGrid.Items.Refresh();
                ProcessesDGrid.UpdateLayout();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "The process can't have memory size of zero unit", "New Process Creation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region GUI related functions
        private void RefreshLayout()
        {
            HoleDGrid.Items.Refresh();
            ProcessesDGrid.Items.Refresh();
            HoleDGrid.UpdateLayout();
            ProcessesDGrid.UpdateLayout();
            //todo : refresh DataGrid for AssignedHoles when added

        }

        private void NumericInputOnly_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        #endregion


    }
}