using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfApp1
{
    public class Entity
    {
        // data for a number of entities
        public static int numberOfEntities = 25;
        // each entity has an ID, position (along x and y axis)
        // and speed (along x and y axis)
        public int[] ID = new int[numberOfEntities];
        public double[] xPosition = new double[numberOfEntities];
        public double[] yPosition = new double[numberOfEntities];
        public double[] xSpeed = new double[numberOfEntities];
        public double[] ySpeed = new double[numberOfEntities];

        // graphics for each entity
        public Ellipse[] icon = new Ellipse[numberOfEntities];
        int iconDiameter = 10;
        //random number generator
        Random generator;

        public int getNumberOfEntities()
        {
            return numberOfEntities;
        }

        public void Initialise_Entities(Color color, Canvas TheCanvas)
        {
            generator = new Random();

            // initialise entities        
            for (int i = 0; i < numberOfEntities; i++)
            {
                ID[i] = i;
                xPosition[i] = generator.NextDouble() * TheCanvas.Width;
                yPosition[i] = generator.NextDouble() * TheCanvas.Height;
                xSpeed[i] = (generator.NextDouble() - 0.5) * TheCanvas.Width / 100;
                ySpeed[i] = (generator.NextDouble() - 0.5) * TheCanvas.Height / 100;
            }

            // Create a set of circles, one for each entity, add them to the canvas.

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = color;

            for (int i = 0; i < numberOfEntities; i++)
            {
                icon[i] = new Ellipse();
                icon[i].Fill = mySolidColorBrush;
                icon[i].StrokeThickness = 2;
                icon[i].Stroke = mySolidColorBrush;

                icon[i].Width = iconDiameter;
                icon[i].Height = iconDiameter;

                TheCanvas.Children.Add(icon[i]);
                Canvas.SetLeft(icon[i], xPosition[i]);
                Canvas.SetTop(icon[i], yPosition[i]);
            }
        }

    }
}
