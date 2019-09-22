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
using System.IO;
using System.IO.Compression;
using System.Net;
using WinSCP;
using System.Xml.Linq;
using System.Xml;

namespace Банковская_гарантия
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TextBlock.Text = "";
            if (TextBox.Text == "")
            {
                TextBlock.Text = "Введите пожалуйста номер закупки !";
                return;
            }
            DirectoryInfo directoryInfo = new DirectoryInfo(@"d:\folders");
            int i = 0;
            string o = null;
            if (Directory.Exists(@"d:\extract\"))
            {
                Directory.Delete(@"d:\extract\", true);
            }
            foreach (FileInfo fileInfo in directoryInfo.GetFiles("*.zip"))
            {
                ZipFile.ExtractToDirectory(@"d:\folders\" + fileInfo.Name, @"d:\extract\" + i.ToString());
                DirectoryInfo directoryInfo2 = new DirectoryInfo(@"d:\extract\" + i.ToString());
                if (directoryInfo2.GetFiles("*.xml").Length > 0)
                {
                    foreach (FileInfo fileInfo2 in directoryInfo2.GetFiles("*.xml"))
                    {
                        XmlDocument xElement = new XmlDocument();
                        xElement.Load(@"d:\extract\" + i + @"\" + fileInfo2.Name);
                        XmlNode xmlNode = xElement.GetElementsByTagName("purchaseNumber").Item(0);
                        if (xmlNode != null && xmlNode.InnerText == TextBox.Text.Trim())
                        {
                            TextBlock.Text = $"Банковская гарантия : {xElement.GetElementsByTagName("guaranteeAmount").Item(0).InnerText}";
                            break;
                        }
                    }
                }
                i++;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(@"d:\folders\"))
            {
                Directory.Delete(@"d:\folders\", true);
            }
            SessionOptions sessionOptions = new SessionOptions
            {
                Protocol = Protocol.Ftp,
                HostName = "ftp.zakupki.gov.ru",
                UserName = "free",
                Password = "free",
            };
            using (Session session = new Session())
            {
                session.Open(sessionOptions);
                session.GetFiles("/fcs_banks/bank_04160000009/", @"d:\folders").Check();
            }
        }
    }
}
