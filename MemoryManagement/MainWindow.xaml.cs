using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MemoryManagement.Data;

namespace MemoryManagement
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private readonly ObservableCollection<Hole> holes;
        private readonly ObservableCollection<Process> processes;
        
        private readonly ObservableCollection<Hole> assignedHoles;

        public MainWindow()
        {
            InitializeComponent();
            holes = new ObservableCollection<Hole>();
            processes = new ObservableCollection<Process>();
            assignedHoles = new ObservableCollection<Hole>();
            HoleDGrid.ItemsSource = holes;
            
            ProcessesDGrid.ItemsSource = processes;
            AllocatedHolesDGrid.ItemsSource = assignedHoles;
        }

        #region Algorithms Related functions
        private void AllocateSpacesAmongHoles()
        {
            Comparison<Hole> hC = (hole, hole1) => hole.BaseReg.CompareTo(hole1.BaseReg);
            var sortedHoles = holes.ToList();
            sortedHoles.Sort(hC);
            //Check if the first hole of memory is allocated
            if (sortedHoles[0].BaseReg != 0)
            {
                uint basereg = 0;
                uint size = sortedHoles[0].BaseReg ;
                assignedHoles.Add(new Hole(basereg, size));

            }
            for (int i = 0; i < sortedHoles.Count() - 1; i++)
            {
                uint basereg = sortedHoles[i].BaseReg + sortedHoles[i].Size + 1;
                uint size = sortedHoles[i + 1].BaseReg - sortedHoles[i].BaseReg - sortedHoles[i].Size;
                assignedHoles.Add(new Hole(basereg, size));
            }

        }
        private void AllocatedHolesDGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {

            if (Key.Delete == e.Key)
            {
                var selectedHole = (Hole)AllocatedHolesDGrid.SelectedItem;
                assignedHoles.Remove(selectedHole);

                AddHole(selectedHole);
                RunAlgorthim();
            }
        }
        private void RunAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            AlgorthimCb.IsEnabled = false;
            ResetButton.IsEnabled = true;
            CreateNewHoleBtn.IsEnabled = false;
            CreateProcessBtn.IsEnabled = false;
            ProcessMemorySize.IsEnabled = false;
            HoleBaseReg.IsEnabled = false;
            HoleSize.IsEnabled = false;
            AllocateSpacesAmongHoles();
            RunAlgorthim();
        }

        private void RunAlgorthim()
        {
            processes.Reverse();
            bool allocated ;
            do {
                allocated = false;
                for (int i = processes.Count - 1; i >= 0; i--)
                {
                    Hole h = null;
                    switch (AlgorthimCb.SelectedIndex)
                    {
                        case 0:


                            h = FirstFitAlgorithm(processes[i]);

                            break;
                        case 1:
                            h = WorstFitAlgorithm(processes[i]);

                            break;
                        case 2:
                            h = BestFitAlgorithm(processes[i]);

                            break;
                    }
                    if (h != null) { 
                        AssignHoleToProcess(h, processes[i]);
                        allocated = true;
                    }
                }
            }
            while (allocated);
            processes.Reverse();
        }

        private void ResetAlgorithm_Click(object sender, RoutedEventArgs e)
        {
            //Enable Buttons
            StartButton.IsEnabled = true;
            AlgorthimCb.IsEnabled = true;
            ResetButton.IsEnabled = false;
            CreateNewHoleBtn.IsEnabled = true;
            CreateProcessBtn.IsEnabled = true;
            ProcessMemorySize.IsEnabled = true;
            HoleBaseReg.IsEnabled = true;
            HoleSize.IsEnabled = true;
            //Clear Processes, Holes and Assigned
            holes.Clear();
            processes.Clear();
            assignedHoles.Clear();
            //Refresh Layout
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

            //Add the hole to the assignedHoles
            assignedHoles.Add(h);
        }


        private Hole BestFitAlgorithm(Process p)
        {
            int min=-1;
            for (int i=0; i < holes.Count; i++) {

                if (holes[i].Size >= p.Size && min == -1)
                    min = i;
                else if (holes[i].Size >= p.Size && holes[i].Size <holes[min].Size)
                    min = i; 
                 }
            if(min != -1)
                return holes[min];
            return null;
            
        }


        private Hole WorstFitAlgorithm(Process p)
        {
            int max = 0;

            for (int i = 1; i < holes.Count; i++)
            {
                if (holes[i].Size > holes[max].Size)
                    max = i;
            }
            if (holes[max].Size >= p.Size)
                return holes[max];


            return null;
        }


        private Hole FirstFitAlgorithm(Process p)
        {
            //We must first sort the holes according to their base register value before we do linear search
            var sortedHoles = holes.OrderBy(x => x.BaseReg);
            for (int i = 0; i < sortedHoles.Count(); i++)
            {
                if (sortedHoles.ElementAt(i).Size >= p.Size)
                {
                    return sortedHoles.ElementAt(i);
                }
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
                AddHole(new Hole(baseReg, size));
            }
            catch (Exception)
            {
                MessageBox.Show(this, "You must enter valid values for the base register and the size of a new hole",
                    "New Hole Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddHole(Hole newHole)
        {
            Hole lower = null;
            Hole upper = null;
            foreach (var hole in holes)
            {
                if (hole.BaseReg + hole.Size == newHole.BaseReg)
                {
                    lower = hole;
                }
                else if (newHole.BaseReg + newHole.Size == hole.BaseReg)
                {
                    upper = hole;
                }
            }
            if (upper != null)
            {
                holes.Remove(upper);
                newHole.Size += upper.Size;
            }
            if (lower != null)
            {

                holes.Remove(lower);
                lower.Size += newHole.Size;
                newHole = lower;
            }

            holes.Add(newHole);

      
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
   

            }
            catch (Exception)
            {
                MessageBox.Show(this, "The process can't have memory size of zero unit", "New Process Creation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

        #region GUI related functions

       

        private void NumericInputOnly_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
                e.Handled = true;
        }

        #endregion




    }
}