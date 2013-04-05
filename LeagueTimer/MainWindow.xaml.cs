using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using System.Windows.Threading;
using LeagueTimer.GKH;
using System.Media;


namespace LeagueTimer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DispatcherTimer tInterval;
        private List<Button> buttonList = new List<Button>();
        private KeyboardListener gkh = new KeyboardListener();
        private bool shiftState = false;
        private KeyConverter conv = new KeyConverter();
        private bool settingMode = false;
        private int setWhichKey;
        private SoundPlayer snd;

        public MainWindow()
        {
            InitializeComponent();

            //make a 1 second timer
            this.tInterval = new DispatcherTimer();
            tInterval.Interval = new TimeSpan(0, 0, 1);
            tInterval.Tick += new EventHandler(ticker);
            tInterval.Start();
            
            //prepare global keyhook
            gkh.KeyUp += new RawKeyEventHandler(gkh_KeyUp);
            gkh.KeyDown += new RawKeyEventHandler(gkh_KeyDown);

            //read user defined settings
            readSettings();

            //prepare sound
            snd = new SoundPlayer(
                        LeagueTimer.App.GetResourceStream(
                            new Uri("pack://application:,,,/Resources/notify_pcm.wav")
                      ).Stream
                  );

        }

        void gkh_KeyUp(object sender, RawKeyEventArgs e)
        {

            //shift is off now
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.shiftState = false;
            }

            //check if settings mode is on
            if (settingMode) {

                if (setWhichKey == 1) Properties.Settings.Default.obKey = e.Key.ToString();
                if (setWhichKey == 2) Properties.Settings.Default.orKey = e.Key.ToString();
                if (setWhichKey == 3) Properties.Settings.Default.tbKey = e.Key.ToString();
                if (setWhichKey == 4) Properties.Settings.Default.trKey = e.Key.ToString();
                if (setWhichKey == 5) Properties.Settings.Default.dKey = e.Key.ToString();
                if (setWhichKey == 6) Properties.Settings.Default.bKey = e.Key.ToString();

                Properties.Settings.Default.Save();
                settingMode = false;
                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => readSettings())
                );
                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = true)
                );
                
            }

            //not setting any keys, normal operation
            else
            {
                if (shiftState)
                {
                    handleTimerReset(e.Key);
                }
                else
                {
                    handleTimerStart(e.Key);
                }
            }

        }

        
        void gkh_KeyDown(object sender, RawKeyEventArgs e)
        {

            //shift is off now
            if (e.Key == Key.LeftShift || e.Key == Key.RightShift)
            {
                this.shiftState = true;
            }

        }


        void handleTimerStart(Key k)
        {

            //own blue buff
            if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.obKey))
            {
                if (!buttonList.Contains(_5v5OwnBlueButton)) buttonList.Add(_5v5OwnBlueButton);
            }
                
            //own red buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.orKey))
            {
                if (!buttonList.Contains(_5v5OwnRedButton)) buttonList.Add(_5v5OwnRedButton);
            }

            //ennemy blue buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.tbKey))
            {
                if (!buttonList.Contains(_5v5EnemyBlueButton)) buttonList.Add(_5v5EnemyBlueButton);
            }

            //ennemy red buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.trKey))
            {
                if (!buttonList.Contains(_5v5EnemyRedButton)) buttonList.Add(_5v5EnemyRedButton);
            }

            //drake
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.dKey))
            {
                if (!buttonList.Contains(_5v5DrakeButton)) buttonList.Add(_5v5DrakeButton);
            }

            //baron nashor
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.bKey))
            {
                if (!buttonList.Contains(_5v5BaronButton)) buttonList.Add(_5v5BaronButton);
            }

        }

        void handleTimerReset(Key k)
        {
            
            //own blue buff
            if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.obKey))
            {
                if (buttonList.Contains(_5v5OwnBlueButton)) buttonList.Remove(_5v5OwnBlueButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5OwnBlueButton.Content = "5:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5OwnBlueButton.Tag = "")
                );
               
                
            }

            //own red buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.orKey))
            {
                if (buttonList.Contains(_5v5OwnRedButton)) buttonList.Remove(_5v5OwnRedButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5OwnRedButton.Content = "5:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5OwnRedButton.Tag = "")
                );
                 
            }

            //ennemy blue buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.tbKey))
            {
                if (buttonList.Contains(_5v5EnemyBlueButton)) buttonList.Remove(_5v5EnemyBlueButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5EnemyBlueButton.Content = "5:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5EnemyBlueButton.Tag = "")
                );

            }

            //ennemy red buff
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.trKey))
            {
                if (buttonList.Contains(_5v5EnemyRedButton)) buttonList.Remove(_5v5EnemyRedButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5EnemyRedButton.Content = "5:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5EnemyRedButton.Tag = "")
                );
            }

            //drake
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.dKey))
            {
                if (buttonList.Contains(_5v5DrakeButton)) buttonList.Remove(_5v5DrakeButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5DrakeButton.Content = "6:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5DrakeButton.Tag = "")
                );
            }

            //baron nashor
            else if (k == (Key)conv.ConvertFromString(Properties.Settings.Default.bKey))
            {
                if (buttonList.Contains(_5v5BaronButton)) buttonList.Remove(_5v5BaronButton);

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5BaronButton.Content = "7:00")
                );

                Application.Current.Dispatcher.BeginInvoke(
                      DispatcherPriority.Background,
                        new Action(() => _5v5BaronButton.Tag = "")
                );
            }

        }


        void ticker(object sender, EventArgs e)
        {

            foreach (Button btn in this.buttonList)
            {

                try
                {

                    string[] MinSec = btn.Content.ToString().Split(':');
                    int timer = (Convert.ToInt32(MinSec[0]) * 60) + Convert.ToInt32(MinSec[1]); //timer in sec
                    if (timer > 0) timer--;

                    //write new timer
                    string newLabel = Math.Floor(timer / 60f).ToString() + ":" + (timer % 60).ToString("0#");
                    btn.Content = newLabel;

                }
                catch (Exception)
                {
                    btn.Content = "Err";
                }

                //sound alarm
                try
                {

                    if (btn.Content.ToString() == "0:00" && (string)btn.Tag != "played" && Properties.Settings.Default.playSound)
                    {
                        snd.Play();
                        btn.Tag = "played";
                    }

                }
                catch (Exception)
                {
                    //nothing to do here
                }

            }//foreach(Label label in this.labelList)

        }//void ticker()

        void readSettings()
        {
            //read settings and set labels & tooltips
            _5v5OwnBlueButton.ToolTip = obLabel.Text = Properties.Settings.Default.obKey;
            _5v5OwnRedButton.ToolTip = orLabel.Text = Properties.Settings.Default.orKey;
            _5v5EnemyBlueButton.ToolTip = tbLabel.Text = Properties.Settings.Default.tbKey;
            _5v5EnemyRedButton.ToolTip = trLabel.Text = Properties.Settings.Default.trKey;
            _5v5DrakeButton.ToolTip = dLabel.Text = Properties.Settings.Default.dKey;
            _5v5BaronButton.ToolTip = bLabel.Text = Properties.Settings.Default.bKey;
            playSoundCheckbox.IsChecked = Properties.Settings.Default.playSound;
        }

        private void _5v5OwnBlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.obKey));
            }
            else {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.obKey));
            }
        }

        private void _5v5OwnRedButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.orKey));
            }
            else
            {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.orKey));
            }
        }

        private void _5v5EnemyBlueButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.tbKey));
            }
            else
            {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.tbKey));
            }
        }

        private void _5v5EnemyRedButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.trKey));
            }
            else
            {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.trKey));
            }
        }

        private void _5v5DrakeButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.dKey));
            }
            else
            {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.dKey));
            }
        }

        private void _5v5BaronButton_Click(object sender, RoutedEventArgs e)
        {
            if (shiftState)
            {
                handleTimerReset((Key)conv.ConvertFromString(Properties.Settings.Default.bKey));
            }
            else
            {
                handleTimerStart((Key)conv.ConvertFromString(Properties.Settings.Default.bKey));
            }
        }

        private void githubLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/mrtnmueller/LeagueTimer");
        }

        private void donateLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=YUJF8KDS4HYEE");
        }

        private void bSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 6;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void obSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 1;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void orSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 2;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void tbSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 3;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void trSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 4;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void dSetterButton_Click(object sender, RoutedEventArgs e)
        {
            settingMode = true;
            setWhichKey = 5;
            _5v5Tab.IsEnabled = _3v3Tab.IsEnabled = _DominionTab.IsEnabled = _AboutTab.IsEnabled = false;
        }

        private void playSoundCheckbox_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.playSound = (bool)playSoundCheckbox.IsChecked;
            Properties.Settings.Default.Save();
        }

    }
}
