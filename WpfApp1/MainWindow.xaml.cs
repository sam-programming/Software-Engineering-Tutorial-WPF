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
using System.Windows.Threading;
using WpfApp1;

/* CSI2312 - Foundations of Software Engineering
 * An emergent behaviour system based on some of the rules
 * of flocking.
 * Copyright 2021 - Edith Cowan University
 * Author: Martin Masek
 * 
 * This is a data-centric design - all the variables to do with the entities are declared
 * at the top (of class MainWindow) and the subsequent code works on them.
 * There is minimal use of functions.
*/

namespace FlockingDemoNonOO
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    /* Example of inheritance - the MainWindow class inherits from
     * the Window class
     */
    public partial class MainWindow : Window
    {

        Entity blueEntity = new Entity();
        Entity redEntity = new Entity();
        // Timer for the update routine
        DispatcherTimer dispatcherTimer;
        int updateFrequency = 10; // 10ms between updates


        // flocking variables
        int flockRadius = 100;
        int iconDiameter = 10;

        /* Constructor for the MainWindow.
         * Here all the entities are set up, given initial
         * position and velocity and their graphics registered
         * with the canvas element.
         * Lastly, the 'update' routine is set to run periodially
         * in parallel to update the entities
         */
        public MainWindow()
        {
            InitializeComponent();

            Color red = Colors.Red;
            Color blue = Colors.Blue;

            blueEntity.Initialise_Entities(blue, TheCanvas);
            redEntity.Initialise_Entities(red, TheCanvas);
            // start the update routine
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(update_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, updateFrequency);
            dispatcherTimer.Start();
        }

        
        private void update_Tick(object sender, EventArgs e)
        {
            updateEntities(blueEntity);
            updateEntities(redEntity);
        }

        private void updateEntities(Entity entity)
        {
            // update position of the entities 
            for (int i = 0; i < entity.getNumberOfEntities(); i++)
            {
                // calculate flock centre of mass and average heading
                double centreOfMassX = 0;
                double centreOfMassY = 0;
                double flockDirectionX = 0;
                double flockDirectionY = 0;
                int numberInFlock = 0;
                // for each 'other' entity check interactions
                for (int j = 0; j < entity.getNumberOfEntities(); j++)
                {
                    //
                    if (Math.Sqrt(Math.Pow((entity.xPosition[i] - entity.xPosition[j]), 2) + Math.Pow((entity.yPosition[i] - entity.yPosition[j]), 2)) < flockRadius)
                    {
                        numberInFlock++;
                        centreOfMassX += entity.xPosition[j];
                        centreOfMassY += entity.yPosition[j];
                        flockDirectionX += entity.xSpeed[j];
                        flockDirectionY += entity.ySpeed[j];
                    }
                }
                centreOfMassX /= numberInFlock;
                centreOfMassY /= numberInFlock;
                flockDirectionX /= numberInFlock;
                flockDirectionY /= numberInFlock;

                // calculate forces acting on entity
                double cohesionForceX = (centreOfMassX - entity.xPosition[i]) / flockRadius;
                double cohesionForceY = (centreOfMassY - entity.yPosition[i]) / flockRadius;

                // calculate total force
                double forceX = MassSlider.Value * entity.xSpeed[i] + CohesionSlider.Value * cohesionForceX + HeadingSlider.Value * flockDirectionX;
                double forceY = MassSlider.Value * entity.ySpeed[i] + CohesionSlider.Value * cohesionForceY + HeadingSlider.Value * flockDirectionY;

                // calculate acceleration due to force
                double accelerationX = forceX / MassSlider.Value;
                double accelerationY = forceY / MassSlider.Value;

                // calculate new velocity
                entity.xSpeed[i] += accelerationX;
                entity.ySpeed[i] += accelerationY;

                // normalise velocity (divide by its magnitude) - note: when this is enabled, each entity travels at the same speed, so gravity and friction will not work as intended.
                double oldXSpeed = entity.xSpeed[i];
                entity.xSpeed[i] = entity.xSpeed[i] / (Math.Sqrt(Math.Pow(entity.xSpeed[i], 2) + Math.Pow(entity.ySpeed[i], 2)));
                entity.ySpeed[i] = entity.ySpeed[i] / (Math.Sqrt(Math.Pow(oldXSpeed, 2) + Math.Pow(entity.ySpeed[i], 2)));

                // calculate new position
                entity.xPosition[i] += entity.xSpeed[i];
                entity.yPosition[i] += entity.ySpeed[i];

                // accelarate due to gravity
                if (GravityOn.IsChecked == true)
                {
                    entity.ySpeed[i] += 0.1 * MassSlider.Value; // bit of a hack - increase the speed in the down direction.
                }

                // check for boundary collisions - since boundaries are alligned with axes, we just flip the speed on the appropriate axis.
                if (entity.xPosition[i] < 0 || entity.xPosition[i] > (TheCanvas.Width - iconDiameter))
                {
                    if (FrictionOn.IsChecked == true)
                    {
                        entity.xSpeed[i] *= -0.8; // speed lost during collision (along x-axis)
                    }
                    else
                    {
                        entity.xSpeed[i] *= -1; // 100% elastic collision (along y-axis)
                    }
                    if (entity.xPosition[i] < 0) entity.xPosition[i] = 0;
                    if (entity.xPosition[i] > (TheCanvas.Width - iconDiameter))
                    { entity.xPosition[i] = (TheCanvas.Width - iconDiameter); }
                }
                if (entity.yPosition[i] < 0 || entity.yPosition[i] > (TheCanvas.Height - iconDiameter))
                {
                    if (FrictionOn.IsChecked == true)
                    {
                        entity.ySpeed[i] *= -0.8; // same for y as for x
                    }
                    else
                    {
                        entity.ySpeed[i] *= -1;
                    }
                    if (entity.yPosition[i] < 0) entity.yPosition[i] = 0;
                    if (entity.yPosition[i] > (TheCanvas.Height - iconDiameter)) entity.yPosition[i] = (TheCanvas.Height - iconDiameter);
                }
            }
            // update entity icons on canvas
            for (int i = 0; i < entity.getNumberOfEntities(); i++)
            {
                Canvas.SetLeft(entity.icon[i], entity.xPosition[i]);
                Canvas.SetTop(entity.icon[i], entity.yPosition[i]);
            }
        }

        // The code below updates the text displays whenever the slider
        // values change.
        private void MassSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MassLabel != null)
            {
                MassLabel.Content = MassSlider.Value.ToString("0.##");
            }
        }

        private void CohesionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (CohesionLabel != null)
            {
                CohesionLabel.Content = CohesionSlider.Value.ToString("0.##");
            }
        }

        private void HeadingSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (HeadingLabel != null)
            {
                HeadingLabel.Content = HeadingSlider.Value.ToString("0.##");
            }
        }

    }
}
