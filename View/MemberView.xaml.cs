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

namespace MusicBand_Manager.View
{
    /// <summary>
    /// Logique d'interaction pour MemberView.xaml
    /// </summary>
    public partial class MemberView : Page
    {
        private static MemberView _instance;

        public static MemberView Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new MemberView();

                return _instance;
            }
        }

        private MemberView()
        {
            InitializeComponent();
        }
    }
}
