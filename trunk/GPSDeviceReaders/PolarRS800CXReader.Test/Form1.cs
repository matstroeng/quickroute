using System;
using System.Collections.Generic;
using System.Windows.Forms;
using QuickRoute.GPSDeviceReaders.Polar;

namespace PolarRS800CXReaderTest
{
  public partial class Form1 : Form
  {
    public Form1()
    {
      InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
      //List<DateTime> allExercises = PolarRS800CXReader.GetAllExercises();

      var exercise = new PolarRS800CXReader.PolarExercise(); 
      bool b = PolarRS800CXReader.GetExercise(0, out exercise);

    }
  }
}
